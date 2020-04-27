using UnityEngine;

namespace Dash.Scripts.GamePlay.Levels
{
    public class LevelDoorManager : MonoBehaviour
    {
        public Animator animator;
        public AudioSource audioSource;

        public void Open()
        {
            audioSource.time = 0;
            audioSource.Play();
            animator.Play("open");
        }

        public void Close()
        {
            audioSource.time = 0;
            audioSource.Play();
            animator.Play("close");
        }
    }
}
