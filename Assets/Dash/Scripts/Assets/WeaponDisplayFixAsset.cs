using System;
using UnityEngine;

namespace Dash.Scripts.Assets
{
    [CreateAssetMenu(fileName = "WeaponDisplayFix", menuName = "Info/WeaponDisplayFix")]
    public class WeaponDisplayFixAsset : ScriptableObject
    {
        public Item[] items;

        [Serializable]
        public struct Item
        {
            public WeaponType type;
            public Vector2 offset;
        }
    }
}