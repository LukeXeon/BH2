using UnityEngine;

namespace Dash.Scripts.Config
{
    [CreateAssetMenu(fileName = "QuanQia", menuName = "Info/GuanQia")]
    public class GuanQiaInfoAsset : ScriptableObject
    {
        public string displayName;
        public Sprite image;
        public TextAsset miaoShu;
        public AudioClip music;
        public string sceneName;
        public int typeId;
    }
}