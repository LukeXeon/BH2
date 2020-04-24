using System;
using System.Linq;
using System.Threading.Tasks;
using Dash.Scripts.Cloud;
using Dash.Scripts.Levels.Config;
using Dash.Scripts.UI;
using ExitGames.Client.Photon;
using Michsky.UI.ModernUIPack;
using Photon.Pun;
using UnityEngine;

namespace Dash.Scripts.UIManager
{
    public class BeforeJoinRoomAction
    {
        private readonly NotificationManager onError;
        private readonly Animator loadingMask;

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
                LocalPlayerInfo.Prepare(player);
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