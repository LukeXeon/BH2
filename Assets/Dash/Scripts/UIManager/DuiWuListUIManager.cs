using System.Collections.Generic;
using System.Linq;
using Dash.Scripts.Config;
using Dash.Scripts.Cloud;
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
        private BeforeJoinRoomAction beforeJoinRoomAction;
#pragma warning disable 414
        private int currentRoomTypeId = -1;
#pragma warning restore 414
        private Dictionary<string, RoomItemUIManager> cacheRooms;

        private void Awake()
        {
            cacheRooms = new Dictionary<string, RoomItemUIManager>();
            var newItem = Instantiate(typeItem, typesRoot);
            var roomType = newItem.GetComponent<RoomTypeItemUIManager>();
            roomType.Apply("全部队伍", () => { });
            foreach (var guanQiaInfoAsset in GameConfigManager.guanQiaInfoTable)
            {
                newItem = Instantiate(typeItem, typesRoot);
                roomType = newItem.GetComponent<RoomTypeItemUIManager>();
                roomType.Apply(guanQiaInfoAsset.Value.displayName, () => { });
            }

            beforeJoinRoomAction = new BeforeJoinRoomAction(loadingMask, notifyError);

            back.onClick.AddListener(() =>
            { ;
                if (PhotonNetwork.InLobby)
                {
                    PhotonNetwork.LeaveLobby();
                }

                animator.Play("Fade-out");
            });
            randomJoin.onClick.AddListener(async () =>
            {
                await beforeJoinRoomAction.DoPrepare();
                PhotonNetwork.JoinRandomRoom();
            });
            createRoom.onClick.AddListener(async () =>
            {
                await beforeJoinRoomAction.DoPrepare();
                var table = new Hashtable
                {
                    ["displayName"] = CloudManager.GetNameInGame() + "的房间",
                    ["typeId"] = 0,
                    ["0playerTypeId"] = CloudManager.GetCurrentPlayer().typeId
                };
                PhotonNetwork.CreateRoom(null, new RoomOptions
                {
                    MaxPlayers = 3,
                    CustomRoomProperties = table,
                    CustomRoomPropertiesForLobby = new[]
                    {
                        "displayName",
                        "typeId",
                        "0playerTypeId",
                        "1playerTypeId",
                        "2playerTypeId"
                    }
                });
            });
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
            var rooms = roomList.GetRange(0, Mathf.Min(MAX_DISPLAY_COUNT, roomList.Count))
                .Where(info => info.IsOpen && info.IsVisible && !info.RemovedFromList).ToList();
            BuildRooms(rooms);
        }

        private void BuildRooms(List<RoomInfo> rooms)
        {
            var newCache = new Dictionary<string, RoomItemUIManager>();
            var oldCache = cacheRooms;
            foreach (var info in rooms)
            {
                var id = info.Name;
                oldCache.TryGetValue(id, out var manager);
                if (manager != null)
                {
                    newCache.Add(id, manager);
                }
                else
                {
                    var go = Instantiate(roomItem, roomsRoot);
                    manager = go.GetComponent<RoomItemUIManager>();
                    manager.Apply(info,
                        async () =>
                        {
                            await beforeJoinRoomAction.DoPrepare();
                            PhotonNetwork.JoinRoom(id);
                        });
                    newCache.Add(id, manager);
                }
            }

            foreach (var newCacheKey in newCache.Keys)
            {
                oldCache.Remove(newCacheKey);
            }

            foreach (var roomItemUiManager in oldCache.Values)
            {
                Destroy(roomItemUiManager.gameObject);
            }

            cacheRooms = newCache;
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