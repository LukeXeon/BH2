using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Dash.Scripts.Config;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Dash.Scripts.GamePlay
{
    public class GameplayManager : MonoBehaviourPunCallbacks
    {

        public GameObject playerPrefab;
        public GameObject[] prefabRefs;
        public OnWeaponChangedEvent weaponChanged;

        [Serializable]
        public class OnWeaponChangedEvent : UnityEvent<WeaponInfoAsset>
        {
        }

        private Transform[] playerChuShengDian;
        private CinemachineVirtualCamera virtualCamera;
        private GameplayUIManager uiManager;
        internal List<GameObject> players = new List<GameObject>();

        private void Awake()
        {
            playerChuShengDian = GameObject.FindGameObjectsWithTag("PlayerChuShengDian")
                .Select(g => g.transform).ToArray();
            var cam = Camera.main;
            Assert.IsNotNull(cam, "cam != null");
            virtualCamera = cam.GetComponent<CinemachineVirtualCamera>();
            uiManager = FindObjectOfType<GameplayUIManager>();
            if (weaponChanged == null)
            {
                weaponChanged = new OnWeaponChangedEvent();
            }
        }

        private IEnumerator Start()
        {
            yield return Resources.UnloadUnusedAssets();
            var go = PhotonNetwork.Instantiate(
                playerPrefab.name,
                playerChuShengDian[
                    Array.FindIndex(PhotonNetwork.PlayerList,
                        p => p.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                ].position,
                Quaternion.identity
            );
            virtualCamera.Follow = go.transform;
            yield return new WaitUntil(() =>
                players.Count >= PhotonNetwork.PlayerList.Length);
            yield return new WaitForEndOfFrame();
            yield return new WaitForFixedUpdate();
            uiManager.Prepared();
        }
    }
}