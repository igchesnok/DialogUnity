using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Touchpad : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Range(1f, 10f)]
    public float sensitivity = 1f;
    [Range(5f, 25f)]
    public float axesLagSpeed = 10f;

    private Vector2 defaultPosition, 
        currentPosition,
        currentDirection;

    private bool touchDown;
    private float  axisX = 0,
        axisY = 0;

    public void OnDrag(PointerEventData eventData)
    {
        UpdatePosition(eventData.position);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UpdatePosition(eventData.position);
        ResetAxes();
        touchDown = false;
       
    }

    public float Horizontal()
    {
        if (axisX != 0)
        {
            return axisX;
        }
        else
        {
            return 0;
        }
    }

    public float Vertical()
    {
        if (axisY != 0)
        {
            return axisY;
        }
        else
        {
            return 0;
        }
    }

    private void UpdatePosition(Vector2 touchPos)
    {
        if (touchDown)
        {
            currentPosition.x = touchPos.x;
            currentPosition.y = touchPos.y;

            currentDirection = currentPosition - defaultPosition;

            float touchForce = Vector2.Distance(defaultPosition, currentPosition) * 2f;
            defaultPosition = currentPosition;

            SetAxes(currentDirection.normalized * touchForce / 100f * sensitivity);
        }
        else
        {
            touchDown = true;
            currentPosition = defaultPosition = touchPos;
            UpdatePosition(touchPos);
            ResetAxes();
        }
    }

    private void SetAxes(Vector2 axes)
    {
        SetAxes(axes.x, axes.y);
    }

    private void ResetAxes()
    {
        axisY=0f;
        axisX = 0f;
    }

    private float SetValue(float value)
    {
       return (float)Math.Round((double)value, 3);
    }
    private void SetAxes(float x, float y)
    {
        StopCoroutine("SmoothAxisX");
        StartCoroutine("SmoothAxisX", x);
        
        StopCoroutine("SmoothAxisY");
        StartCoroutine("SmoothAxisY", y);
    }

    // Smooth AxisX
    private IEnumerator SmoothAxisX(float targetValue)
    {
        while (Math.Round((double)axisX, 2) != Math.Round((double)targetValue, 2))
        {
            axisX=SetValue(Mathf.Lerp(axisX, targetValue, Time.smoothDeltaTime * axesLagSpeed));
            yield return null;
        }
        axisX=SetValue(targetValue);
    }

    // Smooth AxisY
    private IEnumerator SmoothAxisY(float targetValue)
    {
        while (Math.Round((double)axisY, 2) != Math.Round((double)targetValue, 2))
        {
            axisY=SetValue(Mathf.Lerp(axisY, targetValue, Time.smoothDeltaTime * axesLagSpeed));
            yield return null;
        }
        axisY=SetValue(targetValue);
    }

}
