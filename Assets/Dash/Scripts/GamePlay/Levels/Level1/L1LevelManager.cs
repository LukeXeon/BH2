using System.Collections;
using System.Linq;
using Dash.Scripts.Core;
using Photon.Pun;
using UnityEngine;

namespace Dash.Scripts.GamePlay.Levels.Level1
{
    public class L1LevelManager : LevelManager
    {
        public GuidIndexer[] NPCs;

        public Transform[] room1Locators;

        protected override IEnumerator LevelLogic()
        {
            var list = (from t in room1Locators
                    let npc = NPCs[Random.Range(0, NPCs.Length)]
                    let v3 = t.position
                    select PhotonNetwork.InstantiateSceneObject(npc.guid, v3, Quaternion.identity))
                .ToList();
            yield return new WaitUntil(() => list.All(i => !i));
        }
    }
}