using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Dash.Scripts.Core;
using Dash.Scripts.GamePlay.UIManager;
using Photon.Pun;
using UnityEngine;

namespace Dash.Scripts.GamePlay.Levels.Level1
{
    public class L1LevelManager : LevelManager
    {
        public PhotonView photonView;
        public GuidIndexer[] NpcList;
        public Transform[] room1Locators;
        [Header("Room1")] public Animator root1ShipAnimator;
        public LevelDoorManager room1Door;
        public CinemachineVirtualCamera virtualCameraRoom1;
        [Header("Room2")] public Transform[] room2Locators;
        private LevelUIManager uiManager;
        private HashSet<int> room2Players = new HashSet<int>();
        
        private void OnPlayerEnterRoom2(int value)
        {
            room2Players.Add(value);
        }

        protected override void Awake()
        {
            base.Awake();
            uiManager = FindObjectOfType<LevelUIManager>();
        }

        public void OnShipFinish()
        {
            Destroy(root1ShipAnimator.gameObject);
            root1ShipAnimator = null;
        }
        
        
        protected override void OnLevelStart()
        {
            root1ShipAnimator.Play("Fade-in");
        }

        protected override IEnumerator LevelLogic()
        {
            var list = (from t in room1Locators
                let npc = NpcList[0]
                let v3 = t.position
                select PhotonNetwork.InstantiateSceneObject(npc.guid, v3, Quaternion.identity)
                    .GetComponent<PhotonView>().ViewID).ToList();
            yield return new WaitUntil(() => list.All(i => PhotonView.Find(i) == null));
            room1Door.Open();
            yield return new WaitUntil(() =>
            {
                return PhotonNetwork.PlayerList.All(i => room2Players.Contains(i.ActorNumber));
            });
        }
    }
}