using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class CartPushAnimationResponse : Component
    {
        public Entity durian;
        public Entity cart;
        private EventResponse eventRes;

        public Entity startPos;
        public Entity startDurianPos;
        public Entity endPos;

        public Entity cutsceneTrigger;

        public Entity finishedText;

        public float travelSpeed = 3.0f;
        public bool despawn = false;

        bool startedPush = false;
        bool finishedPush = false;
        Vector3 travelDir;

        private AudioSource myAudioSource;

        void Start()
        {
            eventRes = this.entity.GetComponent<EventResponse>();
            travelDir = (endPos.GetComponent<Transform>().globalPosition - startPos.GetComponent<Transform>().globalPosition).normalized;
            //triggerEntities = Entity.GetEntitiesWithComponent<EventTrigger>();
            myAudioSource = this.entity.AddComponent<AudioSource>();
            myAudioSource.isPlayedOnStart = false;
            myAudioSource.loop = true;
            myAudioSource.sourceName = "SFX_Cart_Low";
        }

        void Update()
        {
            if (eventRes.fired == true && startedPush == false)
            {
                cart.GetComponent<Transform>().globalPosition = startPos.GetComponent<Transform>().globalPosition;
                cart.GetComponent<Transform>().globalRotation = startPos.GetComponent<Transform>().globalRotation;
                cart.GetComponent<Animator>().animationIndex = 1;
                cart.GetComponent<Animator>().animateCount = -1;

                durian.GetComponent<Transform>().globalPosition = startDurianPos.GetComponent<Transform>().globalPosition;
                durian.GetComponent<Transform>().globalRotation = startDurianPos.GetComponent<Transform>().globalRotation;
                durian.GetComponent<Animator>().animationIndex = 16;
                durian.GetComponent<Animator>().animateCount = -1;

                startedPush = true;
                Audio.PlaySource(myAudioSource);
                cutsceneTrigger.GetComponent<CutsceneTrigger>().ManuallyTriggerCutscene();
            }
            else if (startedPush && finishedPush == false)
            {
                cart.GetComponent<Transform>().globalPosition += travelDir * travelSpeed * Time.deltaTime;
                durian.GetComponent<Transform>().globalPosition += travelDir * travelSpeed * Time.deltaTime;

                if ((endPos.GetComponent<Transform>().globalPosition - cart.GetComponent<Transform>().globalPosition).magnitude <= 0.1f)
                {
                    travelSpeed = 0.0f;
                    cart.GetComponent<Animator>().animationIndex = 0;
                    durian.GetComponent<Animator>().animationIndex = 7;
                    //durian.GetComponent<Transform>().globalRotation;
                    finishedPush = true;
                    Audio.StopSource(myAudioSource.channel);

                    if (finishedText != null)
                        finishedText.active = true;

                    if (eventRes.eventID == 69) //Hardcoded event for scene 2, fix bridge part 2. Enable the roll power cutscene only after crossing bridge
                    {
                        durian.GetComponent<Transform>().localEulerAngles = new Vector3(0.0f, -100.0f, 0.0f);
                        //SceneController.canGiveRollPowerup = true;
                        Entity rollColliderEntity = Entity.GetEntitiesWithComponent<Unlockable>()[0];
                        rollColliderEntity.GetComponent<Collider>().active = true;
                    }
                    if (despawn)
                    {
                        cart.active = false;
                        durian.active = false;
                    }
                }
            }
        }

        
    }
}
