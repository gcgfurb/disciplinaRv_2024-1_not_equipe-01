#if UNITY_EDITOR

using Leap.Examples;
using UnityEditor;

namespace Samples.Ultraleap_Tracking._6._15._0.URP_Examples.Example_Assets.Scripts
{
    public static class URPSampleDependencyImporter
    {
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void CreateAssetWhenReady()
        {
            EditorApplication.delayCall += () =>
            {
                SampleDependencyImporter.FindAndImportSampleDependencies
                (
                    "com.ultraleap.tracking",
                    "URP Examples",
                    new string[] { "Shared Example Assets REQUIRED" }
                );
            };
        }
    }
}

#endif