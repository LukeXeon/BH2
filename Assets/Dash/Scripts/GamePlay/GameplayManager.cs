using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
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
        internal List<GameObject> players = new List<GameObject>();

        private void Awake()
        {
            Resources.UnloadUnusedAssets();
            playerChuShengDian = GameObject.FindGameObjectsWithTag("PlayerChuShengDian")
                .Select(g => g.transform).ToArray();
            var cam = Camera.main;
            Assert.IsNotNull(cam, "cam != null");
            virtualCamera = cam.GetComponent<CinemachineVirtualCamera>();
            uiManager = FindObjectOfType<GameplayUIManager>();
        }

        private void Start()
        {
            var go = PhotonNetwork.Instantiate(
                playerPrefab.name,
                playerChuShengDian[
                    Array.FindIndex(PhotonNetwork.PlayerList,
                        p => p.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                ].position,
                Quaternion.identity
            );
            virtualCamera.Follow = go.transform;
            StartCoroutine(WaitAllPlayersPrepared());
        }

        private IEnumerator WaitAllPlayersPrepared()
        {
            yield return new WaitUntil(() =>
                players.Count >= PhotonNetwork.PlayerList.Length);
            yield return new WaitForEndOfFrame();
            yield return new WaitForFixedUpdate();
            uiManager.Prepared();
        }
    }
}