/**
 * @file LynxR1Feature.cs
 * 
 * @author Geoffrey Marhuenda
 * 
 * @brief Create an OpenXR Feature for Lynx R1.
 */

using UnityEngine.XR.OpenXR.Features;
using UnityEngine;
using UnityEngine.XR.OpenXR.NativeTypes;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif


namespace lynx.openxr
{
#if UNITY_EDITOR
    [OpenXRFeature(
        BuildTargetGroups = new[] { BuildTargetGroup.Android },
        CustomRuntimeLoaderBuildTargets = new[] { BuildTarget.Android },
        UiName = "Lynx-R1",
        Company = "Lynx",
        Desc = "OpenXR support for Lynx-R1 headset.",
        DocumentationLink = "",
        Version = "1.0.0",
        FeatureId = featureId
    )]
#endif
    public class LynxR1Feature : OpenXRFeature
    {
        public const string featureId = "com.lynx.openxr.feature";

        public static LynxR1Feature Instance { get; private set; } = null;

        [Tooltip("Check to start application in AR mode. Otherwise, the application starts in VR (can be change at runtime using LynxAPI).")]
        public bool startInAR = true;

        protected override void OnSessionCreate(ulong xrSession)
        {
            base.OnSessionCreate(xrSession);
            Instance = this;

            SetAR(startInAR);
        }

        /// <summary>
        /// Retrieve current Video See Through status.
        /// </summary>
        /// <returns>True: the current mode is VR. False: the current mode is AR. </returns>
        public bool IsVR()
        {
            return GetEnvironmentBlendMode() == UnityEngine.XR.OpenXR.NativeTypes.XrEnvironmentBlendMode.Opaque;
        }

        /// <summary>
        /// Retrieve current Video See Through status.
        /// </summary>
        /// <returns>True: the current mode is AR. False: the current mode is VR. </returns>
        public bool IsAR()
        {
            return !IsVR();
        }

        /// <summary>
        /// Set video see through mode.
        /// </summary>
        /// <param name="isAR">True (default) to set video see through mode. False to set VR mode.</param>
        public void SetAR(bool isAR = true)
        {
            SetEnvironmentBlendMode(isAR ? XrEnvironmentBlendMode.AlphaBlend : XrEnvironmentBlendMode.Opaque);
        }

        /// <summary>
        /// Set VR mode.
        /// </summary>
        /// <param name="isVR">True (default) to set VR mode. False to set video see through mode.</param>
        public void SetVR(bool isVR = true)
        {
            SetAR(!isVR);
        }
    }
}