using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class PlankBehaviour : Component
    {
        public Vector3 spawnPoint;
        private Transform transform;
        private RigidBody rigidBody;
        public float punchDirectionForce;
        public float punchUpwardForce;

        //animation variables
        private float currIntervalTimer;
        private float maxIntervalTimer;
        private Random rand;

        public bool hasSavedPoint;
        public bool toRespawnBack;

        void Start()
        {
            rigidBody = this.entity.GetComponent<RigidBody>();
            transform = this.entity.GetComponent<Transform>();
            hasSavedPoint = false;
            toRespawnBack = false;

            //animation variables
            maxIntervalTimer = 1.0f;
            currIntervalTimer = 0;
            rand = new Random();
        }

        void Update()
        {
            if (hasSavedPoint==false)
            {
                spawnPoint = transform.globalPosition;
                hasSavedPoint = true;
            }
            if (toRespawnBack == true)
            {
                transform.globalPosition = spawnPoint;
                toRespawnBack = false;
            }

            //interval "hopping" animation
            if (currIntervalTimer < maxIntervalTimer)
            {                
                currIntervalTimer += Time.deltaTime;
            }
            else
            {
                //PHYSICS FORCES ATTEMPT for animation here
                if (rigidBody.linearVelocity.y >= -1 && rigidBody.linearVelocity.y <= 1)
                    rigidBody.AddForce(0, 2000, 0);
                //rigidBody.AddTorque(1000, 0, 0);

                currIntervalTimer = 0;
                maxIntervalTimer = 1f + (float)rand.NextDouble();
            }
        }

        private void OnTriggerEnter(Entity collision)
        {
            if (collision.GetComponent<RespawnPoint>() != null)
            {
                rigidBody.active = false;
                toRespawnBack = true;
            }
        }

        public void PushPlank(Vector3 punchDirection)
        {
            AudioController.PlaySFX(SFXType.PLAYER_PUNCH, AudioController.AudioVolume.VOLUME_100);
            rigidBody.AddForce(punchDirection * punchDirectionForce);
            rigidBody.AddForce(Vector3.up * 1 * punchUpwardForce);
        }
    }
}