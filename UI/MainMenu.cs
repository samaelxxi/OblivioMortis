using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Image _fadeImage;

    bool _isStarting = false;

    void Awake()
    {
        if (Globals.IsGameInitialized)
        {
            Game.Instance.OnGameMenuEntered();
            return;
        }

        Instantiate(Resources.Load<Game>("Game"));
        Game.Instance.OnGameMenuEntered();
        Game.Instance.gameObject.SetActive(false);
        Globals.IsGameInitialized = true;
    }

    public void NewGame()
    {
        if (_isStarting) return;
        _isStarting = true;
        _fadeImage.DOFade(1, 1).OnComplete(
            () => ServiceLocator.Get<SceneTransitionManager>().StartNewGame());
    }

    public void Settings()
    {
        if (_isStarting) return;
        JSAM.AudioManager.PlaySound(OblivioSounds.Settings);
    }

    public void Exit()
    {
        if (_isStarting) return;
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
