using System;
using System.Linq;
using Dash.Scripts.Cloud;
using Dash.Scripts.GamePlay.Info;
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

        public void DoAction(Action action)
        {
            loadingMask.gameObject.SetActive(true);
            loadingMask.Play("Fade-in");
            CloudManager.GetCompletePlayer((player, s) =>
            {
                if (s != null)
                {
                    loadingMask.gameObject.SetActive(false);
                    onError.Show("网络错误", "拉取玩家信息失败");
                }
                else
                {
                    GameplayInfoManager.current = player;
                    action();
                }
            });
        }
    }
}