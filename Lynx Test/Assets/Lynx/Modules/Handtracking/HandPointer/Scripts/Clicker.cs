/**
* @file Clicker.cs
* 
* @author Geoffrey Marhuenda
* 
* @brief Generate ray pointer when enabled.
*/
using Leap.Unity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace lynx
{
    public class Clicker : MonoBehaviour
    {
        #region INSPECTOR
        [Header("Hand and source point")]
        [Tooltip("Hand model used for pinch detection")]
        [SerializeField] private Chirality m_chirality = Chirality.Right;
        [Tooltip("Define if the event should be fired once at each pinch or continuously")]
        [SerializeField] private bool m_isOnePinch = true;
        [SerializeField] private PointerClickEvent m_pinchEvent = null;
        [SerializeField] private UnityEvent m_OnStart = null;
        [SerializeField] private UnityEvent m_OnEnd = null;

        [Header("Visual helpers")]
        [Tooltip("object to display at the end of the pointer")]
        [SerializeField] private GameObject m_prefabEndRay = null;
        [Tooltip("Material for the pointer ray")]
        [SerializeField] private Material m_material = null;
        #endregion

        #region PRIVATE VARIABLES
        private const float SMOOTHNESS_FACTOR = 0.6f;
        private LineRenderer m_line = null;
        private bool m_isRunning = false;
        private GameObject m_EndRayPointInstance = null;
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Start edit mode to create points.
        /// It create LineRenderer as helper and starts to run updates.
        /// </summary>
        /// <param name="origin">Base for the raycast.</param>
        public void StartDrawingLine()
        {
            if (!m_isRunning)
            {
                m_isRunning = true;

                if (!m_line)
                {
                    m_line = this.gameObject.AddComponent<LineRenderer>();
                    m_line.startWidth = m_line.endWidth = 0.05f;
                    m_line.material = m_material;
                    m_line.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });
                }


                //if (!m_pinchDetector)
                //{
                //    m_pinchDetector = this.gameObject.AddComponent<PinchDetector>();
                //    m_pinchDetector.ActivateDistance = 0.03f;
                //    m_pinchDetector.DeactivateDistance = 0.05f;
                //    m_pinchDetector.ShowGizmos = false;
                //    m_pinchDetector.OnActivate = new UnityEvent();
                //    m_pinchDetector.OnActivate.AddListener(() => m_pinchEvent?.Invoke(m_line.GetPosition(1)));
                //    m_pinchDetector.OnDeactivate = new UnityEvent();
                //    m_pinchDetector.HandModel = m_handModel;
                //    m_pinchDetector.enabled = true;
                //}

                m_OnStart?.Invoke();


                StartCoroutine(DrawLineFromPoint());
            }
        }

        /// <summary>
        /// Stop edition mode.
        /// </summary>
        public void StopDrawingLine()
        {
            m_isRunning = false;
        }

        /// <summary>
        /// When enabled, update the tracing position.
        /// </summary>
        /// <param name="origin">Source point for raycast.</param>
        /// <returns></returns>
        public IEnumerator DrawLineFromPoint()
        {
            if (m_EndRayPointInstance == null)
                m_EndRayPointInstance = Instantiate(m_prefabEndRay);

            bool bFirstPinch = true; // Define if we are checking for first pinch to avoid continuous event
            while (m_isRunning)
            {
                // Define start point and direction for raycast
                //Vector3 startPoint = origin.position - origin.up * 0.30f + origin.right * 0.05f;
                //Vector3 direction = origin.rotation * new Vector3(-.25f, 1.0f, 0.0f);
                Leap.Hand hand = m_chirality == Chirality.Right ? Hands.Right : Hands.Left;
                if (hand == null)
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }

                Vector3 startPoint = hand.Arm.WristPosition + hand.Arm.Direction * 0.2f; // + delta to not touch hand
                Vector3 direction = (hand.Arm.Direction + 2.0f * hand.PalmNormal).normalized;
                direction += startPoint;
                Vector3 endPoint;

                // Raycast to the given direction
                RaycastHit hit;
                if (Physics.Raycast(startPoint, direction, out hit, 100.0f))
                {
                    endPoint = hit.point;
                    m_EndRayPointInstance.SetActive(true);
                }
                else
                {
                    endPoint = startPoint + direction * 100.0f;
                    m_EndRayPointInstance.SetActive(false);
                }
                endPoint = Vector3.Lerp(m_line.GetPosition(1), endPoint, SMOOTHNESS_FACTOR);

                // Place the two points
                m_line.SetPosition(0, startPoint);
                m_line.SetPosition(1, endPoint);

                // Move the end helper
                m_EndRayPointInstance.transform.position = endPoint;


                // Activate click event if pinch detected
                if (m_isOnePinch)
                {
                    if (hand.IsPinching())
                    {
                        if (bFirstPinch)
                        {
                            m_pinchEvent?.Invoke(m_line.GetPosition(1));
                            bFirstPinch = false;
                        }
                    }
                    else
                        bFirstPinch = true;
                }
                else if (hand.IsPinching())
                    m_pinchEvent?.Invoke(m_line.GetPosition(1));

                yield return new WaitForEndOfFrame();
            }

            m_line.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });


            if (m_EndRayPointInstance)
                DestroyImmediate(m_EndRayPointInstance);

            if (m_line != null)
                Destroy(m_line);

            m_OnEnd?.Invoke();
        }

        /// <summary>
        /// Toggle betwee, start pointing and end pointing.
        /// </summary>
        public void ToggleClicker()
        {
            if (m_isRunning)
                StopDrawingLine();
            else
                StartDrawingLine();
        }
        #endregion

        [Serializable]
        public class PointerClickEvent : UnityEvent<Vector3> { }
    }
}