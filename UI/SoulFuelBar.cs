using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoulFuelBar : MonoBehaviour
{
    [SerializeField] Image _soulFuelBarImage;
    [SerializeField] Image _backgroundImage;
    [SerializeField] Transform _soulBarContainer;


    int _currentSoul = 0;
    int _currentParts = 0;
    int _maxSoulFuel = 0;
    int _maxSoulParts = 0;


    float SOUL_BAR_WIDTH = HealthBar.HP_BAR_WIDTH * 1.5f + HealthBar.HP_BAR_SPACING * 0.5f;

    public void SetupSoulFuel(int maxSoulFuel)
    {
        float bgWidth = HealthBar.BG_PADDING * 2 + SOUL_BAR_WIDTH * maxSoulFuel + HealthBar.HP_BAR_SPACING * (maxSoulFuel - 1);
        _backgroundImage.rectTransform.sizeDelta = new Vector2(bgWidth, _backgroundImage.rectTransform.sizeDelta.y);
        _maxSoulFuel = maxSoulFuel;
        int barsToAdd = maxSoulFuel - _soulBarContainer.childCount;
        for (int i = 0; i < barsToAdd; i++)
        {
            var soulBarInstance = Instantiate(_soulFuelBarImage, _soulBarContainer);
            soulBarInstance.color = Color.white;
        }
    }

    public void SetupSoulParts(int maxSoulParts)
    {
        _maxSoulParts = maxSoulParts;
    }

    public void SetSoulFuel(int soulFuel)
    {
        _currentSoul = soulFuel;
        for (int i = 0; i < _maxSoulFuel; i++)
        {
            var child = _soulBarContainer.GetChild(i).GetComponent<Image>();
            child.color = i < soulFuel ? Color.white : Color.grey;
            child.fillAmount = 1;
        }
        SetSoulParts(_currentParts);
    }

    public void SetSoulParts(int soulParts)
    {
        _currentParts = soulParts;
        if (_currentSoul == _maxSoulFuel)
            return;
        var current = _soulBarContainer.GetChild(_currentSoul).GetComponent<Image>();
        current.color = Color.red;
        current.fillAmount = soulParts / (float)_maxSoulParts;
    }
}
