using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class TankEnemyBehaviour : Component
    {
        enum OnionState
        {
            BURROWED,
            EXPOSED
        }

        public float exposeDuration;
        public float burrowDuration;
        public bool useCustomForce;
        public float atkKnockbackNormal;
        public float atkKnockbackSpecial;
        private float currTimer;
        
        OnionState currState;
        private CommonEnemyBehaviour commonEnemyBehaviour;
        private Transform myTransform;

        void Start()
        {
            myTransform = this.entity.GetComponent<Transform>();
            commonEnemyBehaviour = this.entity.GetComponent<CommonEnemyBehaviour>();
            currState = OnionState.BURROWED;
            currTimer = 0;
        }

        void Update()
        {
            switch (currState)
            {
                case OnionState.BURROWED:
                    if (currTimer >= burrowDuration)
                    {
                        //coming out animation
                        currState = OnionState.EXPOSED;
                        currTimer = 0;
                    }
                    break;
                case OnionState.EXPOSED:
                    if (currTimer >= exposeDuration)
                    {
                        //burrowing animation
                        currState = OnionState.BURROWED;
                        currTimer = 0;
                    }
                    break;
            }

            if (commonEnemyBehaviour.enemyModel.GetComponent<Animator>().animationIndex == 2)
            {
                commonEnemyBehaviour.enemyModel.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);
            }
            else if (commonEnemyBehaviour.enemyModel.GetComponent<Animator>().animationIndex == 1)
            {
                commonEnemyBehaviour.enemyModel.GetComponent<Transform>().localPosition = new Vector3(0, 1.0f, 0);
            }
            else
            {
                commonEnemyBehaviour.enemyModel.GetComponent<Transform>().localPosition = new Vector3(0, 0.5f, 0);
            }
            currTimer += Time.deltaTime;

            //CHECK FOR STATES FROM COMMON ENEMY BEHAVIOUR SCRIPT
            switch (commonEnemyBehaviour.aiState)
            {
                case CommonEnemyBehaviour.AIState.ATTACK:
                    if (commonEnemyBehaviour.isAttacking == false)
                    {
                        if (commonEnemyBehaviour.targetPlayer.GetComponent<PlayerBehaviour>().getPlayerState() != PlayerBehaviour.PlayerState.ROLLING)
                        {
                            Entity fartParticle = Entity.InstantiatePrefab("fart");
                            fartParticle.GetComponent<Transform>().globalPosition = myTransform.globalPosition;
                            Vector3 punchDirection = new Vector3(commonEnemyBehaviour.targetPlayer.GetComponent<Transform>().globalPosition.x - myTransform.globalPosition.x, 0.0f, commonEnemyBehaviour.targetPlayer.GetComponent<Transform>().globalPosition.z - myTransform.globalPosition.z).normalized;
                            if (useCustomForce)
                                commonEnemyBehaviour.targetPlayer.GetComponent<PlayerBehaviour>().ReceivePunch(1, punchDirection, atkKnockbackNormal, atkKnockbackSpecial, false, true);
                            else
                                commonEnemyBehaviour.targetPlayer.GetComponent<PlayerBehaviour>().ReceivePunch(1, punchDirection, SceneController.punchForceNormal / 2.0f, SceneController.punchForceNormal / 2.0f, false, true);

                            commonEnemyBehaviour.isAttacking = true;
                            AudioController.PlaySFX(SFXType.ONION_FART, AudioController.AudioVolume.VOLUME_50);
                        }
                    }
                    break;
            }
        }

        void OnCollisionEnter(Entity collider)
        {
            if (collider.GetComponent<PlayerBehaviour>() != null && 
                collider.GetComponent<PlayerBehaviour>().getPlayerState() == PlayerBehaviour.PlayerState.ROLLING)
            {
                commonEnemyBehaviour.ReceivePunch(10, new Vector3(0, -1, 0), 0, 0, true);
            }
        }
    }
}