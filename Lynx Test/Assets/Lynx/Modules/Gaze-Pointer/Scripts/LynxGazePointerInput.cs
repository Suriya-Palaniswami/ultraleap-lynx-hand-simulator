/**
 * @file LynxGazePointerInput.cs
 *
 * @author Geoffrey Marhuenda
 *
 * @brief Gaze pointer input for Lynx headset.
 */

using UnityEngine;
using UnityEngine.Events;

namespace lynx
{
    public class LynxGazePointerInput : MonoBehaviour
    {
        #region INSPECTOR
        [Tooltip("object to spawn as cursor")]
        [SerializeField] private GameObject m_cursorPrefab = null;

        [Range(0.0001f, 1.0f)]
        [Tooltip("Smothness used to make the cursor follow gaze")]
        [SerializeField] private float m_followCoeff = 0.25f;

        [Tooltip("Fire events when button is clicked")]
        [SerializeField] private UnityEvent m_onClickEvent = null;
        #endregion

        #region PUBLIC VARIABLES
#if UNITY_EDITOR
        public const KeyCode LYNX_BUTTON = KeyCode.Space;
#else
        public const KeyCode LYNX_BUTTON = KeyCode.JoystickButton0;
#endif
        #endregion

        #region PRIVATE VARIABLES
        private Transform m_camTransform = null;

        // Visual cursor objet spawned
        private GameObject m_currentCursor = null;

        // Current interactable target
        private LynxSelectable m_targetSelectable = null;

        private bool m_bIsPressed = false; // Status of the button
        private Vector3 m_vToTarget = Vector3.zero; // Distance to the grabbed object (to move it keeping its relative position to the camera)
        private Vector3 m_vToCursor = Vector3.zero; // Distance to the grabbed object (to move it keeping its relative position to the camera)

        #endregion

        #region PROPERTIES
        public Vector3 CursorPos { get; private set; } = Vector3.zero; // Get cursor position
        #endregion

        #region SINGLEON
        protected LynxGazePointerInput() { }
        public static LynxGazePointerInput Instance { get; private set; } = null;
        #endregion

        #region UNITY API
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            m_camTransform = Camera.main.transform;
        }

        private void Update()
        {
            // Manage release click of the headset.
            if (Input.GetKeyUp(LYNX_BUTTON))
            {
                m_onClickEvent?.Invoke();
                // Disable pressed status, fire click exit event and release target.
                m_bIsPressed = false;
                if (m_targetSelectable)
                    m_targetSelectable.OnClickExit?.Invoke();
            }


            // Manage raycast from the camera source
            Vector3 fw = m_camTransform.forward;
            RaycastHit hit;
            if (Physics.Raycast(m_camTransform.position, fw, out hit, 100))
            {
                if (!m_bIsPressed)
                {
                    // Check if the collided object is a lynx selectable to fire corresponding events and make appears the cursor
                    LynxSelectable selectable = hit.collider.GetComponent<LynxSelectable>();
                    if (selectable)
                    {
                        // If a previous object was selected, quit hovering
                        if (m_targetSelectable && selectable != m_targetSelectable)
                            m_targetSelectable.OnHoverExit?.Invoke();

                        // If there is no visible cursor, create one
                        if (!m_currentCursor)
                        {
                            m_currentCursor = Instantiate(m_cursorPrefab, this.transform, false);
                            m_currentCursor.transform.position = hit.point;
                        }

                        // Place this object as current selectable and invoke hover method
                        m_targetSelectable = selectable;
                        m_targetSelectable.OnHoverEnter?.Invoke();

                        // Set cursor position to the hit point
                        m_currentCursor.transform.position = Vector3.Lerp(m_currentCursor.transform.position, hit.point, m_followCoeff);

                    }
                    else // Object is not selectable, unselect previous one
                    {
                        Unselect();
                    }
                }
                else // Pressed
                {
                    UpdateCursorAndGrabbedObject();
                }
            }
            else if (m_bIsPressed) // There is not hit available, unselect previous one
            {
                UpdateCursorAndGrabbedObject();
            }
            else // There is not hit available, unselect previous one
            {
                Unselect();
            }


            // Manage input click of the headset.
            if(Input.GetKey(LYNX_BUTTON))
            {
                // If button was not already pressed, make it pressed and store the target
                if (!m_bIsPressed)
                {
                    m_bIsPressed = true;
                    if (m_targetSelectable)
                    {
                        m_vToTarget = Quaternion.Inverse(m_camTransform.rotation) * (m_targetSelectable.transform.position - m_camTransform.position); // Keep current object distance (used for grabbing)

                        m_vToCursor = Quaternion.Inverse(m_camTransform.rotation) * (hit.point - m_camTransform.position); // // Keep current cursor distance (used to customize event when click update)
                        CursorPos = m_camTransform.position + m_camTransform.rotation * m_vToCursor;

                        // Fire Click event
                        m_targetSelectable.OnClickEnter?.Invoke();


                    }
                }
            }
        }

#endregion


#region PUBLIC METHODS
        private void UpdateCursorAndGrabbedObject()
        {
            Vector3 TargetPos = m_camTransform.position + m_camTransform.rotation * m_vToTarget;
            CursorPos = m_camTransform.position + m_camTransform.rotation * m_vToCursor;


            if (m_targetSelectable)
            {

                // Update grabbed object
                if (m_targetSelectable.GetComponent<LynxGrabbable>())
                    m_targetSelectable.transform.position = Vector3.Lerp(m_targetSelectable.transform.position, TargetPos, m_followCoeff);

                // Send update event
                m_targetSelectable.OnClickUpdate?.Invoke();
            }


            // Update cursor
            if(m_currentCursor)
                m_currentCursor.transform.position = Vector3.Lerp(m_currentCursor.transform.position, TargetPos, m_followCoeff);
        }

        /// <summary>
        /// Destroy cursor and remove selected target.
        /// </summary>
        public void Unselect()
        {
            if (m_currentCursor)
                DestroyImmediate(m_currentCursor);

            if (m_targetSelectable)
            {
                m_targetSelectable.OnHoverExit?.Invoke();
                m_targetSelectable = null;
            }
        }
#endregion

    }
}