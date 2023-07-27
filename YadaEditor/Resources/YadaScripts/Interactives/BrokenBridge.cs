using System;
using YadaScriptsLib;

namespace YadaScripts
{
    public class BrokenBridge : Component
    {
        private Random getRandom;
        private Transform myTransform;
        private Vector3 startingPos;
        private Vector3 endingPos;
        private Vector3 skyPos;
        private Vector3 steppedPos;
        private float timerShakeCurrent;
        private float timerShakeMaximum;
        private float timerSinkCurrent;
        private float timerSinkMaximum;
        private float shakeValue;
        private float floatingSpeed;
        private bool playerStepped;
        private bool isSinking;
        private bool isFloatingDown;
        private bool toRespawnBack;

        private void Start()
        {
            getRandom = new Random();
            myTransform = this.entity.GetComponent<Transform>();
            startingPos = myTransform.localPosition;
            endingPos = startingPos - Vector3.up;
            skyPos = startingPos + (Vector3.up * 50.0f);
            timerShakeMaximum = 0.5f;
            timerSinkMaximum = 1.0f;
            shakeValue = 0.1f;
            floatingSpeed = GetRandomNumber(0.05f, 0.1f);
            steppedPos = startingPos - (Vector3.up * 0.1f);
        }

        private void Update()
        {
            if (playerStepped == true)
            {
                if (isSinking == true)
                {
                    if (timerSinkCurrent < timerSinkMaximum)
                    {
                        myTransform.localPosition = Vector3.Lerp(myTransform.localPosition, endingPos, Time.deltaTime);
                        timerSinkCurrent += Time.deltaTime;
                    }
                    else
                    {
                        myTransform.localPosition = skyPos;
                        timerSinkCurrent = 0.0f;
                        isSinking = false;
                        playerStepped = false;
                        toRespawnBack = true;
                    }
                }
                else
                {
                    if (SceneController.isInCutscene == true)
                    {
                        timerShakeCurrent = 0.0f;
                    }
                    else
                    {
                        if (timerShakeCurrent < timerShakeMaximum)
                        {
                            timerShakeCurrent += Time.deltaTime;
                            float shakingValue = shakeValue * timerShakeCurrent;
                            if (shakingValue > 0.1f)
                            {
                                shakingValue = 0.1f;
                            }
                            myTransform.localPosition = new Vector3(startingPos.x, myTransform.localPosition.y, startingPos.z) + (new Vector3(GetRandomNumber(-shakingValue, shakingValue), 0.0f, GetRandomNumber(-shakingValue, shakingValue)));
                        }
                        else
                        {
                            AudioController.PlaySFX("SFX Step Floating");
                            timerShakeCurrent = 0.0f;
                            isSinking = true;
                        }
                    }
                }
            }
            else
            {
                if (toRespawnBack == true)
                {
                    if (GetDistance(myTransform.localPosition, startingPos) < 0.01f)
                    {
                        toRespawnBack = false;
                    }
                    else
                    {
                        myTransform.localPosition = Vector3.Lerp(myTransform.localPosition, startingPos, 10.0f * Time.deltaTime);
                    }
                }
                else
                {
                    if (isFloatingDown == false)
                    {
                        if (GetDistance(myTransform.localPosition, startingPos) < 0.01f)
                        {
                            isFloatingDown = true;
                        }
                        else
                        {
                            myTransform.localPosition = Vector3.MoveTowards(myTransform.localPosition, startingPos, floatingSpeed * Time.deltaTime);
                        }
                    }
                    else
                    {
                        if (GetDistance(myTransform.localPosition, steppedPos) < 0.01f)
                        {
                            isFloatingDown = false;
                        }
                        else
                        {
                            myTransform.localPosition = Vector3.MoveTowards(myTransform.localPosition, steppedPos, floatingSpeed * Time.deltaTime);
                        }
                    }
                }
            }
        }

        private float GetRandomNumber(float min, float max)
        {
            lock (getRandom)
            {
                return (float)getRandom.NextDouble() * (max - min) + min;
            }
        }

        void OnTriggerStay(Entity collider)
        {
            GroundCheck touchedGroundCheck = collider.GetComponent<GroundCheck>();
            if (touchedGroundCheck != null && touchedGroundCheck.playerBehaviour.isInRespawnSequence == false && playerStepped == false)
            {
                timerSinkCurrent = 0.0f;
                playerStepped = true;
            }
        }

        private float GetDistance(Vector3 point1, Vector3 point2)
        {
            return (point1 - point2).magnitude;
        }
    }
}