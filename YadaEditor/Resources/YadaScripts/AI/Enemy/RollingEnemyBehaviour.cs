using System;
using YadaScriptsLib;

namespace YadaScripts
{
    public class RollingEnemyBehaviour : Component
    {
        private Transform transform;
        private Vector3 targetPosition;
        public RigidBody rBody;
        private Vector3 movementVec;
        public float speed;
        private float currRot;
        public float rollSpeed;
        public float lifeSpan;
        private float currLifeTimer;
        private Quaternion looking;

        public Transform enemyTransform;
        private bool useCustomForce;
        private float punchForceNormal;
        private float punchForceSpecial;

        private bool fired;

        void Start()
        {
            currRot = 0;
            transform = this.entity.GetComponent<Transform>();
            rBody = this.entity.GetComponent<RigidBody>();
            fired = false;
            currLifeTimer = 0;
        }

        void Update()
        {
            if (!fired)
            {
                rBody.AddForce(movementVec * speed);
                fired = true;
                transform.globalRotation = looking;
            }
                        
            //rBody.AddTorque(movementVec.x * 50, 0, movementVec.z * 50);
            //rBody.linearVelocity = movementVec * speed;
            rollingAnim();

            currLifeTimer += Time.deltaTime;

            if (currLifeTimer >= lifeSpan)
            {
                this.entity.active = false;
                Entity.DestroyEntity(this.entity);
            }
                
        }

        void OnCollisionEnter(Entity collider)
        {
            //damage player
            if (collider.GetComponent<PlayerBehaviour>() != null)
            {
                Vector3 punchDirection = (collider.GetComponent<Transform>().globalPosition - transform.globalPosition).normalized;

                if(useCustomForce)
                    collider.GetComponent<PlayerBehaviour>().ReceivePunch(1, punchDirection, /*punchForceNormal*/ 20000, /*punchForceNormal*/ 20000, false, true);
                else
                    collider.GetComponent<PlayerBehaviour>().ReceivePunch(1, punchDirection, SceneController.punchForceNormal / 2.0f, SceneController.punchForceNormal / 2.0f, false);
                //this.entity.active = false;
                Entity.DestroyEntity(this.entity);
            }
        }

        private void rollingAnim() //should probably use torque instead
        {
            currRot -= rollSpeed * Time.deltaTime;

            Quaternion quat1 = Quaternion.CreateFromAxisAngle(new Vector3(1f, 0f, 0f), currRot); //rotate around z axis
            //Quaternion quat2 = Quaternion.LookAt(enemyTransform.globalPosition, targetPosition);
            Quaternion quat = looking * quat1 ;

            transform.globalRotation = quat;
        }

        public void SetTargetDirection(Vector3 target, Vector3 startingPosition, Quaternion rotation)
        {
            targetPosition = target;
            movementVec = new Vector3(targetPosition.x - startingPosition.x, 0, targetPosition.z - startingPosition.z);
            //movementVec = new Vector3(transform.globalPosition.x - targetPosition.x, 0, transform.globalPosition.z - targetPosition.z);
            movementVec.Normalize();

            looking = rotation;
        }

        public void setCustomPunch(bool custom, float normal = 0, float special =0)
        {
            useCustomForce = custom;
            punchForceNormal = normal;
            punchForceSpecial = special;
        }
    }
}
