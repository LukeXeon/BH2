using UnityEngine;

namespace Michsky.UI.ModernUIPack
{
    public class NotificationManager : MonoBehaviour
    {
        Animator notificationAnimator;

        void Start()
        {
            notificationAnimator = gameObject.GetComponent<Animator>();
        }

        public void OpenNotification()
        {
            notificationAnimator.Play("In");
        }

        public void CloseNotification()
        {
            notificationAnimator.Play("Out");
        }
    }
}