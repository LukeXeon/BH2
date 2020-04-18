using System;
using System.Collections;
using System.Linq;
using Cinemachine;
using Dash.Scripts.GamePlay.View;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Assertions;

namespace Dash.Scripts.GamePlay
{
    public class GameplayManager : MonoBehaviourPunCallbacks
    {

        public GameObject playerPrefab;
        public GameObject[] prefabRefs;
        
        private Transform[] playerChuShengDian;
        private CinemachineVirtualCamera virtualCamera;
        private GameplayUIManager uiManager;
        private int playerCompleteCount;

        private void Awake()
        {
            playerChuShengDian = GameObject.FindGameObjectsWithTag("PlayerChuShengDian")
                .Select(g => g.transform).ToArray();
            var cam = Camera.main;
            Assert.IsNotNull(cam, "cam != null");
            virtualCamera = cam.GetComponent<CinemachineVirtualCamera>();
            uiManager = FindObjectOfType<GameplayUIManager>();
        }

        public void PlayerComplete()
        {
            ++playerCompleteCount;
        }
        
        private IEnumerator Start()
        {
            var go = PhotonNetwork.Instantiate(
                playerPrefab.name,
                playerChuShengDian[
                    Array.FindIndex(PhotonNetwork.PlayerList,
                        p => p.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                ].position,
                Quaternion.identity
            );
            var controller = go.GetComponent<PlayerView>();
            uiManager.weaponChanged.AddListener(info =>
            {
                controller.OnLocalWeaponChanged(info.typeId);
            });
            virtualCamera.Follow = go.transform;
            yield return new WaitUntil(() =>
                playerCompleteCount >= PhotonNetwork.PlayerList.Length);
            yield return Resources.UnloadUnusedAssets();
            yield return new WaitForEndOfFrame();
            yield return new WaitForFixedUpdate();
            uiManager.OnPrepared();
        }
    }
}