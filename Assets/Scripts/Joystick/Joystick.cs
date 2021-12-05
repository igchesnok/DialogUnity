using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    Image joystick;
    Image joystickBack;
    Vector2 inputVector;
    public RectTransform joystickRT, backgroundRT;
    // Start is called before the first frame update
    void Start()
    {
        backgroundRT = transform.GetChild(0) as RectTransform;
        joystickRT = backgroundRT.GetChild(0) as RectTransform;

        joystickBack = backgroundRT.GetComponent<Image>();
        joystick = joystickRT.GetComponent<Image>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBack.rectTransform, eventData.position, eventData.pressEventCamera, out pos))
        {
           pos.x = (pos.x / joystickBack.rectTransform.sizeDelta.x);
           pos.y = (pos.y / joystickBack.rectTransform.sizeDelta.y);
        }
        inputVector = new Vector2(pos.x , pos.y);
       inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

        joystick.rectTransform.anchoredPosition = new Vector2(inputVector.x * (joystickBack.rectTransform.sizeDelta.x / 2), inputVector.y * (joystickBack.rectTransform.sizeDelta.y / 2));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        joystick.rectTransform.anchoredPosition = Vector2.zero;
    }

    public float Horizontal()
    {
        if (inputVector.x !=0)
        {
            return inputVector.x;
        }
        else
        {
            return Input.GetAxis("Horizontal");
        }
    }

    public float Vertical()
    {
        if (inputVector.y != 0)
        {
            return inputVector.y;
        }
        else
        {
            return Input.GetAxis("Vertical");
        }
    }
}

