using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Button_Manger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    public static bool _press = false;

    public void OnPointerDown(PointerEventData eventdata ) {
        _press = true;
    }

    public void OnPointerUp(PointerEventData eventdata) {
        _press = false;
    }
}
