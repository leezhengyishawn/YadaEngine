using System;
using YadaScriptsLib;

namespace YadaScripts
{
	public class PlayerBehaviour : Component
	{
		public PlayerState playerState;
		public PlayerState playerStatePrevious;
		public enum PlayerState
		{
			IDLE,
			WALK,
			DASH,
			PING,
			JUMPING,
			FALLING,
			LAND,
			ROLLING,
			CHARGING,
			PUNCHING,
			PUNCH_FAIL,
			DAMAGED,
			STUN,
			DEATH,
			ABDUCTED,
			RESPAWN
		}

		public enum AnimationState //This needs to be perfectly parallel with the model animaton list
		{
			WALK,       //Use masteranim_04
			IDLE_LOOK,  //Use masteranim_04
			RUN,
			CHARGE_IDLE,
			CHARGE_WALK,
			PUNCH_RIGHT,
			PUNCH_FAIL,
			PUNCH_UPPERCUT,
			PUNCHED_UP,
			FALLING,
			LAND,
			STUN,
			DROWN,
			DAMAGED_RIGHT,
			CLAW_MACHINE,
			LOOK_UP,
			ROLL,
			PUNCH_LEFT,
			PING
		}

		//Game Object References
		public PlayerMovement movementScript;
		public Entity playerModel;
		public Entity punchCheckObject;
		public Entity groundCheckObject;
		public Entity chargingParticle;
		public Entity punchParticle;
		public Entity LandPredictor;
		public Entity directionIndicator;
		public Entity spotlightObject;
		public Entity pingIcon;

		//Bool Settings
		public uint gameID;
		public bool isPlayer1;
		public bool isGamepad;
		public bool isGodMode;

		//Bool Check (Do not touch)
		public bool isOnGround;
		public bool isInRespawnSequence;
		public bool isReadyToRespawn;
		public bool waitForPlayers;

		//Private Variables
		private Random getRandom;
		private Animator playerAnimator;
		private RigidBody playerBody;
		private Collider playerCollider;
		private Renderer playerRenderer;
		private Renderer indicatorRenderer;
		private Transform playerTransform;
		private Transform cutsceneLookAtTransform;
		private PunchCheck punchCheck;
		private GroundCheck groundCheck;
		private Vector3 mySavedPoint;
		private Vector3 mySpawnPoint;
		private Vector3 myDeathPoint;
		private Vector3 pushRollDirection;
		private Vector4 indicatorColour;
		private int healthCurrent;
		public int chargeCountCurrent;
		private int punchCountCurrent;

		//Timers and Cooldown (Current)
		private float timerDashDurationCurrent;
		private float timerDashCooldownCurrent;
		private float timerPunchDurationCurrent;
		private float timerPunchCooldownCurrent;
		private float timerPunchFailCurrent;
		private float timerChargeDurationCurrent;
		private float timerStunDurationCurrent;
		private float timerStunResetDurationCurrent;
		private float timerInvisDurationCurrent;
		private float timerRollDurationCurrent;
		private float timerDeathDurationCurrent;
		private float timerFootStepIntervalCurrent;

		//Bool Check
		public bool isDashing;
		public bool isDashOnCooldown;
		private bool isJumping;
		private bool isPunching;
		private bool isPunchOnCooldown;
		public bool isPunchCharging;
		private bool isFailedPunch;
		private bool isDamaged;
		public bool isStunned;
		private bool isInvis;
		private bool isOnJuices;
		private bool isRolling;
		private bool hasPunched;
		private bool isDead;
		private bool isAbducted;
		private bool isRespawning;
		public bool isPunchingLeft = true;
		private bool isEmptyHit;
		private bool isDrowning;
		private bool hasReachedSky;
		private bool isPlayingClawSFX;

		//Bool Spawn
		private bool hasSpawnStunParticle;
		private bool hasSpawnDashParticle;

		//Init Setting
		private bool hasInit;

		private void Start()
		{
			hasInit = false;
		}

		private void Update()
		{
			InitialiseComponent();
			CheckTimer();
			CheckGround();
			CheckInput();
			CheckDecision();
			CheckState();
		}

		private void FixedUpdate()
		{
			CheckMovement();
		}

		private void CheckMovement()
		{
			if (punchCheck != null)
			{
				punchCheckObject.GetComponent<Transform>().globalPosition = playerTransform.globalPosition + (-playerTransform.forward);
				punchCheckObject.GetComponent<Transform>().globalRotation = playerTransform.globalRotation;
			}
		}

		public void InitialiseComponent()
		{
			if (hasInit == false)
			{
				getRandom = new Random();

				playerAnimator = playerModel.GetComponent<Animator>();
				playerBody = this.entity.GetComponent<RigidBody>();
				playerCollider = this.entity.GetComponent<Collider>();
				playerRenderer = playerModel.GetComponent<Renderer>();
				playerTransform = this.entity.GetComponent<Transform>();
				punchCheck = punchCheckObject.GetComponent<PunchCheck>();
				groundCheck = groundCheckObject.GetComponent<GroundCheck>();

				indicatorRenderer = directionIndicator.GetComponent<Renderer>();
				indicatorColour = indicatorRenderer.color;

				chargingParticle.GetComponent<ParticleEmitter>().Pause();
				punchParticle.GetComponent<ParticleEmitter>().Pause();

				movementScript = this.entity.GetComponent<PlayerMovement>();
				movementScript.InitScripts();

				chargeCountCurrent = 1;
				healthCurrent = SceneController.healthMaximum;

				punchCheck.SetOwner(this.entity, isPlayer1);
				punchCheck.reInit(); //more init
				groundCheck.SetOwner(this.entity);

				SetSavePoint(playerTransform.globalPosition);

				//isGamepad = true;

				hasInit = true;
			}
		}

		private void CheckGround()
		{
			isOnGround = groundCheck.isTouchingGround;
		}

		private void CheckInput()
		{
			//Gamepad and Keyboard Switch
			//if (Input.GetKeyPress(KEYCODE.KEY_1))
			//{
			//	isGamepad = !isGamepad;
			//}
			//isGamepad = true;

			//Ping
			if ((/*isGamepad &&*/ Input.GetGamepadButtonPress(GAMEPADCODE.GAMEPAD_LEFT_SHOULDER, gameID) || Input.GetGamepadButtonPress(GAMEPADCODE.GAMEPAD_RIGHT_SHOULDER, gameID)) ||
				(/*!isGamepad &&*/ ((isPlayer1 && Input.GetKeyPress(KEYCODE.KEY_X)) || (!isPlayer1 && Input.GetKeyPress(KEYCODE.KEY_I)))))
			{
				if (isPlayer1)
				{
					pingIcon.GetComponent<PopUp>().SpritePopUp(new Vector3(0.05f, 2f, 0f), new Vector3(0.8f, 1.0f, 0.0001f), 1.0f);
					AudioController.PlaySFX(SFXType.MELON_OI);
				}

				else
				{
					pingIcon.GetComponent<PopUp>().SpritePopUp(new Vector3(0.0f, 2f, 0f), new Vector3(0.8f, 1.0f, 0.0001f), 1.0f);
					AudioController.PlaySFX(SFXType.PINEAPPLE_OI);
				}

				Entity pingParticle = Entity.InstantiatePrefab("ParticleBurst");
				Entity pingParticle2 = Entity.InstantiatePrefab("ParticleRing");
				Vector3 newParticlePos = new Vector3(playerTransform.globalPosition.x, playerTransform.globalPosition.y + 2.1f, playerTransform.globalPosition.z);
				pingParticle.GetComponent<Transform>().globalPosition = newParticlePos;
				pingParticle2.GetComponent<Transform>().globalPosition = newParticlePos;

				pingParticle.GetComponent<Transform>().parent = playerTransform;
				pingParticle2.GetComponent<Transform>().parent = playerTransform;
				AudioController.PlaySFX("SFX Button Next page");

			}

            //Cheatcode Start
            //if (Input.GetKeyPress(KEYCODE.KEY_2))
            //{
            //    isGodMode = !isGodMode;
            //}
            //if (isGodMode == true && Input.GetKeyPress(KEYCODE.KEY_SPACE))
            //{
            //    PlayerJump(Vector3.up, SceneController.punchForceCharged);
            //}
            //Cheatcode End

            if (SceneController.isInCutscene == false && isInRespawnSequence == false)
			{
				//Movement
				if (movementScript != null && movementScript.active)
				{
					movementScript.ListenInput();
				}

				//Dash
				if (isDashing == false && isDashOnCooldown == false && isOnGround == true)
				{
					if ((/*isGamepad &&*/ Input.GetGamepadButtonPress(GAMEPADCODE.GAMEPAD_A, gameID)) || 
						(/*!isGamepad &&*/ ((isPlayer1 && Input.GetKeyPress(KEYCODE.KEY_C)) || (!isPlayer1 && Input.GetKeyPress(KEYCODE.KEY_O)))))
					{
						isDashing = true;
					}
				}

				//Punch
				if (isPunching == false && isPunchOnCooldown == false /*&& isOnGround == true*/)
				{
					//Holding down onto punch button and is charging
					if (SceneController.hasChargedAttack == true && CheckPunchInput(true) == true)
					{
						isPunchCharging = true;
					}
					//Releasing punch button after charging
					else if (SceneController.hasChargedAttack == true && isPunchCharging == true)
					{
						isPunching = true;
					}
					//Normal punch
					else if (CheckPunchInput(false) == true)
					{
						isPunching = true;
					}
				}
				else
				{
					SetCharging(false);
				}
			}
			else
			{
				SetCharging(false);
			}
		}

		private bool CheckPunchInput(bool detectButtonHold)
		{
			if (!detectButtonHold && /*isGamepad &&*/ Input.GetGamepadButtonPress(GAMEPADCODE.GAMEPAD_B, gameID))
			{
				return true;
			}
			else if (detectButtonHold && /*isGamepad &&*/ Input.GetGamepadButton(GAMEPADCODE.GAMEPAD_B, gameID))
			{
				return true;
			}
			else
			{
				if (isPlayer1)
				{
					if (!detectButtonHold && /*!isGamepad &&*/ Input.GetKeyPress(KEYCODE.KEY_V))
					{
						return true;
					}
					else if (detectButtonHold && /*!isGamepad &&*/ Input.GetKey(KEYCODE.KEY_V))
					{
						return true;
					}
				}
				else
				{
					if (!detectButtonHold && /*!isGamepad &&*/ Input.GetKeyPress(KEYCODE.KEY_P))
					{
						return true;
					}
					else if (detectButtonHold && /*!isGamepad &&*/ Input.GetKey(KEYCODE.KEY_P))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void CheckDecision()
		{
			if (SceneController.isInCutscene == true || SceneController.isTransitioningLevel == true)
            {
				playerState = PlayerState.IDLE;
            }
			else if (isRespawning == true)
			{
				playerState = PlayerState.RESPAWN;
			}
			else if (isAbducted == true)
			{
				playerState = PlayerState.ABDUCTED;
			}
			else if (isDead == true)
			{
				playerState = PlayerState.DEATH;
			}
			else if (healthCurrent <= 0 || isStunned == true)
			{
				playerState = PlayerState.STUN;
			}
			else if (isRolling == true)
			{
				playerState = PlayerState.ROLLING;
			}
			else if (playerBody.linearVelocity.y < -0.1f && isOnGround == false)
			{
				playerState = PlayerState.FALLING;
			}
			else if (Math.Abs(playerBody.linearVelocity.y) <= 0.1f && isOnGround == true && isJumping == true)
			{
				playerState = PlayerState.LAND;
			}
			else if (isDamaged == true)
			{
				playerState = PlayerState.DAMAGED;
			}
			else if (playerBody.linearVelocity.y > 2.0f && isJumping == true)
			{
				playerState = PlayerState.JUMPING;
			}
			else if (isPunching == true)
			{
				playerState = PlayerState.PUNCHING;
			}
			else if (isFailedPunch == true)
			{
				playerState = PlayerState.PUNCH_FAIL;
			}
			else if (isPunchCharging == true)
			{
				playerState = PlayerState.CHARGING;
			}
			else if (isDashing == true)
			{
				playerState = PlayerState.DASH;
			}
			else if (movementScript.isMoving == true)
			{
				playerState = PlayerState.WALK;
			}
			else
			{
				playerState = PlayerState.IDLE;
			}
		}

		private void CheckState()
		{

			switch (playerState)
			{
				case PlayerState.IDLE:
					if (SceneController.isInCutscene == true || SceneController.isTransitioningLevel == true)
                    {
						playerBody.linearVelocity = new Vector3(0.0f, playerBody.linearVelocity.y, 0.0f);
						if (cutsceneLookAtTransform != null)
                        {
							Quaternion quat = Quaternion.LookAt(playerTransform.globalPosition, cutsceneLookAtTransform.globalPosition);
							Quaternion newRotation = Quaternion.RotateTowards(playerTransform.globalRotation, quat, 5.0f);
							playerTransform.globalRotation = newRotation;
						}
					}
					else
                    {
						isInRespawnSequence = false;
						PlayerMove(0.0f);
					}
					PlayerAnimation((int)AnimationState.IDLE_LOOK, -1, true); // Correspond to idling
					isJumping = false;
					break;

				case PlayerState.WALK:
					PlayerMove(1.0f);
					PlayerAnimation((int)AnimationState.WALK, -1, true); // Correspond to walking
					break;

				case PlayerState.DASH:
					PlayerDash(-playerTransform.forward, SceneController.dashMoveModifier);
					PlayerAnimation((int)AnimationState.RUN, 1, true); // Correspond to Dash
					if (hasSpawnDashParticle == false)
					{
						Entity dashParticle = Entity.InstantiatePrefab("dash");
						Vector3 newParticlePos = new Vector3(playerTransform.globalPosition.x, playerTransform.globalPosition.y, playerTransform.globalPosition.z);
						dashParticle.GetComponent<Transform>().globalPosition = newParticlePos;
						AudioController.PlaySFX("SFX Player Dash");
						hasSpawnDashParticle = true;
					}
					break;

				case PlayerState.JUMPING:
					PlayerMove(1.0f);
					PlayerAnimation((int)AnimationState.PUNCHED_UP, 1, true);
					break;

				case PlayerState.FALLING:
					if (isInRespawnSequence == true)
					{
						playerBody.linearVelocity = new Vector3(0.0f, playerBody.linearVelocity.y, 0.0f);
					}
					else
					{
						PlayerMove(1.0f);
					}
					PlayerAnimation((int)AnimationState.FALLING, -1, true); // Correspond to Falling
					isJumping = true;
					isDamaged = false;
					break;

				case PlayerState.LAND:
					PlayerAnimation((int)AnimationState.LAND, 1, true);  // Correspond to Landing
					if (isInRespawnSequence == true)
					{
						isInvis = true;
						isInRespawnSequence = false;
					}
					isJumping = false;
					isDamaged = false;
					break;

				case PlayerState.ROLLING:
					PlayerAnimation((int)AnimationState.ROLL, -1, true); // Correspond to Rolling
					PlayerRoll(pushRollDirection, SceneController.rollMoveModifier);
					break;

				case PlayerState.CHARGING:
					if (movementScript.isMoving == true)
					{
						//Play moving charging
						if (isPunchingLeft)
						{/*
							playerModel.GetComponent<Transform>().localScale = new Vector3(playerModel.GetComponent<Transform>().localScale.x, 
																						   playerModel.GetComponent<Transform>().localScale.y, 
																						  -playerModel.GetComponent<Transform>().localScale.z);*/
							PlayerAnimation((int)AnimationState.CHARGE_WALK, -1, true);
						}
						else
						{/*
                            playerModel.GetComponent<Transform>().localScale = new Vector3(playerModel.GetComponent<Transform>().localScale.x, 
																						   playerModel.GetComponent<Transform>().localScale.y, 
																						   playerModel.GetComponent<Transform>().localScale.z);*/
							PlayerAnimation((int)AnimationState.CHARGE_WALK, -1, true);
						}
					}
					else
					{
						//Play idle charging
						if (isPunchingLeft)
						{/*
							playerModel.GetComponent<Transform>().localScale = new Vector3(playerModel.GetComponent<Transform>().localScale.x, 
																						   playerModel.GetComponent<Transform>().localScale.y, 
																						  -playerModel.GetComponent<Transform>().localScale.z);*/
							PlayerAnimation((int)AnimationState.CHARGE_IDLE, -1, true);
						}
						else
						{/*
                            playerModel.GetComponent<Transform>().localScale = new Vector3(playerModel.GetComponent<Transform>().localScale.x, 
																						   playerModel.GetComponent<Transform>().localScale.y, 
																						   playerModel.GetComponent<Transform>().localScale.z);*/
							PlayerAnimation((int)AnimationState.CHARGE_IDLE, -1, true);
						}
					}
					if (chargingParticle.GetComponent<ParticleEmitter>().isPlaying == false)
					{
						chargingParticle.GetComponent<ParticleEmitter>().Play();
					}
					if (chargeCountCurrent == SceneController.chargeCountMaximum)
					{
						if (isPlayer1 == true)
						{
							SetRenderColourCustom(0.6f, GetRandomNumber(0.6f, 1.0f), 1f);
						}
						else
						{
							SetRenderColourCustom(GetRandomNumber(0.6f, 0.9f), 0.6f, GetRandomNumber(0.6f, 0.9f));
						}
					}
					PlayerMove(0.5f);
					break;

				case PlayerState.PUNCHING:
					SetRenderColourAll(1.0f);
					//PlayerMove(0.0f);
					chargingParticle.GetComponent<ParticleEmitter>().Pause();
					if (hasPunched == false)
					{
						isEmptyHit = true;
						if (isPunchCharging == true)
						{
							ActivatePunch(true, chargeCountCurrent);
						}
						else
						{
							ActivatePunch(true, 1);
						}

						isPunchingLeft = !isPunchingLeft;

						//float dist = punchCheck.

						//                  if (/*punchCheck.isOverlappingPartner &&*/ chargeCountCurrent > 0 && isPunchCharging)
						//                  {
						//                      PlayerAnimation((int)AnimationState.PUNCH_UPPERCUT, 1, true);

						//	//Only do a minor jump if one person punching the other
						//	if ((SceneController.mainPlayer1.GetComponent<PlayerBehaviour>().isPunchCharging && SceneController.mainPlayer2.GetComponent<PlayerBehaviour>().isPunchCharging) == false)
						//		PlayerJump(Vector3.up, SceneController.punchForceCharged * 0.5f);
						//}
						//                  else
						//                  {
						//                      Transform playerModelTransform = playerModel.GetComponent<Transform>();
						//                      if (isPunchingLeft)
						//                          PlayerAnimation((int)AnimationState.PUNCH_LEFT, 1, true);
						//                      else
						//                          PlayerAnimation((int)AnimationState.PUNCH_RIGHT, 1, true);
						//                  }

						//The animation for upper cutting ally is in Punch Check - Shawn

						if (isPunchingLeft)
                            PlayerAnimation((int)AnimationState.PUNCH_LEFT, 1, true);

                        else
                            PlayerAnimation((int)AnimationState.PUNCH_RIGHT, 1, true);
                        hasPunched = true;
					}

					break;

				case PlayerState.PUNCH_FAIL:
					//PlayerMove(0.0f);
					
					PlayerAnimation((int)AnimationState.PUNCH_FAIL, 1, true); //Damaged
					break;

				case PlayerState.DAMAGED:
					PlayerAnimation((int)AnimationState.DAMAGED_RIGHT, 1, true); //Damaged
					break;

				case PlayerState.STUN:
					PlayerAnimation((int)AnimationState.STUN, -1, true); // Correspond to Stun
					if (hasSpawnStunParticle == false)
					{
						Entity stunParticle = Entity.InstantiatePrefab("stars");
						Vector3 newParticlePos = new Vector3(playerTransform.globalPosition.x, playerTransform.globalPosition.y + 1f, playerTransform.globalPosition.z);
						stunParticle.GetComponent<Transform>().globalPosition = newParticlePos;
						hasSpawnStunParticle = true;
					}
					isStunned = true;
					break;

				case PlayerState.DEATH:
					playerBody.linearVelocity = new Vector3(0.0f, playerBody.linearVelocity.y, 0.0f);
					if (isDrowning == true)
					{
						PlayerAnimation((int)AnimationState.DROWN, 1, true); //Drowning Anim
					}
					else
					{
						//PlayerAnimation(13, -1, true); //Handpick Anim
					}
					break;

				case PlayerState.ABDUCTED:
					PlayerAnimation((int)AnimationState.CLAW_MACHINE, -1, true);
					if (hasReachedSky == false)
					{
						if (isPlayingClawSFX == false)
						{
							if (isPlayer1)
							{
								AudioController.PlaySFX("Melon_Clawmachine", AudioController.AudioVolume.VOLUME_150);
							}
							else
							{
								AudioController.PlaySFX("Pineapple_Clawmachine");
							}
							isPlayingClawSFX = true;
						}
						if (GetDistance(playerTransform.globalPosition, myDeathPoint) > 0.1f)
						{
							playerTransform.globalPosition = Vector3.MoveTowards(playerTransform.globalPosition, myDeathPoint, SceneController.clawMachineHeight * Time.deltaTime);
						}
						else
						{
							isPlayingClawSFX = false;
							hasReachedSky = true;
						}
					}
					else
					{
						if (GetDistance(playerTransform.globalPosition, mySpawnPoint) > 0.05f)
						{
							float lerpDistance = GetDistance(playerTransform.globalPosition, mySpawnPoint);
							if (lerpDistance > 10.0f)
							{
								lerpDistance = 10.0f;
							}
							else if (lerpDistance < 2.0f)
							{
								lerpDistance = 2.0f;
							}
							playerTransform.globalPosition = Vector3.MoveTowards(playerTransform.globalPosition, mySpawnPoint, lerpDistance * Time.deltaTime);
						}
						else
						{
							isReadyToRespawn = true;
							if (waitForPlayers == true)
							{
								if (SceneController.mainPlayer1.GetComponent<PlayerBehaviour>().isReadyToRespawn == true && SceneController.mainPlayer2.GetComponent<PlayerBehaviour>().isReadyToRespawn == true)
								{
									SceneController.ToggleWorldLight(true);
									isRespawning = true;
									isAbducted = false;
								}
							}
							else
							{
								isRespawning = true;
								isAbducted = false;
							}
						}
					}
					break;

				case PlayerState.RESPAWN:
					healthCurrent = SceneController.healthMaximum;
					playerBody.active = true;
					playerBody.useGravity = true;
					playerCollider.isTrigger = false;
					playerCollider.active = true;
					isDrowning = false;
					hasReachedSky = false;
					isDamaged = false;
					isJumping = true;
					isRespawning = false;
					//isInRespawnSequence = false;
					break;
			}
			if (playerStatePrevious != playerState)
			{
				playerStatePrevious = playerState;
				if (isPlayer1 == true)
				{
					//Console.WriteLine("PLAYER 1 STATE: " + playerState);
				}
				else
				{
					//Console.WriteLine("PLAYER 2 STATE: " + playerState);
				}
			}
		}

		public void PlayerAnimation(int index, int count, bool canAnimate)
		{
			if (playerAnimator.animationIndex == (int)AnimationState.PUNCH_UPPERCUT)
			{
				if (playerAnimator.animateCount > 0)
					return;
			}

			if (playerAnimator.animationIndex != index)
			{
				playerAnimator.animationIndex = index;
				playerAnimator.animateCount = count;
				playerAnimator.animate = canAnimate;
			}
		}

		private void CheckTimer()
		{
			if (isPunching == true)
			{
				if (timerPunchDurationCurrent < SceneController.timerPunchDurationMaximum)
				{
					timerPunchDurationCurrent += Time.deltaTime;
				}
				else
				{
					if (isEmptyHit == true)
					{
						AudioController.PlaySFX("SFX Punch Empty", AudioController.AudioVolume.VOLUME_25);
					}
					ActivatePunch(false, 0);
					SetPunching(false);
					SetCharging(false);
					isPunchOnCooldown = true;
					hasPunched = false;
				}
			}
			else
			{
				ActivatePunch(false, 0);
				if (isPunchCharging == true)
				{
					if (chargeCountCurrent < SceneController.chargeCountMaximum && punchCheck.savedUpgradeValue == (int)SceneController.PunchUpgrade.AUTO_CHARGED)
                    {
						AudioController.PlaySFX("SFX Player Charge", AudioController.AudioVolume.VOLUME_50);
						Entity chargeParticle = Entity.InstantiatePrefab("charge");
						Vector3 newParticlePos = new Vector3(playerTransform.globalPosition.x, playerTransform.globalPosition.y + 0.2f, playerTransform.globalPosition.z + 0.2f);
						chargeParticle.GetComponent<Transform>().globalPosition = newParticlePos;
						chargeCountCurrent = SceneController.chargeCountMaximum;
						timerChargeDurationCurrent = 0.0f;
					}
					else
                    {
						if (timerChargeDurationCurrent < SceneController.timerChargeDurationMaximum)
						{
							timerChargeDurationCurrent += Time.deltaTime;
						}
						else
						{
							if (chargeCountCurrent < SceneController.chargeCountMaximum)
							{
								AudioController.PlaySFX("SFX Player Charge");
								Entity chargeParticle = Entity.InstantiatePrefab("charge");
								Vector3 newParticlePos = new Vector3(playerTransform.globalPosition.x, playerTransform.globalPosition.y + 0.2f, playerTransform.globalPosition.z + 0.2f);
								chargeParticle.GetComponent<Transform>().globalPosition = newParticlePos;
								++chargeCountCurrent;
								timerChargeDurationCurrent = 0.0f;
							}
						}
					}
				}
				if (isFailedPunch == true)
				{
					if (timerPunchFailCurrent < SceneController.timerPunchFailMaximum)
					{
						timerPunchFailCurrent += Time.deltaTime;
					}
					else
					{
						timerPunchFailCurrent = 0.0f;
						isFailedPunch = false;
					}
				}
				else if (isPunchOnCooldown == true)
				{
                    //if (timerPunchCooldownCurrent < SceneController.timerPunchCooldownMaximum)
                    //{
                    //    timerPunchCooldownCurrent += Time.deltaTime;
                    //}
                    //else
                    //{
                    //    timerPunchCooldownCurrent = 0.0f;
                    //    isPunchOnCooldown = false;
                    //}
                    if (punchCountCurrent == 0 && timerPunchCooldownCurrent < SceneController.timerPunchCooldownMaximum / 1.5f)
                    {
                        timerPunchCooldownCurrent += Time.deltaTime;
						playerModel.GetComponent<Animator>().SetKeySegmentSpeed(5, 0.7f);
						playerModel.GetComponent<Animator>().SetKeySegmentSpeed(17, 0.55f);
                    }
                    else if (punchCountCurrent == 1 && timerPunchCooldownCurrent < SceneController.timerPunchCooldownMaximum / 2f)
                    {
                        timerPunchCooldownCurrent += Time.deltaTime;
						playerModel.GetComponent<Animator>().SetKeySegmentSpeed(5, 0.75f);
						playerModel.GetComponent<Animator>().SetKeySegmentSpeed(17, 0.58f);

					}
                    else if (punchCountCurrent == 2 && timerPunchCooldownCurrent < SceneController.timerPunchCooldownMaximum)
                    {
                        timerPunchCooldownCurrent += Time.deltaTime;
						playerModel.GetComponent<Animator>().SetKeySegmentSpeed(5, 0.8f);
						playerModel.GetComponent<Animator>().SetKeySegmentSpeed(17, 0.61f);

					}
					else
                    {
                        punchCountCurrent++;
                        timerPunchCooldownCurrent = 0.0f;
                        isPunchOnCooldown = false;
                        if (punchCountCurrent == 3)
                        {
                            punchCountCurrent = 0;
                        }
                    }
                }
            }
			if (isStunned == true)
			{
				if (timerStunDurationCurrent < SceneController.timerStunDurationMaximum)
				{
					timerStunDurationCurrent += Time.deltaTime;
				}
				else
				{
					healthCurrent = SceneController.healthMaximum;
					timerStunResetDurationCurrent = 0.0f;
					timerStunDurationCurrent = 0.0f;
					hasSpawnStunParticle = false;
					SetInvis(true);
					isStunned = false;
				}
			}
			else if (healthCurrent < SceneController.healthMaximum)
			{
				if (timerStunResetDurationCurrent < SceneController.timerStunResetDurationMaximum)
				{
					timerStunResetDurationCurrent += Time.deltaTime;
				}
				else
				{
					healthCurrent = SceneController.healthMaximum;
					timerStunResetDurationCurrent = 0.0f;
				}
			}
			if (isInvis == true)
			{
				if (timerInvisDurationCurrent < SceneController.timerInvisDurationMaximum)
				{
					SetRenderColourAll(GetRandomNumber(0.2f, 0.8f));
					timerInvisDurationCurrent += Time.deltaTime;
				}
				else
				{
					SetRenderColourAll(1.0f);
					SetInvis(false);
				}
			}
			if (isRolling == true)
			{
				if (timerRollDurationCurrent < SceneController.timerRollDurationMaximum)
				{
					timerRollDurationCurrent += Time.deltaTime;
					playerModel.GetComponent<Transform>().localEulerAngles = new Vector3(playerModel.GetComponent<Transform>().localEulerAngles.x, playerModel.GetComponent<Transform>().localEulerAngles.y + ((360.0f * 2) / SceneController.timerRollDurationMaximum) * Time.deltaTime, playerModel.GetComponent<Transform>().localEulerAngles.z);
				}
				else
				{
					playerModel.GetComponent<Transform>().localEulerAngles = new Vector3(0, 90.0f, 0);
					timerRollDurationCurrent = 0.0f;
					isRolling = false;
					isOnJuices = false;
				}
			}
			if (isDashing == true)
			{
				if (timerDashDurationCurrent < SceneController.timerDashDurationMaximum)
				{
					timerDashDurationCurrent += Time.deltaTime;
				}
				else
				{
					timerDashDurationCurrent = 0.0f;
					hasSpawnDashParticle = false;
					isDashOnCooldown = true;
					isDashing = false;
				}
			}
			if (isDashOnCooldown == true)
			{
				if (timerDashCooldownCurrent < SceneController.timerDashCooldownMaximum)
				{
					timerDashCooldownCurrent += Time.deltaTime;
				}
				else
				{
					timerDashCooldownCurrent = 0.0f;
					isDashOnCooldown = false;
				}
			}
			if (isDead == true)
			{
				if (timerDeathDurationCurrent < SceneController.timerDeathDurationMaximum)
				{
					SetRenderColourCustom(GetRandomNumber(0.1f, 0.8f), 0.0f, 0.0f);
					timerDeathDurationCurrent += Time.deltaTime;
				}
				else
				{
					SetRenderColourAll(1.0f);
					timerDeathDurationCurrent = 0.0f;
					playerBody.active = false;
					playerBody.useGravity = false;
					playerCollider.isTrigger = true;
					playerCollider.active = false;
					isAbducted = true;
					isDead = false;
				}
			}
			if (playerState == PlayerState.WALK)
			{
				if (timerFootStepIntervalCurrent < SceneController.timerFootStepIntervalMaximum)
				{
					timerFootStepIntervalCurrent += Time.deltaTime;
				}
				else
				{
					if (isPlayer1)
                    {
						AudioController.PlaySFX("SFX Player 1 Footstep", AudioController.AudioVolume.VOLUME_50);
					}
					else
                    {
						AudioController.PlaySFX("SFX Player 2 Footstep", AudioController.AudioVolume.VOLUME_50);
					}
					timerFootStepIntervalCurrent = 0.0f;
				}
			}
			else if (timerFootStepIntervalCurrent > 0.0f)
            {
				timerFootStepIntervalCurrent = 0.0f;
            }
		}

		private void SetRenderColourAll(float value)
		{
			playerRenderer.color = new Vector4(value, value, value, 1.0f);
		}

		private void SetRenderColourCustom(float valueR, float valueG, float valueB)
		{
			playerRenderer.color = new Vector4(valueR, valueG, valueB, 1.0f);
		}

		private float GetRandomNumber(float min, float max)
		{
			lock (getRandom)
			{
				return (float)getRandom.NextDouble() * (max - min) + min;
			}
		}

		private void SetInvis(bool check)
		{
			isInvis = check;
			timerInvisDurationCurrent = 0.0f;
		}

		private void SetCharging(bool check)
		{
			isPunchCharging = check;
			timerChargeDurationCurrent = 0.0f;
			if (check == false)
			{
				chargeCountCurrent = 1;
				chargingParticle.GetComponent<ParticleEmitter>().Pause();
			}
		}

		private void SetPunching(bool check)
		{
			isPunching = check;
			timerPunchDurationCurrent = 0.0f;
		}

		public void ActivatePunch(bool check, int value)
		{
			punchCheck.ActivatePunch(value);
			punchCheck.setActive(check);

			punchCheck.punchWaveCharging = value > 1 ? true : false;
		}

		private void PlayerMove(float movementModifier)
		{
			playerModel.GetComponent<Transform>().localEulerAngles = new Vector3(0, 90.0f, 0);
			movementScript.SetMovement(movementModifier);
		}

		private void PlayerDash(Vector3 direction, float force)
		{
			float myDashCap = SceneController.dashingMoveCap;
			if (punchCheck.savedUpgradeValue == (int)SceneController.PunchUpgrade.SUPER_DASH)
            {
				force = force * 4.0f;
			}
			if (playerBody.linearVelocity.magnitude < myDashCap)
			{
				playerBody.AddForce(direction * force);
			}
			
		}

		public void PlayerJump(Vector3 direction, float force, bool nullY = false)
		{
			if (nullY)
				direction = new Vector3(direction.x, 0.1f, direction.z);
			playerBody.AddForce(direction * force);
			isJumping = true;
		}

		private void PlayerRoll(Vector3 direction, float force)
		{
			if (playerBody.linearVelocity.magnitude < SceneController.rollingMoveCap)
			{
				direction.y = 0.0f;
				playerBody.AddForce(direction * force);
			}
		}

		private float GetDistance(Vector3 point1, Vector3 point2)
		{
			return (point1 - point2).magnitudeSq;
		}

		void OnTriggerStay(Entity collider)
		{
			if (collider.name == "Juice" || collider.GetComponent<JuicePuddle>() != null)
			{
				isOnJuices = true;
			}
			RespawnPoint respawnObject = collider.GetComponent<RespawnPoint>();
			if (respawnObject != null && isInRespawnSequence == false)
			{
				if (isPlayer1 == true)
				{
					SetDeath(!respawnObject.isFallImpact, respawnObject.isFallImpact, false, mySavedPoint);
				}
				else
				{
					SetDeath(!respawnObject.isFallImpact, respawnObject.isFallImpact, false, mySavedPoint);
				}
			}
		}

		void OnTriggerExit(Entity collider)
		{
			if (collider.name == "Juice" || collider.GetComponent<JuicePuddle>() != null)
			{
				isOnJuices = false;
			}
		}

		public void ReceivePunch(int damage, Vector3 punchDirection, float forceNormal, float forceSpecial, bool isFriendlyPunch, bool nullY = false)
		{
			punchDirection = punchDirection / 3;
			punchDirection.y = 1.0f;
			if (!isDamaged && !isStunned && !isInvis)
			{
				if (isFriendlyPunch == true)
				{
					if (isOnJuices == true && SceneController.hasRollAttack == true && isRolling == false)
					{
						AudioController.PlaySFX("SFX Player Roll");
						pushRollDirection = punchDirection;
						isRolling = true;
					}
					else
					{
						if (damage == SceneController.chargeCountMaximum)
						{
							AudioController.PlaySFX("SFX Punch Jumping");
							AudioController.PlaySFX(SFXType.PLAYER_CHARGEDPUNCH, AudioController.AudioVolume.VOLUME_100);
							PlayerJump(Vector3.up, forceSpecial, nullY);
							//PlayerJump(Vector3.up, forceSpecial);
							SceneController.mainCamera.GetComponent<CameraBehaviour>().ShakeCamera(true);
						}
						else if (damage > 0)
						{
							//AudioController.PlaySFX("SFX Punch Jumping");
							PlayerJump(punchDirection, forceNormal, nullY);
							//PlayerJump(Vector3.up, forceNormal);
						}
						SetInvis(true);
					}
					if (isPlayer1)
					{
						AudioController.PlaySFX(SFXType.MELON_FRIENDLYDAMAGED, AudioController.AudioVolume.VOLUME_200);

					}
					else
					{
						AudioController.PlaySFX(SFXType.PINEAPPLE_FRIENDLYDAMAGED);

					}
				}
				else if (isGodMode == false)
				{
					if (isPlayer1)
                    {
						AudioController.PlaySFX(SFXType.MELON_DAMAGED, AudioController.AudioVolume.VOLUME_200);
                    }
					else
                    {
						AudioController.PlaySFX(SFXType.PINEAPPLE_DAMAGED);
					}
					//AudioController.PlaySFX("SFX Player Damaged");
					healthCurrent -= damage;
					PlayerJump(punchDirection, forceNormal, nullY);
					SetInvis(true);
					isDamaged = true;
				}
				SetCharging(false);
			}
		}

		public bool getIsJumping()
		{
			return !isOnGround;
		}

		public void SetDeath(bool checkDrown, bool checkFall, bool waitAllPlayer, Vector3 spawnLocation)
		{
			if (isGodMode == false)
			{
				if (checkDrown == true || checkFall == true)
				{
					mySpawnPoint = spawnLocation;
				}
				else
				{
					mySpawnPoint = mySavedPoint;
				}
				waitForPlayers = waitAllPlayer;
				if (isInRespawnSequence == false)
				{
					if (checkDrown == true)
					{
						AudioController.PlaySFX("SFX Respawn Drown");
						if (isPlayer1)
						{
							AudioController.PlaySFX("Melon_Drown", AudioController.AudioVolume.VOLUME_150);
						}
						else
						{
							AudioController.PlaySFX("Pineapple_Drown");
						}
						Entity spawnParticle = Entity.InstantiatePrefab("Splash");
						spawnParticle.GetComponent<Transform>().globalPosition = playerTransform.globalPosition + (Vector3.up * 0.5f);
					}
					else if (checkFall == true)
                    {
						AudioController.PlaySFX("SFX Fall Impact");
						if (isPlayer1)
						{
							AudioController.PlaySFX(SFXType.MELON_FARFROMPLAYER, AudioController.AudioVolume.VOLUME_150);
						}
						else
						{
							AudioController.PlaySFX(SFXType.PINEAPPLE_FARFROMPLAYER);
						}
						Entity spawnParticle = Entity.InstantiatePrefab("eneDeath");
						spawnParticle.GetComponent<Transform>().globalPosition = playerTransform.globalPosition;
					}
					else
					{
						AudioController.PlaySFX("SFX Respawn Distance");
						if (isPlayer1)
						{
							AudioController.PlaySFX(SFXType.MELON_FARFROMPLAYER, AudioController.AudioVolume.VOLUME_150);
						}
						else
						{
							AudioController.PlaySFX(SFXType.PINEAPPLE_FARFROMPLAYER);
						}
						Entity spawnParticle = Entity.InstantiatePrefab("eneDeath");
						spawnParticle.GetComponent<Transform>().globalPosition = playerTransform.globalPosition;
					}
					myDeathPoint = playerTransform.globalPosition + (Vector3.up * SceneController.clawMachineHeight);
					PlayerJump(Vector3.up, SceneController.punchForceNormal);
					SceneController.mainCamera.GetComponent<CameraBehaviour>().ShakeCamera(true);
					isInRespawnSequence = true;
					isReadyToRespawn = false;
					isDrowning = checkDrown;
					isDead = true;
				}
			}
		}

		public void SetSpotlight(float lightValue)
		{
			spotlightObject.GetComponent<Light>().intensity = lightValue;
		}

		public void SetSavePoint(Vector3 savedPos)
		{
			mySavedPoint = savedPos;
		}

		public void SetIndicatorFlash(bool check)
		{
			if (check == true)
			{
				indicatorRenderer.color = new Vector4(GetRandomNumber(0.1f, 1.0f), 0.0f, 0.0f, 1.0f);
			}
			else
			{
				indicatorRenderer.color = indicatorColour;
			}
		}

		public bool getIsOnPuddle()
		{
			return isOnJuices;
		}

		public void SetPunchEffects(bool hitInteractiveCheck)
		{
			if (hitInteractiveCheck == true)
			{
				isEmptyHit = false;
				isFailedPunch = false;
				
				if (isPunching == true)
				{

					AudioController.PlaySFX(SFXType.PLAYER_PUNCH, AudioController.AudioVolume.VOLUME_100);
					if (chargeCountCurrent == SceneController.chargeCountMaximum)
					{
						punchParticle.GetComponent<ParticleEmitter>().Restart();
					}
					else
					{
						punchParticle.GetComponent<ParticleEmitter>().Restart();
					}
				}
			}
			else
            {
				isEmptyHit = false;
				isFailedPunch = true;
				AudioController.PlaySFX("SFX Punch Failed");
            }
		}

		public PlayerState getPlayerState()
        {
			return playerState;
        }

		public float getChargingPercentage()
		{
			//total time taken in charge
			float temp = chargeCountCurrent * SceneController.timerChargeDurationMaximum;
			temp += timerChargeDurationCurrent;

			float totalChargeTime = SceneController.timerChargeDurationMaximum * SceneController.chargeCountMaximum;

			return temp / totalChargeTime;
		}

		public void SetLookAt(Transform lookTransform)
        {
			cutsceneLookAtTransform = lookTransform;
		}
	}
}