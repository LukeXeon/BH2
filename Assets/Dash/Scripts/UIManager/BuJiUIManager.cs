using System;
using System.Collections;
using Dash.Scripts.Cloud;
using Dash.Scripts.UI;
using Dash.Scripts.UIManager.ItemUIManager;
using Michsky.UI.ModernUIPack;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Dash.Scripts.UIManager
{
    public class BuJiUIManager : MonoBehaviour
    {
        public Animator animator;
        public Button begin;
        public VideoPlayer buJiPlayer;
        public GameObject content;
        public Button finish;
        public float keyTime;
        public Animator loadingMask;
        public NotificationManager notifyError;
        [Header("UI")] public NotificationManager notifySucceed;

        [Header("Assets")] public GameObject playerAddExpCardPrefab;
        public GameObject playerUnLockCardPrefab;
        public GameObject shengHenPrefab;
        public GameObject weaponCardPrefab;

        private void Awake()
        {
            begin.onClick.AddListener(async () =>
            {
                BeginWaitNetwork();
                try
                {
                    var r = await CloudManager.LuckDraw();
                    buJiPlayer.gameObject.SetActive(true);
                    buJiPlayer.Stop();
                    buJiPlayer.frame = 0;
                    buJiPlayer.Prepare();
                    StartCoroutine(WaitVideoTargetTime(buJiPlayer, keyTime,
                        () => { animator.Play("Result-in"); }));
                    LuckCardItemUIManager luckCardUiManager = null;
                    switch (r.resultType)
                    {
                        case LuckDrawResult.Type.UnLockPlayer:
                        {
                            luckCardUiManager = Instantiate(playerUnLockCardPrefab, content.transform)
                                .GetComponent<LuckCardItemUIManager>();
                        }
                            break;
                        case LuckDrawResult.Type.AddPlayerExp:
                        {
                            luckCardUiManager = Instantiate(playerAddExpCardPrefab, content.transform)
                                .GetComponent<LuckCardItemUIManager>();
                        }
                            break;
                        case LuckDrawResult.Type.Weapon:
                        {
                            luckCardUiManager = Instantiate(weaponCardPrefab, content.transform)
                                .GetComponent<LuckCardItemUIManager>();
                        }
                            break;
                        case LuckDrawResult.Type.Seal:
                        {
                            luckCardUiManager = Instantiate(shengHenPrefab, content.transform)
                                .GetComponent<LuckCardItemUIManager>();
                        }
                            break;
                    }

                    if (luckCardUiManager != null) luckCardUiManager.Apply(r);
                }
                catch (Exception e)
                {
                    notifyError.Show("网络异常", e.Message);
                    throw;
                }
                finally
                {
                    EndWaitNetWork();
                }
            });
            buJiPlayer.prepareCompleted += p => { p.Play(); };
            buJiPlayer.loopPointReached += p => { p.gameObject.SetActive(false); };
            finish.onClick.AddListener(() =>
            {
                animator.Play("Result-out");
                foreach (Transform item in content.transform) Destroy(item.gameObject);
            });
        }

        private static IEnumerator WaitVideoTargetTime(VideoPlayer videoPlayer, float time, Action callback)
        {
            while (videoPlayer.time < time) yield return null;

            callback();
        }


        private void BeginWaitNetwork()
        {
            loadingMask.gameObject.SetActive(true);
            loadingMask.Play("Fade-in");
        }

        private void EndWaitNetWork()
        {
            loadingMask.gameObject.SetActive(false);
        }
    }
}