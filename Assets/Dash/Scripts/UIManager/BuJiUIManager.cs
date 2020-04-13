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
        [Header("UI")]
        public NotificationManager notifySucceed;
        public NotificationManager notifyError;
        public Animator loadingMask;
        public VideoPlayer buJiPlayer;
        public float keyTime;
        public Button begin;
        public Button finish;
        public Animator animator;
        public GameObject content;

        [Header("Assets")] public GameObject playerAddExpCardPrefab;
        public GameObject playerUnLockCardPrefab;
        public GameObject weaponCardPrefab;
        public GameObject shengHenPrefab;

        private void Awake()
        {
            begin.onClick.AddListener(() =>
            {
                BeginWaitNetwork();
                CloudManager.LuckDraw((r, e) =>
                {
                    EndWaitNetWork();
                    if (e != null)
                    {
                        notifyError.Show("网络异常", e);
                    }
                    else
                    {
                        buJiPlayer.gameObject.SetActive(true);
                        buJiPlayer.Stop();
                        buJiPlayer.frame = 0;
                        buJiPlayer.Prepare();
                        StartCoroutine(WaitVideoTargetTime(buJiPlayer, keyTime,
                            () =>
                            {
                                animator.Play("Result-in");
                            }));
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
                            case LuckDrawResult.Type.ShengHen:
                            {
                                luckCardUiManager = Instantiate(shengHenPrefab, content.transform)
                                    .GetComponent<LuckCardItemUIManager>();
                            }
                                break;
                        }

                        if (luckCardUiManager != null)
                        {
                            luckCardUiManager.Apply(r);
                        }
                    }
                });
            });
            buJiPlayer.prepareCompleted += p =>
            {
                p.Play();
            };
            buJiPlayer.loopPointReached += p =>
            {
                p.gameObject.SetActive(false);
    };
            finish.onClick.AddListener(() =>
            {
                animator.Play("Result-out");
                foreach (Transform item in content.transform)
                {
                    Destroy(item.gameObject);
                }
            });
        }

        private static IEnumerator WaitVideoTargetTime(VideoPlayer videoPlayer, float time, Action callback)
        {
            while (videoPlayer.time < time)
            {
                yield return null;
            }

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