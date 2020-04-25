using System;
using System.Threading.Tasks;
using Dash.Scripts.Cloud;
using Dash.Scripts.GamePlay.Config;
using Dash.Scripts.UI;
using Michsky.UI.ModernUIPack;
using UnityEngine;

namespace Dash.Scripts.UIManager
{
    public class BeforeJoinRoomAction
    {
        private readonly Animator loadingMask;
        private readonly NotificationManager onError;

        public BeforeJoinRoomAction(Animator loadingMask, NotificationManager onError)
        {
            this.loadingMask = loadingMask;
            this.onError = onError;
        }

        public async Task DoPrepare()
        {
            loadingMask.gameObject.SetActive(true);
            loadingMask.Play("Fade-in");
            try
            {
                var player = await CloudManager.GetCompletePlayer();
                GamePlayConfigManager.Prepare(player);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                loadingMask.gameObject.SetActive(false);
                onError.Show("网络错误", "拉取玩家信息失败");
                throw;
            }
        }
    }
}