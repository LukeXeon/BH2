using UnityEngine;

namespace Dash.Scripts.Config
{
    [CreateAssetMenu(fileName = "ShengHen", menuName = "Info/ShengHen")]
    public class SealInfoAsset : ScriptableObject, ITypedAsset
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

        public int TypeId
        {
            get => typeId;
            set => typeId = value;
        }
    }
}