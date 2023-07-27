using System;
using YadaScriptsLib;

namespace YadaScripts
{
    public class BossBehaviour : Component
    {
        public Transform bossTransform;
        public Entity bossModel;
        public Entity burrowModel;
        public Entity bossSpeechBubbleEntity;

        public Animator bossAnimator;
        public Collider bossCollider;
        
        public bool playBossBurrowSpawn = true;

        public bool canAttack = false;

        public bool pausePlayers = true; //Do we stop the players while spawning
        //public Entity minionsToSpawn;

        public float stunnedTimer = 1.0f;

        public int spawnWave = 0;

        public Entity camRef; //The camera that pans to the boss before panning back to player

        //Animation Variables
        private bool burrowMoving = false;
        private Vector3 bossModelFakeVel = new Vector3(0, 0, 0);
        private bool delayWaitingBeforePunch = true;
        private float delayAfterLanding = 1.0f;

        //How far away the player must be before the boss escapes
        private float escapeRange = 4.0f;
        private bool escaping = false;

        //Attack ranges
        private float defaultAttackRange = 5.0f;
        private float defaultAttackForce = 200f;
        public float attackRange = 15.0f;
        public float attackForce = 8000f;
        public float attackCooldownLength = 5.0f;
        private float attackCooldownTimer = 0.0f;
        public float distanceAttack = 0;
        private bool attackSpeakToggle = true;
        public enum BossAnimState
        {
            WALK = 0,
            IDLE_LOOK,
            LEFT_PUNCH,
            RIGHT_PUNCH,
            RIGHT_DAMAGED,
            DETECT,
            STUN,
            PUNCHED_UP,
            FALLING,
            DEFEAT,
            IDLE_FRONT, //Use this one for idle
            DIG
        }

        void Start()
        {
            bossTransform = this.entity.GetComponent<Transform>();
            bossModel = bossTransform.GetChildByIndex(0).entity;
            burrowModel = bossTransform.GetChildByIndex(1).entity;
            bossSpeechBubbleEntity = bossTransform.GetChildByIndex(2).entity;
            bossAnimator = bossModel.GetComponent<Animator>();
            bossCollider = this.entity.GetComponent<Collider>();

            if (playBossBurrowSpawn == false)
            {
                burrowModel.active = false;
                bossModel.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);
                BossAnimation((int)BossAnimState.IDLE_FRONT, -1, true);
            }
            else
            {
                burrowMoving = true;
                bossModel.GetComponent<Transform>().localPosition = new Vector3(0, -1.5f, 0);
            }
            attackCooldownLength = 5.0f;
            attackCooldownTimer += 5.0f;
            //attackForce = 1000f;
            //bossSpeechBubbleUI.texture = "Scene7_Durian";
        }



        void Update()
        {
            if (playBossBurrowSpawn)
                CheckSpawnAnimation();

            if (SceneController.bossMinionWave != 2)
                CheckEscapeAnimation();
            else 
                CheckDeath();

            if (canAttack && escaping == false)
                CheckAttack();

            if (bossAnimator.animateCount == 0)
                BossAnimation((int)BossAnimState.IDLE_FRONT, -1, true);
        }


        private void BossAnimation(int index, int count, bool canAnimate)
        {
            if (bossAnimator.animationIndex != index)
            {
                bossAnimator.animationIndex = index;
                bossAnimator.animateCount = count;
                bossAnimator.animate = canAnimate;
            }
        }

        private void Speak(bool spawningTalk = true)
        {
            UI bossSpeechBubbleUI = bossSpeechBubbleEntity.GetComponent<UI>(); //bossSpeechBubbleEntity.GetComponent<UI>()
            if (SceneController.bossMinionWave == 0)
            {
                if (spawningTalk)
                {
                    bossSpeechBubbleUI.texture = "Scene8.1_Eggplant";
                    bossSpeechBubbleEntity.GetComponent<PopUp>().SpritePopUp(new Vector3(0.25f, 3f, 0), new Vector3(3f, 3f, 3f), 4f);
                }
                else
                {
                    bossSpeechBubbleUI.texture = "Scene8.1_2_Eggplant";
                    bossSpeechBubbleEntity.GetComponent<PopUp>().SpritePopUp(new Vector3(0.25f, 3f, 0), new Vector3(3f, 3f, 3f), 4f);
                }
            }
            else if (SceneController.bossMinionWave == 1)
            {
                if (spawningTalk)
                {
                    bossSpeechBubbleUI.texture = "Scene8.2_Eggplant";
                    bossSpeechBubbleEntity.GetComponent<PopUp>().SpritePopUp(new Vector3(0.25f, 5f, 0), new Vector3(3f, 3f, 3f), 4f);
                }
                else
                {
                    bossSpeechBubbleUI.texture = "Scene8.2_2_Eggplant";
                    bossSpeechBubbleEntity.GetComponent<PopUp>().SpritePopUp(new Vector3(0.25f, 5f, 0), new Vector3(3f, 3f, 3f), 4f);
                }
            }
            else if (SceneController.bossMinionWave == 2)
            {
                if (spawningTalk)
                {
                    bossSpeechBubbleUI.texture = "Scene8.3_Eggplant";
                    bossSpeechBubbleEntity.GetComponent<PopUp>().SpritePopUp(new Vector3(-1f, 2f, -0.250f), new Vector3(4f, 4f, 4f), 5f);
                }
                else
                {
                    bossSpeechBubbleUI.texture = "Scene8.3_2_Eggplant";
                    bossSpeechBubbleEntity.GetComponent<PopUp>().SpritePopUp(new Vector3(-1f, 2f, -0.250f), new Vector3(3f, 3f, 3f), 4f);
                }
            }
            //if (spawningTalk && SceneController.bossMinionWave == 1) //Special case for this because camera is far away
            //    bossSpeechBubbleEntity.GetComponent<PopUp>().SpritePopUp(new Vector3(0.25f, 5f, 0), new Vector3(5f, 5f, 5f), 4f);
            //else
            //    bossSpeechBubbleEntity.GetComponent<PopUp>().SpritePopUp(new Vector3(0.25f, 3f, 0), new Vector3(3f, 3f, 3f), 4f);
        }

        void CheckSpawnAnimation()
        {
            //Animation Part 1: It takes 1 second for burrow to ascend to ground level
            if (burrowMoving) 
            {
                burrowModel.GetComponent<Transform>().localPosition = new Vector3(burrowModel.GetComponent<Transform>().localPosition.x,
                                                                                  burrowModel.GetComponent<Transform>().localPosition.y + 0.5f * Time.deltaTime,
                                                                                  burrowModel.GetComponent<Transform>().localPosition.z);

                if (burrowModel.GetComponent<Transform>().localPosition.y >= 0.0f)
                {
                    burrowMoving = false;
                    burrowModel.GetComponent<BurrowBehaviour>().ScaleTo(2.0f, 0.5f);
                    bossModelFakeVel = new Vector3(0, 7.5f, 0);
                    BossAnimation((int)BossAnimState.PUNCHED_UP, -1, true);
                }
            }

            //Animation Part 2: Boss is flying up from ground
            if (bossModelFakeVel.y > 0 && bossAnimator.animationIndex == (int)BossAnimState.PUNCHED_UP) //Ascending animation
            {
                bossModel.GetComponent<Transform>().localPosition = new Vector3(bossModel.GetComponent<Transform>().localPosition.x,
                                                                                  bossModel.GetComponent<Transform>().localPosition.y + (bossModelFakeVel.y * Time.deltaTime),
                                                                                  bossModel.GetComponent<Transform>().localPosition.z);

                bossModelFakeVel = new Vector3(0, bossModelFakeVel.y - (9.8f * Time.deltaTime), 0);

                // In case the current fake vel - 9.8 * Time.delta time is perfectly 0, we set it below 0
                if (bossModelFakeVel.y <= 0)
                {
                    bossModelFakeVel.y = (-9.8f * Time.deltaTime);
                    BossAnimation((int)BossAnimState.FALLING, -1, true);
                    burrowModel.GetComponent<BurrowBehaviour>().ScaleTo(0.01f, 0.5f); //If scale = 0, the burrow destroys, so set to very small
                }
            }
            //Animation Part 3: Descending
            else if (bossModelFakeVel.y < 0 && bossAnimator.animationIndex == (int)BossAnimState.FALLING) //Descending animation
            {
                bossModel.GetComponent<Transform>().localPosition = new Vector3(bossModel.GetComponent<Transform>().localPosition.x,
                                                                  bossModel.GetComponent<Transform>().localPosition.y + (bossModelFakeVel.y * Time.deltaTime),
                                                                  bossModel.GetComponent<Transform>().localPosition.z);

                bossModelFakeVel = new Vector3(0, bossModelFakeVel.y - (9.8f * Time.deltaTime), 0);

                bossCollider.isTrigger = false;

                if (bossModel.GetComponent<Transform>().localPosition.y <= 0)
                {
                    bossModel.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);
                    bossModelFakeVel.y = 0;
                    BossAnimation((int)BossAnimState.IDLE_FRONT, -1, true);
                }
            }

            //Animation Part 4: landed. Delay fro 1 sec
            if (delayAfterLanding > 0 && bossModelFakeVel.y == 0 && delayWaitingBeforePunch)
            {
                delayAfterLanding -= Time.deltaTime;
            }
            else if (delayAfterLanding <= 0 && bossModelFakeVel.y == 0 && delayWaitingBeforePunch)
            {
                if (bossAnimator.animationIndex != (int)BossAnimState.RIGHT_PUNCH)
                {
                    BossAnimation((int)BossAnimState.RIGHT_PUNCH, 1, true);
                    if (SceneController.bossMinionWave == 1)
                        Speak(true);
                    delayWaitingBeforePunch = false;
                }
            }

            //Animation Part 5: Perform punch that summons minions
            if (bossAnimator.animationIndex == (int)BossAnimState.RIGHT_PUNCH && bossAnimator.animateCount == 0)
            {
                BossAnimation((int)BossAnimState.IDLE_FRONT, -1, true);

                Entity[] spawnEnemyWave = Entity.GetEntitiesWithComponent<BossSpawnEnemyPoint>();
                for (int i = 0; i < spawnEnemyWave.Length; ++i)
                {
                    if (spawnEnemyWave[i].GetComponent<BossSpawnEnemyPoint>().spawnWave == spawnWave)
                    {
                        Entity.GetEntitiesWithComponent<BossSpawnEnemyPoint>()[i].GetComponent<BossSpawnEnemyPoint>().SpawnFromGround();
                    }
                }
                canAttack = true;
                ++AudioController.enemyDetectCount;
                //Console.WriteLine("ADD");
                SceneController.mainPlayer1.GetComponent<PlayerMovement>().active = true;
                SceneController.mainPlayer2.GetComponent<PlayerMovement>().active = true;
                camRef.GetComponent<CameraTrigger>().cameraLookPos = new Vector3(0, 0, 0);
            }
        }

        public void ReceivePunch()
        {
            if (SceneController.bossMinionWave != 2)
                return;

            SceneController.bossHealth--;
            this.entity.GetComponent<Collider>().isTrigger = true;
            Entity stunParticle = Entity.InstantiatePrefab("stars");
            stunParticle.GetComponent<Transform>().globalPosition = new Vector3(bossTransform.globalPosition.x, bossTransform.globalPosition.y + 1f, bossTransform.globalPosition.z);

            BossAnimation((int)BossAnimState.DIG, -1, true);
            canAttack = false;

            AudioController.PlaySFX("Eggplant_Damaged2");

            bossSpeechBubbleEntity.GetComponent<UI>().texture = "Scene8.4_EggplantDED";
            bossSpeechBubbleEntity.GetComponent<PopUp>().SpritePopUp(new Vector3(-1f, 2f, -0.250f), new Vector3(3f, 3f, 3f), 10f);

            Entity[] eventResponses = Entity.GetEntitiesWithComponent<EventResponse>();

            Entity finalCutscene = Entity.GetEntity("CutsceneBossDeathCam");

            for (int i = 0; i < eventResponses.Length; ++i)
            {
                if (eventResponses[i].GetComponent<EventResponse>().eventID == 123)
                {
                    finalCutscene = eventResponses[i];
                    //finalCutscene.GetComponent<EventResponse>().fired = true;
                    break;
                }
            }
            Input.GetRightTrigger(0);
            bossModel.GetComponent<Transform>().localEulerAngles = new Vector3(0, 0, 30);

            //camRef.GetComponent<CameraBehaviour>().customFixedCamera = false;
            finalCutscene.GetComponent<CutsceneTrigger>().ManuallyTriggerCutscene();// = true;
            //camRef.GetComponent<CameraBehaviour>().SetCustomLookAndFollow(false, finalCutscene.GetComponent<Transform>().globalPosition, finalCutscene.GetComponent<Transform>().GetChildByIndex(0).entity.GetComponent<Transform>().localPosition);

        }

        void CheckEscapeAnimation()
        {
            Vector3 dispFromP1 = SceneController.mainPlayer1.GetComponent<Transform>().globalPosition - bossTransform.globalPosition;
            Vector3 dispFromP2 = SceneController.mainPlayer2.GetComponent<Transform>().globalPosition - bossTransform.globalPosition;


            //if (bossAnimator.animationIndex == (int)BossAnimState.STUN && stunnedTimer > 0)
            //{
            //    stunnedTimer -= Time.deltaTime;
            //    if (stunnedTimer < 0)
            //    {

            //        burrowModel.GetComponent<BurrowBehaviour>().ScaleTo(2.0f, 2.0f);
            //        BossAnimation((int)BossAnimState.DIG, -1, true);
            //    }

            //}
            if (!escaping && (dispFromP1.magnitudeSq <= escapeRange * escapeRange || dispFromP2.magnitudeSq <= escapeRange * escapeRange))
            {
                burrowModel.GetComponent<BurrowBehaviour>().ScaleTo(2.0f, 2.0f);
                BossAnimation((int)BossAnimState.DIG, -1, true);
                escaping = true;
                canAttack = false;
                --AudioController.enemyDetectCount;
                //Console.WriteLine("MINUS");
            }
            else if (bossAnimator.animationIndex == (int)BossAnimState.DIG)
            {
                bossModel.GetComponent<Transform>().localPosition = new Vector3(bossModel.GetComponent<Transform>().localPosition.x,
                                                                                bossModel.GetComponent<Transform>().localPosition.y - (0.75f * Time.deltaTime),
                                                                                bossModel.GetComponent<Transform>().localPosition.z);

                if (burrowModel.GetComponent<Transform>().localScale.x >= 2.0f)
                {
                    burrowModel.GetComponent<BurrowBehaviour>().ScaleTo(0.0f, 0.5f);
                }
                else if (burrowModel.GetComponent<Transform>().localScale.x <= 0f)
                {
                    this.entity.active = false;
                }
            }
        }

        void CheckDeath()
        {
            if (bossAnimator.animationIndex == (int)BossAnimState.DEFEAT)
            {
                //Defeat anim is 48 frames long
                if (bossAnimator.GetKeySegmentStartFrame((int)BossAnimState.DEFEAT) >= 45)
                    this.entity.active = false;
            }

            if (SceneController.bossHealth <= 0 && bossSpeechBubbleEntity.GetComponent<UI>().color.w <= 0f)
            {
                --AudioController.enemyDetectCount;
                SceneController.mainCamera.GetComponent<CameraBehaviour>().SetCustomLookAndFollow(false, Vector3.zero, Vector3.zero);
                //Console.WriteLine("MINUS");
                BossAnimation((int)BossAnimState.DEFEAT, 1, true);
                AudioController.PlaySFX("Eggplant_Defeated");
                SceneController.bossHealth = 99;
            }


        }

        void CheckAttack()
        {
            //After attacking, set attackCooldown to attackCooldownTimer
            if (attackCooldownTimer > 0.0)
            {
                attackCooldownTimer -= Time.deltaTime;
            }
            else
            {
                float p1 = (entity.GetComponent<Transform>().localPosition - SceneController.mainPlayer1.GetComponent<Transform>().localPosition).magnitude;
                float p2 = (entity.GetComponent<Transform>().localPosition - SceneController.mainPlayer2.GetComponent<Transform>().localPosition).magnitude;
                if (p1 < attackRange || p2 < attackRange)
                {
                    Speak(attackSpeakToggle);
                    attackSpeakToggle = !attackSpeakToggle;

                    BossAnimation((int)BossAnimState.LEFT_PUNCH, 1, true);

                    Entity projectile = Entity.InstantiatePrefab("BossProjectile");
                    Transform projTrans = projectile.GetComponent<Transform>();
                    projectile.GetComponent<Transform>().localPosition = bossTransform.localPosition + bossTransform.forward + new Vector3(0,1,0);

                    Vector3 target = p1 < p2 ? SceneController.mainPlayer1.GetComponent<Transform>().localPosition : SceneController.mainPlayer2.GetComponent<Transform>().localPosition;
                    Vector3 distance = (target - projTrans.localPosition);
                    distanceAttack = distance.magnitude;
                    distance.y = 0;

                    projectile.GetComponent<RigidBody>().AddForce(new Vector3(distance.normalized.x, 1.5f, distance.normalized.z) * 
                                                                  (attackForce - 300f * (attackRange - distanceAttack)));
                    attackCooldownTimer = attackCooldownLength;
                }
            }
        }
    }
}
