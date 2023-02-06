/**
 * @file LynxAPI.cs
 * 
 * @author Geoffrey Marhuenda
 * 
 * @brief Static API to ease use of Lynx features.
 */

using lynx.openxr;

namespace lynx
{
    public class LynxAPI
    {
        private LynxAPI() { }


        /// <summary>
        /// Retrieve current Video See Through status.
        /// </summary>
        /// <returns>True: the current mode is VR. False: the current mode is AR. </returns>
        public static bool IsVR()
        {
            return LynxR1Feature.Instance.IsVR();
        }

        /// <summary>
        /// Retrieve current Video See Through status.
        /// </summary>
        /// <returns>True: the current mode is AR. False: the current mode is VR. </returns>
        public static bool IsAR()
        {
            return LynxR1Feature.Instance.IsAR();
        }

        /// <summary>
        /// Set video see through mode.
        /// </summary>
        /// <param name="isAR">True (default) to set video see through mode. False to set VR mode.</param>
        public static void SetAR(bool isAR = true)
        {
            LynxR1Feature.Instance.SetAR(isAR);
        }

        /// <summary>
        /// Set VR mode.
        /// </summary>
        /// <param name="isVR">True (default) to set VR mode. False to set video see through mode.</param>
        public static void SetVR(bool isVR = true)
        {
            LynxR1Feature.Instance.SetVR(isVR);
        }

        /// <summary>
        /// Switch between AR and VR modes.
        /// </summary>
        public static void ToogleAR()
        {
            LynxR1Feature.Instance.SetAR(!LynxR1Feature.Instance.IsAR());
        }

    }
}