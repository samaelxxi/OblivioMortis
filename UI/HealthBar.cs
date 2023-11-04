using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image _backgroundImage;
    [SerializeField] Image _healthBarImage;
    [SerializeField] Transform _healthBarContainer;

    int _maxHealth = 0;

    public static int HP_BAR_WIDTH = 91;
    public static int HP_BAR_SPACING = 4;
    public static int BG_PADDING = 7;

    public void Setup(int maxHealth)
    {
        int bgWidth = BG_PADDING * 2 + HP_BAR_WIDTH * maxHealth + HP_BAR_SPACING * (maxHealth - 1);
        _backgroundImage.rectTransform.sizeDelta = new Vector2(bgWidth, _backgroundImage.rectTransform.sizeDelta.y);
        _maxHealth = maxHealth;
        int barsToAdd = maxHealth - _healthBarContainer.childCount;
        for (int i = 0; i < barsToAdd; i++)
        {
            var healthBarInstance = Instantiate(_healthBarImage, _healthBarContainer);
            healthBarInstance.color = Color.white;
        }
    }

    public void SetHealth(int health)
    {
        for (int i = 0; i < _maxHealth; i++)
        {
            _healthBarContainer.GetChild(i).GetComponent<Image>().color = i < health ? Color.white : Color.gray;
        }
    }
}
