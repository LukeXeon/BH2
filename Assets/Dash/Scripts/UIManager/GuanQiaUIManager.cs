using Dash.Scripts.Cloud;
using Dash.Scripts.Config;
using Dash.Scripts.GamePlay.Info;
using Dash.Scripts.UI;
using Michsky.UI.ModernUIPack;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager
{
    public class GuanQiaUIManager : MonoBehaviour
    {
        public Button piPei;
        public Button duiWu;
        public TextMeshProUGUI title;
        public Button left;
        public Button right;
        public Animator animator;
        public Image image;
        public TextMeshProUGUI textContent;
        public DuiWuListUIManager duiWuList;
        public Animator loadingMask;
        public NotificationManager onError;

        private void Awake()
        {
            piPei.onClick.AddListener(() =>
            {
                BeginWaitNetwork();
                CloudManager.GetCompletePlayer((player, s) =>
                {
                    if (s != null)
                    {
                        EndWaitNetWork();
                        onError.Show("匹配失败", "拉取玩家信息失败");
                    }
                    else
                    {
                        PhotonNetwork.JoinRandomRoom();
                        GameplayInfoManager.Prepare(player);
                    }
                });
            });
            duiWu.onClick.AddListener(() => { duiWuList.Open(); });
        }

        private int currentId;

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
            var info = GameGlobalInfoManager.guanQiaInfoTable[id];
            var player = FindObjectOfType<BackgroundMusicPlayer>();
            player.Play(info.music);
            image.sprite = info.image;
            animator.Play("Load-in");
            title.text = info.displayName;
            if (info.miaoShu)
            {
                textContent.text = info.miaoShu.text;
            }
            else
            {
                textContent.text = "";
            }
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
