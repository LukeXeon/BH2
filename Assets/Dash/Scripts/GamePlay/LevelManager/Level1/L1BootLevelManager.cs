using Photon.Pun;
using UnityEngine;

namespace Dash.Scripts.GamePlay.LevelManager.Level1
{

    public class L1BootLevelManager : MonoBehaviour
    {

        public GameObject[] NPCs;
        public Transform[] NpcChuShengDian;

        private void Start()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                foreach (var t in NpcChuShengDian)
                {
                    var npc = NPCs[Random.Range(0, NPCs.Length)];
                    var v3 = t.position;
                    PhotonNetwork.InstantiateSceneObject(npc.name, v3, Quaternion.identity);
                }
            }
        }
    }
}