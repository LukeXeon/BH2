using System;
using UnityEngine;

namespace Dash.Scripts.Setting
{
    [CreateAssetMenu(fileName = "WeaponType", menuName = "Info/WeaponType")]
    [Serializable]
    public class WeaponTypeInfoAsset : ScriptableObject
    {
        public bool canLianShe;
        public string matchName;
        public Vector2 offset;
    }
}