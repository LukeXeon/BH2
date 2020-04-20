using Dash.Scripts.Levels.Core;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dash.Scripts.Levels.LevelManager.Level1
{

    public class L1BootLevelManager : MonoBehaviour
    {
        public GameObject[] NPCs;
        public Transform[] NpcChuShengDian;

        private void Awake()
        {
            FindObjectOfType<LevelLoadManager>().onLevelLoadedEvent.AddListener(() =>
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    foreach (var t in NpcChuShengDian)
                    {
                        var npc = NPCs[Random.Range(0, NPCs.Length)];
                        var v3 = t.position;
                        PhotonNetwork.InstantiateSceneObject(
                            npc.GetKey(),
                            v3,
                            Quaternion.identity
                        );
                    }
                }
            });
        }
    }
}