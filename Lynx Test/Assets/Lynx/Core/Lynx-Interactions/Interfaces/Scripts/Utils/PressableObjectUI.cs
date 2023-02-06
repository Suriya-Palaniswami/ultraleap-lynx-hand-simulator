/**
 * @file PressableObjectUI.cs
 *
 * @author Geoffrey Marhuenda
 *
 * @brief Simple script to simulate move of the current object when pressed or toggled.
 */

using System.Collections;
using UnityEngine;

namespace lynx
{
    public class PressableObjectUI : MonoBehaviour
    {
        #region INSPECTOR
        [Tooltip("Transition to apply on the current object")]
        [SerializeField] public Vector3 m_moveDelta = Vector3.forward;
        [Tooltip("Duration for foward or backward press animation.")]
        [SerializeField] public float m_moveDuration = 0.5f;
        [Tooltip("If checked, the object will be affecte by its local scale for animation.")]
        [SerializeField] public bool m_isUsingScale = false;
        #endregion

        #region PRIVATE VARIABLES
        private bool m_isRunning = false; // Avoid multiple press or unpress making the object in unstable state
        private bool m_isCurrentlyPressed = false; // Status of the current object
        private Vector3 m_basePose = Vector3.zero; // Store base position when pressed.
        #endregion

        #region PROPERTIES
        public bool IsPressed { get => m_isCurrentlyPressed; }
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Start press forward and backward animation.
        /// </summary>
        public void Pressed()
        {
            StartCoroutine(PressedCoroutine());
        }

        /// <summary>
        /// Start forward press animation.
        /// </summary>
        public void Press()
        {
            StartCoroutine(PressingCoroutine());
        }

        /// <summary>
        /// Start backward press animation.
        /// </summary>
        public void Unpress()
        {
            StartCoroutine(PressingCoroutine());
        }
        #endregion

        #region ANIMATIONS

        /// <summary>
        /// Press animation forward and backward.
        /// </summary>
        private IEnumerator PressedCoroutine()
        {
            yield return PressingCoroutine();
            yield return new WaitForEndOfFrame();
            yield return UnpressingCoroutine();

        }

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
            if (!m_isRunning && m_isCurrentlyPressed)
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
}
