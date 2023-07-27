using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class SceneManager : Component
    {
        private enum SplashScreenState
        {
            INPUT_WAIT,
            DIGIPEN_FADE_IN,
            DIGIPEN_PAUSE,
            DIGIPEN_FADE_OUT,
            ONE_TOUCH_FADE_IN,
            START_GAME
        }

        private enum OneTouchState
        {
            ONE_FADE_IN,
            HAND_SPAWN_AND_MOVE,
            ONE_TOUCHED,
            ENDING_ROTATION,
            BEFORE_FADE_OUT,
            FADE_OUT,
        }

        // public Entity blackScreenEnt;
        public Entity digipenLogoEnt;

        // private UI blackScreenComp;
        private UI digipenComp;


        public Entity logoOne;
        public Entity logoLeftHand;
        public Entity logoRightHand;
        public Entity logoTouch;
        public Entity logoBlueBG;
        public Entity logoBlackSpark;
        public Entity logoWhiteRing;

        private UI uiOne;
        private UI uiLeftHand;
        private UI uiRightHand;
        private UI uiTouch;
        private UI uiBlueBG;
        private UI uiBlackSpark;
        private UI uiWhiteRing;


        public float overallFadeSpeedMultiplier = 1.0f;
        private SplashScreenState m_splashState = SplashScreenState.DIGIPEN_FADE_IN;
        private OneTouchState m_logoState = OneTouchState.ONE_FADE_IN;

        private Vector3 leftHandStart;
        private Vector3 rightHandStart;

        private Vector3 leftHandEnd;
        private Vector3 rightHandEnd;

        private Vector3 leftVelocity = Vector3.zero;
        private Vector3 rightVelocity = Vector3.zero;
        private Vector3 whiteVelocity = Vector3.zero;

        private Vector3 whiteRingFinalScale;

        private Transform transLeft;
        private Transform transRight;
        private Transform transBlueBG;
        private Transform transWhiteRing;

        public Entity oneTouchAudioEnt;
        public AudioSource oneTouchAudioComp;

        private float threshold = 200.0f;
        private float delay = 0.25f;

        private float extraTimer;

        void Start()
        {
            digipenComp = digipenLogoEnt.GetComponent<UI>();

            uiOne = logoOne.GetComponent<UI>();
            uiLeftHand = logoLeftHand.GetComponent<UI>();
            uiRightHand = logoRightHand.GetComponent<UI>();
            uiTouch = logoTouch.GetComponent<UI>();
            uiBlueBG = logoBlueBG.GetComponent<UI>();
            uiBlackSpark = logoBlackSpark.GetComponent<UI>();
            uiWhiteRing = logoWhiteRing.GetComponent<UI>();

            oneTouchAudioComp = oneTouchAudioEnt.GetComponent<AudioSource>();

            transLeft = logoLeftHand.GetComponent<Transform>();
            transRight = logoRightHand.GetComponent<Transform>();
            transBlueBG = logoBlueBG.GetComponent<Transform>();
            transWhiteRing = logoWhiteRing.GetComponent<Transform>();


            leftHandEnd = transLeft.localPosition;
            rightHandEnd = transRight.localPosition;

            leftHandStart = new Vector3(-750.0f, -750.0f, leftHandEnd.z);
            rightHandStart = new Vector3(750.0f, 750.0f, rightHandEnd.z);

            transLeft.localPosition = leftHandStart;
            transRight.localPosition = rightHandStart;

            whiteRingFinalScale = transWhiteRing.localScale;
            transWhiteRing.localScale = new Vector3(2000.0f, 2000.0f, 1.0f);
        }

        void FixedUpdate()
        {
            if (Input.GetKeyPress(KEYCODE.KEY_SPACE))
                m_splashState = SplashScreenState.START_GAME;

            switch (m_splashState)
            {
                case SplashScreenState.INPUT_WAIT:
                    if (Input.GetKeyPress(KEYCODE.KEY_ESCAPE))
                        m_splashState = SplashScreenState.DIGIPEN_FADE_IN;
                    break;
                case SplashScreenState.DIGIPEN_FADE_IN:
                    if (FadeIn(digipenComp, 1.5f))
                        m_splashState = SplashScreenState.DIGIPEN_PAUSE;
                    break;
                case SplashScreenState.DIGIPEN_PAUSE:
                    extraTimer += Time.fixedDeltaTime;
                    if (extraTimer >= 2.0f) // Show for 2 Seconds
                        m_splashState = SplashScreenState.DIGIPEN_FADE_OUT;
                    break;
                case SplashScreenState.DIGIPEN_FADE_OUT:
                    if (FadeOut(digipenComp, 1.0f))
                    {
                        m_splashState = SplashScreenState.ONE_TOUCH_FADE_IN;
                    }
                    break;
                case SplashScreenState.ONE_TOUCH_FADE_IN:
                    UpdateLogo();
                    break;
                case SplashScreenState.START_GAME:
                    StartGame();
                    break;
            }
        }

        void UpdateLogo()
        {
            switch (m_logoState)
            {
                case OneTouchState.ONE_FADE_IN:
                    {
                        Vector4 color = uiOne.color;
                        //uiOne.fill += Time.fixedDeltaTime * 2.0f;

                        color.w = 1.0f;
                        uiOne.color = color;

                        delay -= Time.fixedDeltaTime;
                        if (delay <= 0.0f)
                            m_logoState = OneTouchState.HAND_SPAWN_AND_MOVE;
                    }
                    break;

                case OneTouchState.HAND_SPAWN_AND_MOVE:
                    {
                        bool temp1 = MoveTowardsEnd();
                        bool temp2 = FadeIn(uiLeftHand, 1.0f);
                        bool temp3 = FadeIn(uiRightHand, 1.0f);
                        if (temp1 && temp2 && temp3)
                            m_logoState = OneTouchState.ONE_TOUCHED;
                    }
                    break;

                case OneTouchState.ONE_TOUCHED:
                    {
                        MoveTowardsEnd();
                        RotateBlackBG(15.0f);

                        if (FadeIn(uiBlueBG, 1.35f))
                        {
                            m_logoState = OneTouchState.ENDING_ROTATION;
                            Audio.PlaySource(oneTouchAudioComp);
                        }
                    }
                    break;
                case OneTouchState.ENDING_ROTATION:
                    {
                        RotateBlackBG(15.0f);
                        FadeIn(uiWhiteRing, 1.75f);
                        FadeIn(uiTouch, 1.75f);
                        FadeIn(uiBlackSpark, 1.75f);
                        transWhiteRing.localScale = Vector3.SmoothDamp(transWhiteRing.localScale, whiteRingFinalScale, ref whiteVelocity, 0.25f, 7500.0f, Time.fixedDeltaTime);

                        Vector3 diff = transWhiteRing.localScale - whiteRingFinalScale;
                        if (diff.magnitudeSq <= threshold)
                        {
                            m_logoState = OneTouchState.BEFORE_FADE_OUT;

                            delay = 0.25f;

                            CapAlpha(uiOne);
                            CapAlpha(uiLeftHand);
                            CapAlpha(uiRightHand);
                            CapAlpha(uiTouch);
                            CapAlpha(uiBlueBG);
                            CapAlpha(uiBlackSpark);
                            CapAlpha(uiWhiteRing);
                        }
                    }
                    break;

                case OneTouchState.BEFORE_FADE_OUT:
                    RotateBlackBG(15.0f);

                    delay -= Time.fixedDeltaTime;
                    if (delay <= 0.0f)
                        m_logoState = OneTouchState.FADE_OUT;

                    break;


                case OneTouchState.FADE_OUT:
                    {
                        RotateBlackBG(15.0f);

                        FadeOut(uiOne        , 2.0f);  
                        FadeOut(uiLeftHand   , 2.0f);  
                        FadeOut(uiRightHand  , 2.0f);  
                        FadeOut(uiTouch      , 2.0f);  
                        FadeOut(uiBlueBG     , 2.0f);  
                        FadeOut(uiBlackSpark , 2.0f);

                        if (FadeOut(uiWhiteRing, 2.0f))
                            m_splashState = SplashScreenState.START_GAME;
                    }
                    break;
            }
        }

        void RotateBlackBG(float speed)
        {
            Vector3 rotate = transBlueBG.localEulerAngles;
            rotate.z -= Time.fixedDeltaTime * speed;
            transBlueBG.localEulerAngles = rotate;
        }

        bool MoveTowardsEnd()
        {
            transLeft.localPosition = Vector3.SmoothDamp(transLeft.localPosition, leftHandEnd, ref leftVelocity, 0.5f, 1000, Time.fixedDeltaTime);
            transRight.localPosition = Vector3.SmoothDamp(transRight.localPosition, rightHandEnd, ref rightVelocity, 0.5f, 1000, Time.fixedDeltaTime);

            Vector3 diff = transLeft.localPosition - leftHandEnd;
            if (diff.magnitudeSq <= threshold)
                return true;

            return false;
        }

        void CapAlpha(UI ui)
        {
            Vector4 color = ui.color;
            color.w = 1.0f;
            ui.color = color;
        }

        bool FadeIn(UI ui, float fadeSpeed)
        {
            Vector4 color = ui.color;
            color.w += Time.fixedDeltaTime * fadeSpeed;
            ui.color = color;
            if (color.w >= 1.0f)
                return true;

            return false;
        }

        bool FadeOut(UI ui, float fadeSpeed)
        {
            Vector4 color = ui.color;
            color.w -= Time.fixedDeltaTime * fadeSpeed;
            ui.color = color;

            if (color.w <= 0.0f)
                return true;

            return false;
        }

        void StartGame()
        {
            Scene.ChangeScene("MainMenu");
        }

    }
}
