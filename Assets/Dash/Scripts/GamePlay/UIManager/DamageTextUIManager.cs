using System.Collections;
using Dash.Scripts.GamePlay.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.GamePlay.UIManager
{
    public class DamageTextUIManager : MonoBehaviour, IPoolLifecycle
    {
        private Transform follow;
        private CanvasScaler scaler;
        public Text text;

        public void Reusing()
        {
        }

        public void Recycle()
        {
            follow = null;
        }

        public void Initialize(Transform tf, int value)
        {
            scaler = text.canvas.GetComponent<CanvasScaler>();
            text.text = value.ToString();
            follow = tf;
            StartCoroutine(ToRecycle());
            Update();
        }

        private void Update()
        {
            if (follow) transform.localPosition = WorldToUI(follow.position + Vector3.up * 4);
        }

        private IEnumerator ToRecycle()
        {
            yield return new WaitForSeconds(1);
            ObjectPool.TryGlobalRelease(gameObject);
        }

        private Vector3 WorldToUI(Vector3 pos)
        {
            var resolutionX = scaler.referenceResolution.x;
            var resolutionY = scaler.referenceResolution.y;
            var viewportPos = Camera.main.WorldToViewportPoint(pos);
            var uiPos = new Vector3(viewportPos.x * resolutionX - resolutionX * 0.5f,
                viewportPos.y * resolutionY - resolutionY * 0.5f, 0);
            return uiPos;
        }
    }
}