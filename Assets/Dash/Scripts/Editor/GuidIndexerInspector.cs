using System;
using Dash.Scripts.Core;
using UnityEditor;

namespace Dash.Scripts.Editor
{
    [CustomEditor(typeof(GuidIndexer))]
    public class GuidIndexerInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var view = (GuidIndexer) target;
            if (EditorUtility.IsPersistent(view.gameObject))
            {
                int index = -1;
                var views = GlobalIndexManager.GetOrCreateInstance().indexers;
                if (views == null || string.IsNullOrEmpty(view.guid) ||
                    (index = Array.IndexOf(views, view)) == -1)
                {
                    GlobalIndexManager.UpdatePrefabList();
                    views = GlobalIndexManager.GetOrCreateInstance().indexers;
                }

                var id = (index == -1 ? Array.IndexOf(views, view) : index).ToString();
                if (view.guid != id)
                {
                    view.guid = id;
                    EditorUtility.SetDirty(view);
                }
            }
            else
            {
                view.guid = null;
            }

            base.OnInspectorGUI();
        }
    }
}