using UnityEngine;

namespace Dash.Scripts.UIManager
{
    public class ErrorPanelUIManager : MonoBehaviour
    {
        private void Update()
        {
            transform.SetAsLastSibling();
        }
    }
}