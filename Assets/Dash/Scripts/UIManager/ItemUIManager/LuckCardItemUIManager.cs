using Dash.Scripts.Config;
using Dash.Scripts.Cloud;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager.ItemUIManager
{
    public class LuckCardItemUIManager : MonoBehaviour
    {
        public Image image;
        public TextMeshProUGUI title;
        public TextMeshProUGUI content;

        public void Apply(LuckDrawResult result)
        {
            switch (result.resultType)
            {
                case LuckDrawResult.Type.UnLockPlayer:
                {
                    var info = GameInfoManager.playerTable[result.typeId];
                    image.sprite = info.icon;
                    title.text = info.displayName;
                    content.text = "角色已解锁";
                }
                    break;
                case LuckDrawResult.Type.AddPlayerExp:
                {
                    var info = GameInfoManager.playerTable[result.typeId];
                    image.sprite = info.icon;
                    title.text = info.displayName;
                    content.text = "已转换为经验值（+" + GameInfoManager.levelInfo.playerLuckDrawExpAddOnce + "）";
                }
                    break;
                case LuckDrawResult.Type.Weapon:
                {
                    var info = GameInfoManager.weaponTable[result.typeId];
                    image.sprite = info.sprite;
                    title.text = info.displayName;
                    content.text = "武器已获得";
                }
                    break;
                case LuckDrawResult.Type.ShengHen:
                {
                    var info = GameInfoManager.shengHenTable[result.typeId];
                    image.sprite = info.image;
                    title.text = info.displayName;
                    content.text = "圣痕已获得";
                }
                    break;
            }
        }
    }
}