using UnityEngine;
using UnityEngine.Events;

namespace lynx
{
    public class GazeHandlerMulticlick : MonoBehaviour
    {
        #region INSPECTOR
        [SerializeField] private UnityEvent m_doubleClickEvent = null;
        [SerializeField] private UnityEvent m_tripleClickEvent = null;
        #endregion

        private const float interval_max = 1.1f;
        private int counter = 0;
        private float elapsedTime = 0.0f;

        private void Update()
        {
            if (elapsedTime > interval_max)
            {
                // Fire event depending on counter

                if(counter == 3)
                {
                    m_tripleClickEvent?.Invoke();
                }
                else if (counter == 2)
                {
                    m_doubleClickEvent?.Invoke();
                }

                counter = 0;
            }

#if UNITY_EDITOR
            if (Input.GetMouseButtonUp(0))
#else
            if (Input.GetKeyUp(LynxGazePointerInput.LYNX_BUTTON))
#endif
            {
                if (counter == 0)
                    elapsedTime = 0.0f;

                ++counter;
            }

            elapsedTime += Time.deltaTime;
        }
    }
}