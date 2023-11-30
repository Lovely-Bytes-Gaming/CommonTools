
using UnityEditor;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    internal static class UIBuilderUtils 
    {
        public static string GetParentDirectory(string className)
        {
            string filePath = GetFilePath(className);
            int index = filePath.LastIndexOf('/');
            return filePath[..index];
        }
        
        private static string GetFilePath(string className)
        {
            string[] guids = AssetDatabase.FindAssets($"t:Script {className}");
            
            return guids is { Length: > 0 } 
                ? AssetDatabase.GUIDToAssetPath(guids[0]) 
                : null;
        }
    }
}