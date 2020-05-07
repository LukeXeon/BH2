using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Dash.Scripts.Core;
using Dash.Scripts.GamePlay.Setting;
using Dash.Scripts.GamePlay.UIManager;
using Dash.Scripts.GamePlay.View;
using Photon.Pun;
using UnityEngine;

namespace Dash.Scripts.GamePlay.Levels
{
    public abstract class LevelManager : MonoBehaviour
    {
        public PhotonView photonView;
        public Transform[] playerOutLocators;
        public GuidIndexer playerPrefab;
        [HideInInspector] public CinemachineVirtualCamera mainCamera;
        protected LevelUIManager uiManager;
        private HashSet<int> diePlayers;

        protected virtual void Awake()
        {
            photonView = GetComponent<PhotonView>();
            diePlayers = new HashSet<int>();
            uiManager = FindObjectOfType<LevelUIManager>();
            var loader = FindObjectOfType<LevelLoadManager>();
            mainCamera = GameObject.FindWithTag("CameraController")
                .GetComponent<CinemachineVirtualCamera>();
            loader.onNetworkSceneLoaded += CreatePlayer;
            loader.onLevelStart += OnLevelStart;
        }

        protected virtual void OnLevelStart()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(WaitAllPlayerDie());
                StartCoroutine(MasterLevelLogic());
            }
        }

        [PunRPC]
        public void SendLocalPlayerLiveEvent(int id, bool isLive)
        {
            if (isLive)
            {
                diePlayers.Remove(id);
            }
            else
            {
                diePlayers.Add(id);
            }
        }

        [PunRPC]
        public void SendStopTheGame()
        {
            uiManager.OnAllPlayerDie();
        }

        private void CreatePlayer()
        {
            var pos = playerOutLocators[
                Array.FindIndex(PhotonNetwork.PlayerList,
                    p => p.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            ].position;
            var go = PhotonNetwork.Instantiate(
                playerPrefab.guid,
                pos,
                Quaternion.identity,
                data: new object[]
                {
                    PlayerConfigManager.playerInfo.Item1.typeId,
                    PlayerConfigManager.weaponInfos.Select(i => i.Item1.typeId).ToArray()
                }
            );
            var controller = go.GetComponent<PlayerView>();
            controller.onActorDie.AddListener(() =>
            {
                photonView.RPC(
                    nameof(SendLocalPlayerLiveEvent),
                    RpcTarget.MasterClient,
                    PhotonNetwork.LocalPlayer.ActorNumber,
                    false
                );
            });
            controller.onPlayerRelive.AddListener(() =>
            {
                photonView.RPC(
                    nameof(SendLocalPlayerLiveEvent),
                    RpcTarget.MasterClient,
                    PhotonNetwork.LocalPlayer.ActorNumber,
                    true
                );
            });
            uiManager.weaponChanged.AddListener(info =>
            {
                controller.photonView.RPC(nameof(controller.OnWeaponChanged), RpcTarget.All, info.typeId);
            });
            mainCamera.Follow = go.transform;
        }

        private IEnumerator WaitAllPlayerDie()
        {
            yield return new WaitUntil(() => PhotonNetwork.PlayerList.All(i => diePlayers.Contains(i.ActorNumber)));
            yield return new WaitForSeconds(1);
            photonView.RPC(nameof(SendStopTheGame), RpcTarget.All);
        }

        protected abstract IEnumerator MasterLevelLogic();
    }
}