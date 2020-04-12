using System.Collections.Generic;
using Dash.Scripts.Config;
using Dash.Scripts.Network.Cloud;
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
        private Dictionary<string, RoomInfo> cachedRoomList;

        private void Awake()
        {
            cachedRoomList = new Dictionary<string, RoomInfo>();
            var newItem = Instantiate(typeItem, typesRoot);
            var roomType = newItem.GetComponent<RoomTypeItemUIManager>();
            roomType.Apply("全部队伍", () => { });
            foreach (var guanQiaInfoAsset in GameInfoManager.guanQiaInfoTable)
            {
                newItem = Instantiate(typeItem, typesRoot);
                roomType = newItem.GetComponent<RoomTypeItemUIManager>();
                roomType.Apply(guanQiaInfoAsset.Value.displayName, () => { });
            }

            back.onClick.AddListener(() =>
            {
                if (PhotonNetwork.InLobby)
                {
                    PhotonNetwork.LeaveLobby();
                }

                animator.Play("Fade-out");
            });
            randomJoin.onClick.AddListener(() =>
            {
                BeginWaitNetwork();
                PhotonNetwork.JoinRandomRoom();
            });
            createRoom.onClick.AddListener(() =>
            {
                BeginWaitNetwork();
                var table = new Hashtable
                {
                    ["displayName"] = CloudManager.GetNameInGame() + "的房间",
                    ["playerIconIds"] = new[] {CloudManager.GetCurrentPlayer().typeId},
                    ["typeId"] = 0,
                };
                PhotonNetwork.CreateRoom(null, new RoomOptions
                {
                    MaxPlayers = 3,
                    CustomRoomProperties = table,
                });
            });
        }


        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            EndWaitNetWork();
            notifyError.Show("匹配失败", "暂时没有可用的队伍");
        }

        public void Open()
        {
            animator.Play("Fade-in");
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            ClearRooms();
            var rooms = roomList.GetRange(0, Mathf.Min(MAX_DISPLAY_COUNT, roomList.Count));
            UpdateCachedRoomList(rooms);
            BuildRooms();
        }

        private void BuildRooms()
        {
            foreach (var info in cachedRoomList.Values)
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
                    PhotonNetwork.JoinRoom(id);
                });
            }
        }

        public override void OnJoinedRoom()
        {
            EndWaitNetWork();
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
            cachedRoomList.Clear();
            ClearRooms();
        }

        
        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            foreach (RoomInfo info in roomList)
            {
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                {
                    if (cachedRoomList.ContainsKey(info.Name))
                    {
                        cachedRoomList.Remove(info.Name);
                    }

                    continue;
                }

                // Update cached room info
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }
                // Add new room info to cache
                else
                {
                    cachedRoomList.Add(info.Name, info);
                }
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