using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextColorChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Color _unhoveredColor;
    [SerializeField] Color _hoveredColor;

    [SerializeField] TMPro.TextMeshProUGUI _text;


    public void OnPointerEnter(PointerEventData eventData)
    {
        _text.color = _hoveredColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _text.color = _unhoveredColor;
    }
}
