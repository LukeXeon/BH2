using Michsky.UI.ModernUIPack;
using TMPro;

namespace Dash.Scripts.UI
{
    public static class Notifications
    {
        public static void Show(this NotificationManager v, string text, string text2)
        {
            v.transform.Find("Content/Title").GetComponent<TextMeshProUGUI>().text = text;
            v.transform.Find("Content/Description").GetComponent<TextMeshProUGUI>().text = text2;
            v.OpenNotification();
        }
    }
}