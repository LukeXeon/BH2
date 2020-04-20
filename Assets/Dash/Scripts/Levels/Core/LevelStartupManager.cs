using System;
using System.Collections;
using System.Linq;
using Cinemachine;
using Dash.Scripts.Levels.View;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace Dash.Scripts.Levels.Core
{
    public class LevelStartupManager : MonoBehaviour
    {
        public OnLevelLoadedEvent onLevelLoadedEvent;
        private Transform[] playerChuShengDian;
        private CinemachineVirtualCamera virtualCamera;
        private LevelUIManager uiManager;
        private int playerCount;

        [Serializable]
        public class OnLevelLoadedEvent : UnityEvent { }

        private void Awake()
        {
            playerChuShengDian = GameObject.FindGameObjectsWithTag("PlayerChuShengDian")
                .Select(g => g.transform).ToArray();
            var cam = Camera.main;
            virtualCamera = cam != null ? cam.GetComponent<CinemachineVirtualCamera>() : null;
            uiManager = FindObjectOfType<LevelUIManager>();
            if (onLevelLoadedEvent == null)
            {
                onLevelLoadedEvent = new OnLevelLoadedEvent();
            }
        }

        public void OnPlayerLoadedNotified()
        {
            playerCount++;
        }
        
        private IEnumerator Start()
        {
            var pos = playerChuShengDian[
                Array.FindIndex(PhotonNetwork.PlayerList,
                    p => p.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            ].position;
            var pool = (LevelPrefabPool)PhotonNetwork.PrefabPool;
            var go = PhotonNetwork.Instantiate(
                pool.GetPlayerKey(),
                pos,
                Quaternion.identity
            );
            var controller = go.GetComponent<PlayerView>();
            controller.onPlayerLoadedEvent.AddListener(OnPlayerLoadedNotified);
            uiManager.weaponChanged.AddListener(info => { controller.OnLocalWeaponChanged(info.typeId); });
            virtualCamera.Follow = go.transform;
            yield return new WaitUntil(() =>
                playerCount >= PhotonNetwork.PlayerList.Length);
            yield return Resources.UnloadUnusedAssets();
            yield return new WaitForEndOfFrame();
            yield return new WaitForFixedUpdate();
            onLevelLoadedEvent.Invoke();
        }
    }
}