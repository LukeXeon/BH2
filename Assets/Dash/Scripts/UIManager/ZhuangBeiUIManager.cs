using System;
using System.Collections;
using System.Collections.Generic;
using Dash.Scripts.Cloud;
using Dash.Scripts.UI;
using Dash.Scripts.UIManager.ItemUIManager;
using Michsky.UI.ModernUIPack;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager
{
    public class ZhuangBeiUIManager : MonoBehaviour
    {
        public Animator animator;
        public Button backOnly;
        public Button backToDesktop;

        private Button[] buttons;
        public GameObject caiLiaoContent;
        public GameObject caiLiaoPanel;
        [Header("UI")] public Animator loadingMask;
        public NotificationManager onError;
        public NotificationManager onSucceed;
        public Button openCaiLiao;
        public Button openShengHen;

        public Button openWeapon;
        private GameObject[] panels;


        [Header("Assets")] public Sprite selectSprite;
        public GameObject shengHenContent;
        public ShengHenInfoUIManager shengHenInfo;
        public GameObject shengHenPanel;
        public GameObject shengHenPrefab;
        private Dictionary<string, SealEntity> shengHens;
        public GameObject typeList;
        public GameObject unLoadShengHenPrefab;
        public GameObject unLoadWeaponPrefab;
        public Sprite unSelectSprite;
        public GameObject weaponContent;
        public WeaponInfoUIManager weaponInfo;
        public GameObject weaponPanel;
        public GameObject weaponPrefab;
        private Dictionary<string, WeaponEntity> weapons;

        private void Awake()
        {
            buttons = new[] {openWeapon, openShengHen, openCaiLiao};
            panels = new[] {weaponPanel, shengHenPanel, caiLiaoPanel};
            openWeapon.onClick.AddListener(async () =>
            {
                if (weapons == null)
                {
                    BeginWaitNetwork();
                    try
                    {
                        SelectButton(openWeapon);
                        weapons = await CloudManager.GetUserWeapons();
                        LoadWeapons(weapons.Values, o => { weaponInfo.Open("强化", o, o1 => { }, null); });
                    }
                    catch (Exception e)
                    {
                        onError.Show("网络异常", e.Message);
                    }
                    finally
                    {
                        EndWaitNetWork();
                    }
                }
                else
                {
                    SelectButton(openWeapon);
                }
            });
            openShengHen.onClick.AddListener(async () =>
            {
                if (shengHens == null)
                {
                    BeginWaitNetwork();
                    try
                    {
                        SelectButton(openShengHen);
                        shengHens = await CloudManager.GetUserSeals();
                        LoadShengHens(shengHens.Values, o => { shengHenInfo.Open("强化", o, o1 => { }, null); });
                    }
                    catch (Exception e)
                    {
                        onError.Show("网络异常", e.Message);
                    }
                    finally
                    {
                        EndWaitNetWork();
                    }
                }
                else
                {
                    SelectButton(openShengHen);
                }
            });
            openCaiLiao.onClick.AddListener(() => { SelectButton(openCaiLiao); });
            backOnly.onClick.AddListener(() =>
            {
                Clear();
                animator.Play("Fade-out");
            });
        }

        private void LoadWeapons(ICollection<WeaponEntity> ws, Action<WeaponEntity> callback)
        {
            StartCoroutine(LoadWeapons0(ws, callback));
        }

        private IEnumerator LoadWeapons0(ICollection<WeaponEntity> ws, Action<WeaponEntity> callback)
        {
            yield return null;
            foreach (var item in ws)
            {
                var go = Instantiate(weaponPrefab, weaponContent.transform);
                go.GetComponent<Animator>().Play("Fade-in");
                go.GetComponent<WeaponCardUIManager>().Apply(item, callback);
                yield return new WaitForSeconds(0.05f);
            }
        }

        private void LoadShengHens(ICollection<SealEntity> ss, Action<SealEntity> callback)
        {
            StartCoroutine(LoadShengHens0(ss, callback));
        }

        private IEnumerator LoadShengHens0(ICollection<SealEntity> ss, Action<SealEntity> callback)
        {
            yield return null;
            foreach (var item in ss)
            {
                var go = Instantiate(shengHenPrefab, shengHenContent.transform);
                go.GetComponent<Animator>().Play("Fade-in");
                go.GetComponent<ShengHenCardUIManager>().Apply(item, callback);
                yield return new WaitForSeconds(0.05f);
            }
        }

        private void SelectButton(Button button)
        {
            for (var i = 0; i < buttons.Length; i++)
            {
                buttons[i].GetComponent<Image>().sprite = unSelectSprite;
                panels[i].SetActive(false);
            }

            var index = Array.IndexOf(buttons, button);
            if (index != -1) panels[index].SetActive(true);

            button.GetComponent<Image>().sprite = selectSprite;
        }

        public void OpenToSelectWeapon(List<WeaponEntity> weapons, Action onUnload, Action<WeaponEntity> callback)
        {
            typeList.SetActive(false);
            backOnly.gameObject.SetActive(true);
            backToDesktop.gameObject.SetActive(false);
            animator.Play("Fade-in");
            for (var i = 0; i < buttons.Length; i++) panels[i].SetActive(false);

            panels[0].SetActive(true);
            if (onUnload != null)
                Instantiate(unLoadWeaponPrefab, weaponContent.transform).GetComponent<Button>().onClick
                    .AddListener(() => onUnload());

            LoadWeapons(weapons, callback);
        }

        public void OpenToSelectShengHen(List<SealEntity> shengHens, Action onUnload, Action<SealEntity> callback)
        {
            typeList.SetActive(false);
            backOnly.gameObject.SetActive(true);
            backToDesktop.gameObject.SetActive(false);
            animator.Play("Fade-in");
            for (var i = 0; i < buttons.Length; i++) panels[i].SetActive(false);

            panels[1].SetActive(true);
            if (onUnload != null)
                Instantiate(unLoadShengHenPrefab, shengHenContent.transform).GetComponent<Button>().onClick
                    .AddListener(() => onUnload());

            LoadShengHens(shengHens, callback);
        }

        public void OpenNormal()
        {
            animator.Play("Fade-in");
            typeList.SetActive(true);
            backOnly.gameObject.SetActive(false);
            backToDesktop.gameObject.SetActive(true);
            for (var i = 0; i < buttons.Length; i++)
            {
                buttons[i].GetComponent<Image>().sprite = unSelectSprite;
                panels[i].SetActive(false);
            }

            openWeapon.onClick.Invoke();
        }

        public void Close()
        {
            Clear();
            animator.Play("Fade-out");
        }

        public void FastClose()
        {
            Clear();
            animator.Play("Fade-out-fast");
        }

        private void Clear()
        {
            StopAllCoroutines();
            weapons = null;
            shengHens = null;
            foreach (Transform tf in weaponContent.transform) Destroy(tf.gameObject);

            foreach (Transform tf in shengHenContent.transform) Destroy(tf.gameObject);
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