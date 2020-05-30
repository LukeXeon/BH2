using System;
using System.Linq;
using Dash.Scripts.Core;
using Dash.Scripts.Setting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Dash.Scripts.Editor
{
    [InitializeOnLoad]
    public static class ResManager
    {
        static ResManager()
        {
            void OnInitialHierarchyChanged()
            {
                EditorApplication.hierarchyChanged -= OnInitialHierarchyChanged;
                UpdateIndexersList();
                UpdateInfoSettings();
            }

            EditorApplication.hierarchyChanged += OnInitialHierarchyChanged;
            EditorApplication.projectChanged += UpdateIndexersList;
            EditorApplication.projectChanged += UpdateInfoSettings;
        }

        [DidReloadScripts]
        internal static void UpdateIndexersList()
        {
            var asset = Resources.Load<GlobalSettingAsset>("GlobalSetting");
            var indexers = AssetDatabase.FindAssets("t:prefab")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<GuidIndexer>)
                .Where(o => o != null)
                .ToArray();
            Debug.Log(indexers.Length);
            foreach (var view in indexers)
            {
                var id = Array.IndexOf(indexers, view).ToString();
                if (view.guid != id)
                {
                    view.guid = id;
                    EditorUtility.SetDirty(view);
                }
            }

            asset.networkIndexers = indexers;
            EditorUtility.SetDirty(asset);
        }

        private static void UpdateInfoSettings()
        {
            var asset = Resources.Load<GlobalSettingAsset>("GlobalSetting");
            asset.playerInfoAssets = LoadRes<PlayerInfoAsset>();
            asset.weaponInfoAssets = LoadRes<WeaponInfoAsset>();
            asset.weaponTypeInfoAssets = asset.weaponInfoAssets.Select(i => i.weaponType)
                .Distinct()
                .Where(i => i)
                .ToArray();
            asset.levelInfoAssets = LoadRes<LevelInfoAsset>();
            asset.sealInfoAssets = LoadRes<SealInfoAsset>();
            asset.storeItemInfoAssets = LoadRes<StoreItemInfoAsset>();
            EditorUtility.SetDirty(asset);
        }

        private static T[] LoadRes<T>() where T : ScriptableObject
        {
            return AssetDatabase.FindAssets($"t:{typeof(T).FullName}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<T>)
                .ToArray();
        }
    }
}