using System;
using YadaScriptsLib;

namespace YadaScripts
{
    public class RangedEnemyBehaviour : Component
    {
        private CommonEnemyBehaviour commonEnemyBehaviour;
        private Transform myTransform;
        public float projectileSpeed;
        public float projectileRotateSpeed;

        void Start()
        {
            myTransform = this.entity.GetComponent<Transform>();
            commonEnemyBehaviour = this.entity.GetComponent<CommonEnemyBehaviour>();
            commonEnemyBehaviour.movementSpeed = 0.0f;
        }

        void Update()
        {
            //commonEnemyBehaviour.enemyModel.GetComponent<Transform>().localEulerAngles = new Vector3(-90.0f,
            //                commonEnemyBehaviour.enemyModel.GetComponent<Transform>().localEulerAngles.y,
            //                commonEnemyBehaviour.enemyModel.GetComponent<Transform>().localEulerAngles.z);
            //if (commonEnemyBehaviour.isAttacking)
            //    commonEnemyBehaviour.aiState = CommonEnemyBehaviour.AIState.WALK;
            switch (commonEnemyBehaviour.aiState)
            {
                case CommonEnemyBehaviour.AIState.ATTACK:
                    if (commonEnemyBehaviour.isAttacking == false)
                    {
                        AudioController.PlaySFX("SFX Enemy Attack Projectile");
                        Entity projectile = Entity.InstantiatePrefab("EnemyProjectile");
                        projectile.GetComponent<Transform>().globalPosition = myTransform.globalPosition;
                        EnemyProjectileBehaviour projectileB = projectile.GetComponent<EnemyProjectileBehaviour>();
                        projectileB.SetTargetDirection(commonEnemyBehaviour.targetPlayer);
                        projectileB.speed = projectileSpeed;
                        projectileB.rotateSpeed = projectileRotateSpeed;
                        projectileB.enemyTransform = myTransform;
                        Vector3 offset = -myTransform.forward + (Vector3.up * 0.5f);
                        projectile.GetComponent<Transform>().globalPosition = myTransform.globalPosition + offset;
                        commonEnemyBehaviour.isAttacking = true;
                        //commonEnemyBehaviour.enemyModel.GetComponent<Transform>().localEulerAngles = new Vector3(-90.0f,
                        //    commonEnemyBehaviour.enemyModel.GetComponent<Transform>().localEulerAngles.y,
                        //    commonEnemyBehaviour.enemyModel.GetComponent<Transform>().localEulerAngles.z);
                    }
                    break;

                //case CommonEnemyBehaviour.AIState.IDLE:
                //case CommonEnemyBehaviour.AIState.ATTACKING:
                //    Vector3 dir = (commonEnemyBehaviour.targetPlayer.GetComponent<Transform>().globalPosition - myTransform.globalPosition).normalized;
                //    commonEnemyBehaviour.enemyModel.GetComponent<Transform>().localEulerAngles = new Vector3(-90.0f,
                //            commonEnemyBehaviour.enemyModel.GetComponent<Transform>().localEulerAngles.y,
                //            dir.z);
                //    break;
            }


            //Vector3 dir = (commonEnemyBehaviour.targetPlayer.GetComponent<Transform>().globalPosition - globalPosition);
            //commonEnemyBehaviour.enemyModel.GetComponent<Transform>().localEulerAngles = new Vector3(-90.0f,
            //        commonEnemyBehaviour.enemyModel.GetComponent<Transform>().localEulerAngles.y,
            //        dir.y);
        }
    }
}