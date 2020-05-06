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

namespace Dash.Scripts.GamePlay.Levels
{
    public sealed class LevelLoadManager : MonoBehaviour, IOnEventCallback
    {
        private readonly HashSet<int> tempTable1 = new HashSet<int>();
        private readonly HashSet<int> tempTable2 = new HashSet<int>();
        public Image loadingRoot;
        public event Action onBeginLoadScene;
        public event Action onNetworkSceneLoaded;
        public event Action onLevelStart;
        public GuidIndexer player;
        public Image progress;
        public TextMeshProUGUI text;

        public void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Send(byte code, int value)
        {
            PhotonNetwork.RaiseEvent(code, value, new RaiseEventOptions() {Receivers = ReceiverGroup.All},
                new SendOptions {Reliability = true});
        }

        public void LoadRoomLevel()
        {
            Send(11, 0);
        }

        private IEnumerator DoLoadLevel()
        {
            text.text = "Loading";
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("typeId", out var typeId);
            var scene = GameConfigManager.LevelsInfoTable[typeId as int? ?? 0];
            loadingRoot.gameObject.SetActive(true);
            loadingRoot.sprite = scene.image;
            var op = SceneManager.LoadSceneAsync(scene.sceneName);
            while (op.progress < 0.9f)
            {
                progress.fillAmount = op.progress;
                yield return null;
            }

            progress.fillAmount = 1;
            text.text = "等待游戏开始";
            yield return op;
            //Send And Wait All Player Scene Loaded
            Send(12, PhotonNetwork.LocalPlayer.ActorNumber);
            yield return new WaitUntil(() =>
            {
                return PhotonNetwork.PlayerList.All(i => tempTable1.Contains(i.ActorNumber));
            });
            onNetworkSceneLoaded?.Invoke();
            Send(13, PhotonNetwork.LocalPlayer.ActorNumber);
            //Wait All Player
            yield return new WaitUntil(() =>
            {
                return PhotonNetwork.PlayerList.All(i => tempTable2.Contains(i.ActorNumber));
            });
            yield return Resources.UnloadUnusedAssets();
            yield return new WaitForEndOfFrame();
            Destroy(gameObject);
            onLevelStart?.Invoke();
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == 11)
            {
                StartCoroutine(DoLoadLevel());
                onBeginLoadScene?.Invoke();
            }
            else if (photonEvent.Code == 12)
            {
                tempTable1.Add((int) photonEvent.CustomData);
            }
            else if (photonEvent.Code == 13)
            {
                tempTable2.Add((int) photonEvent.CustomData);
            }
        }
    }
}