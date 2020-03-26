using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager
{
    public class PlayerWayUIManager : MonoBehaviour
    {
        public Button piPei;

        private void Awake()
        {
            piPei.onClick.AddListener(() => { SceneManager.LoadScene("Gameplay"); });
        }
    }
}
