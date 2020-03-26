using System;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEditor;
using UnityEngine;

namespace Dash.Scripts.Assets
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Info/Weapon")]
    [Serializable]
    public class WeaponInfoAsset : ScriptableObject
    {
        public int typeId;

        //显示名称
        public string displayName;

        public Sprite sprite;

        public TextAsset xiaoGuo;

        //替换列表
        public SlotItem[] slots;

        public WeaponType weaponType = WeaponType.NoWeapon;

        [Header("基础值")] public int gongJiLi;

        public int sheSu;
        [Header("成长值")] public int gongJiLi2;
        public int sheSu2;

        public MonoBehaviour effect;

        [Serializable]
        public struct SlotItem
        {
            public string name;
            public AttachmentItem[] attachments;
        }

        [Serializable]
        public struct AttachmentItem
        {
            public string name;
            public Sprite sprite;
        }

    }
}