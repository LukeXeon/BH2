using System.Collections;
using Dash.Scripts.Core;
using Dash.Scripts.Levels.Core;
using Photon.Pun;
using UnityEngine;

namespace Dash.Scripts.Levels.LevelManager.Level1
{
    public class L1LevelManager : MonoBehaviour
    {
        public Transform[] NpcChuShengDian;
        public GuidIndexer[] NPCs;

        private void Awake()
        {
            FindObjectOfType<LevelLoadManager>().onLevelLoadedEvent.AddListener(Call);
        }

        private void Call()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                foreach (var t in NpcChuShengDian)
                {
                    var npc = NPCs[Random.Range(0, NPCs.Length)];
                    var v3 = t.position;
                    PhotonNetwork.InstantiateSceneObject(npc.guid, v3, Quaternion.identity);
                }

                StartCoroutine(LevelLogic());
            }
        }

        private IEnumerator LevelLogic()
        {
            yield break;
        }
    }
}