using System;
using Dash.Scripts.GamePlay.View;
using UnityEditor;
using UnityEngine;

namespace Dash.Scripts.Config
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Info/Weapon")]
    [Serializable]
    public class WeaponInfoAsset : ScriptableObject
    {
        //显示名称
        public string displayName;

        [Header("基础值")] public int gongJiLi;
        [Header("成长值")] public int gongJiLi2;

        [Header("射程")] public float sheCheng;

        [Header("每秒发射子弹数量")] public float sheSu;

        //替换列表
        public SlotItem[] slots;

        public Sprite sprite;
        public int typeId;

        public WeaponTypeInfoAsset weaponType;

        public GameObject weaponView;

        public TextAsset xiaoGuo;

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