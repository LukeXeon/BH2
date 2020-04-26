using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dash.Scripts.Config;
using Dash.Scripts.Core;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dash.Scripts.GamePlay.Core
{
    public class LevelLoadManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        public const int OnStartLoad = 1;
        public const int OnSceneLoaded = 2;
        private readonly HashSet<int> tempTable = new HashSet<int>();
        public Image loadingRoot;
        public event Action onNetworkSceneLoaded;
        public GuidIndexer player;
        public Image progress;
        public TextMeshProUGUI text;

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == OnStartLoad)
                StartCoroutine(DoLoadLevel());
            else if (photonEvent.Code == OnSceneLoaded)
                tempTable.Add((int) photonEvent.CustomData);
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void LoadRoomLevel()
        {
            PhotonNetwork.RaiseEvent(
                OnStartLoad,
                null,
                new RaiseEventOptions {Receivers = ReceiverGroup.All},
                new SendOptions {Reliability = true}
            );
        }

        private void NotifySceneLoaded()
        {
            PhotonNetwork.RaiseEvent(
                OnSceneLoaded,
                PhotonNetwork.LocalPlayer.ActorNumber,
                new RaiseEventOptions {Receivers = ReceiverGroup.All},
                new SendOptions {Reliability = true}
            );
        }

        private IEnumerator DoLoadLevel()
        {
            text.text = "Loading";
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("typeId", out var typeId);
            var scene = GameConfigManager.guanQiaInfoTable[(int) typeId];
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
            NotifySceneLoaded();
            yield return new WaitUntil(() =>
            {
                return PhotonNetwork.PlayerList.All(i => tempTable.Contains(i.ActorNumber));
            });
            tempTable.Clear();
            onNetworkSceneLoaded?.Invoke();
            NotifySceneLoaded();
            //Wait All Player
            yield return new WaitUntil(() =>
            {
                return PhotonNetwork.PlayerList.All(i => tempTable.Contains(i.ActorNumber));
            });
            yield return Resources.UnloadUnusedAssets();
            yield return new WaitForEndOfFrame();
            Destroy(gameObject);
        }
    }
}