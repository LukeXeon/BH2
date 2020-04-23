using System;
using Dash.Scripts.Levels.View;
using UnityEngine;


namespace Dash.Scripts.Config
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

        [Header("每秒发射子弹数量")]
        public float sheShu;

        public WeaponView weaponView;

        //替换列表
        public SlotItem[] slots;

        public WeaponTypeInfoAsset weaponType;

        [Header("基础值")] public int gongJiLi;

        public int sheSu;
        [Header("成长值")] public int gongJiLi2;
        public int sheSu2;

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