using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Dash.Scripts.Config;
using Dash.Scripts.Levels.Config;
using Dash.Scripts.Levels.View;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dash.Scripts.Levels.Core
{
    public class LevelLoadManager : MonoBehaviour
    {
        public Image loadingRoot;
        public Image progress;
        public TextMeshProUGUI text;
        public PhotonView photonView;
        public OnLevelLoadedEvent onLevelLoadedEvent;
        private readonly HashSet<int> loadedPlayers = new HashSet<int>();

        public class OnLevelLoadedEvent : UnityEvent
        {
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (onLevelLoadedEvent == null)
            {
                onLevelLoadedEvent = new OnLevelLoadedEvent();
            }
        }

        public void LoadRoomLevel()
        {
            photonView.RPC(nameof(StartLoadLevel), RpcTarget.All);
        }

        [PunRPC]
        private void StartLoadLevel()
        {
            StartCoroutine(DoLoadLevel());
        }

        [PunRPC]
        private void OnLoaded(int userId)
        {
            loadedPlayers.Add(userId);
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
            photonView.RPC(nameof(OnLoaded), RpcTarget.All,
                PhotonNetwork.LocalPlayer.ActorNumber
            );
            var waitAll = new WaitUntil(() =>
            {
                return PhotonNetwork.PlayerList.All(i => loadedPlayers.Contains(i.ActorNumber));
            });
            yield return waitAll;
            loadedPlayers.Clear();
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
                "player",
                pos,
                Quaternion.identity,
                data: new object[]
                {
                    InLevelConfigManager.playerInfo.Item1.typeId,
                    InLevelConfigManager.weaponInfos.First().Item1.typeId
                }
            );
            var controller = go.GetComponent<PlayerView>();
            controller.onPlayerLoadedEvent.AddListener(() =>
            {
                photonView.RPC(nameof(OnLoaded), RpcTarget.All,
                    PhotonNetwork.LocalPlayer.ActorNumber
                );
            });
            uiManager.weaponChanged.AddListener(info =>
            {
                controller.photonView.RPC(nameof(controller.WeaponChanged), RpcTarget.All, info.typeId);
            });
            virtualCamera.Follow = go.transform;
            //Wait All Player
            yield return new WaitForFixedUpdate();
            yield return Resources.UnloadUnusedAssets();
            yield return waitAll;
            onLevelLoadedEvent.Invoke();
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }
}