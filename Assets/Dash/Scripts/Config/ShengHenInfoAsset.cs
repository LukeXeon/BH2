using UnityEditor;
using UnityEngine;

namespace Dash.Scripts.Config
{
    [CreateAssetMenu(fileName = "ShengHen", menuName = "Info/ShengHen")]
    public class ShengHenInfoAsset : ScriptableObject
    {
        public int typeId;
        public string displayName;
        public Sprite image;
        public TextAsset xiaoGuo;
        public MonoBehaviour effect;
        
        [Header("基础值")]
        public int fangYuLi;
        public int shengMingZhi;
        public int nengLiangZhi;
        [Header("成长值")]
        public int fangYuLi2;
        public int shengMingZhi2;
        public int nengLiangZhi2;
    }
}