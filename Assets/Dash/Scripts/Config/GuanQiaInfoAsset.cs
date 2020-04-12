using UnityEngine;

namespace Dash.Scripts.Config
{
    [CreateAssetMenu(fileName = "QuanQia", menuName = "Info/GuanQia")]
    public class GuanQiaInfoAsset: ScriptableObject
    {
        public int typeId;
        public string displayName;
        public AudioClip music;
        public TextAsset miaoShu;
        public Sprite image;
    }
}