using Spine.Unity;
using UnityEngine;

namespace Dash.Scripts.Config
{
    [CreateAssetMenu(fileName = "Player", menuName = "Info/Player")]
    public class PlayerInfoAsset : ScriptableObject
    {
        public string displayName;
        public int fangYuLi;
        public int fangYuLi2;
        [Header("人物基础值")] public int gongJiLi;
        [Header("人物成长值")] public int gongJiLi2;
        public Sprite icon;
        public TextAsset jieShao;
        public int nengLiangZhi;
        public int nengLiangZhi2;
        public int shengMingZhi;
        public int shengMingZhi2;
        public SkeletonDataAsset skel;
        public int typeId;
    }
}