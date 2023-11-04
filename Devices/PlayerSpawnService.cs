using System.Linq;
using System.Collections;
using Services;
using UnityEngine;
using DesignPatterns;


[Attributes.AutoRegisteredService]
public class PlayerSpawnService : MonoRegistrable
{
    [SerializeField] Vector3 _activeRespawnPoint;
    [SerializeField] PlayerRespawnPoint _respawnPoint;
    
    Player _player;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void RegisterPlayer(Player player)
    {
        _player = player;
    }

    public Vector3 GetRespawnPoint()
    {
        return _activeRespawnPoint;
    }

    public PlayerRespawnPoint GetRespawnObject()
    {
        return _respawnPoint;
    }

    public void SetRespawnObject(PlayerRespawnPoint respawnPoint)
    {
        _respawnPoint = respawnPoint;
        SetRespawnPoint(_respawnPoint.transform.position);
    }

    public void SetRespawnPoint(Vector3 respawnPoint)
    {
        _activeRespawnPoint = respawnPoint;
    }

    public void RespawnPlayer()
    {
        _player.TeleportTo(_activeRespawnPoint);
        _player.Respawn();
    }

    public void InitInNewSceneOnLocation(int locationIndex)
    {
        var locationEntrances = FindObjectsByType<SceneEntrancePoint>(FindObjectsSortMode.None);
        if (locationEntrances.Length == 0)
            return;

        try 
        {
            var entrance = locationEntrances.First(x => x.EntranceIndex == locationIndex);
            _player.TeleportTo(entrance.transform.position);
            SetRespawnPoint(entrance.transform.position);
        }
        catch (System.InvalidOperationException)
        {
            Debug.LogError($"No entrance with location index {locationIndex} found");
            return;
        }
    }

    public void InitInNewScene()
    {
        var locationEntrances = FindObjectsByType<SceneEntrancePoint>(FindObjectsSortMode.None);
        if (locationEntrances.Length == 0)
            return;
        var min = locationEntrances.Min(x => x.EntranceIndex);
        var entrance = locationEntrances.First(x => x.EntranceIndex == min);
        _player.TeleportTo(entrance.transform.position);
        SetRespawnPoint(entrance.transform.position);
    }
}
