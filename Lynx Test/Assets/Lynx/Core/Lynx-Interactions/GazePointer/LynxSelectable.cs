/**
 * @file LynxSelectable.cs
 *
 * @author Geoffrey Marhuenda
 *
 * @brief Set this object as a selectable object usable with Lynx inputs. Manage hovering and click events.
 */

using UnityEngine;
using UnityEngine.Events;

namespace lynx
{
    public class LynxSelectable : MonoBehaviour
    {
        [SerializeField] public UnityEvent OnClickEnter = null;
        [SerializeField] public UnityEvent OnClickUpdate = null;
        [SerializeField] public UnityEvent OnClickExit = null;
        [SerializeField] public UnityEvent OnHoverEnter = null;
        [SerializeField] public UnityEvent OnHoverExit = null;
    }
}