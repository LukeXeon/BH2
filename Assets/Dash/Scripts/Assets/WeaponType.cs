using System;
using System.Collections.Generic;
using System.Linq;
using LeanCloud;
using UnityEngine;

namespace Dash.Scripts.Assets
{
    public enum WeaponType
    {
        NoWeapon,
        TuJiBuQiang,
        JiaTeLin,
        QingShouQiang,
        ZhongShouQiang,
    }

    public static class WeaponTypeEx
    {
        public static readonly Dictionary<WeaponType, string> weaponNames;
        public static readonly Dictionary<WeaponType, Vector2?> offsets;

        static WeaponTypeEx()
        {
            int index = 0;
            weaponNames = new Dictionary<WeaponType, string>();
            foreach (var value in Enum.GetValues(typeof(WeaponType)))
            {
                var name = value.ToString().ToLower();
                weaponNames[(WeaponType) value] = name;
            }

            weaponNames[WeaponType.NoWeapon] = WeaponType.QingShouQiang.ToString().ToLower();
            offsets = Resources.LoadAll<WeaponDisplayFixAsset>("Config/Game").Single().items
                .ToDictionary(o => o.type, o => new Vector2?(o.offset));
        }
    }
}