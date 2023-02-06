/**
 * @file TestARVR.cs
 * 
 * @author Geoffrey Marhuenda
 * 
 * @brief Basic script to switch between AR and VR using a button.
 */

using lynx;
using UnityEngine;

namespace lynx
{
    public class TestARVR : MonoBehaviour
    {
        private const string ARText = "Switch to VR";
        private const string VRText = "Switch to AR";
        public void SwitchARVR(TMPro.TMP_Text txt)
        {
            if (txt.text.Equals(ARText))
            {
                txt.text = VRText;
                LynxAPI.SetVR();
            }
            else
            {
                txt.text = ARText;
                LynxAPI.SetAR();
            }
        }
    }
}