using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DesignPatterns;
using System;

[Attributes.AutoRegisteredService]
public class SceneTransitionManager : Services.IRegistrable
{
    string _currentSceneName;
    string _newSceneName;

    public event Action OnSceneTransitionStarted;
    public event Action OnSceneTransitionEnded;


    const string START_SCENE = "World 1-1";
    // const string START_SCENE = "SaveScene1";


    public void StartNewGame()
    {
        ServiceLocator.Get<SceneSaver>().ClearSaveData();
        StaticCoroutine.Start(StartNewGameCoroutine());
    }

    IEnumerator StartNewGameCoroutine()
    {
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        Game.Instance.gameObject.SetActive(true);
        yield return SceneManager.LoadSceneAsync(START_SCENE, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(START_SCENE));
        ServiceLocator.Get<SceneSaver>().LoadScene(SceneManager.GetActiveScene());
        ServiceLocator.Get<PlayerSpawnService>().InitInNewScene();
        Game.Instance.OnNewGameStarted();
    }

    public void GoToMainMenu()
    {
        StaticCoroutine.Start(GoToMainMenuCoroutine());
    }

    IEnumerator GoToMainMenuCoroutine()
    {
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        Game.Instance.gameObject.SetActive(false);
        yield return SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainMenu"));
    }

    public void TransitionTo(string sceneName, int locationIndex)
    {
        _currentSceneName = SceneManager.GetActiveScene().name;
        _newSceneName = sceneName;
        StaticCoroutine.Start(InnerTransition(locationIndex));
    }

    IEnumerator InnerTransition(int locationIndex)
    {
        OnSceneTransitionStarted?.Invoke();

        // TODO may want to make async
        ServiceLocator.Get<SceneSaver>().SaveScene(SceneManager.GetActiveScene());

        yield return new WaitForSeconds(0.5f); // TODO ehh ui delay ehh

        yield return SceneManager.UnloadSceneAsync(_currentSceneName);
        yield return SceneManager.LoadSceneAsync(_newSceneName, LoadSceneMode.Additive);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(_newSceneName));
        ServiceLocator.Get<SceneSaver>().LoadScene(SceneManager.GetActiveScene());

        ServiceLocator.Get<PlayerSpawnService>().InitInNewSceneOnLocation(locationIndex);

        OnSceneTransitionEnded?.Invoke();
    }
}
