using System.Collections;
using Dash.Scripts.Levels.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.Levels.UIManager
{
    public class DamageTextUIManager : MonoBehaviour, IPoolLifecycle
    {
        public Text text;
        private Transform follow;
        private CanvasScaler scaler;

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
            if (follow)
            {
                transform.localPosition = WorldToUI(follow.position + Vector3.up * 4);
            }
        }

        public void Reusing()
        {
        }

        private IEnumerator ToRecycle()
        {
            yield return new WaitForSeconds(1);
            ObjectPool.TryGlobalRelease(gameObject);
        }

        private Vector3 WorldToUI(Vector3 pos)
        {
            float resolutionX = scaler.referenceResolution.x;
            float resolutionY = scaler.referenceResolution.y;
            Vector3 viewportPos = Camera.main.WorldToViewportPoint(pos);
            Vector3 uiPos = new Vector3(viewportPos.x * resolutionX - resolutionX * 0.5f,
                viewportPos.y * resolutionY - resolutionY * 0.5f, 0);
            return uiPos;
        }

        public void Recycle()
        {
            follow = null;
        }
    }
}