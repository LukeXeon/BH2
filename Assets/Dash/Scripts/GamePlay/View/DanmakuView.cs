using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Dash.Scripts.Gameplay.View
{
    public class DanmakuView : BulletView
    {
        public string[] texts;
        public Color[] colors;
        public TextMeshProUGUI text;

        public override void Initialize(int id, int layer, int damage)
        {
            base.Initialize(id, layer, damage);
            text.text = texts[Random.Range(0, texts.Length)];
            text.color = colors[Random.Range(0, colors.Length)];
        }


        protected override void OnTriggerEnter(Collider other)
        {
            if (photonView.IsMine && other.gameObject.layer == targetLayer)
            {
                var view = other.GetComponent<ActorView>();
                if (view && !view.isDie)
                {
                    view.photonView.RPC(nameof(view.OnDamage), RpcTarget.All,
                        viewId, damage);
                }
            }
        }
    }
}