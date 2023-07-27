using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class MainMenuControl : Component
    {
        private enum MainMenuState
        {
            INIT,
            MAIN_PAGE,
            HOW_TO_PLAY_PANEL,
            SETTINGS_PANEL,
            CREDITS_PANEL,
            CONFIRM,
            LEVEL_SELECTED
        }

        private enum E_BUTTON
        {
            NONE = 0,
            START,
            CONTROLS,
            SETTINGS,
            CREDITS,
            QUIT,
            YES,
            NO
        }

        //Analog Stick
        private bool analogLeft = false;
        private bool analogRight = false;
        private bool analogUp = false;
        private bool analogDown = false;
        private float analogTimer = 0.0f;

        //States
        private MainMenuState mainMenuState;
        private E_BUTTON m_currentButton;

        // buttonCredits.GetComponent<MenuButton>().active = false;

        // Main menu buttons
        public Entity startButtonEnt;
        public Entity controlsButtonEnt;
        public Entity settingsButtonEnt;
        public Entity creditsButtonEnt;
        public Entity quitButtonEnt;
        public Entity backButtonEnt;
        public Entity yesButtonEnt;
        public Entity noButtonEnt;

        //Panels
        public Entity controlsPanelEnt;
        public Entity settingsPanelEnt;
        public Entity creditsPanelEnt;
        public Entity blurPanelEnt;
        public Entity quitPanelEnt;
        public Entity titleLogo;

        private MainMenuButton startButtonScript;
        private MainMenuButton controlsButtonScript;
        private MainMenuButton settingsButtonScript;
        private MainMenuButton creditsButtonScript;
        private MainMenuButton quitButtonScript;
        private MainMenuButton backButtonScript;

        private UI startButtonComp;
        private UI controlsButtonComp;
        private UI settingsButtonComp;
        private UI creditsButtonComp;
        private UI quitButtonComp;
        private UI backButtonComp;

        // Fade In/Out
        public Entity whiteBGEnt;
        private UI whiteBGComp;

        //Title Logo anim
        private UI ui;
        private float cTimer;
        private Vector3 velocity = Vector3.zero;
        private Vector3 titleStartPos;
        private Vector3 titleStartScale;

        // SFX
        public Entity hoverSFXent;
        public Entity clickSFXent;

        public AudioSource hoverSFXcomp;
        public AudioSource clickSFXcomp;

        private AudioSource titleSFX;

        void Start()
        {
            mainMenuState = MainMenuState.INIT;
            m_currentButton = E_BUTTON.NONE;

            // Get UI
            startButtonScript = startButtonEnt.GetComponent<MainMenuButton>();
            quitButtonScript = quitButtonEnt.GetComponent<MainMenuButton>();
            controlsButtonScript = controlsButtonEnt.GetComponent<MainMenuButton>();
            settingsButtonScript = settingsButtonEnt.GetComponent<MainMenuButton>();
            creditsButtonScript = creditsButtonEnt.GetComponent<MainMenuButton>();
            backButtonScript = backButtonEnt.GetComponent<MainMenuButton>();

            startButtonComp = startButtonEnt.GetComponent<UI>();
            controlsButtonComp = controlsButtonEnt.GetComponent<UI>();
            settingsButtonComp = settingsButtonEnt.GetComponent<UI>();
            creditsButtonComp = creditsButtonEnt.GetComponent<UI>();
            quitButtonComp = quitButtonEnt.GetComponent<UI>();
            backButtonComp = backButtonEnt.GetComponent<UI>();

            blurPanelEnt.active = false;

            // White BG
            whiteBGComp = whiteBGEnt.GetComponent<UI>();
            Vector4 color = whiteBGComp.color;
            color.w = 1.0f;
            whiteBGComp.color = color;

            // SFX
            hoverSFXcomp = hoverSFXent.GetComponent<AudioSource>();
            clickSFXcomp = clickSFXent.GetComponent<AudioSource>();
            titleSFX = titleLogo.GetComponent<AudioSource>();

            startButtonEnt.active = false;
            settingsButtonEnt.active = false;
            quitButtonEnt.active = false;
            creditsButtonEnt.active = false;
            controlsButtonEnt.active = false;

            // Load the master volume, override the scene's master volume (if available)
            File.ReadJsonFile("tempSave");
            if (File.CheckDataExists("MasterVolume"))
                Audio.masterVolume = File.ReadDataAsFloat("MasterVolume");
        }

        void Update()
        {

            if (Input.GetKeyPress(KEYCODE.KEY_1))
            {
                Scene.ChangeScene("SplashScreen");
            }

            // Update Fade in Fade Out
            WhiteBGUpdate();
            TitleSequence();

            // Switch Cases for Main Menu State
            switch (mainMenuState)
            {
                case MainMenuState.INIT:
                    {
                        startButtonEnt.active = false;
                        controlsButtonEnt.active = false;
                        settingsButtonEnt.active = false;
                        creditsButtonEnt.active = false;
                        quitButtonEnt.active = false;

                        backButtonEnt.active = false;
                        controlsPanelEnt.active = false;
                        settingsPanelEnt.active = false;
                        creditsPanelEnt.active = false;
                        quitPanelEnt.active = false;

                        blurPanelEnt.active = false;
                        titleLogo.active = true;
                    }
                    break;
                case MainMenuState.MAIN_PAGE:
                    {
                        startButtonEnt.active = true;
                        controlsButtonEnt.active = true;
                        settingsButtonEnt.active = true;
                        creditsButtonEnt.active = true;
                        quitButtonEnt.active = true;


                        backButtonEnt.active = false;
                        controlsPanelEnt.active = false;
                        settingsPanelEnt.active = false;
                        creditsPanelEnt.active = false;
                        quitPanelEnt.active = false;

                        blurPanelEnt.active = false;
                        titleLogo.active = true;

                    }
                    break;
                case MainMenuState.HOW_TO_PLAY_PANEL:
                    {
                        startButtonEnt.active = false;
                        controlsButtonEnt.active = false;
                        settingsButtonEnt.active = false;
                        creditsButtonEnt.active = false;
                        quitButtonEnt.active = false;
                        

                        backButtonEnt.active = true;
                        controlsPanelEnt.active = true;
                        settingsPanelEnt.active = false;
                        creditsPanelEnt.active = false;
                        quitPanelEnt.active = false;

                        blurPanelEnt.active = true;
                        titleLogo.active = false;
                    }
                    break;
                case MainMenuState.SETTINGS_PANEL:
                    {
                        startButtonEnt.active = false;
                        controlsButtonEnt.active = false;
                        settingsButtonEnt.active = false;
                        creditsButtonEnt.active = false;
                        quitButtonEnt.active = false;
                        quitPanelEnt.active = false;

                        backButtonEnt.active = true;
                        controlsPanelEnt.active = false;
                        settingsPanelEnt.active = true;
                        creditsPanelEnt.active = false;

                        blurPanelEnt.active = true;
                        titleLogo.active = false;

                    }
                    break;
                case MainMenuState.CREDITS_PANEL:
                    {
                        startButtonEnt.active = false;
                        controlsButtonEnt.active = false;
                        settingsButtonEnt.active = false;
                        creditsButtonEnt.active = false;
                        quitButtonEnt.active = false;
                        

                        backButtonEnt.active = true;
                        controlsPanelEnt.active = false;
                        settingsPanelEnt.active = false;
                        creditsPanelEnt.active = true;
                        quitPanelEnt.active = false;

                        blurPanelEnt.active = true;
                        titleLogo.active = false;

                    }
                    break;
                case MainMenuState.CONFIRM:
                    {
                        startButtonEnt.active = false;
                        controlsButtonEnt.active = false;
                        settingsButtonEnt.active = false;
                        creditsButtonEnt.active = false;
                        quitButtonEnt.active = false;


                        backButtonEnt.active = false;
                        controlsPanelEnt.active = false;
                        settingsPanelEnt.active = false;
                        creditsPanelEnt.active = false;
                        quitPanelEnt.active = true;

                        blurPanelEnt.active = true;
                        titleLogo.active = false;
                    }
                    break;
            }

            UpdateMainPage();
        }

        void UpdateMainPage()
        {
            // Mouse Hover Check
            if (UpdateMainPageMouse())
                return;

            // KeyBoard Check
            if (UpdateMainPageGamePadKeyBoard())
                return;
        }

        bool UpdateMainPageMouse()
        {
            //Console.WriteLine("Currentstate = " + mainMenuState);
            //if (startButtonScript.hover)
            //    m_currentButton = E_BUTTON.START;

            //if (controlsButtonScript.hover)
            //    m_currentButton = E_BUTTON.CONTROLS;

            //if (settingsButtonScript.hover)
            //    m_currentButton = E_BUTTON.SETTINGS;

            //if (creditsButtonScript.hover)
            //    m_currentButton = E_BUTTON.CREDITS;

            //if (quitButtonScript.hover)
            //    m_currentButton = E_BUTTON.QUIT;


            if (startButtonScript.click)
            {
                ReverseBGUpdate();
                Scene.ChangeScene("CinematicPrologue");
            }

            if (controlsButtonScript.click)
            {
                controlsButtonScript.hover = false;
                controlsButtonScript.click = false;
                mainMenuState = MainMenuState.HOW_TO_PLAY_PANEL;
                return true;
            }

            if (settingsButtonScript.click)
            {
                settingsButtonScript.hover = false;
                settingsButtonScript.click = false;
                mainMenuState = MainMenuState.SETTINGS_PANEL;
                return true;
            }

            if (creditsButtonScript.click)
            {
                creditsButtonScript.hover = false;
                creditsButtonScript.click = false;
                mainMenuState = MainMenuState.CREDITS_PANEL;
                return true;
            }

            if (yesButtonEnt.GetComponent<MainMenuButton>().click)
            {
                Scene.Quit();
                return true;
            }

            if (noButtonEnt.GetComponent<MainMenuButton>().click)
            {
                noButtonEnt.GetComponent<MainMenuButton>().hover = false;
                noButtonEnt.GetComponent<MainMenuButton>().click = false;
                mainMenuState = MainMenuState.MAIN_PAGE;
                return true;
            }


            if (quitButtonScript.click)
            {
                quitButtonScript.hover = false;
                quitButtonScript.click = false;
                mainMenuState = MainMenuState.CONFIRM;
                return true;
            }

            if (backButtonScript.click)
            {
                backButtonScript.hover = false;
                backButtonScript.click = false;
                mainMenuState = MainMenuState.MAIN_PAGE;
                return true;
            }

            
            return false;
        }

        bool UpdateMainPageGamePadKeyBoard()
        {
            E_BUTTON prev = m_currentButton;
            if (NavCheckEnter())
            {
                Audio.PlaySource(clickSFXcomp);

                switch (m_currentButton)
                {
                    case E_BUTTON.START:
                        {
                            Save.resetValues();
                            Scene.ChangeScene("CinematicPrologue");
                        }
                        break;
                    case E_BUTTON.CONTROLS:
                        {
                            mainMenuState = MainMenuState.HOW_TO_PLAY_PANEL;
                        }
                        break;
                    case E_BUTTON.SETTINGS:
                        {
                            mainMenuState = MainMenuState.SETTINGS_PANEL;
                        }
                        break;
                    case E_BUTTON.CREDITS:
                        {
                            mainMenuState = MainMenuState.CREDITS_PANEL;
                        }
                        break;
                    case E_BUTTON.QUIT:
                        {
                            m_currentButton = E_BUTTON.NO;
                            mainMenuState = MainMenuState.CONFIRM;
                        }
                        break;
                    case E_BUTTON.YES:
                        {
                            Scene.Quit();
                        }
                        break;
                    case E_BUTTON.NO:
                        {
                            m_currentButton = E_BUTTON.NONE;
                            mainMenuState = MainMenuState.MAIN_PAGE;
                        }
                        break;
                    default:
                        break;
                }
                    
                
            }
            else if (NavCheckUp())
            {
                if (mainMenuState == MainMenuState.MAIN_PAGE)
                {
                    --m_currentButton;
                    if ((int)m_currentButton < (int)E_BUTTON.START)
                        m_currentButton = E_BUTTON.QUIT;

                    Audio.PlaySource(hoverSFXcomp);
                }
                //Console.WriteLine("current btn = " + m_currentButton);
            }
            else if (NavCheckDown())
            {
                if (mainMenuState == MainMenuState.MAIN_PAGE)
                {
                    ++m_currentButton;
                    if ((int)m_currentButton > (int)E_BUTTON.QUIT)
                        m_currentButton = E_BUTTON.START;

                    Audio.PlaySource(hoverSFXcomp);
                }
                    
                //Console.WriteLine("current btn = " + m_currentButton);
            }
            else if (NavCheckLeft())
            {
                if (mainMenuState == MainMenuState.CONFIRM)
                {
                    m_currentButton = (m_currentButton == E_BUTTON.YES) ? E_BUTTON.NO : E_BUTTON.YES;
                    Audio.PlaySource(hoverSFXcomp);
                }
                
                //Console.WriteLine("current btn = " + m_currentButton);
            }
            else if (NavCheckRight())
            {
                if (mainMenuState == MainMenuState.CONFIRM)
                {
                    m_currentButton = (m_currentButton == E_BUTTON.YES) ? E_BUTTON.NO : E_BUTTON.YES;
                    Audio.PlaySource(hoverSFXcomp);
                }

                //Console.WriteLine("current btn = " + m_currentButton);
            }
            else if (NavCheckEscape())
            {
                if (mainMenuState != MainMenuState.MAIN_PAGE)
                {
                    mainMenuState = MainMenuState.MAIN_PAGE;
                }

            }

            if (prev == m_currentButton)
                HighlightCurrentButton();
            else
                HighlightCurrentButton(prev);

            return false;
        }

        void HighlightCurrentButton()
        {
            switch (m_currentButton)
                    {
                    case E_BUTTON.START:
                    {
                        startButtonComp.SetHighlight();
                    }
                    break;
                case E_BUTTON.CONTROLS:
                    {
                        controlsButtonComp.SetHighlight();
                    }
                    break;
                case E_BUTTON.SETTINGS:
                    {
                        settingsButtonComp.SetHighlight();
                    }
                    break;
                case E_BUTTON.CREDITS:
                    {
                        creditsButtonComp.SetHighlight();
                    }
                    break;
                case E_BUTTON.QUIT:
                    {
                        quitButtonComp.SetHighlight();
                    }
                    break;
                case E_BUTTON.YES:
                    {
                        yesButtonEnt.GetComponent<UI>().SetHighlight();
                    }
                    break;
                case E_BUTTON.NO:
                    {
                        noButtonEnt.GetComponent<UI>().SetHighlight();
                    }
                    break;
                default:
                    break;
            }

        }

        void HighlightCurrentButton(E_BUTTON previous)
        {

            //if (previous == E_BUTTON.START)

            switch (m_currentButton)
                {
                    case E_BUTTON.START:
                        {
                            startButtonComp.SetHighlight();
                        }
                        break;
                    case E_BUTTON.CONTROLS:
                        {
                        controlsButtonComp.SetHighlight();
                        }
                        break;
                    case E_BUTTON.SETTINGS:
                        {
                            settingsButtonComp.SetHighlight();
                        }
                        break;
                    case E_BUTTON.CREDITS:
                        {
                            creditsButtonComp.SetHighlight();
                        }
                        break;
                    case E_BUTTON.QUIT:
                        {
                            quitButtonComp.SetHighlight();
                        }
                        break;
                    case E_BUTTON.YES:
                        {
                            yesButtonEnt.GetComponent<UI>().SetHighlight();
                        }
                        break;
                    case E_BUTTON.NO:
                        {
                            noButtonEnt.GetComponent<UI>().SetHighlight();
                        }
                        break;
                    default:
                        break;
                }

        }

        void WhiteBGUpdate()
        {
            Vector4 color = whiteBGComp.color;
            color.w -= Time.deltaTime;
            whiteBGComp.color = color;
        }

        void ReverseBGUpdate()
        {
            Vector4 color = whiteBGComp.color;
            color.w += Time.deltaTime;
            whiteBGComp.color = color;
        }

        void FadeIn(Entity sprite, float speed, UI ui)
        {
            ui = sprite.GetComponent<UI>();
            Vector4 color = ui.color;
            color.w += Time.deltaTime * speed;
            ui.color = color;
        }

        void TitleSequence()
        {
            if(titleLogo.GetComponent<UI>().color.w < 1f)
                FadeIn(titleLogo, 3f, ui);

            if(!Audio.IsSourcePlaying(titleSFX.channel) && cTimer<0.3f && cTimer >0.2f)
                Audio.PlaySource(titleSFX);

            if (cTimer < 2f && startButtonEnt.GetComponent<UI>().color.w < 1f)
            {
                cTimer += Time.deltaTime;
            }

            titleStartPos = titleLogo.GetComponent<Transform>().localPosition;
            titleStartScale = titleLogo.GetComponent<Transform>().localScale;

            if (cTimer > 0.2f )
                titleLogo.GetComponent<Transform>().SmoothResize(titleStartScale, new Vector3(700, 400, 1), ref velocity, 4f, 25f, 1f);

            if (cTimer > 1.5f)
                titleLogo.GetComponent<Transform>().localPosition = Vector3.SmoothDamp(titleStartPos, new Vector3(0, 290, -0.25f), ref velocity, 1f, 10f, 1f);


            if (startButtonEnt.GetComponent<UI>().color.w < 1f && cTimer >= 2f)
            {
                mainMenuState = MainMenuState.MAIN_PAGE;

                FadeIn(startButtonEnt, 1f, ui);
                FadeIn(settingsButtonEnt, 1f, ui);
                FadeIn(quitButtonEnt, 1f, ui);
                FadeIn(creditsButtonEnt, 1f, ui);
                FadeIn(controlsButtonEnt, 1f, ui);
                cTimer = 2.1f;
            }
        }

        bool NavCheckUp()
        {
            if (Input.GetKeyPress(KEYCODE.KEY_UP) ||
                Input.GetKeyPress(KEYCODE.KEY_W) ||
                Input.GetGamepadButtonPress(GAMEPADCODE.GAMEPAD_UP, 0))
            {
                return true;
            }

            if (Input.GetGamepadLeftStickY(0) > GAMEPADDEADZONES.LEFT_THUMBSTICK + 5000)
            {
                //analogTimer += Time.deltaTime;
                if (!analogUp /*|| analogTimer > 1.0f*/)
                {
                    analogUp = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            analogUp = false;
            //analogTimer = 0.0f;

            return false;
        }

        bool NavCheckDown()
        {
            if (Input.GetKeyPress(KEYCODE.KEY_DOWN) ||
                Input.GetKeyPress(KEYCODE.KEY_S) ||
                Input.GetGamepadButtonPress(GAMEPADCODE.GAMEPAD_DOWN, 0))
            {
                return true;
            }

            if (Input.GetGamepadLeftStickY(0) < -GAMEPADDEADZONES.LEFT_THUMBSTICK - 5000)
            {
                //analogTimer += Time.deltaTime;
                if (!analogDown /*|| analogTimer > 1.0f*/)
                {
                    analogDown = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            analogDown = false;
            //analogTimer = 0.0f;

            return false;
        }

        bool NavCheckLeft()
        {
            if (Input.GetKeyPress(KEYCODE.KEY_LEFT) ||
                Input.GetKeyPress(KEYCODE.KEY_A) ||
                Input.GetGamepadButtonPress(GAMEPADCODE.GAMEPAD_LEFT, 0))
            {
                return true;
            }

            if (Input.GetGamepadLeftStickX(0) < -GAMEPADDEADZONES.LEFT_THUMBSTICK - 5000)
            {
                //analogTimer += Time.deltaTime;
                if (!analogLeft /*|| analogTimer > 1.0f*/)
                {
                    analogLeft = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            analogLeft = false;
            //analogTimer = 0.0f;

            return false;
        }

        bool NavCheckRight()
        {
            if (Input.GetKeyPress(KEYCODE.KEY_RIGHT) ||
                Input.GetKeyPress(KEYCODE.KEY_D) ||
                Input.GetGamepadButtonPress(GAMEPADCODE.GAMEPAD_RIGHT, 0))
            {
                return true;
            }

            if (Input.GetGamepadLeftStickX(0) > GAMEPADDEADZONES.LEFT_THUMBSTICK + 5000)
            {
                //analogTimer += Time.deltaTime;
                if (!analogRight /*|| analogTimer > 1.0f*/)
                {
                    analogRight = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            analogRight = false;
            //analogTimer = 0.0f;

            return false;
        }

        bool NavCheckEnter()
        {
            if (Input.GetKeyPress(KEYCODE.KEY_ENTER) ||
                Input.GetKeyPress(KEYCODE.KEY_SPACE) ||
                Input.GetKeyPress(KEYCODE.KEY_V) ||
                Input.GetKeyPress(KEYCODE.KEY_P) ||
                Input.GetGamepadButtonPress(GAMEPADCODE.GAMEPAD_B, 0))
            {
                return true;
            }
            return false;
        }

        bool NavCheckEscape()
        {
            if (Input.GetKeyPress(KEYCODE.KEY_ESCAPE) ||
                Input.GetKeyPress(KEYCODE.KEY_C) ||
                Input.GetKeyPress(KEYCODE.KEY_O) ||
                Input.GetGamepadButtonPress(GAMEPADCODE.GAMEPAD_A, 0) ||
                Input.GetGamepadButtonPress(GAMEPADCODE.GAMEPAD_START, 0))
            {
                return true;
            }
            return false;
        }
    }
}
