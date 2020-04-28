using System;
using System.Collections;
using System.Linq;
using Cinemachine;
using Dash.Scripts.Core;
using Dash.Scripts.GamePlay.Config;
using Dash.Scripts.GamePlay.UIManager;
using Dash.Scripts.GamePlay.View;
using Photon.Pun;
using UnityEngine;

namespace Dash.Scripts.GamePlay.Levels
{
    public abstract class LevelManager : MonoBehaviour
    {
        public Transform[] playerOutLocators;
        public GuidIndexer playerPrefab;
        [HideInInspector]
        public CinemachineVirtualCamera mainCamera;

        protected virtual void Awake()
        {
            var loader = FindObjectOfType<LevelLoadManager>();
            mainCamera = GameObject.FindWithTag("CameraController")
                .GetComponent<CinemachineVirtualCamera>();
            loader.onNetworkSceneLoaded += CreatePlayer;
            loader.onLevelStart+=OnLevelStart;
        }

        protected virtual void OnLevelStart()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(LevelLogic());
            }
        }

        private void CreatePlayer()
        {
            var uiManager = FindObjectOfType<LevelUIManager>();
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
            uiManager.weaponChanged.AddListener(info =>
            {
                controller.photonView.RPC(nameof(controller.OnWeaponChanged), RpcTarget.All, info.typeId);
            });
            mainCamera.Follow = go.transform;
        }

        protected abstract IEnumerator LevelLogic();
    }
}