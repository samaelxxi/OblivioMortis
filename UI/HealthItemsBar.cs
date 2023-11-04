using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthItemsBar : MonoBehaviour
{
    [SerializeField] Transform _itemsParent;


    public void SetupHealthItems(int maxItems)
    {
        var healthItem = _itemsParent.GetChild(0).gameObject;
        var itemsCount = _itemsParent.childCount;

        for (int i = 1; i < itemsCount; i++)
            Destroy(_itemsParent.GetChild(i).gameObject);
        
        for (int i = 1; i < maxItems; i++)
            Instantiate(healthItem, _itemsParent);
    }

    public void UpdateHealthItems(int healthItems)
    {
        for (int i = 0; i < _itemsParent.childCount; i++)
        {
            if (i < healthItems)
                _itemsParent.GetChild(i).GetComponent<Image>().color = Color.white;
            else
                _itemsParent.GetChild(i).GetComponent<Image>().color = Color.gray;
        }
    }
}
