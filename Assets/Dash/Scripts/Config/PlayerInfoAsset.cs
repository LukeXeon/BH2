using Spine.Unity;
using UnityEngine;

namespace Dash.Scripts.Config
{
    [CreateAssetMenu(fileName = "Player", menuName = "Info/Player")]
    public class PlayerInfoAsset : ScriptableObject
    {
        [Header("用于显示")] public string displayName;
        public TextAsset jieShao;
        public SkeletonDataAsset skel;
        [Header("人物基础值")] public int gongJiLi;
        public int fangYuLi;
        public int nengLiangZhi;
        public int shengMingZhi;
        [Header("人物成长值")] public int gongJiLi2;
        public int fangYuLi2;
        public int nengLiangZhi2;
        public int shengMingZhi2;
        public Sprite icon;
        [Header("其他")] public int typeId;
    }
}