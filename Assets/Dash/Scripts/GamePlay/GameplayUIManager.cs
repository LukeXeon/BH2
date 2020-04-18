using System;
using System.Linq;
using Dash.Scripts.Config;
using Dash.Scripts.GamePlay.Info;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Dash.Scripts.GamePlay
{
    public class GameplayUIManager : MonoBehaviour
    {
        public Button leftWeapon;
        public Button rightWeapon;
        public Image weapon;
        public Image mask;
        public OnWeaponChangedEvent weaponChanged;
        private int currentWeaponIndex = 0;

        [Serializable]
        public class OnWeaponChangedEvent : UnityEvent<WeaponInfoAsset>
        {
        }


        private void Awake()
        {
            mask.gameObject.SetActive(true);
            var typeId = (int) PhotonNetwork.CurrentRoom.CustomProperties["typeId"];
            mask.sprite = GameGlobalInfoManager.guanQiaInfoTable[typeId].image;
            if (weaponChanged == null)
            {
                weaponChanged = new OnWeaponChangedEvent();
            }

            leftWeapon.onClick.AddListener(() =>
            {
                var last = currentWeaponIndex;
                if (currentWeaponIndex < 0)
                {
                    currentWeaponIndex = GameplayInfoManager.weaponInfos.Count - 1;
                }
                --currentWeaponIndex;
                if (last == currentWeaponIndex)
                {
                    return;
                }
                var info = GameplayInfoManager.weaponInfos[currentWeaponIndex].Item1;
                weaponChanged.Invoke(info);
            });
            rightWeapon.onClick.AddListener(() =>
            {
                var last = currentWeaponIndex;
                currentWeaponIndex = (last + 1) % GameplayInfoManager.weaponInfos.Count;
                if (last == currentWeaponIndex)
                {
                    return;
                }

                var info = GameplayInfoManager.weaponInfos[currentWeaponIndex].Item1;
                weaponChanged.Invoke(info);
            });
            weaponChanged.AddListener(info => { weapon.sprite = info.sprite; });
        }



        public void OnPrepared()
        {
            mask.gameObject.SetActive(false);
            weapon.sprite = GameplayInfoManager.weaponInfos.First().Item1.sprite;
        }
    }
}