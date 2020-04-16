using System.Collections;
using System.Collections.Generic;
using System.Linq;
using agora_gaming_rtc;
using Dash.Scripts.Config;
using Dash.Scripts.Cloud;
using Dash.Scripts.UI;
using Dash.Scripts.UIManager.ItemUIManager;
using Michsky.UI.ModernUIPack;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Dash.Scripts.UIManager
{
    public class RoomUIManager : MonoBehaviourPunCallbacks
    {
        [Header("MainUI")] public TextMeshProUGUI idText;
        public PlayerInRoomItemUIManager[] playerItems;
        public Button back;
        public Button start;
        public Button ready;
        public TextMeshProUGUI readyText;
        public SwitchManager openYuYing;
        public SwitchManager openMaiKeFeng;
        public Animator animator;
        public Button idBtn;
        public NotificationManager onError;
        public NotificationManager onSuccess;
        public Animator loadingMask;
        [Header("LoadingUI")] public Image loadingRoot;
        public Image progress;

        [Header("Asset")] public Color readyColor;
        public Color unReadyColor;


        private readonly HashSet<int> loadedPlayers = new HashSet<int>();

        private readonly Dictionary<int, PlayerInRoomItemUIManager> noLocalPlayerItems =
            new Dictionary<int, PlayerInRoomItemUIManager>(3);

        private bool isOpen;

        private void Awake()
        {
            idBtn.onClick.AddListener(() =>
            {
                GUIUtility.systemCopyBuffer = idText.text;
                onSuccess.Show("已复制房间ID", "邀请好友加入游戏吧");
            });
            back.onClick.AddListener(() =>
            {
                PhotonNetwork.LeaveRoom();
            });
            ready.onClick.AddListener(() =>
            {
                if (!PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("isReady", out var value);
                    var isReady = !(value as bool? ?? false);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
                    {
                        ["isReady"] = isReady
                    });
                    playerItems[1].Apply(PhotonNetwork.LocalPlayer);
                    var image = (Image) ready.targetGraphic;
                    if (isReady)
                    {
                        image.color = readyColor;
                    }
                    else
                    {
                        image.color = unReadyColor;
                    }
                }
            });
            start.onClick.AddListener(() =>
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    PhotonNetwork.CurrentRoom.IsVisible = false;
                    photonView.RPC(nameof(BeginLoadScene), RpcTarget.All);
                }
            });
            var rtcEngine = IRtcEngine.QueryEngine();
            rtcEngine.OnJoinChannelSuccess += OnJoinChannelSuccess;
            rtcEngine.OnError += OnError;
            rtcEngine.DisableAudio();
            rtcEngine.MuteLocalAudioStream(true);
            openYuYing.OnEvents.AddListener(() => { IRtcEngine.QueryEngine().EnableAudio(); });
            openYuYing.OffEvents.AddListener(() => { IRtcEngine.QueryEngine().DisableAudio(); });
            openMaiKeFeng.OnEvents.AddListener(() => { IRtcEngine.QueryEngine().MuteLocalAudioStream(false); });
            openMaiKeFeng.OffEvents.AddListener(() => { IRtcEngine.QueryEngine().MuteLocalAudioStream(true); });
            openYuYing.isOn = false;
            openMaiKeFeng.isOn = false;
        }

        private void OnError(int error, string msg)
        {
            Debug.Log(msg);
        }

        private void OnJoinChannelSuccess(string channelname, uint uid, int elapsed)
        {
            Debug.Log(channelname);
        }

        private void OnDestroy()
        {
            var rtcEngine = IRtcEngine.QueryEngine();
            if (rtcEngine != null)
            {
                rtcEngine.OnJoinChannelSuccess -= OnJoinChannelSuccess;
                rtcEngine.OnError -= OnError;
            }
        }

        public override void OnJoinedRoom()
        {
            var player = FindObjectOfType<BackgroundMusicPlayer>();
            player.PlayRoom();
            animator.Play("Fade-in");
            var room = PhotonNetwork.CurrentRoom;
            idText.text = room.Name;
            var rtcEngine = IRtcEngine.QueryEngine();
            rtcEngine.JoinChannel(room.Name, "", 0u);
            var table = new Hashtable
            {
                ["displayName"] = CloudManager.GetNameInGame(),
                ["isReady"] = PhotonNetwork.IsMasterClient
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(table);
            InstallUIMasterOrClient();
            ClearRoomPlayers();
            InitRoomPlayers();
        }

        public override void OnLeftRoom()
        {
            animator.Play("Fade-out");
            var rtcEngine = IRtcEngine.QueryEngine();
            rtcEngine?.LeaveChannel();
            ClearRoomPlayers();
            FindObjectOfType<BackgroundMusicPlayer>()?.Back();
        }
        
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            CheckCanStartIfMaster();
            if (targetPlayer.IsLocal)
            {
                var local = playerItems[1];
                local.Apply(targetPlayer);
            }
            else
            {
                noLocalPlayerItems[targetPlayer.ActorNumber].Apply(targetPlayer);
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            CheckCanStartIfMaster();
            var left = playerItems[0];
            var right = playerItems[2];
            if (!noLocalPlayerItems.ContainsValue(left))
            {
                noLocalPlayerItems.Add(newPlayer.ActorNumber, left);
                left.Apply(newPlayer);
            }
            else
            {
                noLocalPlayerItems.Add(newPlayer.ActorNumber, right);
                right.Apply(newPlayer);
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            CheckCanStartIfMaster();
            var item = noLocalPlayerItems[otherPlayer.ActorNumber];
            noLocalPlayerItems.Remove(otherPlayer.ActorNumber);
            item.Clear();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            newMasterClient.SetCustomProperties(new Hashtable
            {
                ["isReady"] = true
            });
            InstallUIMasterOrClient();
            CheckCanStartIfMaster();
        }

        private void CheckCanStartIfMaster()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                var canStart = true;
                foreach (var player in PhotonNetwork.PlayerList)
                {
                    if (!player.IsMasterClient)
                    {
                        player.CustomProperties.TryGetValue("isReady", out var value);
                        if (value == null || !(bool) value)
                        {
                            canStart = false;
                            break;
                        }
                    }
                }

                var img = (Image) start.targetGraphic;
                if (canStart)
                {
                    start.enabled = true;
                    img.color = readyColor;
                }
                else
                {
                    start.enabled = false;
                    img.color = unReadyColor;
                }
            }
        }

        private void InstallUIMasterOrClient()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                ready.gameObject.SetActive(false);
                start.gameObject.SetActive(true);
            }
            else
            {
                ready.gameObject.SetActive(true);
                start.gameObject.SetActive(false);
            }
        }

        private void ClearRoomPlayers()
        {
            foreach (var playerInRoomItemUiManager in playerItems)
            {
                playerInRoomItemUiManager.Clear();
            }

            noLocalPlayerItems.Clear();
        }

        private void InitRoomPlayers()
        {
            var local = playerItems[1];
            local.Apply(PhotonNetwork.LocalPlayer);
            var left = playerItems[0];
            var right = playerItems[2];
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (!player.IsLocal)
                {
                    if (!noLocalPlayerItems.ContainsValue(left))
                    {
                        noLocalPlayerItems.Add(player.ActorNumber, left);
                        left.Apply(player);
                    }
                    else
                    {
                        noLocalPlayerItems.Add(player.ActorNumber, right);
                        right.Apply(player);
                    }
                }
            }
        }

        [PunRPC]
        public void SceneLoaded(int id)
        {
            loadedPlayers.Add(id);
        }

        [PunRPC]
        public void BeginLoadScene()
        {
            StartCoroutine(LoadScene());
        }

        private IEnumerator LoadScene()
        {
            loadedPlayers.Clear();
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("typeId", out var typeId);
            if (!(typeId is int))
            {
                yield break;
            }

            var scene = GameGlobalInfoManager.guanQiaInfoTable[(int) typeId];
            loadingRoot.gameObject.SetActive(true);
            loadingRoot.sprite = scene.image;
            var op = SceneManager.LoadSceneAsync(scene.sceneName);
            op.allowSceneActivation = false;
            while (op.progress < 0.9f)
            {
                progress.fillAmount = op.progress;
                yield return null;
            }

            progress.fillAmount = 1;
            photonView.RPC(nameof(SceneLoaded), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
            yield return new WaitUntil(() =>
            {
                return PhotonNetwork.PlayerList.All(i => loadedPlayers.Contains(i.ActorNumber));
            });
            op.allowSceneActivation = true;
        }
    }
}