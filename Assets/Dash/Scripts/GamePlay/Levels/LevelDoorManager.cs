using UnityEngine;

namespace Dash.Scripts.GamePlay.Levels
{
    public class LevelDoorManager : MonoBehaviour
    {
        public Animator animator;

        public void Open()
        {
            animator.Play("open");
        }

        public void Close()
        {
            animator.Play("close");
        }
    }
}
