using System;
using YadaScriptsLib;

namespace YadaScripts
{
    public class GateResponse : Component
    {
        public float gateDelay = 0.0f;
        private Random getRandom;

        private Entity gatePrefab;
        private ParticleEmitter gateParticle;
        private Transform myTransform;
        private Collider myCollider;
        private Vector3 startingPos;
        private EventResponse eventRes;
        private float openGateAmount;
        private float shakeAmount;
        private float timerShakeCurrent = 0.0f;
        private float timerShakeMaximum = 0.5f;
        private float timerOpenCurrent = 0.0f;
        private float timerOpenMaximum = 1.0f;
        private bool hasTriggeredOnce;
        private bool isOpeningGate;
        private bool hasPlayedOnce;

        void Start()
        {
            getRandom = new Random();
            eventRes = this.entity.GetComponent<EventResponse>();
            myTransform = this.entity.GetComponent<Transform>();
            myCollider = this.entity.GetComponent<Collider>();
            startingPos = myTransform.localPosition;
            openGateAmount = -((myCollider.halfExtents.y * 2.0f) + 1.0f);
            shakeAmount = 0.05f;
            timerShakeMaximum = 0.7f;
            timerOpenMaximum = 3.0f;
            gatePrefab = Entity.InstantiatePrefab("GateParticle");
            Transform particleTransform = gatePrefab.GetComponent<Transform>();
            particleTransform.globalPosition = myTransform.globalPosition;
            particleTransform.globalRotation = myTransform.globalRotation;
            gateParticle = particleTransform.GetChildByIndex(0).entity.GetComponent<ParticleEmitter>();
            gateParticle.Pause();
        }

        void Update()
        {
            CheckFire();
            CheckTimer();

        }

        private void CheckFire()
        {
            if (eventRes.fired == true)
            {
                if (gateDelay > 0)
                {
                    gateDelay -= Time.deltaTime;
                }
                else if (hasPlayedOnce == false)
                {
                    AudioController.PlaySFX("SFX Gate Rock Crumbling");
                    gateParticle.Restart();
                    isOpeningGate = true;
                    hasPlayedOnce = true;
                }
            }
            else if (eventRes.intermediateFired == true)
            {
                AudioController.PlaySFX("SFX Gate Rock Shaking");
                gateParticle.Restart();
                timerShakeCurrent = 0.0f;
                hasTriggeredOnce = true;
                eventRes.SetIntermediateEvent(false);
            }
        }

        private void CheckTimer()
        {
            if (isOpeningGate == true)
            {
                if (timerOpenCurrent < timerOpenMaximum)
                {
                    timerOpenCurrent += Time.deltaTime;
                    myTransform.localPosition = new Vector3(startingPos.x + (GetRandomNumber(-shakeAmount, shakeAmount) / 2.0f), myTransform.localPosition.y + (openGateAmount / timerOpenMaximum) * Time.deltaTime, startingPos.z + GetRandomNumber(-shakeAmount, shakeAmount));
                }
                else
                {
                    gateParticle.Pause();
                    timerOpenCurrent = 0.0f;
                    isOpeningGate = false;
                    myCollider.isTrigger = true;
                    this.active = false;
                }
            }
            else if (hasTriggeredOnce == true)
            {
                if (timerShakeCurrent < timerShakeMaximum)
                {
                    myTransform.localPosition = startingPos + new Vector3(GetRandomNumber(-shakeAmount, shakeAmount)/2.0f, 0.0f, GetRandomNumber(-shakeAmount, shakeAmount));
                    timerShakeCurrent += Time.deltaTime;
                }
                else
                {
                    gateParticle.Pause();
                    myTransform.localPosition = startingPos;
                    timerShakeCurrent = 0.0f;
                    hasTriggeredOnce = false;
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
    }
}