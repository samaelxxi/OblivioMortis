using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Rendering.Universal;

public class UIView : MonoBehaviour, Services.IRegistrable
{
    [SerializeField] Canvas _inGameCanvas;
    [SerializeField] Canvas _menuCanvas;
    [SerializeField] Canvas _dialogCanvas;

    [SerializeField] HealthBar _healthBar;
    [SerializeField] HealthItemsBar _healthItemsBar;
    [SerializeField] SoulFuelBar _soulFuelBar;
    [SerializeField] AmmoBar _ammoBar;
    [SerializeField] MouseCursor _mouseCursor;
    [SerializeField] UIBillboard _billboard;
    [SerializeField] Image _fadeImage;
    [SerializeField] CheatsBar _cheatsBar;
    [SerializeField] EffectsOverlay _effectsOverlay;
    [SerializeField] DialogWindow _dialogWindow;

    [SerializeField] RenderTexture _menuRenderTexture;
    [SerializeField] RawImage _menuBG;
    [SerializeField] MouseCursor _menuMouseCursor;
    [SerializeField] Image _menuFadeImage;


    int _prevHealth;
    int _prevSouls;
    bool _isPaused = false;
    public bool IsPaused => _isPaused;

    Player _player;


    void Awake()
    {
        // Cursor.lockState = CursorLockMode.Confined;

        ServiceLocator.Register(this);

        ServiceLocator.Get<SceneTransitionManager>().OnSceneTransitionStarted += () => FadeIn();
        ServiceLocator.Get<SceneTransitionManager>().OnSceneTransitionEnded += () => FadeOut();

        var inputs = ServiceLocator.Get<PlayerInputs>();
        inputs.OnCheatsClicked += () => _cheatsBar.gameObject.SetActive(!_cheatsBar.gameObject.activeSelf);
        inputs.OnMenuClicked += MenuClick;
        inputs.OnInteractClicked += () => _dialogWindow.OnContinueClicked();
    }


    void Update()
    {
        if (_isPaused)
            _menuMouseCursor.SetCursorPos(Input.mousePosition);
        else
            _mouseCursor.SetCursorPos(Input.mousePosition);
    }

    public void Initialize(Player player)
    {
        _healthBar.Setup(player.Stats.MaxHealth);
        _prevHealth = player.Stats.MaxHealth;
        _healthItemsBar.SetupHealthItems(player.Stats.MaxHealthItems);
        _soulFuelBar.SetupSoulFuel(player.Stats.MaxSoulFuel);
        _prevSouls = player.Stats.MaxSoulFuel;
        _soulFuelBar.SetupSoulParts(player.Stats.MaxSoulParts);
        _ammoBar.Setup(player.MaxAmmo);
        _ammoBar.SetAmmo(player.MaxAmmo);
        _billboard.SetPlayer(player);
        _player = player;

        player.OnHealthChanged += OnHealthChanged;
        player.OnMaxHealthChanged += _healthBar.Setup;
        player.OnSoulFuelChanged += OnSoulsChanged;
        player.OnSoulPartsChanged += _soulFuelBar.SetSoulParts;
        player.OnMaxSoulFuelChanged += _soulFuelBar.SetupSoulFuel;
        player.OnHealthItemsChanged += _healthItemsBar.UpdateHealthItems;
        player.OnAmmoChanged += _ammoBar.SetAmmo;
        player.OnMaxAmmoChanged += _ammoBar.Setup;
        player.OnBulletEffectChanged += OnBulletEffectChanged;
        player.OnInputTypeChanged += OnInputTypeChanged;
        _dialogWindow.OnDialogFinished += EndDialog;
    }

    void OnHealthChanged(int newHealth)
    {
        _healthBar.SetHealth(newHealth);
        if (newHealth < _prevHealth)
            _effectsOverlay.GetDamaged();
        _prevHealth = newHealth;
    }

    void OnSoulsChanged(int newSouls)
    {
        _soulFuelBar.SetSoulFuel(newSouls);
        if (newSouls > _prevSouls)
            _effectsOverlay.RestoreSouls();
        _prevSouls = newSouls;
    }

    void OnBulletEffectChanged(BulletEffectType type)
    {
        Color newColor = ServiceLocator.Get<BulletFactory>().GetBulletUIColor(type);
        _ammoBar.ChangeAmmoColor(newColor);
    }

    void OnInputTypeChanged(PlayerInputType type)
    {
        _mouseCursor.gameObject.SetActive(type == PlayerInputType.KeyboardMouse);
    }

    public void EnableInteractBillboard(bool enabled)
    {
        _billboard.Enable(enabled);
    }

    public void FadeIn(float duration = 0.5f)
    {
        _fadeImage.DOFade(1, duration);
        _fadeImage.gameObject.SetActive(true);
    }

    public void FadeOut(float duration = 0.5f)
    {
        _fadeImage.DOFade(0, duration);
        this.InSeconds(duration, () => _fadeImage.gameObject.SetActive(false));
    }


    public void StartDialog(DialogData dialogData, bool isFinished)
    {
        if (_dialogWindow.IsDialogActive)
            return;

        _inGameCanvas.gameObject.SetActive(false);
        _dialogCanvas.gameObject.SetActive(true);
        _player.SetDisabled(true);
        _dialogWindow.ActivateDialog(dialogData, isFinished);
    }

    void EndDialog()
    {
        _inGameCanvas.gameObject.SetActive(true);
        _dialogCanvas.gameObject.SetActive(false);
        _player.SetDisabled(false);
    }


    # region InGameMenu

    void MenuClick()
    {
        if (_dialogWindow.IsDialogActive)
            return;

        if (_isPaused)
        {
            DisableMenu();
            _player.SetDisabled(false);
        }
        else
        {
            EnableMenu();
            _player.SetDisabled(true);
        }
    }

    public void DisableMenu()
    {
        Time.timeScale = 1;
        _menuCanvas.gameObject.SetActive(false);
        _inGameCanvas.gameObject.SetActive(true);
        _isPaused = false;
    }

    void EnableMenu()
    {
        // TODO its complicated... should blur the background
        // RenderTexture activeRenderTexture = Camera.main.targetTexture;
        // Camera.main.targetTexture = _menuRenderTexture;
        // Camera.main.Render();
        // Camera.main.targetTexture = activeRenderTexture;
        // _menuBG.texture = _menuRenderTexture;
        // StartCoroutine(MakeMenuBG());

        Time.timeScale = 0;
        _menuCanvas.gameObject.SetActive(true);
        _inGameCanvas.gameObject.SetActive(false);
        _isPaused = true;
    }

    public void MenuFadeIn(float duration = 0.5f)
    {
        _menuFadeImage.DOFade(1, duration).SetUpdate(true);
        _menuFadeImage.gameObject.SetActive(true);
    }

    public void MenuFadeOut(float duration = 0.5f)
    {
        _menuFadeImage.DOFade(0, duration).SetUpdate(true);
        this.InSeconds(duration, () => _menuFadeImage.gameObject.SetActive(false));
    }

    public void OnResumeClick()
    {
        // TODO change colors to grey
        DisableMenu();
        _player.SetDisabled(false);
    }

    public void OnLoadCheckpointClick()
    {
        Game.Instance.LoadLastCheckpoint();
        DisableMenu();
    }

    public void OnSettingsClick()
    {
        JSAM.AudioManager.PlaySound(OblivioSounds.Settings);
    }

    public void OnQuitToMenuClick()
    {
        MenuFadeIn(1f);
        this.InSecondsRealtime(1f, () => ServiceLocator.Get<SceneTransitionManager>().GoToMainMenu());
    }

    public void OnQuitClick()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    # endregion
}
