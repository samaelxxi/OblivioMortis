using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseCursor : MonoBehaviour
{
    [SerializeField] Image _cursorImage;

    public void SetCursorPos(Vector3 pos)
    {
        _cursorImage.transform.position = pos;
    }
}
