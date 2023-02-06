/**
 * @file LynxFeatureSet.cs
 * 
 * @author Geoffrey Marhuenda
 * 
 * @brief Create a Feature group for Lynx headset (available in Project Settigns under OpenXR tab).
 */

#if LYNX_OPENXR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine;
using UnityEngine.XR.OpenXR;
#endif


namespace lynx.openxr
{
#if LYNX_OPENXR
    [OpenXRFeatureSet(
        SupportedBuildTargets = new BuildTargetGroup[] { BuildTargetGroup.Android },
        RequiredFeatureIds = new string[]
        {
            LynxR1Feature.featureId
        },
        FeatureIds = new string[]
        {
            LynxR1Feature.featureId,
            ultraleapFeatureId
        },
        UiName = "Lynx",
        Description = "Feature set that allows for setting up the best environment for My Company's hardware.",
        FeatureSetId = featureId
    )]
#endif

    public class LynxFeatureSet
    {
        public const string featureId = "com.lynx.openxr.featureset";
        public const string ultraleapFeatureId = "com.ultraleap.tracking.openxr.feature.handtracking";
    }
}
