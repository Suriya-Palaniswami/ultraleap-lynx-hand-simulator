using UnityEngine;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace lynx
{
    /// <summary>
    /// Request permission for camera.
    /// </summary>
    public class RequestCamera : MonoBehaviour
    {
        void Start()
        {
#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
            }
#endif
        }
    }
}