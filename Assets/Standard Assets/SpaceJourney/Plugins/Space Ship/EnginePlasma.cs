using System;
using UnityEngine;

public sealed class EnginePlasma : MonoBehaviour
{
	[SerializeField] private ParticleEmitterProperty[] m_emitters;
	[SerializeField] private LensFlareProperty[] m_lensFlares;
	[SerializeField] private SpaceshipController m_spaceshipController;

	private void Awake()
	{
		foreach (ParticleEmitterProperty emitterProperty in m_emitters)
		{
			emitterProperty.Initialize();
		}

		foreach (LensFlareProperty lensFlareProperty in m_lensFlares)
		{
			lensFlareProperty.Initialize();
		}
	}

	private void Update()
	{
		float speedFactor = m_spaceshipController.SpeedFactor;

		foreach (ParticleEmitterProperty emitterProperty in m_emitters)
		{
			emitterProperty.Update(speedFactor);
		}

		foreach (LensFlareProperty lensFlareProperty in m_lensFlares)
		{
			lensFlareProperty.Update(speedFactor);
		}
	}

	[Serializable]
	private sealed class LensFlareProperty : UpdateableProperty
	{
		public override float Value
		{
			get
			{
				return m_lensFlare.brightness;
			}

			protected set
			{
				m_lensFlare.brightness = value;
			}
		}

		[SerializeField] private LensFlare m_lensFlare;
	}

	[Serializable]
	private sealed class ParticleEmitterProperty : UpdateableProperty
	{
		public override float Value
		{
			get
			{
				return m_emitter.emissionRate;
			}

			protected set
			{
				m_emitter.emissionRate = value;
			}
		}

		[SerializeField] private ParticleSystem m_emitter;
	}

	private abstract class UpdateableProperty
	{
		public abstract float Value { get; protected set; }
		private float m_initialValue;
		[SerializeField] private float m_targetValue;
		[SerializeField] private Mode m_updateValueMode;

		public void Initialize()
		{
			m_initialValue = Value;
		}

		public void Update(float factor)
		{
			switch (m_updateValueMode)
			{
				case Mode.Add:
					Value = m_initialValue + m_targetValue * factor;
					break;

				case Mode.Multiply:
					Value = m_initialValue * Mathf.Lerp(1.0f, m_targetValue, factor);
					break;
			}
		}

		private enum Mode
		{
			Add,
			Multiply
		}
	}
}
