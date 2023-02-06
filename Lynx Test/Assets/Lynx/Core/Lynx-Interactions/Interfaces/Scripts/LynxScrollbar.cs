using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LynxScrollbar : Scrollbar
{
    #region SCRIPT ATTRIBUTES
    // Public attributes
    [SerializeField] public Vector3 m_moveDelta = Vector3.forward;
    [SerializeField] public float m_moveDuration = 0.5f;
    [SerializeField] public bool m_isUsingScale = false;

    [SerializeField] public List<ColorBlock> themes = new List<ColorBlock>();

    // Private attributes
    private bool m_isRunning = false; // Avoid multiple press or unpress making the object in unstable state
    private bool m_isCurrentlyPressed = false; // Status of the current object
    private Vector3 m_basePose = Vector3.zero; // Store base position when pressed.

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
        base.OnPointerUp(eventData);
        if (IsInteractable()) StartCoroutine(UnpressingCoroutine());

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
        }
    }
    #endregion
}
