using System;
using System.Collections.Generic;
using System.Linq;
using Dash.Scripts.Cloud;
using Dash.Scripts.Setting;
using Dash.Scripts.UI;
using Dash.Scripts.UIManager.ItemUIManager;
using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager
{
    public class ShoppingUIManager : MonoBehaviour
    {
        public LiveCharacter character;
        public Button back;
        public Button characterBtn;
        public Button buyBtn;
        public GameObject root;
        public Animator animator;
        public Transform titlesRoot;
        public Transform contentsRoot;
        public NotificationManager onError;
        public Animator loadingMask;
        public TextMeshProUGUI shuiJing;
        public Animator buyAnimator;

        [Header("Assets")] public GameObject titlePrefab;
        public GameObject itemPrefab;
        public Color selectColor;
        public Color unselectColor;

        private static readonly Dictionary<Type, string> names = new Dictionary<Type, string>
        {
            {typeof(PlayerInfoAsset), "角色"},
            {typeof(WeaponInfoAsset), "武器"},
            {typeof(SealInfoAsset), "圣痕"}
        };

        private class Comparer : IComparer<Type>
        {
            private static readonly Type[] types =
            {
                typeof(PlayerInfoAsset),
                typeof(WeaponInfoAsset),
                typeof(SealInfoAsset)
            };

            public int Compare(Type x, Type y)
            {
                return Array.IndexOf(types, x) - Array.IndexOf(types, y);
            }
        }

        private Type type;

        private int currentTitle = -1;

        private int currentItem = -1;

        private bool currentIsBuy;

        private StoreItemInfoAsset storeItemInfoAsset;

        private readonly List<StoreTitleUIManager> titles = new List<StoreTitleUIManager>();

        private readonly List<StoreItemUIManager> items = new List<StoreItemUIManager>();

        private void Awake()
        {
            CloudManager.userInfoChanged += CloudManagerOnUserInfoChanged;
            CloudManagerOnUserInfoChanged(CloudManager.GetUserInfo());
            back.onClick.AddListener(() =>
            {
                animator.Play("Fade-out");
                Close();
            });
            buyBtn.onClick.AddListener(async () =>
            {
                if (!currentIsBuy)
                {
                    return;
                }

                try
                {
                    BeginWaitNetwork();
                    if (type == typeof(PlayerInfoAsset))
                    {
                        await CloudManager.BuyPlayer(storeItemInfoAsset.bagItem.TypeId, storeItemInfoAsset.crystalCost);
                    }
                    else if (type == typeof(SealInfoAsset))
                    {
                        await CloudManager.BuySeal(storeItemInfoAsset.bagItem.TypeId, storeItemInfoAsset.crystalCost);
                    }
                    else if (type == typeof(WeaponInfoAsset))
                    {
                        await CloudManager.BuyWeapon(storeItemInfoAsset.bagItem.TypeId, storeItemInfoAsset.crystalCost);
                    }
                    else
                    {
                        return;
                    }
                }
                catch (Exception e)
                {
                    onError.Show("网络请求失败", e.Message);
                    return;
                }
                finally
                {
                    EndWaitNetWork();
                }

                buyAnimator.Play("Open");
                var index = currentTitle;
                if (index >= 0)
                {
                    currentTitle = -1;
                    titles[index].Select();
                }
            });
            characterBtn.onClick.AddListener(() => { character.SetRandomLiveMotionAndExpression(); });
        }

        private void CloudManagerOnUserInfoChanged(GameUserEntity obj)
        {
            shuiJing.text = obj.crystal.ToString();
        }

        private void OnDestroy()
        {
            CloudManager.userInfoChanged -= CloudManagerOnUserInfoChanged;
        }

        public void Open()
        {
            animator.Play("Fade-in");
            foreach (var manager in titles)
            {
                Destroy(manager.gameObject);
            }

            titles.Clear();
            var titleIndex = 0;
            var dictionary = new SortedDictionary<Type, List<StoreItemInfoAsset>>(new Comparer());
            foreach (var asset in GameSettingManager.setting.storeItemInfoAssets)
            {
                if (!dictionary.TryGetValue(asset.item.GetType(), out var list))
                {
                    list = new List<StoreItemInfoAsset>();
                    dictionary.Add(asset.item.GetType(), list);
                }

                list.Add(asset);
            }

            foreach (var keyValuePair in dictionary)
            {
                var title = Instantiate(titlePrefab, titlesRoot)
                    .GetComponent<StoreTitleUIManager>();
                titles.Add(title);
                var index = titleIndex;
                title.Apply(names[keyValuePair.Key], async () =>
                {
                    if (currentTitle == index)
                    {
                        return;
                    }


                    int[] ownerPlayerTypeId = null;

                    if (keyValuePair.Key == typeof(PlayerInfoAsset))
                    {
                        try
                        {
                            BeginWaitNetwork();
                            ownerPlayerTypeId = await CloudManager.GetOwnerPlayerTypeIds();
                        }
                        catch (Exception e)
                        {
                            onError.Show("网络请求失败", e.Message);
                            return;
                        }
                        finally
                        {
                            EndWaitNetWork();
                        }
                    }

                    buyBtn.enabled = false;
                    ((Image) buyBtn.targetGraphic).color = unselectColor;
                    currentTitle = index;
                    currentItem = -1;
                    type = keyValuePair.Key;
                    foreach (var item in items)
                    {
                        Destroy(item.gameObject);
                    }

                    items.Clear();

                    for (var i = 0; i < titles.Count; i++)
                    {
                        if (i == currentTitle)
                        {
                            titles[i].DoSelect();
                        }
                        else
                        {
                            titles[i].DoUnSelect();
                        }
                    }

                    var itemIndex = 0;
                    foreach (var asset in keyValuePair.Value)
                    {
                        var index1 = itemIndex;

                        var item = Instantiate(itemPrefab, contentsRoot)
                            .GetComponent<StoreItemUIManager>();
                        bool isBuy = ownerPlayerTypeId == null || Array.IndexOf(
                                         ownerPlayerTypeId,
                                         asset.bagItem.TypeId
                                     ) == -1;
                        items.Add(item);
                        item.Apply(
                            asset.bagItem.DisplayName,
                            asset.crystalCost.ToString(),
                            asset.bagItem.Image,
                            keyValuePair.Key == typeof(SealInfoAsset),
                            isBuy,
                            () =>
                            {
                                if (index1 == currentItem)
                                {
                                    return;
                                }

                                currentItem = index1;
                                currentIsBuy = isBuy;
                                storeItemInfoAsset = keyValuePair.Value[index1];
                                if (isBuy)
                                {
                                    buyBtn.enabled = true;
                                    ((Image) buyBtn.targetGraphic).color = selectColor;
                                }
                                else
                                {
                                    buyBtn.enabled = false;
                                    ((Image) buyBtn.targetGraphic).color = unselectColor;
                                }

                                for (var i = 0; i < items.Count; i++)
                                {
                                    if (i == currentItem)
                                    {
                                        items[i].DoSelect();
                                    }
                                    else
                                    {
                                        items[i].DoUnSelect();
                                    }
                                }
                            }
                        );
                        ++itemIndex;
                    }
                });
                ++titleIndex;
            }

            if (titles.Count > 0)
            {
                titles.First().Select();
            }
            character.SetRandomLiveMotionAndExpression();
        }

        private void Close()
        {
            foreach (var manager in titles)
            {
                Destroy(manager.gameObject);
            }

            titles.Clear();
            foreach (var item in items)
            {
                Destroy(item.gameObject);
            }

            items.Clear();
            type = null;
            currentTitle = -1;
            currentItem = -1;
            currentIsBuy = false;
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