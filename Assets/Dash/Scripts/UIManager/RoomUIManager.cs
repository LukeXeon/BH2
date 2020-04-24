using System;
using System.Collections.Generic;
using System.Linq;
using agora_gaming_rtc;
using Dash.Scripts.Cloud;
using Dash.Scripts.Levels.Config;
using Dash.Scripts.Levels.Core;
using Dash.Scripts.UI;
using Dash.Scripts.UIManager.ItemUIManager;
using ExitGames.Client.Photon;
using Michsky.UI.ModernUIPack;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager
{
    public class RoomUIManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        private readonly Dictionary<int, PlayerInRoomItemUIManager> noLocalPlayerItems =
            new Dictionary<int, PlayerInRoomItemUIManager>(3);

        public Animator animator;
        public Button back;
        public Button idBtn;
        [Header("MainUI")] public TextMeshProUGUI idText;

        private bool isOpen;
        public Animator loadingMask;
        [Header("LoadingUI")] public Image loadingRoot;
        public LevelLoadManager loadManager;
        public NotificationManager onError;
        public NotificationManager onSuccess;
        public SwitchManager openMaiKeFeng;
        public SwitchManager openYuYing;
        public PlayerInRoomItemUIManager[] playerItems;
        public Image progress;
        public Button ready;

        [Header("Asset")] public Color readyColor;
        public TextMeshProUGUI readyText;
        public Button start;
        public Color unReadyColor;

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == LevelLoadManager.OnStartLoad) ClearRoomPlayers();
        }

        private void Awake()
        {
            idBtn.onClick.AddListener(() =>
            {
                GUIUtility.systemCopyBuffer = idText.text;
                onSuccess.Show("已复制房间ID", "邀请好友加入游戏吧");
            });
            back.onClick.AddListener(() =>
            {
                var index = Array.FindIndex(PhotonNetwork.PlayerList,
                    p => p.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);
                var table2 = new Hashtable
                {
                    [index + "playerTypeId"] = CloudManager.GetCurrentPlayer().typeId
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(table2);
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
                        image.color = readyColor;
                    else
                        image.color = unReadyColor;
                }
            });
            start.onClick.AddListener(() =>
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    PhotonNetwork.CurrentRoom.IsVisible = false;
                    loadManager.LoadRoomLevel();
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
            isOpen = true;
            ClearRoomPlayers();
            InstallUIMasterOrClient();
            var musicPlayer = FindObjectOfType<BackgroundMusicPlayer>();
            musicPlayer.PlayRoom();
            animator.Play("Fade-in");
            var room = PhotonNetwork.CurrentRoom;
            idText.text = room.Name;
            var rtcEngine = IRtcEngine.QueryEngine();
            rtcEngine.JoinChannel(room.Name, "", 0u);
            var table = new Hashtable
            {
                ["displayName"] = CloudManager.GetNameInGame(),
                ["playerTypeId"] = LocalPlayerInfo.playerInfo.Item1.typeId,
                ["weaponTypeId"] = LocalPlayerInfo.weaponInfos.First().Item1.typeId,
                ["isReady"] = PhotonNetwork.IsMasterClient
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(table);
            var index = Array.FindIndex(PhotonNetwork.PlayerList,
                p => p.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);
            Debug.Log(index + "playerTypeId");
            table = new Hashtable
            {
                [index + "playerTypeId"] = LocalPlayerInfo.playerInfo.Item1.typeId
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(table);
        }

        public override void OnLeftRoom()
        {
            LocalPlayerInfo.Clear();
            var rtcEngine = IRtcEngine.QueryEngine();
            rtcEngine?.LeaveChannel();
            if (isOpen)
            {
                animator.Play("Fade-out");
                ClearRoomPlayers();
                FindObjectOfType<BackgroundMusicPlayer>()?.Back();
                isOpen = false;
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            CheckCanStartIfMaster();
            if (noLocalPlayerItems.Count != PhotonNetwork.PlayerList.Length - 1)
                foreach (var player in PhotonNetwork.PlayerList)
                    ApplyRoomPlayer(player);
            else
                ApplyRoomPlayer(targetPlayer);
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
            if (PhotonNetwork.PlayerList.Length == 2)
            {
                var table = new Hashtable
                {
                    ["2playerTypeId"] = -1
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(table);
            }
            else if (PhotonNetwork.PlayerList.Length == 1)
            {
                var table = new Hashtable
                {
                    ["1playerTypeId"] = -1,
                    ["2playerTypeId"] = -1
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(table);
            }
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
                    if (!player.IsMasterClient)
                    {
                        player.CustomProperties.TryGetValue("isReady", out var value);
                        if (value == null || !(bool) value)
                        {
                            canStart = false;
                            break;
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
            foreach (var playerInRoomItemUiManager in playerItems) playerInRoomItemUiManager.Clear();

            noLocalPlayerItems.Clear();
        }

        private void ApplyRoomPlayer(Player player)
        {
            if (player.IsLocal)
            {
                var local = playerItems[1];
                local.Apply(player);
                return;
            }

            noLocalPlayerItems.TryGetValue(player.ActorNumber, out var already);
            if (already != null)
            {
                already.Apply(player);
                return;
            }

            var left = playerItems[0];
            var right = playerItems[2];
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