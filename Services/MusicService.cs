using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSAM;


[Attributes.AutoRegisteredService]
public class MusicService : Services.IRegistrable
{
    const int BPM = 110;
    const int METER = 4;

    MusicChannelHelper _persistentMusic;
    MusicChannelHelper _fightMusic;
    MusicChannelHelper _stopMusic;

    float _beatDuration;
    float _barDuration;

    bool _goingToStop = false;
    Coroutine _stopCoroutine = null;

    public void Initialize()
    {
        _beatDuration = 60f / BPM;
        _barDuration = _beatDuration * METER;
        var persistentMusicFile = AudioManager.GetMusicSafe(OblivioMusic.OST_Persistent);
        var fightMusicFile = AudioManager.GetMusicSafe(OblivioMusic.OST_Fight);
        var stopMusicFile = AudioManager.GetMusicSafe(OblivioMusic.OST_FightStop);
        _fightMusic = AudioManager.PlayMusic(fightMusicFile);
        _persistentMusic = AudioManager.PlayMusic(persistentMusicFile);
        _stopMusic = AudioManager.PlayMusic(stopMusicFile);
        _fightMusic.Mute();
        _stopMusic.Mute();

        GlobalEvents.OnAnyEnemyAware.Add(PlayFightMusic);
        GlobalEvents.OnNoEnemiesAware.Add(StopFightMusic);
    }

    // TODO should check if any coroutine is running and do smthin
    public void PlayFightMusic()
    {      
        _stopCoroutine = StaticCoroutine.Start(SwitchMusicOnNewBar(_fightMusic, false));
    }

    public void StopFightMusic()
    {
        _goingToStop = true;
        StaticCoroutine.Start(StopFightMusicCor());
    }

    IEnumerator StopFightMusicCor()
    {
        yield return StaticCoroutine.Start(SwitchMusicOnNewBar(_fightMusic, true));
        yield return StaticCoroutine.Start(PlayStopPart());
        _goingToStop = false;
    }

    IEnumerator PlayStopPart()
    {
        _stopMusic.Unmute();
        yield return new WaitForSeconds(_beatDuration * 2);
        _stopMusic.Mute();
    }

    IEnumerator SwitchMusicOnNewBar(MusicChannelHelper music, bool shouldMute)
    {
        float timeToNextBar = CalcTimeToNextBar(music.AudioSource, BPM);
        yield return new WaitForSeconds(timeToNextBar);
        if (shouldMute)
            music.Mute();
        else
            music.Unmute();
    }

    float CalcTimeToNextBar(AudioSource source, float bpm)
    {
        var currentTime = _fightMusic.AudioSource.time;
        var currentBeat = currentTime / _beatDuration;
        var currentBar = Mathf.FloorToInt(currentBeat / METER);
        var nextTime = (currentBar + 1) * _barDuration;
        return nextTime - currentTime;
    }
}
