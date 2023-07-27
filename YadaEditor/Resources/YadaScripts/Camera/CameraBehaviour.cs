using System;
using YadaScriptsLib;

namespace YadaScripts
{
	public class CameraBehaviour : Component
	{
		//public Vector3 camOffset;
		public Entity mainCamera;

		public float moveSmoothTime = 1.0f;
		public float panSmoothTime = 1.0f;
		public float angleSmoothTime = 1.0f;
		public float cutsceneMoveSmoothTime = 1.0f;
		public float cutscenePanSmoothTime = 1.0f;
		public float cutsceneAngleSmoothTime = 1.0f;
		public float followSpeed;
		public float turningSpeed;
		public float panningSpeed;
		public float randomShakeAmount;
		public float randomSwayAmount;
		public float timerSwayDurationMaximum;

		private Random getRandom;
		private Transform followTransform;
		private Transform cameraTransform;
		private Vector3 defaultLocalPosition;
		private Vector3 targetLocalPosition;
		private Vector3 savedLocalPosBoss;
		private Vector3 savedLocalPosCutscene;
		private Vector3 targetFollowPosition;
		private Vector3 targetLookPosition;
		private Vector3 targetLookOffset;
		private Vector3 shakePositionOffset;
		private Vector3 zoomOffsetPosition;
		private float timerShakeDurationCurrent;
		private float timerSwayDurationCurrent;
		private float distanceModifier;
		private bool isShakingCam;
		private bool isFreezeCamAngle;
		private bool isFreezeCamMovement;
		private bool isTeleportShot;

		//Merchant && Cutscene
		private Vector3 cutsceneMainPosition;
		private Vector3 cutsceneLookPosition;
		private Vector3 cutsceneStationPosition;
		private Vector3 cutsceneSwayOffset;
		private bool isInCutscene;

		private Vector3 followVelocity = Vector3.zero;
		private Vector3 panVelocity = Vector3.zero;
		private Vector3 rotateVelocity = Vector3.zero;

		public Vector3 customLookPosition;
		private Vector3 customFollowPosition;
		public bool customFixedCamera;

		private void Start()
		{
			getRandom = new Random();
			followTransform = this.entity.GetComponent<Transform>();
			cameraTransform = mainCamera.GetComponent<Transform>();
			defaultLocalPosition = cameraTransform.localPosition;
			targetLocalPosition = defaultLocalPosition;
			cameraTransform.localPosition = targetLocalPosition;
		}

		private void Update()
		{
			GetFollowPoint();
			CheckMovement();
			CheckZoom();
			CheckPan();
			CheckRotation();
			CheckShake();
			CheckTimer();
		}

		private void GetFollowPoint()
		{
			if (customFixedCamera == true)
			{
				targetLookPosition = customLookPosition + (Vector3.up * 0.5f);// + targetLookOffset;
				targetFollowPosition = customFollowPosition + (Vector3.up * 0.5f);
			}
			else
			{
				targetLookPosition = SceneController.middlePoint + (Vector3.up * 0.5f) + targetLookOffset;
				targetFollowPosition = SceneController.middlePoint + (Vector3.up * 0.5f);
			}
		}

		private void CheckMovement()
		{
			if (isInCutscene == true)
			{
				float distanceMoveSpeed = GetDistance(followTransform.globalPosition, cutsceneMainPosition + cutsceneSwayOffset);
				if (distanceMoveSpeed < 2.0f)
				{
					distanceMoveSpeed = 2.0f;
				}
				followTransform.globalPosition = Vector3.SmoothDamp(followTransform.globalPosition, cutsceneMainPosition + cutsceneSwayOffset, ref followVelocity, cutsceneMoveSmoothTime, followSpeed * distanceMoveSpeed, Time.deltaTime);
			}
			else
			{
				float distanceMoveSpeed = GetDistance(followTransform.globalPosition, targetFollowPosition);
				if (distanceMoveSpeed < 2.0f)
				{
					distanceMoveSpeed = 2.0f;
				}
				followTransform.globalPosition = Vector3.SmoothDamp(followTransform.globalPosition, targetFollowPosition, ref followVelocity, moveSmoothTime, followSpeed * distanceMoveSpeed, Time.deltaTime);
			}
		}

		private void CheckZoom()
		{
			distanceModifier = SceneController.distanceBetweenPlayers;
			if (distanceModifier > SceneController.distanceReturn / 1.5f)
			{
				distanceModifier = SceneController.distanceReturn / 1.5f;
			}
			zoomOffsetPosition = cameraTransform.localPosition.normalized * distanceModifier;
		}

		private void CheckRotation()
		{
			if (isInCutscene == true)
			{
				if (isTeleportShot == true)
                {
					//Vector3 camPos = cameraTransform.globalPosition;
					//Vector3 cutLook = cutsceneLookPosition;
					//cutsceneLookPosition;
					Quaternion quat = Quaternion.LookAt(cameraTransform.globalPosition, cutsceneLookPosition);
					cameraTransform.localEulerAngles = quat.eulerAngles + shakePositionOffset;
					cameraTransform.localRotation = quat;
				}
				else if (isFreezeCamAngle == false)
				{
					Quaternion quat = Quaternion.LookAt(cameraTransform.globalPosition, cutsceneLookPosition);
					cameraTransform.localEulerAngles = Vector3.SmoothDamp(cameraTransform.localEulerAngles, quat.eulerAngles, ref rotateVelocity, cutsceneAngleSmoothTime, turningSpeed, Time.deltaTime);
					cameraTransform.localEulerAngles += shakePositionOffset;
				}
			}
			else
			{
				Quaternion quat = Quaternion.LookAt(cameraTransform.globalPosition, targetLookPosition);
				cameraTransform.localEulerAngles = Vector3.SmoothDamp(cameraTransform.localEulerAngles, quat.eulerAngles, ref rotateVelocity, angleSmoothTime, turningSpeed, Time.deltaTime);
				cameraTransform.localEulerAngles += shakePositionOffset;
			}
		}

		private void CheckPan()
		{
			if (isInCutscene == true)
			{
				if (isTeleportShot == true)
                {
					cameraTransform.localPosition = cutsceneStationPosition;
				}
				else if (isFreezeCamMovement == false)
				{
					float distancePanSpeed = GetDistance(cameraTransform.localPosition, cutsceneStationPosition);
					if (distancePanSpeed > 8.0f)
					{
						distancePanSpeed = 8.0f;
					}
					cameraTransform.localPosition = Vector3.SmoothDamp(cameraTransform.localPosition, cutsceneStationPosition, ref panVelocity, cutscenePanSmoothTime, panningSpeed * distancePanSpeed, Time.deltaTime);
				}
			}
			else
			{
				float distancePanSpeed = GetDistance(cameraTransform.localPosition, targetLocalPosition + zoomOffsetPosition);
				if (distancePanSpeed > 8.0f)
				{
					distancePanSpeed = 8.0f;
				}
				else if (distancePanSpeed < 2.0f)
				{
					distancePanSpeed = 2.0f;
				}
				cameraTransform.localPosition = Vector3.SmoothDamp(cameraTransform.localPosition, targetLocalPosition + zoomOffsetPosition, ref panVelocity, panSmoothTime, panningSpeed * distancePanSpeed, Time.deltaTime);
			}
		}

		private void CheckShake()
		{
			if (isShakingCam == true)
			{
				float shakeValue = randomShakeAmount * timerShakeDurationCurrent * 0.25f;
				shakePositionOffset = new Vector3(GetRandomNumber(-shakeValue, shakeValue), GetRandomNumber(-shakeValue, shakeValue), GetRandomNumber(-shakeValue, shakeValue));
			}
			else
			{
				shakePositionOffset = Vector3.zero;
			}
		}

		private void CheckTimer()
		{
			if (isShakingCam == true)
			{
				if (timerShakeDurationCurrent > 0)
				{
					timerShakeDurationCurrent -= Time.deltaTime;
				}
				else
				{
					ShakeCamera(false);
				}
			}
			if (isInCutscene == true)
			{
				if (timerSwayDurationCurrent < timerSwayDurationMaximum)
				{
					timerSwayDurationCurrent += Time.deltaTime;
				}
				else
				{
					cutsceneSwayOffset = new Vector3(GetRandomNumber(-randomSwayAmount, randomSwayAmount), GetRandomNumber(-randomSwayAmount, randomSwayAmount), GetRandomNumber(-randomSwayAmount, randomSwayAmount) * 2.0f);
					timerSwayDurationCurrent = 0.0f;
				}
			}
		}

		private float GetDistance(Vector3 point1, Vector3 point2)
		{
			return (point1 - point2).magnitudeSq;
		}

		private float GetRandomNumber(float min, float max)
		{
			lock (getRandom)
			{
				return (float)getRandom.NextDouble() * (max - min) + min;
			}
		}

		public void EnterCutscenePosition(Vector3 followPos, Vector3 lookPos, Vector3 stationPos, bool lockRotate, bool lockMovement, bool teleportCam)
		{
			isInCutscene = true;
			savedLocalPosCutscene = targetLocalPosition;
			cutsceneSwayOffset = Vector3.zero;

			isFreezeCamAngle = lockRotate;
			isFreezeCamMovement = lockMovement;
			isTeleportShot = teleportCam;

			cutsceneMainPosition = followPos;
			cutsceneStationPosition = stationPos;
			cutsceneLookPosition = lookPos;

			if (teleportCam == true)
            {
				followTransform.globalPosition = cutsceneMainPosition;
			}
        }

		public void ExitCutscenePosition()
		{
			targetLocalPosition = savedLocalPosCutscene;
			isInCutscene = false;
			isFreezeCamAngle = false;
			isFreezeCamMovement = false;
			isTeleportShot = false;
		}

		public void SetLocalPosition(Vector3 localPos, Vector3 lookPos)
		{
			targetLocalPosition = localPos;
			targetLookOffset = lookPos;
		}

		public void ResetLocalPosition()
		{
			targetLocalPosition = defaultLocalPosition;
		}

		public void ShakeCamera(bool check, float shakingTimer = 0.5f)
		{
			timerShakeDurationCurrent = shakingTimer;
			isShakingCam = check;
		}

		public bool HasReachedStation()
		{
			return GetDistance(cameraTransform.localPosition, cutsceneStationPosition) < 1.0f;
		}

		public void SetCustomLookAndFollow(bool setCusom, Vector3 lookPosition, Vector3 stationPositon)
        {
			customFixedCamera = setCusom;
			customLookPosition = lookPosition;
			customFollowPosition = lookPosition;
			if (setCusom == true)
            {
				savedLocalPosBoss = targetLocalPosition;
				targetLocalPosition = stationPositon;
			}
			else
            {
				targetLocalPosition = savedLocalPosBoss;
			}
		}
	}
}