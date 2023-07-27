using System;
using YadaScriptsLib;

namespace YadaScripts
{
	public class CommonEnemyBehaviour : Component
	{
		private Entity[] playerArray;

		public AIState aiState;
		public Entity targetPlayer;
		public Entity enemyModel;
		public int healthCurrent;

		public float movementSpeed;
		public float runMovementModifier;
		public float detectFollowRange;			//How near enemy is before being alerted and runs towards player
		public float detectFollowWalkRange;		//How near enemy is before they switch to walk instead of run. This value must be lower than detectFollowRange
		public float detectAttackRange;			//How near enemy is before they try to attack

		public float attackingStateMaximum;
		public float attackCooldownMaximum;
		public float invincibleTimerMaximum;

		public bool isAttacking;
		public bool isAwake = false;

		private float attackingStateCurrent;
		private float attackCooldownCurrent;
		private float invincibleTimerCurrent;

		private bool isRolling;
		private bool hasDetectedPlayer;
		private bool hasSpawnedMark;
		private bool isAttackOnCooldown;
		private bool isInvincible;
		private bool isDamaged;
		private bool drownDeath;

		private bool hasBeenTriggered = false;

		private RigidBody myRigidbody;
		private Transform myTransform;
		private Animator myAnimator;

		public Entity linkedBurrow;
		public BurrowBehaviour burrowBehaviour;
        private bool isDead;
		
		public enum AnimationState
        {
			IDLE_UNTRIGGERED = 0,
			TRIGGERED,
			IDLE_TRIGGERED,	//No longer in the ground but not inside the attack range
			ATTACK,
			IDLE_WITHIN_ATTACK_RANGE,
			DAMAGED,
			DEATH,
			WALKING = 4 //Special for the potato
		}

		public enum AIState
		{
			IDLE,
			ALERTED,
			WALK,
			RUN,
			ATTACKING,
			ATTACK,
			DAMAGED,
			DEAD,
			COOLDOWN//,
			//ROLL
		}

		void Start()
		{
			aiState = AIState.IDLE;
			myRigidbody = this.entity.GetComponent<RigidBody>();
			myTransform = this.entity.GetComponent<Transform>();
			enemyModel = myTransform.GetChildByIndex(0).entity;
			enemyModel.GetComponent<Transform>().localEulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
			myAnimator = enemyModel.GetComponent<Animator>();
			playerArray = Entity.GetEntitiesWithComponent<PlayerBehaviour>();
			targetPlayer = playerArray[0];

			linkedBurrow = Entity.InstantiatePrefab("Burrow");
			linkedBurrow.GetComponent<Transform>().localPosition = myTransform.globalPosition;
			if (this.entity.GetComponent<TankEnemyBehaviour>() != null)
				linkedBurrow.GetComponent<Transform>().localScale = new Vector3(2.5f, 2.5f, 2.5f);
			burrowBehaviour = linkedBurrow.GetComponent<BurrowBehaviour>();
		}

		void Update()
		{
			//CheckAirDampen(); //Obsolete since we have our own death animation now
			CheckDeath();
			CheckNearestPlayer();
			EnemyCheckDecision();
			EnemyCheckState();
			CheckTimer();
			CheckBurrow();
		}


		void CheckDeath()
        {
			if (myAnimator.animationIndex == (int)AnimationState.DEATH && myAnimator.animateCount == 0)
			{
				this.entity.active = false;
			}
		}


		void CheckBurrow()
        {
			//After it expands, contract
			if (linkedBurrow.GetComponent<Transform>().localScale.x == 1.5f)
				burrowBehaviour.ScaleTo(1.0f, 0.4f);
			else if (linkedBurrow.GetComponent<Transform>().localScale.x == 4.5f)
				burrowBehaviour.ScaleTo(2.5f, 0.4f);

		}

		//void CheckAirDampen()
		//{
		//	//If object is in air, lower the linear dampening.
		//	if (Math.Abs(myRigidbody.linearVelocity.y) > 1.0f)
		//	{
		//		myRigidbody.linearDamping = 0.6f;
		//	}
		//	else
		//	{
		//		myRigidbody.linearDamping = 0.8f;
		//	}
		//	myRigidbody.angularDamping = 0.7f;
		//}

		void CheckTimer()
		{
			if (isAttacking == true)
            {
				if (attackingStateCurrent < attackingStateMaximum)
                {
					attackingStateCurrent += Time.deltaTime;
                }
				else
                {
					attackingStateCurrent = 0.0f;
					isAttacking = false;
					isAttackOnCooldown = true;
                }
            }
			else
            {
				if (isAttackOnCooldown == true)
				{
					if (attackCooldownCurrent < attackCooldownMaximum)
					{
						attackCooldownCurrent += Time.deltaTime;
					}
					else
					{
						attackCooldownCurrent = 0.0f;
						isAttackOnCooldown = false;
					}
				}
			}


			if (isInvincible == true)
			{
				if (invincibleTimerCurrent < invincibleTimerMaximum)
				{
					invincibleTimerCurrent += Time.deltaTime;
				}
				else
				{
					invincibleTimerCurrent = 0.0f;
					isInvincible = false;
				}
			}

			if (healthCurrent <= 0)
			{
				if (myAnimator.animationIndex <= 2)
				{
					this.entity.active = false;
					linkedBurrow.active = false;
				}
			}
		}

		void EnemyCheckDecision()
		{
			if (healthCurrent <= 0)
			{
				aiState = AIState.DEAD;
            }
			else if (isDamaged == true)
			{
				aiState = AIState.DAMAGED;
            }
			else if (isInvincible || SceneController.isInCutscene)
			{
				aiState = AIState.IDLE;
			}
			else if (isAttacking == true)
			{
				aiState = AIState.ATTACKING;
			}
			else if (isAttackOnCooldown == false && GetDistance(targetPlayer.GetComponent<Transform>().globalPosition, myTransform.globalPosition) < detectAttackRange)
			{
				if (hasDetectedPlayer == true)
				{
					aiState = AIState.ATTACK;
				}
				else
				{
					aiState = AIState.ALERTED;
				}
			}
			else if (isAttackOnCooldown)
			{
				aiState = AIState.COOLDOWN;
			}
			else if (GetDistance(targetPlayer.GetComponent<Transform>().globalPosition, myTransform.globalPosition) < detectFollowWalkRange)
			{
				//From alerted state do we go to walk
				if (hasDetectedPlayer == true)
				{
					aiState = AIState.WALK;
				}
				else
				{
					aiState = AIState.ALERTED;
				}
			}
			else if (GetDistance(targetPlayer.GetComponent<Transform>().globalPosition, myTransform.globalPosition) < detectFollowRange)
			{
				//From alerted state do we go to run
				if (hasDetectedPlayer == true)
				{
					aiState = AIState.RUN;
				}
				else
				{
					aiState = AIState.ALERTED;
				}
			}
			else if (isAwake && hasBeenTriggered)
			{
				aiState = AIState.WALK;
			}
			else if (!isAwake)
			{
				aiState = AIState.IDLE;
			}
		}

		void CheckNearestPlayer()
		{
			//Checks whichever player is nearer. The nearer player is the target for all the following code
			for (int i = 0; i < playerArray.Length; ++i)
			{
				if (targetPlayer == playerArray[i]) continue;
				float currTargetPlayerDist = GetDistance(targetPlayer.GetComponent<Transform>().globalPosition, myTransform.globalPosition);
				float otherTargetPlayerDist = GetDistance(playerArray[i].GetComponent<Transform>().globalPosition, myTransform.globalPosition);
				if (otherTargetPlayerDist < currTargetPlayerDist) targetPlayer = playerArray[i];
			}
		}

		void EnemyCheckState()
		{
			switch (aiState)
			{
				case AIState.IDLE:
					if (hasBeenTriggered == false)
                    {
						//Show inside ground anim
						EnemyAnimation((int)AnimationState.IDLE_UNTRIGGERED, -1, true);
					}
					else
                    {
						if (GetDistance(targetPlayer.GetComponent<Transform>().globalPosition, myTransform.globalPosition) < detectAttackRange)
                        {
							if (isAttackOnCooldown)
							{						
								if (this.entity.GetComponent<MeleeEnemyBehaviour>() != null) //Potato has no idle within attack range anim. So have to use the idle triggered
									EnemyAnimation((int)AnimationState.IDLE_TRIGGERED, -1, true);
								else
									EnemyAnimation((int)AnimationState.IDLE_WITHIN_ATTACK_RANGE, -1, true);
							}
						}
						else //Enemy is idle outside of player's range
						{
							EnemyAnimation((int)AnimationState.IDLE_TRIGGERED, -1, true);
						}

					}

                    if (hasDetectedPlayer == true)
                    {
						--AudioController.enemyDetectCount;
						hasSpawnedMark = false;
                        hasDetectedPlayer = false;
					}
                    break;

				case AIState.ALERTED:
					{
						if (hasBeenTriggered == false)
                        {
							EnemyAnimation((int)AnimationState.TRIGGERED, 1, true);

							if (this.entity.GetComponent<TankEnemyBehaviour>() != null) //Onion expands hole to be bigger
								burrowBehaviour.ScaleTo(4.5f, 0.4f);
							else
								burrowBehaviour.ScaleTo(1.5f, 0.4f);
						}

						if (hasSpawnedMark == false)
						{
							//EnemyAnimation(8, 1, true); //Alert animation
							//myRigidbody.AddForce(new Vector3(0, 4000, 0));
							//AudioController.PlaySFX("SFX Enemy Alert");
							if (this.entity.GetComponent<MeleeEnemyBehaviour>() != null && !this.entity.GetComponent<MeleeEnemyBehaviour>().isRollerPotato())
                            {
								Quaternion LookedAt = Quaternion.LookAt(myTransform.globalPosition, targetPlayer.GetComponent<Transform>().globalPosition);
								myTransform.globalRotation = new Quaternion(0.0f, LookedAt.y, 0.0f, LookedAt.w);
							}
							Entity exclamationPrefab = Entity.InstantiatePrefab("Exclamation_Mark");
							exclamationPrefab.GetComponent<EnemyAlertExcalamtion>().SetFollowTarget(myTransform);
							exclamationPrefab.GetComponent<Transform>().localScale = new Vector3(2.0f, 2.0f, 2.0f);
							hasSpawnedMark = true;
						}

						//More or less has landed and has no vertical velocity
						if (Math.Abs(myRigidbody.linearVelocity.y) < 0.1f)
						{
							++AudioController.enemyDetectCount;
							hasBeenTriggered = true;
							hasDetectedPlayer = true;
						}
					}
					break;

				case AIState.WALK:
					{
						EnemyAnimation((int)AnimationState.WALKING, -1, true); //Walk anim

						Vector3 dir = (targetPlayer.GetComponent<Transform>().globalPosition - myTransform.globalPosition).normalized;
						myRigidbody.linearVelocity = new Vector3(dir.x * movementSpeed, myRigidbody.linearVelocity.y, dir.z * movementSpeed);

						Quaternion LookedAt = Quaternion.LookAt(myTransform.globalPosition, targetPlayer.GetComponent<Transform>().globalPosition);
						this.entity.GetComponent<Transform>().globalRotation = new Quaternion(0.0f, LookedAt.y, 0.0f, LookedAt.w);
					}
					break;

				case AIState.RUN:
					{
						Quaternion LookedAt = Quaternion.LookAt(myTransform.globalPosition, targetPlayer.GetComponent<Transform>().globalPosition);
						myTransform.globalRotation = new Quaternion(0.0f, LookedAt.y, 0.0f, LookedAt.w);

						//skip following and walking animation for onions
						if (this.entity.GetComponent<TankEnemyBehaviour>() != null)
							break;

						EnemyAnimation((int)AnimationState.WALKING, -1, true); //Run anim

						Vector3 dir = (targetPlayer.GetComponent<Transform>().globalPosition - myTransform.globalPosition).normalized;
						myRigidbody.linearVelocity = new Vector3(dir.x * movementSpeed * runMovementModifier, myRigidbody.linearVelocity.y, dir.z * movementSpeed * runMovementModifier);
					}
					break;

				case AIState.ATTACKING:
					//EnemyAnimation((int)AnimationState.ATTACK, 1, true);
					//myAnimator.SetKeySegmentSpeed(5, 3.0f);
					if (isAttackOnCooldown)
						EnemyAnimation((int)AnimationState.IDLE_WITHIN_ATTACK_RANGE, -1, true);
					break;

				case AIState.ATTACK:
					{
						//EnemyAnimation(5, 1, true);
						//myAnimator.SetKeySegmentSpeed(5, 3.0f);


						EnemyAnimation((int)AnimationState.ATTACK, 1, true);
						attackCooldownCurrent = attackCooldownMaximum;
						isAttackOnCooldown = true;

						MeleeEnemyBehaviour rollCheck = this.entity.GetComponent<MeleeEnemyBehaviour>(); //skip look at
						if (rollCheck != null && rollCheck.isCurrentlyRolling())
							break;

						Quaternion LookedAt = Quaternion.LookAt(myTransform.globalPosition, targetPlayer.GetComponent<Transform>().globalPosition);
						myTransform.globalRotation = new Quaternion(0.0f, LookedAt.y, 0.0f, LookedAt.w);

						if (isAttacking == true)
                        {
							AudioController.PlaySFX("SFX Enemy Punch", AudioController.AudioVolume.VOLUME_150);
						}

					}
					break;

				case AIState.DAMAGED:
					//EnemyAnimation(12, 1, true);
					EnemyAnimation((int)AnimationState.DAMAGED, 1, true);
                    if (myAnimator.animateCount == 0)
						isDamaged = false;
					break;

				case AIState.DEAD:
					enemyModel.GetComponent<Renderer>().color = new Vector4(0.3f, 0.3f, 0.3f, 1.0f);
                    //if (drownDeath)
                    //{
                    //	EnemyAnimation(11, 1, true);
                    //}
                    //else
                    //{
                    //	EnemyAnimation(16, 1, true);
                    //}
                    EnemyAnimation((int)AnimationState.DEATH, 1, true);
                    if (isDead == false)
                    {
                        AudioController.PlaySFX("SFX Grunt 2");
						isDead = true;
                    }
                    //myRigidbody.linearDamping = 0.0f;
                    //myRigidbody.angularDamping = 0.0f;
                    if (hasDetectedPlayer == true)
					{
						--AudioController.enemyDetectCount;
						hasSpawnedMark = false;
						hasDetectedPlayer = false;
					}
					break;
				case AIState.COOLDOWN:
					{
						EnemyAnimation((int)AnimationState.IDLE_TRIGGERED, -1, true);
					}
					break;
					//case AIState.ROLL:
					//	EnemyAnimation(17, -1, true);
					//	break;
			}
		}

		public void EnemyAnimation(int index, int count, bool canAnimate)
		{
			if (myAnimator == null) return;

			if (myAnimator.animationIndex == (int)AnimationState.ATTACK && myAnimator.animateCount > 0)
				return;

			if (myAnimator.animationIndex == (int)AnimationState.TRIGGERED && myAnimator.animateCount > 0)
				return;

			if (myAnimator.animationIndex != index)
			{
				myAnimator.animationIndex = index;
				myAnimator.animateCount = count;
				myAnimator.animate = canAnimate;
			}
		}

		public void ReceivePunch(int damage, Vector3 punchDirection, float forceNormal, float forceSpecialforce, bool isRollAttack = false)
		{
			//onion invulnerable to punches
			if (this.entity.GetComponent<TankEnemyBehaviour>() != null && !isRollAttack) 
				return;
			punchDirection.y = 2.0f;
			healthCurrent -= damage;
			if (healthCurrent > 1 && isInvincible == false)
			{
				myRigidbody.AddForce(punchDirection * forceNormal);
				Entity juiceParticle = Entity.InstantiatePrefab("juiceParticle");
				juiceParticle.GetComponent<Transform>().globalPosition = myTransform.globalPosition;
			}
			else if (healthCurrent <= 0)//&& isInvincible == false)
			{
				enemyModel.GetComponent<Renderer>().color = new Vector4(0.3f, 0.3f, 0.3f, 1.0f);
				//myRigidbody.AddForce(Vector3.up * 10000.0f);
				//enemyModel.GetComponent<Transform>().localEulerAngles = new Vector3(0.0f, 89.0f, 89.0f);
				Entity explodeParticle = Entity.InstantiatePrefab("explodeParticle");
				explodeParticle.GetComponent<Transform>().globalPosition = myTransform.globalPosition;
				//Audio.PlaySource(AudioController.enemyDeathSFX);
				burrowBehaviour.ScaleTo(0.0f, 0.5f);
				AudioController.PlaySFX("SFX Enemy Explode");
			}
			isDamaged = true;
			isInvincible = true;
            //invincibleTimerCurrent = invincibleTimerMaximum;
            AudioController.PlaySFX(SFXType.ENEMY_DAMAGED, AudioController.AudioVolume.VOLUME_50);

        }

		void OnCollisionEnter(Entity collider)
		{
			if (healthCurrent > 0)
			{
				//If enemy is still alive, it can be affected by player's rolling state
				PlayerBehaviour touchedPlayerBehaviour = collider.GetComponent<PlayerBehaviour>();
				if (touchedPlayerBehaviour != null && touchedPlayerBehaviour.playerState == PlayerBehaviour.PlayerState.ROLLING)
				{
					Vector3 rollDirection = (myTransform.globalPosition - collider.GetComponent<Transform>().globalPosition).normalized;
					ReceivePunch(10, rollDirection, SceneController.punchForceNormal, SceneController.punchForceCharged);
				}
			}
			else
			{
				//If enemy is dead and comes into contact with solid surfaces, spawn smoke particles
				this.entity.GetComponent<Collider>().isTrigger = true;
				Entity deathParticle = Entity.InstantiatePrefab("eneDeath");
				deathParticle.GetComponent<Transform>().globalPosition = myTransform.globalPosition;
			}
		}

		void OnTriggerStay(Entity collider)
		{
			//If enemy touches water
			if (collider.GetComponent<RespawnPoint>() != null && (this.entity.GetComponent<MeleeEnemyBehaviour>() != null))
			{
				healthCurrent = 0;
				drownDeath = true;

				if (linkedBurrow.GetComponent<BurrowBehaviour>() != null)
					linkedBurrow.GetComponent<BurrowBehaviour>().ScaleTo(0);
			}
		}

		private float GetDistance(Vector3 point1, Vector3 point2)
		{
			return (point1 - point2).magnitude;
		}

		public float getPlayerDistance()
        {
			return GetDistance(targetPlayer.GetComponent<Transform>().globalPosition, myTransform.globalPosition);
		}
	}
}