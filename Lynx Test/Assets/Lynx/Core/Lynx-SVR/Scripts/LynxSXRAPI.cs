/**
* @file LynxSXRAPI.cs
* 
* @author Geoffrey Marhuenda
* 
* @brief Bridge Lynx functionalities from SXR plugin.
*/
using System;
using System.Runtime.InteropServices;

namespace lynx
{
    public class LynxSXRAPI
    {
        private const string SVR_NAME_DLL = "svrplugin";

        /// <summary>
        /// Enable or disable predicted time. Disabling predicted time will stop image and 3D jitter but will add a latency.
        /// </summary>
        /// <param name="enable">True, less latency but image jitter. False, stop jitter but high latency.</param>
        [DllImport(SVR_NAME_DLL)]
        public extern static void EnablePredictedTime(bool enable);
        
        /// <summary>
        /// Get current predicted time status.
        /// </summary>
        /// <returns>True, predicted time is enabled. False, predicted time is disabled.</returns>
        [DllImport(SVR_NAME_DLL)]
        public extern static bool GetPredictedTimeStatus();

        /// <summary>
        /// Inverse current status for predicted time.
        /// </summary>
        public static void TogglePredictedTime()
        {
            LynxSXRAPI.EnablePredictedTime(!LynxSXRAPI.GetPredictedTimeStatus());
        }

        /// <summary>
        /// Get a pointer to a buffer that represents the current image (camera and 3D) in sxrAPI. 
        /// The image is in R8G8B8A8 format.  
        /// </summary>
        /// <param ="data"> a ref to an IntPtr to the image buffer</param>
        [DllImport(SVR_NAME_DLL)]
        public static extern void GetCameraAnd3DImage(ref IntPtr data);

        /// <summary>
        /// Enable or disable the copy of image (camera and 3D) in a public buffer in sxrAPI. 
        /// </summary>
        /// <param name="value">True, to enable. False, to disable</param>
        [DllImport(SVR_NAME_DLL)]
        public static extern void EnableCopyCameraAnd3DImage(bool value);

        /// <summary>
        /// Get the fact that we are in copy mode or not
        /// </summary>
        [DllImport(SVR_NAME_DLL)]
        public static extern bool IsCopyCameraAnd3DImageEnabled(); 
    }
}