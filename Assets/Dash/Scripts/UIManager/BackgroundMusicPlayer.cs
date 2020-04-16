using UnityEngine;

namespace Dash.Scripts.UIManager
{
    public class BackgroundMusicPlayer : MonoBehaviour
    {
        public AudioSource source1;
        public AudioSource source2;
        public Animator animator;
        private int PROPERTY;
        [Header("Assets")] public AudioClip desktop;
        public AudioClip room;
        private int flag = 1;
        private AudioClip back;

        private void Awake()
        {
            PROPERTY = Animator.StringToHash("switch");
        }

        public void Play(AudioClip clip)
        {
            animator.SetTrigger(PROPERTY);
            if (flag == 0)
            {
                flag = 1;
                back = source2.clip;
                source1.clip = clip;
                source1.time = 0;
                source1.Play();
            }
            else
            {
                flag = 0;
                back = source1.clip;
                source2.clip = clip;
                source2.time = 0;
                source2.Play();
            }
        }

        public void Back()
        {
            Play(back);
        }

        public void PlayDesktop()
        {
            Play(desktop);
        }

        public void PlayRoom()
        {
            Play(room);
        }
    }
}