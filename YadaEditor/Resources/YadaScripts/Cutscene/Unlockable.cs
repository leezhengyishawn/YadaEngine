using System;
using YadaScriptsLib;


namespace YadaScripts
{
    public class Unlockable : Component
    {
        public Entity bgSprite = null;
        public Entity lockSprite = null;
        public Entity lockRingSprite = null;
        public Entity unlockedSprite = null;
        public Entity unlockedPrompt = null;
        public Vector3 lockRingStartPos;
        public Vector3 lockRingEndPos;
        public bool isChargedPunch = false;
        public bool triggered = false;

        private UI ui1; //Background
        private UI ui2; //Lock
        private UI ui3; //Lock Ring
        private AudioSource unlockSFX;
        private AudioSource uiAppearSFX;
        private AudioSource achieveSFX;
        private float cTimer1;
        private float cTimer2;
        private bool lerpInLockRing = false;
        private bool unlockPlay = false;
        private bool promptPlay = false;

        // Animating transform purposes
        public float bgRotationSpeed;
        private Vector3 bgScaleSize = new Vector3(480.0f, 480.0f, 1.0f);
        private Vector3 bgScaleVel = Vector3.zero;

        void Start()
        {
            unlockSFX = lockRingSprite.GetComponent<AudioSource>();
            uiAppearSFX = bgSprite.GetComponent<AudioSource>();
            achieveSFX = unlockedSprite.GetComponent<AudioSource>();

            lockRingStartPos = lockRingSprite.GetComponent<Transform>().localPosition;
        }


        void FixedUpdate()
        {
            if (triggered && !unlockPlay)
            {
                // Just keep bgSprite rotating
                if (bgSprite)
                {
                    Transform bgTrans = bgSprite.GetComponent<Transform>();
                    Vector3 currentRotation = bgTrans.localEulerAngles;
                    currentRotation.z += bgRotationSpeed * Time.fixedDeltaTime;
                    bgTrans.localEulerAngles = currentRotation;

                    // Scale bgSprite
                    bgTrans.SmoothResize(bgTrans.localScale, bgScaleSize, ref bgScaleVel, 0.1f, float.MaxValue, Time.fixedDeltaTime);
                }

                if (lockSprite.GetComponent<UI>().color.w < 1 && !unlockPlay && cTimer1 <= 0f)
                {
                    ui1 = bgSprite.GetComponent<UI>();
                    Vector4 color = ui1.color;
                    color.w += Time.fixedDeltaTime * 2f;
                    ui1.color = color;

                    ui2 = lockSprite.GetComponent<UI>();
                    Vector4 color2 = ui2.color;
                    color2.w += Time.fixedDeltaTime * 2f;
                    ui2.color = color2;

                    ui3 = lockRingSprite.GetComponent<UI>();
                    Vector4 color3 = ui3.color;
                    color3.w += Time.fixedDeltaTime * 2f;
                    ui3.color = color3;

                    if (!Audio.IsSourcePlaying(uiAppearSFX.channel))
                        Audio.PlaySource(uiAppearSFX);
                }

                else if (lockSprite.GetComponent<UI>().color.w >= 1)
                {
                    cTimer1 += Time.fixedDeltaTime;
                    lerpInLockRing = true;

                    if (!Audio.IsSourcePlaying(unlockSFX.channel))
                        Audio.PlaySource(unlockSFX);
                }

                if (cTimer1 >= 1f && lockSprite.GetComponent<UI>().color.w > 0)
                {
                    ui1 = bgSprite.GetComponent<UI>();
                    Vector4 color = ui1.color;
                    color.w -= Time.fixedDeltaTime * 3f;
                    ui1.color = color;

                    ui2 = lockSprite.GetComponent<UI>();
                    Vector4 color2 = ui2.color;
                    color2.w -= Time.fixedDeltaTime * 3f;
                    ui2.color = color2;

                    ui3 = lockRingSprite.GetComponent<UI>();
                    Vector4 color3 = ui3.color;
                    color3.w -= Time.fixedDeltaTime * 4f;
                    ui3.color = color3;
                }

                else if (lockSprite.GetComponent<UI>().color.w <= 0)
                {
                    lockRingSprite.GetComponent<Transform>().localPosition = lockRingStartPos;
                    cTimer1 = 0f;
                    unlockPlay = true;
                }
            }

            if (lerpInLockRing)
            {
                cTimer2 += Time.fixedDeltaTime;
                if (cTimer2 > 0.8f)
                    cTimer2 = 0.8f;
                lockRingSprite.GetComponent<Transform>().localPosition = Vector3.Lerp(lockRingStartPos, lockRingEndPos, cTimer2*2f);
            }

            if (unlockPlay && lerpInLockRing)
            {
                if (isChargedPunch)
                {
                    SceneController.hasChargedAttack = true;
                }
                else
                {
                    SceneController.hasRollAttack = true;
                }

                if (unlockedSprite.GetComponent<UI>().color.w < 1 && cTimer1 <= 0f)
                {
                    ui1 = unlockedSprite.GetComponent<UI>();
                    Vector4 color = ui1.color;
                    color.w += Time.fixedDeltaTime * 2f;
                    ui1.color = color;

                    if (!Audio.IsSourcePlaying(achieveSFX.channel))
                        Audio.PlaySource(achieveSFX);
                }

                else if (unlockedSprite.GetComponent<UI>().color.w >= 1)
                {
                    cTimer1 += Time.fixedDeltaTime;
                }

                if (cTimer1 >= 2f && unlockedSprite.GetComponent<UI>().color.w > 0)
                {
                    ui1 = unlockedSprite.GetComponent<UI>();
                    Vector4 color = ui1.color;
                    color.w -= Time.fixedDeltaTime * 3f;
                    ui1.color = color;
                }

                else if (unlockedSprite.GetComponent<UI>().color.w <= 0)
                {
                    promptPlay = true;
                    lerpInLockRing = false;
                    cTimer1 = 0f;
                }
            }

            if (promptPlay)
            {
                if (unlockedPrompt.GetComponent<UI>().color.w < 1 && cTimer1 <= 0f)
                {
                    ui1 = unlockedPrompt.GetComponent<UI>();
                    Vector4 color = ui1.color;
                    color.w += Time.fixedDeltaTime * 2f;
                    ui1.color = color;

                    if (!Audio.IsSourcePlaying(uiAppearSFX.channel) && unlockedPrompt.GetComponent<UI>().color.w < 0.5)
                        Audio.PlaySource(uiAppearSFX);
                }

                else if (unlockedPrompt.GetComponent<UI>().color.w >= 1)
                {
                    cTimer1 += Time.fixedDeltaTime;
                }

                if (cTimer1 >= 5f && unlockedPrompt.GetComponent<UI>().color.w > 0)
                {
                    ui1 = unlockedPrompt.GetComponent<UI>();
                    Vector4 color = ui1.color;
                    color.w -= Time.fixedDeltaTime * 1.5f;
                    ui1.color = color;
                }

                if (unlockedPrompt.GetComponent<UI>().color.w <= 0)
                {
                    promptPlay = false;
                    SceneController.isPromptOpen = false;
                }
            }
        }

    }

}
