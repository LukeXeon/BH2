using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dash.Scripts.Config;
using Dash.Scripts.Core;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dash.Scripts.GamePlay.Core
{
    public class LevelLoadManager : MonoBehaviour
    {
        private readonly HashSet<int> tempTable1 = new HashSet<int>();
        private readonly HashSet<int> tempTable2 = new HashSet<int>();
        public Image loadingRoot;
        public event Action onBeginLoadScene;
        public event Action onNetworkSceneLoaded;
        public GuidIndexer player;
        public Image progress;
        public TextMeshProUGUI text;
        private PhotonView photonView;

        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
            DontDestroyOnLoad(gameObject);
        }

        [PunRPC]
        private void OnLoadRoomLevel()
        {
            StartCoroutine(DoLoadLevel());
            onBeginLoadScene?.Invoke();
        }

        public void LoadRoomLevel()
        {
            photonView.RPC(nameof(OnLoadRoomLevel), RpcTarget.All);
        }

        [PunRPC]
        private void OnNotifySceneLoaded(int step, int id)
        {
            if (step == 0)
            {
                tempTable1.Add(id);
            }
            else
            {
                tempTable2.Add(id);
            }
        }

        private IEnumerator DoLoadLevel()
        {
            text.text = "Loading";
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("typeId", out var typeId);
            var scene = GameConfigManager.guanQiaInfoTable[typeId as int? ?? 0];
            loadingRoot.gameObject.SetActive(true);
            loadingRoot.sprite = scene.image;
            var op = SceneManager.LoadSceneAsync(scene.sceneName);
            while (op.progress < 0.9f)
            {
                progress.fillAmount = op.progress;
                yield return null;
            }

            progress.fillAmount = 1;
            text.text = "等待其他玩家";
            yield return op;
            //Send And Wait All Player Scene Loaded
            photonView.RPC(nameof(OnNotifySceneLoaded), RpcTarget.All, 0, PhotonNetwork.LocalPlayer.ActorNumber);
            yield return new WaitUntil(() =>
            {
                return PhotonNetwork.PlayerList.All(i => tempTable1.Contains(i.ActorNumber));
            });
            onNetworkSceneLoaded?.Invoke();
            photonView.RPC(nameof(OnNotifySceneLoaded), RpcTarget.All, 1, PhotonNetwork.LocalPlayer.ActorNumber);
            //Wait All Player
            yield return new WaitUntil(() =>
            {
                return PhotonNetwork.PlayerList.All(i => tempTable2.Contains(i.ActorNumber));
            });
            yield return Resources.UnloadUnusedAssets();
            yield return new WaitForEndOfFrame();
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}