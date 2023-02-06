
//   ==============================================================================
//   | Lynx HandTracking Sample : LynxInterfaces (2022)                           |
//   | Author : GC                                                                |
//   |======================================                                      |
//   | InterfaceLookPlayer Script                                                 |
//   | Script to make an UI look to the Camera.                                   |
//   ==============================================================================

using UnityEngine;

public class InterfaceLookPlayer : MonoBehaviour
{
    void Update()
    {
        this.transform.rotation = Quaternion.LookRotation(this.transform.position - Camera.main.transform.position) ;
    }
}
