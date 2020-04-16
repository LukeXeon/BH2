using System.Collections.Generic;
using System.Linq;
using Dash.Scripts.Config;
using Dash.Scripts.Cloud;
using Dash.Scripts.GamePlay.Info;
using Dash.Scripts.UI;
using Dash.Scripts.UIManager.ItemUIManager;
using Michsky.UI.ModernUIPack;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Dash.Scripts.UIManager
{
    public class DuiWuListUIManager : MonoBehaviourPunCallbacks
    {
        private const int MAX_DISPLAY_COUNT = 30;

        [Header("UI")] public Transform roomsRoot;
        public Transform typesRoot;
        public TMP_InputField roomId;
        public Button join;
        public Button randomJoin;
        public Button createRoom;
        public Button back;
        public Animator animator;
        public Animator loadingMask;
        public NotificationManager notifySucceed;
        public NotificationManager notifyError;
        public RoomUIManager roomUiManager;

        [Header("Assets")] public GameObject roomItem;
        public GameObject typeItem;
        private int currentRoomTypeId = -1;
        private bool isOpen;

        private void Awake()
        {
            var newItem = Instantiate(typeItem, typesRoot);
            var roomType = newItem.GetComponent<RoomTypeItemUIManager>();
            roomType.Apply("全部队伍", () => { });
            foreach (var guanQiaInfoAsset in GameGlobalInfoManager.guanQiaInfoTable)
            {
                newItem = Instantiate(typeItem, typesRoot);
                roomType = newItem.GetComponent<RoomTypeItemUIManager>();
                roomType.Apply(guanQiaInfoAsset.Value.displayName, () => { });
            }

            back.onClick.AddListener(() =>
            {
                isOpen = false;
                if (PhotonNetwork.InLobby)
                {
                    PhotonNetwork.LeaveLobby();
                }

                animator.Play("Fade-out");
            });
            randomJoin.onClick.AddListener(() =>
            {
                BeginWaitNetwork();
                CloudManager.GetCompletePlayer((player, s) =>
                {
                    if (s != null)
                    {
                        EndWaitNetWork();
                        notifyError.Show("匹配失败", "拉取玩家信息失败");
                    }
                    else
                    {
                        PhotonNetwork.JoinRandomRoom();
                        GameplayInfoManager.Prepare(player);
                    }
                });
            });
            createRoom.onClick.AddListener(() =>
            {
                BeginWaitNetwork();
                CloudManager.GetCompletePlayer((player, s) =>
                {
                    if (s != null)
                    {
                        EndWaitNetWork();
                        notifyError.Show("创建房间失败", "拉取玩家信息失败");
                    }
                    else
                    {
                        var table = new Hashtable
                        {
                            ["displayName"] = CloudManager.GetNameInGame() + "的房间",
                            ["playerIconIds"] = new[] {CloudManager.GetCurrentPlayer().typeId},
                            ["typeId"] = 0
                        };
                        PhotonNetwork.CreateRoom(null, new RoomOptions
                        {
                            MaxPlayers = 3,
                            CustomRoomProperties = table,
                        });
                        GameplayInfoManager.Prepare(player);
                    }
                });
            });
        }

        public void Open()
        {
            isOpen = true;
            animator.Play("Fade-in");
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            ClearRooms();
            var rooms = roomList.GetRange(0, Mathf.Min(MAX_DISPLAY_COUNT, roomList.Count))
                .Where(info => info.IsOpen && info.IsVisible && !info.RemovedFromList).ToList();
            BuildRooms(rooms);
        }

        private void BuildRooms(List<RoomInfo> rooms)
        {
            foreach (var info in rooms)
            {
                var go = Instantiate(roomItem, roomsRoot);
                var room = go.GetComponent<RoomItemUIManager>();
                info.CustomProperties.TryGetValue("typeId", out var typeId);
                info.CustomProperties.TryGetValue("playerIconIds", out var playerIconIds);
                info.CustomProperties.TryGetValue("displayName", out var displayName);
                var id = info.Name;
                room.Apply(displayName as string ?? "", typeId as int? ?? 0, playerIconIds as int[] ?? new int[0], () =>
                {
                    BeginWaitNetwork();
                    CloudManager.GetCompletePlayer((player, s) =>
                    {
                        if (s != null)
                        {
                            EndWaitNetWork();
                            notifyError.Show("加入房间失败", "拉取玩家数据异常");
                        }
                        else
                        {
                            PhotonNetwork.JoinRoom(id);
                            GameplayInfoManager.Prepare(player);
                        }
                    });
                });
            }
        }

        public override void OnJoinedRoom()
        {
            EndWaitNetWork();
            animator.Play("Fade-out");
            notifySucceed.Show("成功", "已进入房间");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            EndWaitNetWork();
            Debug.Log(message);
            notifyError.Show("失败", "加入房间失败");
        }

        private void ClearRooms()
        {
            foreach (Transform room in roomsRoot)
            {
                Destroy(room.gameObject);
            }
        }

        public override void OnLeftLobby()
        {
            ClearRooms();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            EndWaitNetWork();
            notifyError.Show("匹配失败", "暂时没有可用的队伍");
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