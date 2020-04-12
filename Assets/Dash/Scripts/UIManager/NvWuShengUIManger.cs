using System;
using System.Collections;
using System.Linq;
using Dash.Scripts.Config;
using Dash.Scripts.GamePlay;
using Dash.Scripts.GamePlay.Info;
using Dash.Scripts.GamePlay.Spine;
using Dash.Scripts.Network.Cloud;
using Dash.Scripts.UI;
using Dash.Scripts.UIManager.ItemUIManager;
using Michsky.UI.ModernUIPack;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager
{
    public class NvWuShengUIManger : MonoBehaviour
    {
        [Header("网络")] public Animator loadingMask;
        public NotificationManager onError;
        [Header("UI")] public Camera renderCamera;
        public TextMeshProUGUI displayName;
        public SkeletonAnimation skeletonAnimation;
        public PlayerEquipsUIManager equipsUiManager;
        [Header("基本属性")] public TextMeshProUGUI gongJiLi;
        public TextMeshProUGUI fangYuLi;
        public TextMeshProUGUI shengMingZhi;
        public TextMeshProUGUI nengLiangZhi;
        [Header("圣痕和武器")] public GameObject lockMask;
        public GameObject tabs;
        public WeaponItemUIManager[] weapons;
        public ShengHenItemUIManager[] shengHens;
        [Header("玩家角色列表")] public GameObject playerListContent;
        [Header("选为出战")] public GameObject xuanWeiChuZhanRoot;
        public Button xuanWeiChuZhan;
        public Image xuanWeiChuZHanImage;
        public TextMeshProUGUI xuanWeiChuZhanText;
        public Color32 normalColor;
        public Color32 selectColor;
        public ZhuangBeiUIManager zhuangBeiUiManager;

        public Image expBar;
        public TextMeshProUGUI dengji;
        public TextMeshProUGUI expText;

        [Header("Assets")] public Material normalMaterial;
        public GameObject playerItemPrefab;

        private PlayerItemUIManager[] playerItems;

        private int currentIndex;
        private Equipments equipments;

        private void Awake()
        {
            var list = GameInfoManager.playerTable.Values.ToList();
            list.Sort((o1, o2) => o1.typeId.CompareTo(o2.typeId));
            playerItems = list.Select(o =>
            {
                var m = Instantiate(playerItemPrefab, playerListContent.transform).GetComponent<PlayerItemUIManager>();
                m.Init(o);
                return m;
            }).ToArray();
        }

        private void ApplyPlayers()
        {
            for (int i = 0; i < playerItems.Length; i++)
            {
                int index = i;
                var player = equipments.players.Values.FirstOrDefault(o => o.player.typeId == index)?.player;
                playerItems[i].Apply(
                    player,
                    () =>
                    {
                        if (currentIndex != index)
                        {
                            currentIndex = index;
                            ClearAllCallback();
                            xuanWeiChuZhanRoot.SetActive(false);
                            tabs.GetComponent<Canvas>().enabled = false;
                            lockMask.SetActive(true);
                            ApplyNormalAnim();
                        }
                    },
                    () =>
                    {
                        if (currentIndex != index)
                        {
                            currentIndex = index;
                            ApplyPanel();
                        }
                    }
                );
            }
        }

        public void Open(Equipments e)
        {
            currentIndex = 0;
            equipments = e;
            renderCamera.gameObject.SetActive(true);
            ApplyPlayers();
            ApplyPanel(false);
        }

        private void ClearAllCallback()
        {
            foreach (var item in shengHens)
            {
                item.Clear();
            }

            foreach (var item in weapons)
            {
                item.Clear();
            }

            xuanWeiChuZhan.onClick.RemoveAllListeners();
        }

        private void ApplyPanel(bool withAnim = true)
        {
            ClearAllCallback();
            ApplyPlayerChuZhan();
            ApplyInfoPanel(withAnim);
        }

        private void ApplyPlayerChuZhan()
        {
            if (CloudManager.GetCurrentPlayer()?.ObjectId ==
                equipments.players.Values.FirstOrDefault(o => o.player.typeId == currentIndex)?.player?.ObjectId)
            {
                xuanWeiChuZHanImage.color = selectColor;
                xuanWeiChuZhanText.text = "已出战";
            }
            else
            {
                xuanWeiChuZHanImage.color = normalColor;
                xuanWeiChuZhanText.text = "选为出战";
                xuanWeiChuZhan.onClick.AddListener(() =>
                {
                    var myPlayer = equipments.players.Values.FirstOrDefault(o => o.player.typeId == currentIndex)
                        ?.player;
                    if (myPlayer != null)
                    {
                        BeginWaitNetwork();
                        CloudManager.UpdateCurrentPlayer(myPlayer, e =>
                        {
                            EndWaitNetWork();
                            if (e != null)
                            {
                                onError.Show("网络错误", e);
                            }
                            else
                            {
                                xuanWeiChuZhan.onClick.RemoveAllListeners();
                                xuanWeiChuZhanText.text = "已出战";
                                xuanWeiChuZHanImage.color = selectColor;
                            }
                        });
                    }
                });
            }
        }

        private void ApplyInfoPanel(bool withAnim)
        {
            //unlock
            xuanWeiChuZhanRoot.SetActive(true);
            tabs.GetComponent<Canvas>().enabled = true;
            lockMask.SetActive(false);
            //info
            var inUse = equipments.players.Values.First(o => o.player.typeId == currentIndex);
            var player = inUse.player;
            var playerInfo = RuntimePlayerInfo.Build(player, inUse.shengHens);
            dengji.text = GameInfoManager.GetPlayerLevel(player.exp).count.ToString();
            gongJiLi.text = playerInfo.gongJiLi.ToString();
            fangYuLi.text = playerInfo.fangYuLi.ToString();
            shengMingZhi.text = playerInfo.shengMingZhi.ToString();
            nengLiangZhi.text = playerInfo.nengLiangZhi.ToString();
            var level = GameInfoManager.GetPlayerLevel(player.exp);
            expBar.fillAmount = (float) level.currentExp / level.maxExp;
            dengji.text = "LV " + level.count;
            expText.text = level.currentExp + "/" + level.maxExp;
            ApplyNormalAnim();
            ApplyWeaponCallbacks(inUse);
            ApplyShengHenCallbacks(inUse);
            EWeapon currentW = inUse.weapons.FirstOrDefault(o => o.weapon != null)?.weapon;
            ApplyPlayerWeapon(currentW, withAnim);
        }

        private void ApplyPlayerWeapon(EWeapon weapon, bool withAnim = true)
        {
            if (weapon != null)
            {
                var winfo = GameInfoManager.weaponTable[weapon.typeId];
                var list = SpineUtils.GenerateSpineReplaceInfo(winfo,
                    skeletonAnimation.Skeleton);
                foreach (var item in list)
                {
                    equipsUiManager.Equip(item.slotIndex, item.name, item.attachment);
                }

                if (withAnim)
                {
                    TiaoQiang(winfo.weaponType.matchName);
                }
                else
                {
                    skeletonAnimation.AnimationState.SetAnimation(
                        0,
                        winfo.weaponType.matchName + "_idle",
                        true
                    );
                }
            }
        }

        private void ApplyWeaponCallbacks(PlayerWithUsing inUse)
        {
            for (int i = 0; i < 3; i++)
            {
                var inUseWeapon = inUse.weapons[i];
                var weapon = inUseWeapon.weapon;
                Action onShow = null;
                Action onChaKan = null;
                Action onUnload = null;
                Action<EWeapon> onSelect = o =>
                {
                    BeginWaitNetwork();
                    CloudManager.ReplaceWeapon(inUseWeapon, o,
                        (unload, upload, e) =>
                        {
                            EndWaitNetWork();
                            if (e != null)
                            {
                                onError.Show("网络错误", e);
                            }
                            else
                            {
                                if (unload != null)
                                {
                                    equipments.weapons[unload.ObjectId] = unload;
                                }

                                if (upload != null)
                                {
                                    equipments.weapons[upload.ObjectId] = upload;
                                }

                                zhuangBeiUiManager.FastClose();
                                zhuangBeiUiManager.weaponInfo.Close();
                                ApplyPanel();
                            }
                        });
                };
                Action<EWeapon> onOpenPanel = o => { zhuangBeiUiManager.weaponInfo.Open("装备", o, onSelect, null); };
                if (weapon != null)
                {
                    if (inUse.weapons.Count(o => o.weapon != null) != 1)
                    {
                        onUnload = () => onSelect(null);
                    }

                    onShow = () => { zhuangBeiUiManager.weaponInfo.Open(null, weapon, null, null); };
                    onChaKan = () => { ApplyPlayerWeapon(weapon); };
                }

                Action onTihuan = () =>
                {
                    var toSelect = equipments.weapons.Values.Where(www => www.player == null).ToList();
                    zhuangBeiUiManager.OpenToSelectWeapon(toSelect, onUnload, onOpenPanel);
                };
                weapons[i].Apply(weapon, onShow, onChaKan, onTihuan);
            }


            foreach (var item in weapons)
            {
                item.SetShowTiHuan(true);
            }

            var isSingle = inUse.weapons.Count(o => o.weapon != null) == 1;
            if (isSingle)
            {
                var index = inUse.weapons.Single(o => o.weapon != null).index;
                weapons[index].SetShowTiHuan(false);
            }
        }

        private void ApplyShengHenCallbacks(PlayerWithUsing inUse)
        {
            for (int i = 0; i < 3; i++)
            {
                var inUseshengHen = inUse.shengHens[i];
                var shengHen = inUseshengHen.shengHen;
                Action onShow = null;
                Action onUnload = null;
                Action<EShengHen> onOpenPanel;
                Action<EShengHen> onSelect = o =>
                {
                    BeginWaitNetwork();
                    CloudManager.ReplaceShengHen(inUseshengHen, o,
                        (unload, upload, e) =>
                        {
                            EndWaitNetWork();
                            if (e != null)
                            {
                                onError.Show("网络错误", e);
                            }
                            else
                            {
                                if (unload != null)
                                {
                                    equipments.shengHens[unload.ObjectId] = unload;
                                }

                                if (upload != null)
                                {
                                    equipments.shengHens[upload.ObjectId] = upload;
                                }

                                zhuangBeiUiManager.FastClose();
                                zhuangBeiUiManager.shengHenInfo.Close();
                                ApplyPanel();
                            }
                        });
                };
                onOpenPanel = o => { zhuangBeiUiManager.shengHenInfo.Open("装备", o, onSelect, null); };
                if (shengHen != null)
                {
                    onUnload = () => onSelect(null);
                    onShow = () => { zhuangBeiUiManager.shengHenInfo.Open(null, shengHen, null, null); };
                }

                Action onTihuan = () =>
                {
                    var toSelect = equipments.shengHens.Values.Where(www => www.player == null).ToList();
                    zhuangBeiUiManager.OpenToSelectShengHen(toSelect, onUnload, onOpenPanel);
                };
                shengHens[i].Apply(shengHen, onShow, onTihuan);
            }
        }

        private void ApplyNormalAnim()
        {
            var index = currentIndex;
            var playerDisplayInfoAsset = GameInfoManager.playerTable[index];
            if (playerDisplayInfoAsset != null)
            {
                skeletonAnimation.Skeleton.SetToSetupPose();
                skeletonAnimation.AnimationState.ClearTracks();
                skeletonAnimation.skeletonDataAsset = playerDisplayInfoAsset.skel;
                skeletonAnimation.Initialize(true);
                displayName.text = playerDisplayInfoAsset.displayName;
                skeletonAnimation.AnimationState.SetAnimation(
                    0,
                    "qingshouqiang_idle",
                    true
                );
            }
        }

        public void Exit()
        {
            renderCamera.gameObject.SetActive(false);
        }

        private void WaitForFrame(Action action)
        {
            StartCoroutine(WaitForFrame0(action));
        }

        private IEnumerator WaitForFrame0(Action action)
        {
            yield return new WaitForEndOfFrame();
            action();
        }

        private void TiaoQiang(string weaponType)
        {
            StartCoroutine(TiaoQiang0(weaponType));
        }

        public IEnumerator TiaoQiang0(string weaponType)
        {
            yield return new WaitForEndOfFrame();
            var track = skeletonAnimation.AnimationState.SetAnimation(0,
                weaponType + "_taoqiang", false);
            track.AttachmentThreshold = 1f;
            track.MixDuration = 0f;
            var track0 =
                skeletonAnimation.AnimationState.AddAnimation(0, weaponType + "_idle", true, 0);
            track0.MixDuration = 0.2f;
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