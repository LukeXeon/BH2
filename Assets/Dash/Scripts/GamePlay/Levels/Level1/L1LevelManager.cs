using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public Transform[] room1Locators2;
        public Transform[] room1Locators3;
        public TriggerEvent[] room1TriggerEvents;
        [Header("Room2")] public Transform[] room2Locators;
        private LevelUIManager uiManager;
        private HashSet<int> room2Players = new HashSet<int>();
        private HashSet<int>[] room1NodeTriggers;

        private void OnPlayerEnterRoom2(int value)
        {
            room2Players.Add(value);
        }

        [PunRPC]
        private void SetRoom1Trigger(int index, int playerId, bool isAdd)
        {
            if (isAdd)
            {
                room1NodeTriggers[index].Add(playerId);
            }
            else
            {
                room1NodeTriggers[index].Remove(playerId);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            uiManager = FindObjectOfType<LevelUIManager>();
            room1NodeTriggers = new HashSet<int>[3];
            for (var i = 0; i < room1NodeTriggers.Length; i++)
            {
                room1NodeTriggers[i] = new HashSet<int>();
            }

            for (var i = 0; i < room1TriggerEvents.Length; i++)
            {
                var i1 = i;
                room1TriggerEvents[i].onTriggerEnter.AddListener(o =>
                {
                    if (o.gameObject.layer == LayerMask.NameToLayer("Player"))
                    {
                        photonView.RPC(nameof(SetRoom1Trigger), RpcTarget.MasterClient, i1,
                            PhotonNetwork.LocalPlayer.ActorNumber, true);
                    }
                });
                room1TriggerEvents[i].onTriggerExit.AddListener(o =>
                {
                    if (o.gameObject.layer == LayerMask.NameToLayer("Player"))
                    {
                        photonView.RPC(nameof(SetRoom1Trigger), RpcTarget.MasterClient, i1,
                            PhotonNetwork.LocalPlayer.ActorNumber, false);
                    }
                });
            }
        }

        public void OnShipFinish()
        {
            Destroy(root1ShipAnimator.gameObject);
            root1ShipAnimator = null;
        }


        protected override void OnLevelStart()
        {
            base.OnLevelStart();
            root1ShipAnimator.Play("Fade-in");
        }

        protected override IEnumerator LevelLogic()
        {
            yield return new WaitUntil(() =>
                PhotonNetwork.PlayerList.Any(i => room1NodeTriggers[0].Contains(i.ActorNumber)));
            var list = (from t in room1Locators
                let npc = NpcList[0]
                let v3 = t.position
                select PhotonNetwork.InstantiateSceneObject(npc.guid, v3, Quaternion.identity)
                    .GetComponent<PhotonView>().ViewID).ToList();
            yield return new WaitUntil(() => list.All(i => PhotonView.Find(i) == null));
            list.Clear();
            yield return new WaitUntil(() =>
                PhotonNetwork.PlayerList.Any(i => room1NodeTriggers[1].Contains(i.ActorNumber)));
            var npc1 = 2;
            foreach (var t in room1Locators2)
            {
                GameObject go;
                if (Random.Range(0, 2) == 1 && npc1 > 0)
                {
                    --npc1;
                    go = PhotonNetwork.InstantiateSceneObject(
                        NpcList[1].guid,
                        t.position,
                        Quaternion.identity);
                }
                else
                {
                    go = PhotonNetwork.InstantiateSceneObject(
                        NpcList[0].guid,
                        t.position,
                        Quaternion.identity
                    );
                }

                list.Add(go.GetComponent<PhotonView>().ViewID);
            }

            yield return new WaitUntil(() => list.All(i => PhotonView.Find(i) == null));
            list.Clear();
            yield return new WaitUntil(() =>
                PhotonNetwork.PlayerList.Any(i => room1NodeTriggers[2].Contains(i.ActorNumber)));
            list.AddRange(room1Locators3
                .Select(t => PhotonNetwork.InstantiateSceneObject(NpcList[1].guid, t.position, Quaternion.identity))
                .Select(go => go.GetComponent<PhotonView>().ViewID));
            yield return new WaitUntil(() => list.All(i => PhotonView.Find(i) == null));
            list.Clear();
   
            room1Door.Open();
            yield return new WaitUntil(() =>
            {
                return PhotonNetwork.PlayerList.All(i => room2Players.Contains(i.ActorNumber));
            });
        }
    }
}