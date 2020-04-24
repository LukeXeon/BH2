using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Dash.Scripts.Config;
using Dash.Scripts.Core;
using Dash.Scripts.Levels.Config;
using Dash.Scripts.Levels.View;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dash.Scripts.Levels.Core
{
    public class LevelLoadManager : MonoBehaviour, IOnEventCallback
    {
        public Image loadingRoot;
        public Image progress;
        public TextMeshProUGUI text;
        public GuidIndexer player;
        public OnLevelLoadedEvent onLevelLoadedEvent;
        public const int OnStartLoad = 1;
        public const int OnPlayerLoaded = 2;
        public const int OnLoadComplete = 3;
        private readonly HashSet<int> playerLoaded = new HashSet<int>();
        private readonly HashSet<int> loadComplete = new HashSet<int>();

        public class OnLevelLoadedEvent : UnityEvent
        {
        }

        private void Awake()
        {
            PhotonNetwork.AddCallbackTarget(this);
            DontDestroyOnLoad(gameObject);
            if (onLevelLoadedEvent == null)
            {
                onLevelLoadedEvent = new OnLevelLoadedEvent();
            }
        }

        private void OnDestroy()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public void LoadRoomLevel()
        {
            PhotonNetwork.RaiseEvent(
                OnStartLoad,
                null,
                new RaiseEventOptions {Receivers = ReceiverGroup.All},
                new SendOptions {Reliability = true}
            );
        }

        private void NotifyOnLoad(byte type)
        {
            PhotonNetwork.RaiseEvent(
                type,
                PhotonNetwork.LocalPlayer.ActorNumber,
                new RaiseEventOptions {Receivers = ReceiverGroup.All},
                new SendOptions {Reliability = true}
            );
        }

        private IEnumerator DoLoadLevel()
        {

            text.text = "Loading";
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("typeId", out var typeId);
            var scene = GameConfigManager.guanQiaInfoTable[(int) typeId];
            loadingRoot.gameObject.SetActive(true);
            loadingRoot.sprite = scene.image;
            var op = SceneManager.LoadSceneAsync(scene.sceneName);
            while (op.progress < 0.9f)
            {
                progress.fillAmount = op.progress;
                yield return null;
            }

            progress.fillAmount = 1;
            text.text = "等待其他玩家";
            yield return op;
            //Send And Wait All Player Scene Loaded
            NotifyOnLoad(OnPlayerLoaded);
            yield return new WaitUntil(() =>
            {
                return PhotonNetwork.PlayerList.All(i => playerLoaded.Contains(i.ActorNumber));
            });
            //Init Player
            var playerChuShengDian = GameObject.FindGameObjectsWithTag("PlayerChuShengDian")
                .Select(g => g.transform).ToArray();
            var virtualCamera = Camera.main.GetComponent<CinemachineVirtualCamera>();
            var uiManager = FindObjectOfType<LevelUIManager>();
            var pos = playerChuShengDian[
                Array.FindIndex(PhotonNetwork.PlayerList,
                    p => p.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            ].position;
            var go = PhotonNetwork.Instantiate(
                player.guid,
                pos,
                Quaternion.identity,
                data: new object[]
                {
                    LevelConfigManager.playerInfo.Item1.typeId,
                    LevelConfigManager.weaponInfos.First().Item1.typeId
                }
            );
            var controller = go.GetComponent<PlayerView>();
            controller.onPlayerLoadedEvent.AddListener(() => { NotifyOnLoad(OnLoadComplete); });
            uiManager.weaponChanged.AddListener(info =>
            {
                controller.photonView.RPC(nameof(controller.OnWeaponChanged), RpcTarget.All, info.typeId);
            });
            virtualCamera.Follow = go.transform;
            //Wait All Player
            yield return Resources.UnloadUnusedAssets();
            GC.Collect();
            yield return new WaitForFixedUpdate();
            yield return new WaitUntil(() =>
            {
                return PhotonNetwork.PlayerList.All(i => loadComplete.Contains(i.ActorNumber));
            });
            onLevelLoadedEvent.Invoke();
            onLevelLoadedEvent.RemoveAllListeners();

            Destroy(gameObject);
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == OnStartLoad)
            {
                StartCoroutine(DoLoadLevel());
            }
            else if (photonEvent.Code == OnPlayerLoaded)
            {
                playerLoaded.Add((int) photonEvent.CustomData);
            }
            else if (photonEvent.Code == OnLoadComplete)
            {
                loadComplete.Add((int) photonEvent.CustomData);
            }
        }
    }
}