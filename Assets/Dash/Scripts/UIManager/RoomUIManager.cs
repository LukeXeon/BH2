using System;
using agora_gaming_rtc;
using Michsky.UI.ModernUIPack;
using Photon.Pun;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager
{
	public class RoomUIManager : MonoBehaviourPunCallbacks
	{

		[Serializable]
		public struct PlayerItem
		{
			public SkeletonAnimation skel;
			public TextMeshProUGUI name;
			public GameObject zhunBeiMark;
		}

		[Header("UI")] public TextMeshProUGUI idText;
		public PlayerItem[] items;
		public Button back;
		public Button start;
		public SwitchManager openYuYing;
		public SwitchManager openMaiKeFeng;
		public Animator animator;

		[Header("Asset")] public Color readyColor;
		public Color unReadyColor;

		private void Awake()
		{
			back.onClick.AddListener(() => { PhotonNetwork.LeaveRoom(); });
			var rtcEngine = IRtcEngine.QueryEngine();
			rtcEngine.OnJoinChannelSuccess += OnJoinChannelSuccess;
			rtcEngine.OnError += OnError;
		}

		private void OnError(int error, string msg)
		{
			Debug.Log(msg);
		}

		private void OnJoinChannelSuccess(string channelname, uint uid, int elapsed)
		{
			Debug.Log(channelname);
		}

		private void OnDestroy()
		{
			var rtcEngine = IRtcEngine.QueryEngine();
			if (rtcEngine != null)
			{
				rtcEngine.OnJoinChannelSuccess -= OnJoinChannelSuccess;
				rtcEngine.OnError -= OnError;
			}

		}

		public override void OnJoinedRoom()
		{
			animator.Play("Fade-in");
			var room = PhotonNetwork.CurrentRoom;
			var rtcEngine = IRtcEngine.QueryEngine();
			rtcEngine.JoinChannel(room.Name, "", 0u);

		}

		public override void OnLeftRoom()
		{
			animator.Play("Fade-out");
			var rtcEngine = IRtcEngine.QueryEngine();
			rtcEngine?.LeaveChannel();
		}
	}
}
