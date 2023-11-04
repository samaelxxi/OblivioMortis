using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DesignPatterns;


public class Game : Singleton<Game>
{
    [SerializeField] Player _player;
    [SerializeField] UIView _UIView;


    bool _changingState;


    public override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this);
    }

    void Start()
    {
        _UIView.Initialize(_player);

        ServiceLocator.Get<PlayerSpawnService>().InitInNewScene();
        ServiceLocator.Get<MusicService>().Initialize();
        GlobalEvents.OnRespawnActivated.Add(OnRespawnPointActivated);
        _player.OnDeath += RespawnPlayer;
    }

    public void RespawnPlayer()
    {
        if (_changingState)
            return;
        StartCoroutine(RespawnCoroutine());
    }

    IEnumerator RespawnCoroutine()
    {
        _changingState = true;
        yield return new WaitForSeconds(2);
        ServiceLocator.Get<UIView>().FadeIn(1);
        yield return new WaitForSeconds(1);
        ServiceLocator.Get<PlayerSpawnService>().RespawnPlayer();
        ServiceLocator.Get<EnemyManager>().RespawnEnemies();
        ServiceLocator.Get<UIView>().FadeOut(1);
        yield return new WaitForSeconds(1);
        _player.SetDisabled(false);
        _changingState = false;
    }

    public void LoadLastCheckpoint()
    {
        if (_changingState)
            return;
        StartCoroutine(LoadLastCheckpointCoroutine());
    }

    IEnumerator LoadLastCheckpointCoroutine()
    {
        _changingState = true;
        _player.SetDisabled(true);
        ServiceLocator.Get<UIView>().FadeIn(1);
        yield return new WaitForSeconds(1);
        ServiceLocator.Get<PlayerSpawnService>().RespawnPlayer();
        ServiceLocator.Get<EnemyManager>().RespawnEnemies();
        yield return new WaitForSeconds(1);
        ServiceLocator.Get<UIView>().FadeOut(1);
        yield return new WaitForSeconds(1);
        _player.SetDisabled(false);
        _changingState = false;
    }


    public void OnNewGameStarted()
    {
        _UIView.FadeIn(0);
        this.InSeconds(1, () => _UIView.FadeOut(1));
        _UIView.DisableMenu();
        _UIView.MenuFadeOut(0);
        this.InSeconds(2, () => _player.SetDisabled(false));
        _player.Respawn();
    }

    public void OnGameMenuEntered()
    {
        _player.SetDisabled(true);
        Time.timeScale = 1;
    }

    void OnRespawnPointActivated()
    {
        ServiceLocator.Get<EnemyManager>().RespawnEnemies();
    }
}
