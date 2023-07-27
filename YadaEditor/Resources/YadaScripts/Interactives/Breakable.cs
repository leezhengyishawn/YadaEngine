using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class Breakable : Component
    {
        private bool isActivated;

        private float currDeathTimer;
        private float maxDeathTimer;
        private Transform transform;
        private Vector3 ogPosition;
        private Vector3 ogScale;
        private bool stage1;

        void Start()
        {
            maxDeathTimer = 0.1f;
            currDeathTimer = 0;

            transform = this.entity.GetComponent<Transform>();

            ogPosition = transform.localPosition;
            ogScale = transform.localScale;
            stage1 = true;
        }

        void Update()
        {
            if (isActivated == true)
            {
                if (stage1)
                {
                    if (currDeathTimer < maxDeathTimer)
                    {
                        //do animation
                        float xShaker = NextFloat(-0.1f, 0.1f);
                        float zShaker = NextFloat(-0.1f, 0.1f);

                        transform.localPosition = new Vector3(ogPosition.x + xShaker, ogPosition.y, ogPosition.z + zShaker);


                        currDeathTimer += Time.deltaTime;
                    }
                    else
                    {
                        currDeathTimer = 0;
                        stage1 = false;
                    }
                }
                else
                {
                    if (currDeathTimer < maxDeathTimer)
                    {
                        //shrink
                        transform.localScale = Selector.SmoothStep(ogScale, new Vector3(0, 0, 0), currDeathTimer / maxDeathTimer);

                        //shrink in level to ground
                        transform.localPosition = new Vector3(ogPosition.x, ogPosition.y - (transform.localScale.y * 0.5f), ogPosition.z);

                        currDeathTimer += Time.deltaTime;
                    }
                    else
                    {
                        Entity particle = Entity.InstantiatePrefab("ParticleBurst_Bush");
                        particle.GetComponent<Transform>().globalPosition = transform.globalPosition;
                        isActivated = false;
                        transform.localPosition = new Vector3(ogPosition.x, ogPosition.y - 5.0f, ogPosition.z);
                        this.active = false;
                        
                        //Entity.DestroyEntity(this.entity);
                    }
                }
            }
        }

        public void ActivateObject()
        {
            AudioController.PlaySFX("SFX Bush Destroyed");
            isActivated = true;
        }

        static public float NextFloat(float min, float max)
        {
            System.Random random = new System.Random();
            double val = (random.NextDouble() * (max - min) + min);
            return (float)val;
        }
    }
}