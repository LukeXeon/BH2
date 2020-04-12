using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dash.Scripts.Config
{
    [CreateAssetMenu(fileName = "Player", menuName = "Info/Player")]
    public class PlayerInfoAsset : ScriptableObject
    { 
        public int typeId;
        public Sprite icon;
        public SkeletonDataAsset skel;
        public string displayName;
        public TextAsset jieShao;
        [Header("人物基础值")] public int gongJiLi;
        public int fangYuLi;
        public int shengMingZhi;
        public int nengLiangZhi;
        [Header("人物成长值")] public int gongJiLi2;
        public int fangYuLi2;
        public int shengMingZhi2;
        public int nengLiangZhi2;
    }
}