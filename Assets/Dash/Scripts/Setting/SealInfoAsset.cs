using UnityEngine;

namespace Dash.Scripts.Setting
{
    [CreateAssetMenu(fileName = "ShengHen", menuName = "Info/ShengHen")]
    public class SealInfoAsset : ScriptableObject
    {
        public string displayName;
        public MonoBehaviour effect;

        [Header("基础值")] public int fangYuLi;

        [Header("成长值")] public int fangYuLi2;

        public Sprite image;
        public int nengLiangZhi;
        public int nengLiangZhi2;
        public int shengMingZhi;
        public int shengMingZhi2;
        public int typeId;
        public TextAsset xiaoGuo;
    }
}