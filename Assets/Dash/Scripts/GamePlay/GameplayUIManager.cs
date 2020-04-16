using System;
using Dash.Scripts.Config;
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
        public OnWeaponChangedEvent weaponChanged = new OnWeaponChangedEvent();
        
        private void Awake()
        {
            mask.gameObject.SetActive(true);
            var typeId = (int) PhotonNetwork.CurrentRoom.CustomProperties["typeId"];
            mask.sprite = GameGlobalInfoManager.guanQiaInfoTable[typeId].image;
            if (weaponChanged == null)
            {
                weaponChanged = new OnWeaponChangedEvent();
            }
        }

        public void Prepared()
        {
            mask.gameObject.SetActive(false);
        }

        [Serializable]
        public class OnWeaponChangedEvent : UnityEvent<WeaponInfoAsset>
        {
        }
    }
}
