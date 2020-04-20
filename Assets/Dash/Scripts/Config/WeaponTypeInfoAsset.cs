using System;
using UnityEngine;

namespace Dash.Scripts.Config
{
    [CreateAssetMenu(fileName = "WeaponType", menuName = "Info/WeaponType")]
    [Serializable]
    public class WeaponTypeInfoAsset : ScriptableObject
    {
        public string matchName;
        public Vector2 offset = new Vector2(0.5f, 0.5f);
        public bool canLianShe;
    }
}