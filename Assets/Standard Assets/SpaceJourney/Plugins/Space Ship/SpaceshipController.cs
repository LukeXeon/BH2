using System;
using UnityEngine;

public sealed class SpaceshipController : MonoBehaviour
{
	private const float IdleCameraDistanceSmooth = 0.85f;
	private static readonly Vector3[] RotationDirections = { Vector3.right, Vector3.up, Vector3.forward };
	public Transform CachedTransform { get; private set; }

	public Vector3 CameraOffsetVector
	{
		get
		{
			return new Vector3(0.0f, Mathf.Sin(m_camera.Angle * Mathf.Deg2Rad) * m_camera.Offset, -m_camera.Offset);
		}
	}

	public float CurrentSpeed
	{
		get
		{
			return Mathf.Lerp(m_spaceship.SpeedRange.x, m_spaceship.SpeedRange.y, SpeedFactor);
		}
	}

	public Vector4 RawInput { get; private set; }
	public Vector4 SmoothedInput { get; private set; }

	public float SpeedFactor
	{
		get
		{
			return m_spaceship.AccelerationCurve.Evaluate(SmoothedInput.w);
		}
	}

	private Transform m_cachedCameraTransform;

	[SerializeField, Tooltip("Camera options.")] private CameraSettings m_camera = new CameraSettings
	{
		Angle = 18.0f,
		Offset = 44.0f,
		PositionSmooth = 10.0f,
		RotationSmooth = 5.0f,
		OnRollCompensationFactor = 0.5f,
		LookAtPointOffset = new CameraLookAtPointOffsetSettings
		{
			OnIdle = new Vector2(0.0f, 10.0f),
			Smooth = new Vector2(30.0f, 30.0f),
			OnMaxSpeed = new Vector2(20.0f, -20.0f),
			OnTurn = new Vector2(30.0f, -30.0f)
		}
	};

	private float m_idleCameraDistance;
	private Quaternion m_initialAvatarRotation;
	private float m_initialCameraFOV;

	[SerializeField, Tooltip("Input options.")] private InputSettings m_input = new InputSettings
	{
		Mode = InputMode.KeyboardAndMouse,
		Response = new Vector4(6.0f, 6.0f, 6.0f, 0.75f),
		Keyboard = new KeyboardSettings
		{
			Sensitivity = 1.5f,
			SensitivityOnMaxSpeed = 1.0f
		},
		Mouse = new MouseSettings
		{
			ActiveArea = new Vector2(450.0f, 300.0f),
			MovementThreshold = 75.0f,
			Sensitivity = 1.0f,
			SensitivityOnMaxSpeed = 0.85f
		}
	};

	private Vector2 m_lookAtPointOffset;

	[SerializeField, Tooltip("Spaceship options.")] private SpaceshipSettings m_spaceship = new SpaceshipSettings
	{
		AccelerationCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f),
		BankAngleSmooth = 2.5f,
		Maneuverability = new Vector3(75.0f, 75.0f, -50.0f),
		MaxBankAngleOnTurn = 45.0f,
		SpeedRange = new Vector2(30.0f, 600.0f)
	};

	private void Awake()
	{
		RawInput = Vector4.zero;
		SmoothedInput = Vector4.zero;
		CachedTransform = transform;
		m_cachedCameraTransform = m_camera.TargetCamera.transform;
		m_idleCameraDistance = CameraOffsetVector.magnitude;
		m_initialAvatarRotation = m_spaceship.Avatar.localRotation;
		m_initialCameraFOV = m_camera.TargetCamera.fieldOfView;
		m_lookAtPointOffset = m_camera.LookAtPointOffset.OnIdle;

		m_cachedCameraTransform.position = CachedTransform.position + CameraOffsetVector;
	}

	private void LateUpdate()
	{
		UpdateCamera();
	}

	private void Update()
	{
		UpdateInput();
		UpdateOrientationAndPosition();
	}

	private void UpdateCamera()
	{
		Vector2 focalPointOnMoveOffset = Vector2.Lerp(m_camera.LookAtPointOffset.OnTurn,
			m_camera.LookAtPointOffset.OnMaxSpeed, SpeedFactor);

		m_lookAtPointOffset.x = Mathf.Lerp(
			m_lookAtPointOffset.x,
			Mathf.Lerp(
				m_camera.LookAtPointOffset.OnIdle.x,
				focalPointOnMoveOffset.x * Mathf.Sign(SmoothedInput.y),
				Mathf.Abs(SmoothedInput.y)),
			m_camera.LookAtPointOffset.Smooth.x * Time.deltaTime);

		m_lookAtPointOffset.y = Mathf.Lerp(
			m_lookAtPointOffset.y,
			Mathf.Lerp(
				m_camera.LookAtPointOffset.OnIdle.y,
				focalPointOnMoveOffset.y * Mathf.Sign(SmoothedInput.x),
				Mathf.Abs(SmoothedInput.x)),
			m_camera.LookAtPointOffset.Smooth.y * Time.deltaTime);

		Vector3 lookTargetPosition = CachedTransform.position + CachedTransform.right * m_lookAtPointOffset.x +
			CachedTransform.up * m_lookAtPointOffset.y;

		Vector3 lookTargetUpVector = (CachedTransform.up + CachedTransform.right *
			SmoothedInput.z * m_camera.OnRollCompensationFactor).normalized;

		Quaternion targetCameraRotation = Quaternion.LookRotation(lookTargetPosition -
			m_cachedCameraTransform.position, lookTargetUpVector);

		m_cachedCameraTransform.rotation = Quaternion.Slerp(m_cachedCameraTransform.rotation,
			targetCameraRotation, m_camera.RotationSmooth * Time.deltaTime);

		Vector3 cameraOffset = CachedTransform.TransformDirection(CameraOffsetVector);

		m_cachedCameraTransform.position = Vector3.Lerp(m_cachedCameraTransform.position,
			CachedTransform.position + cameraOffset, m_camera.PositionSmooth * Time.deltaTime);

		float idleCameraDistance = cameraOffset.magnitude + (cameraOffset.normalized * m_spaceship.SpeedRange.x *
			Time.deltaTime / m_camera.PositionSmooth).magnitude;

		m_idleCameraDistance = Mathf.Lerp(m_idleCameraDistance, idleCameraDistance, IdleCameraDistanceSmooth * Time.deltaTime);
		float baseFrustumHeight = 2.0f * m_idleCameraDistance * Mathf.Tan(m_initialCameraFOV * 0.5f * Mathf.Deg2Rad);
		m_camera.TargetCamera.fieldOfView = 2.0f * Mathf.Atan(baseFrustumHeight * 0.5f / Vector3.Distance(
			CachedTransform.position, m_cachedCameraTransform.position)) * Mathf.Rad2Deg;
	}

	private void UpdateInput()
	{
		float currentKeyboardSensitivity = Mathf.Lerp(m_input.Keyboard.Sensitivity,
			m_input.Keyboard.SensitivityOnMaxSpeed, SpeedFactor);

		//Calc raw input.
		Vector4 currentRawInput = Vector4.zero;
		switch (m_input.Mode)
		{
			case InputMode.Keyboard:
				currentRawInput.x = Input.GetAxis(m_input.Keyboard.InputNames.AxisX) * currentKeyboardSensitivity;
				currentRawInput.y = Input.GetAxis(m_input.Keyboard.InputNames.AxisY) * currentKeyboardSensitivity;
				break;

			case InputMode.KeyboardAndMouse:
				float currentMouseSensitivity = Mathf.Lerp(m_input.Mouse.Sensitivity,
					m_input.Mouse.SensitivityOnMaxSpeed, SpeedFactor);

				Vector2 mouseOffsetFromScreenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f) -
					(Vector2)Input.mousePosition;

				if (Mathf.Abs(mouseOffsetFromScreenCenter.y) > m_input.Mouse.MovementThreshold)
				{
					float verticalOffsetFromCenter = mouseOffsetFromScreenCenter.y - Mathf.Sign(mouseOffsetFromScreenCenter.y) *
						m_input.Mouse.MovementThreshold;

					currentRawInput.x = Mathf.Clamp(verticalOffsetFromCenter / (m_input.Mouse.ActiveArea.y -
						m_input.Mouse.MovementThreshold), -1.0f, 1.0f) * currentMouseSensitivity;
				}

				if (Mathf.Abs(mouseOffsetFromScreenCenter.x) > m_input.Mouse.MovementThreshold)
				{
					float horizontalOffsetFromCenter = mouseOffsetFromScreenCenter.x - Mathf.Sign(mouseOffsetFromScreenCenter.x) *
						m_input.Mouse.MovementThreshold;

					currentRawInput.y = -Mathf.Clamp(horizontalOffsetFromCenter / (m_input.Mouse.ActiveArea.x -
						m_input.Mouse.MovementThreshold), -1.0f, 1.0f) * currentMouseSensitivity;
				}

				break;
		}

		currentRawInput.z = Input.GetAxis(m_input.Keyboard.InputNames.AxisZ) * currentKeyboardSensitivity;
		currentRawInput.w = Input.GetButton(m_input.Keyboard.InputNames.Throttle) ? 1.0f : 0.0f;

		//Calc smoothed input.
		Vector4 currentSmoothedInput = Vector4.zero;
		for (int i = 0; i < 4; ++i)
		{
			currentSmoothedInput[i] = Mathf.Lerp(SmoothedInput[i], currentRawInput[i], m_input.Response[i] * Time.deltaTime);
		}

		RawInput = currentRawInput;
		SmoothedInput = currentSmoothedInput;
	}

	private void UpdateOrientationAndPosition()
	{
		for (int i = 0; i < 3; ++i)
		{
			CachedTransform.localRotation *= Quaternion.AngleAxis(SmoothedInput[i] *
				m_spaceship.Maneuverability[i] * Time.deltaTime, RotationDirections[i]);
		}

		CachedTransform.localPosition += CachedTransform.forward * CurrentSpeed * Time.deltaTime;

		m_spaceship.Avatar.localRotation = Quaternion.Slerp(
			m_spaceship.Avatar.localRotation,
			m_initialAvatarRotation * Quaternion.AngleAxis(-SmoothedInput.y * m_spaceship.MaxBankAngleOnTurn, Vector3.forward),
			m_spaceship.BankAngleSmooth * Time.deltaTime);
	}

	[Serializable]
	private struct CameraLookAtPointOffsetSettings
	{
		[Tooltip("Offset of the look-at point (relative to the spaceship) when flying straight with a minimum speed.")] public Vector2 OnIdle;
		[Tooltip("Offset of the look-at point (relative to the spaceship) when flying or turning with a maximum speed.")] public Vector2 OnMaxSpeed;
		[Tooltip("Offset of the look-at point (relative to the spaceship) when turning with a minimum speed.")] public Vector2 OnTurn;
		[Tooltip("How fast the look-at point interpolates to the desired value. Higher = faster.")] public Vector2 Smooth;
	}

	[Serializable]
	private struct CameraSettings
	{
		[Tooltip("Angle of the camera. 0 = behind, 90 = top-down.")] public float Angle;
		[Tooltip("Look-at point options.")] public CameraLookAtPointOffsetSettings LookAtPointOffset;
		[Tooltip("Distance between the camera and the spaceship.")] public float Offset;
		[Tooltip("Tilt of the camera when the spaceship is doing a roll. 0 = no tilt.")] public float OnRollCompensationFactor;
		[Tooltip("How fast the camera follows the spaceship's position. Higer = faster.")] public float PositionSmooth;
		[Tooltip("How fast the camera follows the spaceship's rotation. Higer = faster.")] public float RotationSmooth;
		[Tooltip("Camera object.")] public Camera TargetCamera;
	}

	private enum InputMode
	{
		Keyboard,
		KeyboardAndMouse
	}

	[Serializable]
	private struct InputSettings
	{
		[Tooltip("Keyboard options.")] public KeyboardSettings Keyboard;
		[Tooltip("Input mode.")] public InputMode Mode;
		[Tooltip("Mouse options.")] public MouseSettings Mouse;
		[Tooltip("How fast the input interpolates to the desired value. Higher = faster.")] public Vector4 Response;
	}

	[Serializable]
	private struct KeyboardInputNames
	{
		[Tooltip("Rotation around x-axis (vertical movement).")] public string AxisX;
		[Tooltip("Rotation around y-axis (horizontal movement).")] public string AxisY;
		[Tooltip("Rotation around z-axis (roll).")] public string AxisZ;
		[Tooltip("Speed control.")] public string Throttle;
	}

	[Serializable]
	private struct KeyboardSettings
	{
		[Tooltip("Names of input axes (from InputManager).")] public KeyboardInputNames InputNames;
		[Tooltip("Keyboard sensitivity when flying with a minimum speed.")] public float Sensitivity;
		[Tooltip("Keyboard sensitivity when flying with a maximum speed.")] public float SensitivityOnMaxSpeed;
	}

	[Serializable]
	private struct MouseSettings
	{
		[Tooltip("Mouse input is set to a maximum when the cursor is out of bounds of that area.")] public Vector2 ActiveArea;
		[Tooltip("How far the cursor should be moved from the center of the screen to make the spaceship turn.")] public float MovementThreshold;
		[Tooltip("Mouse sensitivity when flying with a minimum speed.")] public float Sensitivity;
		[Tooltip("Mouse sensitivity when flying with a maximum speed.")] public float SensitivityOnMaxSpeed;
	}

	[Serializable]
	private struct SpaceshipSettings
	{
		[Tooltip("Defines how speed changes over time.")] public AnimationCurve AccelerationCurve;
		[Tooltip("The spaceship's model.")] public Transform Avatar;
		[Tooltip("How fast the spaceship tilts when doing a sideways turns. Higher = faster.")] public float BankAngleSmooth;
		[Tooltip("How fast the spaceship turns. Higher = faster.")] public Vector3 Maneuverability;
		[Tooltip("Maximum tilt of the spaceship when doing a sideways turns.")] public float MaxBankAngleOnTurn;
		[Tooltip("Minimum and maximum speed of the spaceship.")] public Vector2 SpeedRange;
	}
}
