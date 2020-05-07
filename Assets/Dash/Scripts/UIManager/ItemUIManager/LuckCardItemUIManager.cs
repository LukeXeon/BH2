using Dash.Scripts.Cloud;
using Dash.Scripts.Setting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager.ItemUIManager
{
    public class LuckCardItemUIManager : MonoBehaviour
    {
        public TextMeshProUGUI content;
        public Image image;
        public TextMeshProUGUI title;

        public void Apply(LuckDrawResult result)
        {
            switch (result.resultType)
            {
                case LuckDrawResult.Type.UnLockPlayer:
                {
                    var info = GameSettingManager.playerTable[result.typeId];
                    image.sprite = info.icon;
                    title.text = info.displayName;
                    content.text = "角色已解锁";
                }
                    break;
                case LuckDrawResult.Type.AddPlayerExp:
                {
                    var info = GameSettingManager.playerTable[result.typeId];
                    image.sprite = info.icon;
                    title.text = info.displayName;
                    content.text = "已转换为经验值（+" + GameSettingManager.setting.playerLuckDrawExpAddOnce + "）";
                }
                    break;
                case LuckDrawResult.Type.Weapon:
                {
                    var info = GameSettingManager.weaponTable[result.typeId];
                    image.sprite = info.sprite;
                    title.text = info.displayName;
                    content.text = "武器已获得";
                }
                    break;
                case LuckDrawResult.Type.Seal:
                {
                    var info = GameSettingManager.SealsTable[result.typeId];
                    image.sprite = info.image;
                    title.text = info.displayName;
                    content.text = "圣痕已获得";
                }
                    break;
            }
        }
    }
}