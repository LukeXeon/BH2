using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager
{
    public class ShoppingUIManager : MonoBehaviour
    {
        public LiveCharacter character;
        public Button back;
        public Button characterBtn;
        public GameObject root;
        public Animator animator;

        private void Awake()
        {
            back.onClick.AddListener(() => { animator.Play("Fade-out"); });
            characterBtn.onClick.AddListener(() => { character.SetRandomLiveMotionAndExpression(); });
        }

        public void Open()
        {
            animator.Play("Fade-in");
        }
    }
}