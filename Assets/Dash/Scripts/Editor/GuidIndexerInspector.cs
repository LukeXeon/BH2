using System;
using Dash.Scripts.Core;
using Dash.Scripts.Setting;
using UnityEditor;
using UnityEngine;

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
                var views = Resources.Load<GlobalSettingAsset>("GlobalSetting")?.networkIndexers;
                if (views == null || string.IsNullOrEmpty(view.guid) ||
                    (index = Array.IndexOf(views, view)) == -1)
                {
                    ResManager.UpdateIndexersList();
                    views = Resources.Load<GlobalSettingAsset>("GlobalSetting").networkIndexers;
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