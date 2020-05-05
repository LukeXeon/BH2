using System;
using System.IO;
using System.Linq;
using Dash.Scripts.Config;
using Dash.Scripts.Core;
using UnityEditor;
using UnityEngine;

namespace Dash.Scripts.Editor
{
    [InitializeOnLoad]
    public static class GlobalIndexManager
    {
        static GlobalIndexManager()
        {
            void OnInitialHierarchyChanged()
            {
                EditorApplication.hierarchyChanged -= OnInitialHierarchyChanged;
                UpdatePrefabList();
            }

            EditorApplication.hierarchyChanged += OnInitialHierarchyChanged;
            EditorApplication.projectChanged += UpdatePrefabList;
        }

        internal static GlobalIndexAsset GetOrCreateInstance()
        {
            var asset = Resources.Load<GlobalIndexAsset>("GlobalIndexAsset");
            if (!asset)
            {
                asset = ScriptableObject.CreateInstance<GlobalIndexAsset>();
                var script = MonoScript.FromScriptableObject(asset);
                var parent = Path.GetDirectoryName(AssetDatabase.GetAssetPath(script));
                if (!AssetDatabase.IsValidFolder(parent + @"\Resources"))
                {
                    AssetDatabase.CreateFolder(parent, "Resources");
                }

                AssetDatabase.CreateAsset(asset, parent + @"\Resources\GlobalIndexAsset.asset");
            }

            return asset;
        }

        internal static void UpdatePrefabList()
        {
            var asset = GetOrCreateInstance();
            var indexers = AssetDatabase.FindAssets("t:prefab")
                .Select(o => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(o)))
                .Select(o => o.GetComponent<GuidIndexer>())
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

            asset.indexers = indexers;
            EditorUtility.SetDirty(asset);
        }
    }
}