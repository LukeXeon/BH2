using System.Collections;
using Dash.Scripts.Core;
using Dash.Scripts.Levels.Core;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dash.Scripts.Levels.LevelManager.Level1
{

    public class L1BootLevelManager : MonoBehaviour
    {
        public GuidIndexer[] NPCs;
        public Transform[] NpcChuShengDian;

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