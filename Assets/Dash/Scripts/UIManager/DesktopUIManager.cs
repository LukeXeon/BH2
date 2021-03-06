﻿using System;
using System.Collections;
using Dash.Scripts.Cloud;
using Dash.Scripts.Setting;
using Dash.Scripts.UI;
using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Dash.Scripts.UIManager
{
    public class DesktopUIManager : MonoBehaviour
    {
        public static Sprite bootBackground;

        public Animator animator;

        [Header("BuJi")] public Animator buJiAnimator;

        public Button buJiBack;

        public GameObject desktop;

        [Header("Desktop")] public GameObject desktopRoot;

        public TextMeshProUGUI displayName;

        public Image expBar;

        public TextMeshProUGUI expText;
        public CanvasGroup fullMask;

        [Header("GuanQia")] public GuanQiaUIManager GuanQia;

        public Button guanQiaBack;

        public Image icon;

        public TextMeshProUGUI levelText;

        public AudioClip[] live2DAudioClips;

        public AudioSource live2DAudioSource;

        public LiveCharacter live2DCharacter;

        public Button live2DPanel;
        public Animator loadingMask;
        public NotificationManager notifyError;
        [Header("Top")] public NotificationManager notifySucceed;

        [Header("NvWuShen")] public Animator nvWuShen;

        public Button nvWuShenBack;
        public NvWuShengUIManger nvWuShengUiManger;

        public Button openBuJi;

        public Button openCharacters;

        public Button openPlayWay;

        public Button openZhuangBei;

        public NvWuShengUIManger playersUiManger;

        public TextMeshProUGUI shuiJingText;

        public TextMeshProUGUI tiLiText;

        public GameObject topBar;

        private Coroutine waitAnimCoroutine;

        [Header("ZhuangBei")] public Animator zhuangBei;
        public Button zhuangBeiBack;
        public ZhuangBeiUIManager zhuangBeiManager;

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
            CloudManagerOnUserInfoChanged(CloudManager.GetUserInfo());
            CloudManagerOnPlayerChanged(CloudManager.GetCurrentPlayer());
            CloudManager.playerChanged += CloudManagerOnPlayerChanged;
            CloudManager.userInfoChanged += CloudManagerOnUserInfoChanged;
            var music = FindObjectOfType<BackgroundMusicPlayer>();
            var info = music.animator.GetCurrentAnimatorStateInfo(0);
            music.animator.Play(info.shortNameHash, 0, 1f);
            //Desktop
            live2DPanel.onClick.AddListener(SetRandomLiveMotion);
            openCharacters.onClick.AddListener(async () =>
            {
                BeginWaitNetwork();
                try
                {
                    var e = await CloudManager.GetEquipments();
                    nvWuShen.Play("Fade-in");
                    playersUiManger.Open(e);
                    WaitAnimFinish(nvWuShen);
                }
                catch (Exception exception)
                {
                    notifyError.Show("网络异常", exception.Message);
                    Debug.Log(exception);
                    throw;
                }
                finally
                {
                    EndWaitNetWork();
                }
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

        private void CloudManagerOnUserInfoChanged(GameUserEntity obj)
        {
            var level = GameSettingManager.GetUserLevel(obj.exp);
            displayName.text = obj.name;
            tiLiText.text = 9999.ToString();
            shuiJingText.text = obj.crystal.ToString();
            expText.text = "经验值：" + level.currentExp;
            levelText.text = "等级：" + level.count;
            expBar.fillAmount = (float) level.currentExp / level.maxExp;
        }

        private void CloudManagerOnPlayerChanged(PlayerEntity obj)
        {
            var info = GameSettingManager.playerTable[obj.typeId];
            icon.sprite = info.icon;
        }

        private void OnDestroy()
        {
            CloudManager.userInfoChanged -= CloudManagerOnUserInfoChanged;
            CloudManager.playerChanged -= CloudManagerOnPlayerChanged;
        }

        private IEnumerator Start()
        {
            topBar.SetActive(false);
            fullMask.gameObject.SetActive(true);
            fullMask.alpha = 1;
            var image = fullMask.GetComponent<Image>();
            if (bootBackground)
            {
                image.sprite = bootBackground;
                bootBackground = null;
            }
            else
            {
                image.color = Color.black;
            }

            yield return Resources.UnloadUnusedAssets();
            yield return new WaitForEndOfFrame();
            while (fullMask.alpha > 0f)
            {
                fullMask.alpha -= Time.deltaTime * 4;
                yield return new WaitForEndOfFrame();
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