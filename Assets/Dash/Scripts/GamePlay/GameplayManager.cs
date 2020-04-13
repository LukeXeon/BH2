using System;
using Cinemachine;
using Photon.Pun;
using UnityEngine;

namespace Dash.Scripts.GamePlay
{
    public class GameplayManager : MonoBehaviourPunCallbacks
    {
        public Transform[] chuShengDian;
        public GameObject playerPrefab;
        public GameObject[] prefabRefs;
        public CinemachineVirtualCamera virtualCamera;

        private void Start()
        {
            var go = PhotonNetwork.Instantiate(
                playerPrefab.name,
                chuShengDian[
                    Array.FindIndex(PhotonNetwork.PlayerList,
                        p => p.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                ].position,
                Quaternion.identity
            );
            virtualCamera.Follow = go.transform;
        }
        
    }
}
