using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

namespace LovelyBytes.CommonTools.FiniteStateMachine
{
    internal static class EditorUtils 
    {
        public static string GetRootDirectory(Object asset)
        {
            string rootDirectory = AssetDatabase.GetAssetPath(asset);
            rootDirectory = rootDirectory.Remove(rootDirectory.LastIndexOf('/'));

            return rootDirectory;
        }

        public static void CreateAsset(Object asset, Object parentAsset, string directory)
        {
            directory = $"{GetRootDirectory(parentAsset)}/{directory}";
            CreateAsset(asset, directory);
        }
        
        public static void CreateAsset(Object asset, string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string path = $"{directory}/{asset.name}";
            string uniquePath = path;
            
            int copyCount = 1;
            while (AssetDatabase.LoadAssetAtPath<Object>($"{uniquePath}.asset"))
                uniquePath = $"{path} {copyCount++}";
            
            AssetDatabase.CreateAsset(asset, $"{uniquePath}.asset");
        }
        
        public static TAsset[] FindAssetsOfType<TAsset>() where TAsset : Object
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(TAsset).Name}");
            TAsset[] assets = new TAsset[guids.Length];
            
            for(int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                assets[i] = AssetDatabase.LoadAssetAtPath<TAsset>(path);
            }

            return assets;
        }
        
        public static void ForceEnterState(FsmState state)
        {
            FsmStateDisablerEditorGuard.FsmStateDisabler.Instance.Monitor(state);
            state.Enter();
        }
    }
}