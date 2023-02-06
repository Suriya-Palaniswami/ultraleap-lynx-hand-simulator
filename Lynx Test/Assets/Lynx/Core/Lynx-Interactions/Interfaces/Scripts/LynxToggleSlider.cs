
//   ==============================================================================
//   | Lynx HandTracking Sample : LynxInterfaces (2022)                           |
//   | Author : GC & Geoffrey Marhuenda & Cedric Morel Francoz                    |
//   |======================================                                      |
//   | LynxToggleButton Script                                                    |
//   | Script to set a UI element as Toggle Button.                               |
//   ==============================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LynxToggleSlider : Button
{
    #region SCRIPT ATTRIBUTES
    // Public attributes
    [SerializeField] public Vector3 m_moveDelta = Vector3.forward;
    [SerializeField] public float m_moveDuration = 0.5f;
    [SerializeField] public bool m_isUsingScale = false;

    [SerializeField] public UnityEvent OnPress;
    [SerializeField] public UnityEvent OnUnpress;

    [SerializeField] public UnityEvent OnToggle;
    [SerializeField] public UnityEvent OnUntoggle;

    [SerializeField] private Slider slider = null;
    [SerializeField] private Image backgroundImage = null;

    [SerializeField] public List<ColorBlock> themes = new List<ColorBlock>();

    // Private attributes
    private bool m_isRunning = false; // Avoid multiple press or unpress making the object in unstable state
    private bool m_isCurrentlyPressed = false; // Status of the current object
    private Vector3 m_basePose = Vector3.zero; // Store base position when pressed.
    private bool isToggle; // Status of the button

    private const float LERP_SPEED = 15f;
    private bool m_bIsRunning = false;

    // Properties
    public new bool IsPressed { get => m_isCurrentlyPressed; }
    #endregion


    #region UNITY API
    protected override void Start()
    {
        base.Start();
    }
    #endregion


    #region UI EVENTS
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!IsInteractable()) return;

        if (!m_bIsRunning)
        {
            m_bIsRunning = true;
            if (isToggle)
            {
                base.OnPointerUp(eventData);

                isToggle = false;
                StartCoroutine(ToggleCoroutine());

                OnUntoggle.Invoke();
            }
            else
            {
                base.OnPointerDown(eventData);

                isToggle = true;
                StartCoroutine(ToggleCoroutine());

                OnToggle.Invoke();
            }
        }
        StartCoroutine(UnpressingCoroutine());
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (IsInteractable()) StartCoroutine(PressingCoroutine());
    }

    public void ChangeTheme(int index)
    {
        if (index <= themes.Count - 1) colors = themes[index];
        else Debug.LogWarning("Index not found.");
    }

    private float ToFloat(bool isOn)
    {
        return (isOn) ? 1 : 0;
    }
    #endregion


    #region UI ANIMATIONS
    /// <summary>
    /// Press animation forward.
    /// </summary>
    private IEnumerator PressingCoroutine()
    {
        if (!m_isRunning && !m_isCurrentlyPressed)
        {
            m_isRunning = true;

            float elapsedTime = 0.0f;


            m_basePose = this.transform.localPosition;
            Vector3 forwardPose = m_basePose;
            if (m_isUsingScale)
            {
                forwardPose.x += m_moveDelta.x * this.transform.localScale.x;
                forwardPose.y += m_moveDelta.y * this.transform.localScale.y;
                forwardPose.z += m_moveDelta.z * this.transform.localScale.z;
            }
            else
            {
                forwardPose += m_moveDelta;
            }


            // Forward
            while (elapsedTime < m_moveDuration)
            {
                this.transform.localPosition = Vector3.Lerp(m_basePose, forwardPose, elapsedTime / m_moveDuration);
                yield return new WaitForEndOfFrame();
                elapsedTime += Time.deltaTime;
            }

            this.transform.localPosition = forwardPose;

            m_isCurrentlyPressed = true;
            m_isRunning = false;

            OnPress.Invoke();
        }
    }

    /// <summary>
    /// Press animation backward.
    /// </summary>
    private IEnumerator UnpressingCoroutine()
    {
        while (m_isRunning)
            yield return new WaitForEndOfFrame();

        if (m_isCurrentlyPressed)
        {
            m_isRunning = true;

            float elapsedTime = 0.0f;
            Vector3 forwardPose = this.transform.localPosition;

            // Backward
            while (elapsedTime < m_moveDuration)
            {
                this.transform.localPosition = Vector3.Lerp(forwardPose, m_basePose, elapsedTime / m_moveDuration);
                yield return new WaitForEndOfFrame();
                elapsedTime += Time.deltaTime;
            }

            this.transform.localPosition = m_basePose;

            m_isCurrentlyPressed = false;
            m_isRunning = false;

            OnUnpress.Invoke();
        }
    }

    /// <summary>
    /// Toggle slide animation.
    /// </summary>
    IEnumerator ToggleCoroutine()
    {
        float duration = 0.5f;
        float elapsedTime = 0.0f;
        Color baseColor = backgroundImage.color;
        while (elapsedTime < duration)
        {
            slider.value = Mathf.Lerp(slider.value, ToFloat(isToggle), Time.deltaTime * LERP_SPEED);
            baseColor.a = slider.value;
            backgroundImage.color = baseColor;
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.deltaTime;
        }
        slider.value = ToFloat(isToggle);
        baseColor.a = slider.value;
        backgroundImage.color = baseColor;
        m_bIsRunning = false;
    }
    #endregion
}