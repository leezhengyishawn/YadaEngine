using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class PunchWave : Component
    {
        //own components
        private Transform transform;
        private Renderer renderer;

        private int punchUpgrade;
        private bool init;
        private bool activated;
        private Vector3 originalScale;
        private Vector3 newScale;
        private Vector3 originalPos;
        private Vector3 movementVector;
        private float waveTimer;
        public float maxWaveTimer;
        public float heightAdjustment;

        public Entity playerPunchCheck;
        private PunchCheck playerPunch;
        private Transform playerTransform;

        public bool isPlayer1;
        private bool shrinkTime;
        public float shrinkTimer;

        void Start()
        {
            waveTimer = 0;
            init = false;
            activated = false;
            shrinkTime = false;
            punchUpgrade = 0;
        }

        void FixedUpdate()
        {
            //initiation - run for first frame only
            if(!init)
            {
                Entity player;

                if (isPlayer1)
                    player = SceneController.mainPlayer1;
                else
                    player = SceneController.mainPlayer2;

                playerTransform = player.GetComponent<Transform>();
                playerPunch = playerPunchCheck.GetComponent<PunchCheck>();

                transform = this.entity.GetComponent<Transform>();
                renderer = this.entity.GetComponent<Renderer>();
                originalScale = transform.globalScale;
                newScale = transform.globalScale;
                renderer.active = false;

                init = true;
            }

            //update if there is an upgrade
            if(playerPunch.punchWaveUpgrade) //to update upgrade
            {
                if (playerPunch.punchWaveUpgradeVal == 1 || playerPunch.punchWaveUpgradeVal/10 == 1) //long punch
                {
                    punchUpgrade = 1;
                    transform.globalScale = new Vector3(originalScale.x * 2f, originalScale.y, originalScale.z);                    
                }
                else if (playerPunch.punchWaveUpgradeVal == 2 || playerPunch.punchWaveUpgradeVal / 10 == 2) //wide punch
                {
                    punchUpgrade = 2;
                    transform.globalScale = new Vector3(originalScale.x, originalScale.y, originalScale.z * 2f);
                }

                newScale = transform.globalScale;
                playerPunch.punchWaveUpgrade = false;
            }

            //check if player just punched
            if (playerPunch.punchWave)
            {
                activated = true;
                transform.globalPosition = playerTransform.globalPosition /*- playerTransform.forward * 0.01f*/ + new Vector3(0, heightAdjustment, 0);
                originalPos = transform.globalPosition;
                transform.globalRotation = playerTransform.globalRotation;
                transform.globalRotation *= Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), -(float)Math.PI * 0.5f);
                movementVector = playerTransform.globalPosition - playerTransform.forward * 0.5f + new Vector3(0, heightAdjustment, 0);
                playerPunch.punchWave = false;

                //particle prefab
                if (punchUpgrade == 1)
                {
                    Entity particle = Entity.InstantiatePrefab("LongPunchPartigirl");
                    particle.GetComponent<Transform>().globalPosition = playerTransform.globalPosition - playerTransform.forward * 0.4f + new Vector3(0, heightAdjustment, 0);
                    particle.GetComponent<Transform>().globalRotation = playerTransform.globalRotation * Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), -(float)Math.PI * 0.5f);
                }
                else if (punchUpgrade == 2)
                {
                    Entity particle = Entity.InstantiatePrefab("WidePunchPartigirl");
                    particle.GetComponent<Transform>().globalPosition = playerTransform.globalPosition - playerTransform.forward * 0.4f + new Vector3(0, heightAdjustment, 0);
                    particle.GetComponent<Transform>().globalRotation = playerTransform.globalRotation * Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), -(float)Math.PI * 0.5f);
                }
            }
            
            //when activated - behaviour of wave
            if (activated)
            {
                if (!shrinkTime)
                {
                    if (waveTimer < maxWaveTimer)
                    {
                        renderer.active = true;
                        transform.globalPosition = Linear(originalPos, movementVector, 1 - (1 - waveTimer / maxWaveTimer) * (1 - waveTimer / maxWaveTimer));

                        if(playerPunch.punchWaveCharging) //shake for charged punch
                        {
                            float xShaker = Breakable.NextFloat(-0.1f, 0.1f);
                            float zShaker = Breakable.NextFloat(-0.1f, 0.1f);
                            transform.globalPosition += new Vector3(xShaker, 0, zShaker);
                        }

                        waveTimer += Time.fixedDeltaTime; //fixed update
                    }
                    else
                    {
                        waveTimer = 0;
                        shrinkTime = true;
                    }
                }
                else
                {
                    if (waveTimer < shrinkTimer)
                    {
                        transform.globalScale = Linear(newScale, Vector3.zero, waveTimer / shrinkTimer);
                        waveTimer += Time.fixedDeltaTime; //fixed update
                    }
                    else
                    {
                        waveTimer = 0;
                        transform.globalScale = newScale;
                        shrinkTime = false;
                        activated = false;
                        renderer.active = false;
                    }
                }
            }
        }

        public static Vector3 Linear(Vector3 v0, Vector3 v1, float time)
        {
            return v0 + time * (v1 - v0);
        }
    }
}
