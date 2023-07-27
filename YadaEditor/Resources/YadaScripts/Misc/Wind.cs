using System;
using YadaScriptsLib;

namespace YadaScripts
{
    public class Wind : Component
    {
        private Transform transform;

        public static Vector3 currWindDirection;

        public float windVariation;
        public float windStrength;
        public static Quaternion windAngle;

        public static float currWindStrength;
        private float newWindStrength;
        private float currTimer;
        private bool isSwaying;
        private bool swayBack;

        public float maxTimer;
        public float pauseTimer;

        void Start()
        {
            transform = this.entity.GetComponent<Transform>();

            currWindDirection = transform.forward;
            currTimer = 0;
        }

        void Update()
        {
            currWindDirection = transform.forward;
            currWindDirection.Normalize();

            //will need the current wind strength to be interpolated

            if (isSwaying)
            {
                switch(swayBack)
                {
                    case false:
                        if (currTimer < maxTimer)
                        {
                            currWindStrength = RollingIndicator.SmoothStep(0, newWindStrength, currTimer / maxTimer);
                            currTimer += Time.deltaTime;
                        }
                        else
                        {
                            currTimer = 0;
                            swayBack = true;
                        }
                        break;

                    case true:
                        if (currTimer < maxTimer)
                        {
                            currWindStrength = RollingIndicator.SmoothStep(newWindStrength, 0, currTimer / maxTimer);
                            currTimer += Time.deltaTime;
                        }
                        else
                        {
                            currTimer = 0;
                            isSwaying = false;
                            swayBack = false;
                        }
                        break;
                }
            }
            else
            {
                if (currTimer < pauseTimer)
                {
                    currTimer += Time.deltaTime;
                }
                else
                {
                    newWindStrength = Breakable.NextFloat(windStrength - windVariation, windStrength + windVariation);
                    currTimer = 0;
                    isSwaying = true;
                }
            }

            windAngle = Quaternion.CreateFromAxisAngle(new Vector3(getXDir(), 0, getZDir()), Wind.currWindStrength);
        }

        public static float getXDir()
        {
            return currWindDirection.x / (currWindDirection.x + currWindDirection.z);
        }

        public static float getZDir()
        {
            return currWindDirection.z / (currWindDirection.x + currWindDirection.z);
        }
    }
}