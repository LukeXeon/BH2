﻿using System;
using System.Collections;
using Dash.Scripts.Cloud;
using Dash.Scripts.UI;
using Michsky.UI.ModernUIPack;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Dash.Scripts.UIManager
{
    public class DesktopUIManager : MonoBehaviour
    {
        [Header("Top")] public NotificationManager notifySucceed;
        public NotificationManager notifyError;
        public Animator loadingMask;
        public CanvasGroup fullMask;

        [Header("Desktop")] public GameObject desktopRoot;

        public Button live2DPanel;

        public LiveCharacter live2DCharacter;

        public AudioSource live2DAudioSource;

        public AudioClip[] live2DAudioClips;

        public GameObject desktop;

        public Button openPlayWay;

        public Button openBuJi;

        public Button openZhuangBei;

        public Button openCharacters;

        public Animator animator;

        public GameObject topBar;

        [Header("GuanQia")] public GuanQiaUIManager GuanQia;

        public Button guanQiaBack;

        [Header("BuJi")] public Animator buJiAnimator;

        public Button buJiBack;

        [Header("NvWuShen")] public Animator nvWuShen;
        public NvWuShengUIManger nvWuShengUiManger;

        public Button nvWuShenBack;

        public NvWuShengUIManger playersUiManger;

        [Header("ZhuangBei")] public Animator zhuangBei;
        public ZhuangBeiUIManager zhuangBeiManager;
        public Button zhuangBeiBack;

        private Coroutine waitAnimCoroutine;

        private void SetRandomLiveMotion()
        {
            var index = Random.Range(1, 18);
            var m = live2DCharacter.motionData.GetLiveMotion(index + 1);
            live2DCharacter.StartMotion(m);
            live2DAudioSource.Stop();
            live2DAudioSource.clip = live2DAudioClips[index];
            live2DAudioSource.time = 0;
            live2DAudioSource.Play();
        }

        private void Awake()
        {
            var music = FindObjectOfType<BackgroundMusicPlayer>();
            var info = music.animator.GetCurrentAnimatorStateInfo(0);
            music.animator.Play(info.shortNameHash, 0, 1f);
            //Desktop
            live2DPanel.onClick.AddListener(SetRandomLiveMotion);
            openCharacters.onClick.AddListener(() =>
            {
                BeginWaitNetwork();
                CloudManager.GetEquipments((eq, e) =>
                {
                    EndWaitNetWork();
                    if (e != null)
                    {
                        notifyError.Show("网络异常", e);
                    }
                    else
                    {
                        nvWuShen.Play("Fade-in");
                        playersUiManger.Open(eq);
                        WaitAnimFinish(nvWuShen);
                    }
                });
            });
            openPlayWay.onClick.AddListener(() =>
            {
                GuanQia.Open();
                live2DAudioSource.Stop();
                desktopRoot.SetActive(false);
            });
            openBuJi.onClick.AddListener(() =>
            {
                buJiAnimator.Play("Fade-in");
                WaitAnimFinish(buJiAnimator);
            });
            openZhuangBei.onClick.AddListener(() =>
            {
                zhuangBeiManager.OpenNormal();
                WaitAnimFinish(zhuangBeiManager.animator);
            });
            //playWay
            guanQiaBack.onClick.AddListener(() =>
            {
                GuanQia.Close();
                desktopRoot.SetActive(true);
                SetRandomLiveMotion();
            });
            //BuJi
            buJiBack.onClick.AddListener(() =>
            {
                CancelWaitFinish();
                buJiAnimator.Play("Fade-out");
            });
            //nvWuShen
            nvWuShenBack.onClick.AddListener(() =>
            {
                CancelWaitFinish();
                nvWuShen.Play("Fade-out");
                nvWuShengUiManger.Exit();
            });
            //zhuangBei
            zhuangBeiManager.backToDesktop.onClick.AddListener(() =>
            {
                CancelWaitFinish();
                zhuangBeiManager.Close();
            });
        }

        private IEnumerator Start()
        {
            topBar.SetActive(false);
            fullMask.gameObject.SetActive(true);
            fullMask.alpha = 1;
            var image = fullMask.GetComponent<Image>();
            image.sprite = BootstrapUIManager.bootBackground;
            yield return Resources.UnloadUnusedAssets();
            yield return new WaitForEndOfFrame();
            while (fullMask.alpha / 10f > 0f)
            {
                fullMask.alpha -= Time.deltaTime;
                yield return null;
            }

            Destroy(fullMask.gameObject);
            fullMask = null;
            topBar.SetActive(true);
            animator.Play("Fade-in");
        }

        private void CancelWaitFinish()
        {
            if (waitAnimCoroutine != null)
            {
                StopCoroutine(waitAnimCoroutine);
                waitAnimCoroutine = null;
            }

            desktopRoot.SetActive(true);
            SetRandomLiveMotion();
        }

        private void WaitAnimFinish(Animator animator)
        {
            waitAnimCoroutine = StartCoroutine(WaitAnimFinish0(animator));
            live2DAudioSource.Stop();
        }

        private IEnumerator WaitAnimFinish0(Animator animator)
        {
            yield return null;
            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
            desktopRoot.SetActive(false);
        }

        public static IEnumerator WaitFinish(float length, Action action)
        {
            yield return new WaitForSeconds(length);
            action();
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