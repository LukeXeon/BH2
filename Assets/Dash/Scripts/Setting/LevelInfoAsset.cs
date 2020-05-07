using UnityEngine;

namespace Dash.Scripts.Setting
{
    [CreateAssetMenu(fileName = "QuanQia", menuName = "Info/GuanQia")]
    public class LevelInfoAsset : ScriptableObject
    {
        public string displayName;
        public Sprite image;
        public TextAsset miaoShu;
        public AudioClip music;
        public string sceneName;
        public int typeId;
    }
}