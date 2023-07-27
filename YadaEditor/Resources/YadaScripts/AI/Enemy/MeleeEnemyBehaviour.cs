using System;
using System.Threading;
using YadaScriptsLib;

namespace YadaScripts
{
    public class MeleeEnemyBehaviour : Component
    {
        private CommonEnemyBehaviour commonEnemyBehaviour;
        private Transform myTransform;

        public bool useCustomForce;
        public float punchForceNormal;
        public float punchForceSpecial;

        public float meleeDistance;
        public float babySpeed;
        public float babyRollingSpeed;
        public float babyLifeSpan;

        public bool useBabyCustomForce;
        public float babyPunchForceNormal;
        public float babyPunchForceSpecial;


        void Start()
        {
            myTransform = this.entity.GetComponent<Transform>();
            commonEnemyBehaviour = this.entity.GetComponent<CommonEnemyBehaviour>();
        
            //commonEnemyBehaviour.healthCurrent = 3;
            //commonEnemyBehaviour.movementSpeed = 1.0f;
            //commonEnemyBehaviour.runMovementModifier = 1.5f;
            //commonEnemyBehaviour.detectFollowRange = 5.0f;
            //commonEnemyBehaviour.detectFollowWalkRange = 3.0f;
            //commonEnemyBehaviour.detectAttackRange = 1;
            commonEnemyBehaviour.attackCooldownMaximum = 3.0f;
            //commonEnemyBehaviour.invincibleTimerMaximum = 0.5f;
        }

        void Update()
        {
            switch (commonEnemyBehaviour.aiState)
            {
                case CommonEnemyBehaviour.AIState.ATTACK:
                    if (commonEnemyBehaviour.isAttacking == false)
                    {
                        if (commonEnemyBehaviour.getPlayerDistance() > meleeDistance) //roll
                        {
                            Entity projectile = Entity.InstantiatePrefab("EnemyRolling");
                            RollingEnemyBehaviour baby = projectile.GetComponent<RollingEnemyBehaviour>();

                            baby.SetTargetDirection(myTransform.globalPosition + myTransform.forward, myTransform.globalPosition,
                                myTransform.globalRotation);
                            baby.speed = babySpeed;
                            baby.rollSpeed = babyRollingSpeed;
                            baby.lifeSpan = babyLifeSpan;
                            baby.enemyTransform = myTransform;
                            if (useBabyCustomForce)
                                baby.setCustomPunch(useBabyCustomForce, babyPunchForceNormal, babyPunchForceSpecial);
                            Vector3 offset = -myTransform.forward + (Vector3.up * 0.5f);
                            projectile.GetComponent<Transform>().globalPosition = myTransform.globalPosition + offset;
                        }
                        else //melee
                        {
                            Vector3 punchDirection = new Vector3(commonEnemyBehaviour.targetPlayer.GetComponent<Transform>().globalPosition.x -
                                myTransform.globalPosition.x, 0.0f, commonEnemyBehaviour.targetPlayer.GetComponent<Transform>().globalPosition.z -
                                myTransform.globalPosition.z).normalized;

                            if (useCustomForce)
                                commonEnemyBehaviour.targetPlayer.GetComponent<PlayerBehaviour>().ReceivePunch(1, punchDirection, punchForceNormal, punchForceNormal, false);
                            else
                                commonEnemyBehaviour.targetPlayer.GetComponent<PlayerBehaviour>().ReceivePunch(1, punchDirection, SceneController.punchForceNormal / 2.0f, SceneController.punchForceNormal / 2.0f, false);
                        }

                        commonEnemyBehaviour.isAttacking = true;
                    }
                    break;
            }
        }

        public bool isRollerPotato()
        {
            return meleeDistance == 0 ? true : false;
        }

        public bool isCurrentlyRolling()
        {
            return commonEnemyBehaviour.getPlayerDistance() > meleeDistance;
        }
    }
}