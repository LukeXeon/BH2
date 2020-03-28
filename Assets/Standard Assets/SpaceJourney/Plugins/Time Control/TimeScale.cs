using UnityEngine;

public sealed class TimeScale : MonoBehaviour
{
	[SerializeField] private float m_timeScale = 1.0f;

	private void Update()
	{
		Time.timeScale = Mathf.Clamp(m_timeScale, 0.0f, 100.0f);
	}
}
