using System.Collections;
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
        public GuidIndexer[] NPCs;
        public Transform[] room1Locators;
        [Header("Room1")]
        public LevelDoorManager room1Door;
        public CinemachineVirtualCamera virtualCameraRoom1;
        [Header("Room2")]
        public Transform[] room2Locators;
        public LevelUIManager uiManager;

        protected override void Awake()
        {
            base.Awake();
            uiManager = FindObjectOfType<LevelUIManager>();
        }

        protected override IEnumerator LevelLogic()
        {
            var list = (from t in room1Locators
                let npc = NPCs[1]
                let v3 = t.position
                select PhotonNetwork.InstantiateSceneObject(npc.guid, v3, Quaternion.identity)
                    .GetComponent<PhotonView>().ViewID).ToList();
            yield return new WaitUntil(() => list.All(i => PhotonView.Find(i) == null));
            room1Door.Open();
        }
    }
}