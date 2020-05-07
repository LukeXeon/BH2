using Dash.Scripts.Setting;
using Michsky.UI.ModernUIPack;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager
{
    public class GuanQiaUIManager : MonoBehaviour
    {
        public Animator animator;

        private BeforeJoinRoomAction beforeJoinRoomAction;

        private int currentId;
        public Button duiWu;
        public DuiWuListUIManager duiWuList;
        public Image image;
        public Button left;
        public Animator loadingMask;
        public NotificationManager onError;
        public Button piPei;
        public Button right;
        public TextMeshProUGUI textContent;
        public TextMeshProUGUI title;

        private void Awake()
        {
            beforeJoinRoomAction = new BeforeJoinRoomAction(loadingMask, onError);
            piPei.onClick.AddListener(async () =>
            {
                await beforeJoinRoomAction.DoPrepare();
                PhotonNetwork.JoinRandomRoom();
            });
            duiWu.onClick.AddListener(() => { duiWuList.Open(); });
        }

        public void Open()
        {
            animator.Play("Fade-in");
            Load(0);
        }

        public void Close()
        {
            FindObjectOfType<BackgroundMusicPlayer>().PlayDesktop();
            animator.Play("Fade-out");
        }

        private void Load(int id)
        {
            currentId = id;
            var info = GameSettingManager.LevelsInfoTable[id];
            var player = FindObjectOfType<BackgroundMusicPlayer>();
            player.Play(info.music);
            image.sprite = info.image;
            animator.Play("Load-in");
            title.text = info.displayName;
            if (info.miaoShu)
                textContent.text = info.miaoShu.text;
            else
                textContent.text = "";
        }

        private void BeginWaitNetwork()
        {
            loadingMask.gameObject.SetActive(true);
            loadingMask.Play("Fade-in");
        }

        private void EndWaitNetWork()
        {
            loadingMask.gameObject.SetActive(false);
        }
    }
}