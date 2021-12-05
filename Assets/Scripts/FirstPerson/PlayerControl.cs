using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    #region PUBLIC FIELDS

    [Range(-70f, 0f)]
    [Header("MIN поворота камеры по оси X")] public float cameraRotateXMin;
    [Range(0f, 70f)]
    [Header("MAX поворота камеры по оси X")] public float cameraRotateXMax;


    [Header("Высота прыжка")] public float playerJumpForce;
    [Header("Текущая скорость игрока")] public float currentSpeed;
    [Header("Интервал между шагами игрока (секунды)")] private float stepTimer = 0.6f;
    [Header("Сылка на joystick")] public Joystick joystick;
    #endregion

    #region PRIVATE FIELDS
    private bool playerIsJumping;
    private float m_xAxis;
    private float m_zAxis;
    private Rigidbody m_rb;
    private bool groundjJump = true;
    private ForceMode appliedForceMode = ForceMode.Impulse;
    private float curT = 0;

    private FootSteps foot;
    private Inventory ivent;

    public Touchpad touchpad;
    private float m_mouseX;
    private float m_mouseY;
    private float m_rotateX;
    private Transform m_parent;
    private Camera m_camera;

    #endregion

    #region UNITY ROUTINES

    private void Awake()
    {
        m_camera = Camera.main;
        m_parent = m_camera.transform.parent;
        m_rb = GetComponent<Rigidbody>();
        foot = GetComponent<FootSteps>();
        ivent = GetComponent<Inventory>();
    }


    private void Update()   
    {
        m_xAxis = joystick.Horizontal();
        m_zAxis = joystick.Vertical();
        MouseInputs();
        RotatePlayerY();
        CameraRotateX();
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Floors")
        {
            groundjJump = true; // окончание столкновения
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Floors")
        {
            foot.PlayStep(FootSteps.StepsOn.Beton, 1);
            groundjJump = false; // начало столкновения
        }
    }

    private void FixedUpdate()
    {
        #region move player
        if (m_xAxis!=0 && m_zAxis != 0)
        {
            m_rb.MovePosition(transform.position + Time.deltaTime * currentSpeed *
            transform.TransformDirection(m_xAxis, 0f, m_zAxis));
            curT += Time.deltaTime;
            if (curT > stepTimer)
            {
                curT = 0;
                foot.PlayStep(FootSteps.StepsOn.Beton, 1);
            }
        }
        #endregion

        #region jump player
        if (playerIsJumping  )
        {
             if (groundjJump!= true)
            {
                m_rb.AddForce(playerJumpForce * m_rb.mass * Time.deltaTime * Vector3.up, appliedForceMode);
                playerIsJumping = false;
            }
            
        }
        #endregion
    }

    #endregion

    #region HELPER ROUTINES
    public void Jumping()
    {
        playerIsJumping = true;
    }
    private void MouseInputs()
    {
        #region Collect Mouse Inputs
        m_mouseX = touchpad.Horizontal();
        m_mouseY = touchpad.Vertical();
        #endregion
    }

    private void RotatePlayerY()
    {
        #region Rotate Player Y Axis
        m_parent.Rotate(Vector3.up * m_mouseX);
        #endregion
    }

    private void CameraRotateX()
    {
        #region Rotate Camera X Axis
        m_rotateX += m_mouseY;
        m_rotateX = Mathf.Clamp(m_rotateX, cameraRotateXMin, cameraRotateXMax);
        m_camera.transform.localRotation = Quaternion.Euler(-m_rotateX, 0f, 0f);
        #endregion
    }


    #endregion


}
