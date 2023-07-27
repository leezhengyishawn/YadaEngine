using System;
using YadaScriptsLib;

namespace YadaScripts
{
	public class SceneController : Component
	{
		//Inspector
		public Entity mainLightSource;
		public Entity mainCameraObject;
		public Entity mainPlayer1Object;
		public Entity mainPlayer2Object;
		public Entity skyboxObject;
		public bool isController;
		public bool canChargePunch;
		public bool canRoll;
		public static bool singleGamepadAssignToP1;

		//Hardcoding for playtest
		public static bool isUsingController;

		//Getting objects
		public static Light mainLightComponent;
		public static Entity mainCamera;
		public static Entity mainPlayer1;
		public static Entity mainPlayer2;

		//Values Settings
		public static int healthMaximum;
		public static int chargeCountMaximum;
		public static float clawMachineHeight;

		//Values Settings
		public static float movementSpeed;
		public static float turningSpeed;
		public static float dashMoveModifier;
		public static float rollMoveModifier;

		public static float punchForceNormal;
		public static float punchForceCharged;

		public static float movementMoveCap;
		public static float dashingMoveCap;
		public static float rollingMoveCap;

		//Timers and Cooldown (Maximum)
		public static float timerDashDurationMaximum;
		public static float timerDashCooldownMaximum;
		public static float timerPunchDurationMaximum;
		public static float timerPunchCooldownMaximum;
		public static float timerPunchFailMaximum;
		public static float timerChargeDurationMaximum;
		public static float timerStunDurationMaximum;
		public static float timerStunResetDurationMaximum;
		public static float timerInvisDurationMaximum;
		public static float timerRollDurationMaximum;
		public static float timerDeathDurationMaximum;
		public static float timerFootStepIntervalMaximum;

		//Upgrade Settings
		public static bool hasChargedAttack;
		public static bool hasRollAttack;
		public static bool isPromptOpen;

		//Upgrades Punch
		public static Vector3 punchDefaultOffset;
		public static Vector3 punchDefaultSize;
		public static Vector3 punchLongOffset;
		public static Vector3 punchLongSize;
		public static Vector3 punchWideOffset;
		public static Vector3 punchWideSize;

		public enum PunchUpgrade
		{
			DEFAULT_PUNCH,
			LONG_PUNCH,
			WIDE_PUNCH,
			SUPER_DASH,
			AUTO_CHARGED
		}

		//Player Threshold Setting
		public static float distanceWarning;
		public static float distanceReturn;

		//Getting Player Distance in Update
		public static Vector3 middlePoint;
		public static float distanceBetweenPlayers;

		//Other variables
		public static float defaultLightIntensity;
        public static Vector3 currentLightColour;
        public static Vector3 defaultLightColour;
        public static Vector3 combatLightColour;

		//Private Variable
		private Transform player1Transform;
		private Transform player2Transform;

		//Cutscene
		public static bool isInCutscene;
		public static bool isInMerchant;
		public static bool allowCutsceneBGM;
		public static bool isTransitioningLevel;
		public static bool finishedLevel;

		//Boss
		public static int bossHealth;
		public static int bossMinionWave;

		private bool hasInit;
		private bool hasInitIndicator = false;

		//Cheat Spawn
		public Entity spawnLocation1;
		public Entity spawnLocation2;
		public Entity spawnLocation3;

		void Start()
		{
			CheckComponents();
		}

		void Update()
		{
			CheckDistance();
			CheckPlayerDistance();
            CheckLightColour();
			//CheatCode();
		}

		private void CheckComponents()
		{
			if (hasInit == false)
			{
				isController = isUsingController;

				mainLightComponent = mainLightSource.GetComponent<Light>();

                defaultLightColour = mainLightComponent.color;
                //combatLightColour = new Vector3(60, 10, 80)/255.0f;
                combatLightColour = (defaultLightColour + new Vector3(66, 21, 129)/255.0f)/2.0f;

                mainCamera = mainCameraObject;
				mainPlayer1 = mainPlayer1Object;
				mainPlayer2 = mainPlayer2Object;

				skyboxObject.active = true;

				healthMaximum = 3;
				chargeCountMaximum = 2;
				clawMachineHeight = 5.0f;

				movementSpeed = 3.0f;
				turningSpeed = 45.0f;
				dashMoveModifier = 2000.0f;
				rollMoveModifier = 5000.0f;

				punchForceNormal = 2500.0f;
				punchForceCharged = 4000.0f;

				movementMoveCap = 3.0f;
				dashingMoveCap = 7.0f;
				rollingMoveCap = 9.0f;

				timerDashDurationMaximum = 0.2f;
				timerDashCooldownMaximum = 0.1f;
				timerPunchDurationMaximum = 0.3f;
				timerPunchCooldownMaximum = 0.1f;
				timerPunchFailMaximum = 0.3f;
				timerChargeDurationMaximum = 0.3f;
				timerStunDurationMaximum = 1.5f;
				timerStunResetDurationMaximum = 5.0f;
				timerInvisDurationMaximum = 1.0f;
				timerRollDurationMaximum = 0.7f;
				timerDeathDurationMaximum = 1.5f;
				timerFootStepIntervalMaximum = 0.3f;

				hasChargedAttack = canChargePunch;
				hasRollAttack = canRoll;
				isPromptOpen = false;

				punchDefaultOffset = new Vector3(0.0f, 0.4f, 0.4f);
				punchDefaultSize = new Vector3(0.6f, 0.3f, 0.3f);
				punchLongOffset = new Vector3(0.0f, 0.4f, 0.1f);
				punchLongSize = new Vector3(0.6f, 0.3f, 0.6f);
				punchWideOffset = new Vector3(0.0f, 0.4f, 0.4f);
				punchWideSize = new Vector3(1.2f, 0.3f, 0.3f);

				distanceWarning = 10.0f;
				distanceReturn = 15.0f;

				defaultLightIntensity = mainLightComponent.intensity;
				ToggleWorldLight(true);

				player1Transform = mainPlayer1.GetComponent<Transform>();
				player2Transform = mainPlayer2.GetComponent<Transform>();

				bossHealth = 1;
				bossMinionWave = 0;

				//Just change this bool if we want a single gamepad to belong to P1 or P2.
				singleGamepadAssignToP1 = false;

				//If only one gamepad is connected, we swap the gamepad ids if singleGamepadAssignToP1 is also set to false
				//Then, P2 will be the one controlled by gamepad
				if (Input.GetGamepadConnected(1) == false && Input.GetGamepadConnected(0) == true)
				{
					if (singleGamepadAssignToP1 == false)
					{
						mainPlayer1.GetComponent<PlayerBehaviour>().gameID = 1;
						mainPlayer2.GetComponent<PlayerBehaviour>().gameID = 0;
					}
				}

				isInCutscene = false;
				isInMerchant = false;
				allowCutsceneBGM = false;
				isTransitioningLevel = false;
				finishedLevel = false;

				hasInit = true;
			}
		}

		private void CheckDistance()
		{
			Vector3 player1Position = player1Transform.globalPosition;
			Vector3 player2Position = player2Transform.globalPosition;
			middlePoint = (player1Position + player2Position) / 2.0f;
            player1Position.y = 0.0f;
            player2Position.y = 0.0f;
            distanceBetweenPlayers = GetDistance(player1Position, player2Position);
		}

		private void CheckPlayerDistance()
		{
			if (!hasInitIndicator)
			{
				mainPlayer1.GetComponent<PlayerBehaviour>().InitialiseComponent();
				mainPlayer2.GetComponent<PlayerBehaviour>().InitialiseComponent();
				hasInitIndicator = true;
			}

			if (distanceBetweenPlayers > distanceReturn)
			{
				TogglePlayersDeath();
			}
			else if (distanceBetweenPlayers > distanceWarning)
			{
				mainPlayer1.GetComponent<PlayerBehaviour>().SetIndicatorFlash(true);
				mainPlayer2.GetComponent<PlayerBehaviour>().SetIndicatorFlash(true);
			}
			else
			{
				mainPlayer1.GetComponent<PlayerBehaviour>().SetIndicatorFlash(false);
				mainPlayer2.GetComponent<PlayerBehaviour>().SetIndicatorFlash(false);
			}
        }

        private void CheckLightColour()
        {
            combatLightColour = (defaultLightColour + new Vector3(66, 21, 129) / 255.0f) / 2.0f;
            mainLightComponent.color = Vector3.Lerp(mainLightComponent.color, currentLightColour, 2.0f * Time.deltaTime);
        }

        private void CheatCode()
        {
			if (Input.GetKeyPress(KEYCODE.KEY_F1))
            {
				Scene.ChangeScene("Level 1");
            }
			else if (Input.GetKeyPress(KEYCODE.KEY_F2))
			{
				Scene.ChangeScene("Level 2");
			}
			else if (Input.GetKeyPress(KEYCODE.KEY_F3))
			{
				Scene.ChangeScene("Level 3");
			}
			else if (Input.GetKeyPress(KEYCODE.KEY_F8))
			{
				Scene.ChangeScene("CinematicEpilogue");
			}
			if (Input.GetKeyPress(KEYCODE.KEY_3))
			{
				ToggleWorldLight(false);
				mainPlayer1.GetComponent<PlayerBehaviour>().SetDeath(false, true, true, spawnLocation1.GetComponent<Transform>().globalPosition + (Vector3.up * clawMachineHeight));
				mainPlayer2.GetComponent<PlayerBehaviour>().SetDeath(false, true, true, spawnLocation1.GetComponent<Transform>().globalPosition + (Vector3.up * clawMachineHeight));
			}
			else if (Input.GetKeyPress(KEYCODE.KEY_4))
			{
				ToggleWorldLight(false);
				mainPlayer1.GetComponent<PlayerBehaviour>().SetDeath(false, true, true, spawnLocation2.GetComponent<Transform>().globalPosition + (Vector3.up * clawMachineHeight));
				mainPlayer2.GetComponent<PlayerBehaviour>().SetDeath(false, true, true, spawnLocation2.GetComponent<Transform>().globalPosition + (Vector3.up * clawMachineHeight));
			}
			else if (Input.GetKeyPress(KEYCODE.KEY_5))
			{
				ToggleWorldLight(false);
				mainPlayer1.GetComponent<PlayerBehaviour>().SetDeath(false, true, true, spawnLocation3.GetComponent<Transform>().globalPosition + (Vector3.up * clawMachineHeight));
				mainPlayer2.GetComponent<PlayerBehaviour>().SetDeath(false, true, true, spawnLocation3.GetComponent<Transform>().globalPosition + (Vector3.up * clawMachineHeight));
			}
		}

		private float GetDistance(Vector3 point1, Vector3 point2)
		{
			return (point1 - point2).magnitude;
		}

		public static void SetPunchUpgrade(int upgradeValue)
		{
            mainPlayer1.GetComponent<PlayerBehaviour>().punchCheckObject.GetComponent<PunchCheck>().SetPunchUpgrade(upgradeValue);
            mainPlayer2.GetComponent<PlayerBehaviour>().punchCheckObject.GetComponent<PunchCheck>().SetPunchUpgrade(upgradeValue);
        }

		public static void ToggleWorldLight(bool trigger)
		{
			if (trigger == true)
			{
				mainLightComponent.intensity = defaultLightIntensity;
				mainPlayer1.GetComponent<PlayerBehaviour>().SetSpotlight(0.0f);
				mainPlayer2.GetComponent<PlayerBehaviour>().SetSpotlight(0.0f);
			}
			else
			{
				mainLightComponent.intensity = 0;
				mainPlayer1.GetComponent<PlayerBehaviour>().SetSpotlight(5.0f);
				mainPlayer2.GetComponent<PlayerBehaviour>().SetSpotlight(5.0f);
			}
		}

		public static void TogglePlayersDeath()
		{
			ToggleWorldLight(false);
			mainPlayer1.GetComponent<PlayerBehaviour>().SetDeath(false, false, true, Vector3.zero);
			mainPlayer2.GetComponent<PlayerBehaviour>().SetDeath(false, false, true, Vector3.zero);
		}

		public static float GetDistanceBetweenPlayers()
        {
			return distanceBetweenPlayers;
        }

		public static void SetCinematics(bool activateCutscene, bool cutsceneBGM, bool activePlayers)
		{
			isInCutscene = activateCutscene;
			allowCutsceneBGM = cutsceneBGM;
			//mainPlayer1.active = activePlayers;
			//mainPlayer2.active = activePlayers;
		}

		public static void SetMerchant(bool activateMerchant)
        {
			isInMerchant = activateMerchant;
		}
	}
}