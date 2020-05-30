using System;
using UnityEngine;

namespace Dash.Scripts.Setting
{
    [CreateAssetMenu(fileName = "StoreItem", menuName = "Info/StoreItem")]
    [Serializable]
    public class StoreItemInfoAsset : ScriptableObject
    {
        public int crystalCost;
        public ScriptableObject item;
        public IBagItem bagItem => (IBagItem) item;
    }
}