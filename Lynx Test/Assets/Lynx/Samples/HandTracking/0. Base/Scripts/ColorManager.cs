/**
 * @file SpawnCube.cs
 *
 * @author Geoffrey Marhuenda
 *
 * @brief While there is some trouble using UL interaction buttons. This script helps to use color changes when an UI element is pressed.
 */
using UnityEngine;

namespace lynx
{
    public class ColorManager : MonoBehaviour
    {
        [SerializeField] private Color m_unpressColor = Color.white;
        [SerializeField] private Color m_pressColor = Color.blue;

        public void Press(Renderer renderer)
        {
            renderer.material.color = m_pressColor;
        }

        public void Unpress(Renderer renderer)
        {
            renderer.material.color = m_unpressColor;
        }
    }
}