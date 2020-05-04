using UnityEditor;
using UnityEngine;

namespace Dash.Scripts.Core
{
    [DisallowMultipleComponent]
    public class GuidIndexer : MonoBehaviour
    {
        public string guid;

#if UNITY_EDITOR
        public void OnValidate()
        {
            hideFlags = 0;
            if (EditorUtility.IsPersistent(gameObject))
            {
                var path = AssetDatabase.GetAssetPath(gameObject);
                var id = AssetDatabase.AssetPathToGUID(path);
                guid = id;
            }
            else
            {
                var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    var id = AssetDatabase.AssetPathToGUID(path);
                    guid = id;
                    return;
                }

                guid = "Only support prefab";
            }
        }
#endif
    }
}