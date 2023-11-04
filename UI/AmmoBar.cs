using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoBar : MonoBehaviour
{
    [SerializeField] GameObject _ammoPrefab;
    [SerializeField] GameObject _ammoContainer;

    [SerializeField] GameObject _threeAmmoBar;
    [SerializeField] GameObject _fourAmmoBar;


    Color _defaultColor = Color.white;

    const int WINDOW_SIZE = 200;

    int _currentAmmo = 0;
    // int _maxAmmo = 0;

    public void SetAmmo(int currentAmmo)
    {
        _currentAmmo = currentAmmo;
        for (int i = 0; i < _ammoContainer.transform.childCount; i++)
        {
            _ammoContainer.transform.GetChild(i).GetComponent<Image>().color = i < currentAmmo ? _defaultColor : Color.gray;
        }
    }

    public void ChangeAmmoColor(Color newColor)
    {
        _defaultColor = newColor;
        SetAmmo(_currentAmmo);
    }

    public void Setup(int maxAmmo)
    {
        if (maxAmmo == 3)
        {
            _threeAmmoBar.SetActive(true);
            _fourAmmoBar.SetActive(false);
            _ammoContainer = _threeAmmoBar;
        }
        else if (maxAmmo == 4)
        {
            _threeAmmoBar.SetActive(false);
            _fourAmmoBar.SetActive(true);
            _ammoContainer = _fourAmmoBar;
        }



        // for (int i = 0; i < _gridLayoutGroup.transform.childCount; i++)
        // {
        //     Destroy(_gridLayoutGroup.transform.GetChild(i).gameObject);
        // }

        // int totalSize = WINDOW_SIZE * WINDOW_SIZE;
        // float filledSize = Mathf.Sqrt((float)totalSize / maxAmmo);
        // int rowElements = Mathf.FloorToInt(WINDOW_SIZE / filledSize);
        // if (rowElements * rowElements < maxAmmo)
        // {
        //     rowElements++;
        //     filledSize = WINDOW_SIZE / (float)rowElements;
        // }
        // for (int i = 0; i < maxAmmo; i++)
        // {
        //     var ammoInstance = Instantiate(_ammoPrefab, _gridLayoutGroup.transform);
        //     _gridLayoutGroup.cellSize = new Vector2(filledSize, filledSize);
        //     ammoInstance.GetComponent<Image>().color = Color.white;
        // }
        // _maxAmmo = maxAmmo;
    }
}
