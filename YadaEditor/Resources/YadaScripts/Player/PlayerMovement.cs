using System;
using YadaScriptsLib;

namespace YadaScripts
{
	public class PlayerMovement : Component
	{
		//Flags for control
		public bool isMoving;
		private bool isInitScripts = false;

		//Cardinal directions control
		private bool isForward;
		private bool isBackward;
		private bool isLeft;
		private bool isRight;

		public Vector2 inputDirection;

		private float[] previousVelocities;
		private Vector3 previousVelocity;

		private Entity mainCam;

		//Components
		public PlayerBehaviour playerBehaviour;
		private RigidBody playerBody;
		private Transform playerTransform;
		private Transform camTransform;
		private Vector3 lookingAt;
		private Vector3 playerDirection;
		private float move;
		private bool camIsLeft;

		static public float getXYAngle(Vector3 vec1, Vector3 vec2)
		{
			//take z cause flat horizontal plane is xz plane
			Vector2 a = new Vector2(vec1.x, vec1.z);
			Vector2 b = new Vector2(vec2.x, vec2.z);
			double d = (double)Vector2.Dot(a, b);
			double m = (double)(a.magnitude * b.magnitude);
			d = Math.Acos(d / m);

			float ret = (float)(d * (180 / Math.PI));

			if (ret > 180) return ret - 360;
			if (ret < 180) return ret + 360;

			return ret;
		}

		//check if vec2 is left of vec1
		static public bool checkIfLeft(Vector3 vec1, Vector3 vec2)
		{
			Vector3 a = new Vector3(vec1.x, vec1.z, vec1.y);
			Vector3 b = new Vector3(vec2.x, vec2.z, vec2.y);

			a = Vector3.Cross(a, b);

			if (a.z > 0) return true;

			return false;
		}

		private void Start()
		{
			isInitScripts = false;
		}

		public void InitScripts()
		{
			if (isInitScripts == false)
			{
				mainCam = SceneController.mainCamera.GetComponent<CameraBehaviour>().mainCamera;
				camTransform = mainCam.GetComponent<Transform>();
				playerBody = this.entity.GetComponent<RigidBody>(); 
				playerTransform = this.entity.GetComponent<Transform>();
				playerBehaviour = this.entity.GetComponent<PlayerBehaviour>();
				entity.GetComponent<PlayerBehaviour>().movementScript = this;
				lookingAt = new Vector3(SceneController.middlePoint.x - camTransform.globalPosition.x, 0, SceneController.middlePoint.z - camTransform.globalPosition.z);
				move = getXYAngle(lookingAt, -playerTransform.forward) + playerTransform.globalEulerAngles.y;
				isInitScripts = true;
			}
		}

		public void ListenInput()
		{
			inputDirection = new Vector2(0, 0);
			//if (playerBehaviour.isGamepad == true)
			//{
			//	ListenInputController();
			//}
			//else
			//{
			//	ListenInputKeyboard();
			//}
			ListenInputController();
			ListenInputKeyboard();
			inputDirection.Normalize();
		}

		public bool CheckIsMoving()
		{
			return isMoving;
		}

		public void SetMovement(float movementModifier)
		{
			if (isMoving)
			{
				//Vector2 directionToMove = new Vector2(0,0);

				//if (isForward)
				//	directionToMove.y = 1;
				//if (isBackward)
				//	directionToMove.y = -1;
				//if (isForward && isBackward)
				//	directionToMove.y = 0;

				//if (isLeft)
				//	directionToMove.x = -1;
				//if (isRight)
				//	directionToMove.x = 1;
				//if (isLeft && isRight)
				//	directionToMove.x = 0;

				float offsetDegrees = (float)Math.Atan2((double)inputDirection.y, (double)inputDirection.x) * 180.0f / (float)Math.PI;
				offsetDegrees -= 90.0f; //Since our 0 is the forward

				playerTransform.globalEulerAngles = Vector3.Lerp(playerTransform.globalEulerAngles, new Vector3(0, move + offsetDegrees, 0), SceneController.turningSpeed * Time.deltaTime);
				playerDirection = -playerTransform.forward;
				Vector3 newVector = playerDirection * SceneController.movementSpeed * movementModifier;
				playerBody.linearVelocity = new Vector3(newVector.x, playerBody.linearVelocity.y, newVector.z);

				isMoving = false;
			}
			else if (playerBehaviour.playerState != PlayerBehaviour.PlayerState.JUMPING)
			{
				playerBody.linearVelocity = new Vector3(0, playerBody.linearVelocity.y, 0);
				playerBody.AngularHalt();

				lookingAt = new Vector3(SceneController.middlePoint.x - camTransform.globalPosition.x, 0, SceneController.middlePoint.z - camTransform.globalPosition.z);

                if (SceneController.mainCamera.GetComponent<CameraBehaviour>().customFixedCamera == true)
                    lookingAt = SceneController.mainCamera.GetComponent<CameraBehaviour>().customLookPosition;// ; mainCam.GetComponent<CameraBehaviour>().Get;


                move = getXYAngle(lookingAt, new Vector3(0, 0, -1));
				camIsLeft = checkIfLeft(new Vector3(0, 0, -1), lookingAt); //check side of camera
				if (camIsLeft) move = -move;
			}
		}

		private void ListenInputController()
		{
			bool isAnyKeyPressed = false;

			if (Input.GetGamepadLeftStickY(playerBehaviour.gameID) > GAMEPADDEADZONES.LEFT_THUMBSTICK || 
				Input.GetGamepadLeftStickY(playerBehaviour.gameID) < -GAMEPADDEADZONES.LEFT_THUMBSTICK || 
				Input.GetGamepadLeftStickX(playerBehaviour.gameID) > GAMEPADDEADZONES.LEFT_THUMBSTICK || 
				Input.GetGamepadLeftStickX(playerBehaviour.gameID) < -GAMEPADDEADZONES.LEFT_THUMBSTICK)
			{
				isMoving = true;
				isAnyKeyPressed = true;
				inputDirection = new Vector2(Input.GetGamepadLeftStickX(playerBehaviour.gameID), Input.GetGamepadLeftStickY(playerBehaviour.gameID));
			}
			//if (Input.GetGamepadLeftStickY(playerBehaviour.gameID) > GAMEPADDEADZONES.LEFT_THUMBSTICK)
			//{
			//	isMoving = true;
			//	isForward = true;
			//	isAnyKeyPressed = true;

			//}
			//else isForward = false;

			////A
			//if (Input.GetGamepadLeftStickX(playerBehaviour.gameID) < -GAMEPADDEADZONES.LEFT_THUMBSTICK)
			//{
			//	isMoving = true;
			//	isLeft = true;
			//	isAnyKeyPressed = true;
			//}
			//else isLeft = false;

			////D
			//if (Input.GetGamepadLeftStickX(playerBehaviour.gameID) > GAMEPADDEADZONES.LEFT_THUMBSTICK)
			//{
			//	isMoving = true;
			//	isRight = true;
			//	isAnyKeyPressed = true;
			//}
			//else isRight = false;

			////S
			//if (Input.GetGamepadLeftStickY(playerBehaviour.gameID) < -GAMEPADDEADZONES.LEFT_THUMBSTICK)
			//{
			//	isMoving = true;
			//	isBackward = true;
			//	isAnyKeyPressed = true;
			//}
			//else isBackward = false;

			//if (!isAnyKeyPressed) isMoving = false;


			//else playerBehaviour.isGamepad = true;
		} //listeninputcontroller

		private void ListenInputKeyboard()
		{
			bool isAnyKeyPressed = false;

			//if ((playerBehaviour.isPlayer1 && Input.GetKey(KEYCODE.KEY_W)) || (!playerBehaviour.isPlayer1 && Input.GetKey(KEYCODE.KEY_UP)))
			//{
			//	isMoving = true;
			//	isForward = true;
			//	isAnyKeyPressed = true;
			//	inputDirection.y += 1;
			//}
			//else isForward = false;

			//if ((playerBehaviour.isPlayer1 && Input.GetKey(KEYCODE.KEY_A)) || (!playerBehaviour.isPlayer1 && Input.GetKey(KEYCODE.KEY_LEFT)))
			//{
			//	isMoving = true;
			//	isLeft = true;
			//	isAnyKeyPressed = true;
			//	inputDirection.x -= -1;
			//}
			//else isLeft = false;

			//if ((playerBehaviour.isPlayer1 && Input.GetKey(KEYCODE.KEY_D)) || (!playerBehaviour.isPlayer1 && Input.GetKey(KEYCODE.KEY_RIGHT)))
			//{
			//	isMoving = true;
			//	isRight = true;
			//	isAnyKeyPressed = true;
			//	inputDirection.x += -1;
			//}
			//else isRight = false;

			//if ((playerBehaviour.isPlayer1 && Input.GetKey(KEYCODE.KEY_S)) || (!playerBehaviour.isPlayer1 && Input.GetKey(KEYCODE.KEY_DOWN)))
			//{
			//	isMoving = true;
			//	isBackward = true;
			//	isAnyKeyPressed = true;
			//	inputDirection.y -= 1;
			//}
			//else isBackward = false;

			if ((playerBehaviour.isPlayer1 && Input.GetKey(KEYCODE.KEY_W)) || (!playerBehaviour.isPlayer1 && Input.GetKey(KEYCODE.KEY_UP)))
			{
				isMoving = true;
				isAnyKeyPressed = true;
				inputDirection.y += 1;
			}

			if ((playerBehaviour.isPlayer1 && Input.GetKey(KEYCODE.KEY_S)) || (!playerBehaviour.isPlayer1 && Input.GetKey(KEYCODE.KEY_DOWN)))
			{
				isMoving = true;
				isAnyKeyPressed = true;
				inputDirection.y -= 1;
			}

			if ((playerBehaviour.isPlayer1 && Input.GetKey(KEYCODE.KEY_A)) || (!playerBehaviour.isPlayer1 && Input.GetKey(KEYCODE.KEY_LEFT)))
			{
				isMoving = true;
				isAnyKeyPressed = true;
				inputDirection.x += -1;
			}

			if ((playerBehaviour.isPlayer1 && Input.GetKey(KEYCODE.KEY_D)) || (!playerBehaviour.isPlayer1 && Input.GetKey(KEYCODE.KEY_RIGHT)))
			{
				isMoving = true;
				isAnyKeyPressed = true;
				inputDirection.x -= -1;
			}

			//if (!isAnyKeyPressed) isMoving = false;



			//else playerBehaviour.isGamepad = false;
		} //listeninput
	} //playermovement
}