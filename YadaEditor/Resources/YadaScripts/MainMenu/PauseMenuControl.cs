using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class PauseMenuControl : Component
    {
        private enum E_PAUSESTATE
        {
            IDLE,
            MAIN_MENU,
            HOW_TO_PLAY_PANEL,
            SETTINGS_PANEL,
            RETURN_GAME,
            QUIT_GAME,
            CONFIRM_MAIN_MENU,
            CONFIRM_QUIT_GAME
        }

        private enum E_BUTTONS
        {
            NONE = 0,
            RETURN = 1,
            CONTROLS = 2,
            SETTINGS = 3,
            MENU = 4,
            QUIT = 5,
            BACK = 6,
            VOLUME = 7,
            SCREEN_MODE = 8,
            YES = 9,
            NO = 10
        }

        private enum E_ANALOG
        {
            NONE = 0,
            LEFT,
            RIGHT,
            UP,
            DOWN
        }

        //States
        private E_PAUSESTATE menuState;
        private E_BUTTONS currentButton;


        //Main Buttons
        public Entity buttonControls;
        public Entity buttonSettings;
        public Entity buttonReturn;
        public Entity buttonQuit;
        public Entity buttonMainMenu;
        //public Entity buttonCross;

        //Controls (How To Play) Buttons
        public Entity pbuttonBack;

        //Settings Buttons


        //Confirm Panel
        public Entity titleMainMenu;
        public Entity titleQuit;
        public Entity pButtonYes;
        public Entity pButtonNo;
        public Entity textMainMenu;
        public Entity textQuit;

        //Panels
        public Entity panelControls;
        public Entity panelSettings;
        public Entity panelConfirm;
        public Entity panelMain;
        public Entity panelSil;


        //Settings
        int volumeSlider;
        bool isFullScreen;

        //SFX
        public Entity hoverSFXent;
        public Entity clickSFXent;
        public Entity musicPlayer;
        public AudioSource hoverSFXcomp;
        public AudioSource clickSFXcomp;


        //Analog Stick
        private bool analogLeft = false;
        private bool analogRight = false;
        private bool analogUp = false;
        private bool analogDown = false;
        private float analogTimer = 0.0f;


        public float timer = 0.0f;
        Vector3 originalScale;
        bool isMenuInit = false;
        bool isMenuExit = false;

        int entryControl = 0;
        public float entrySmoothTime = 0.5f;
        public float entryGrowTime = 0.5f;

        float smoothTime;
        float growTime;



        // Temp for smoothdamp
        Vector3 posVelocity = Vector3.zero;
        Vector3 scaleVelocity = Vector3.zero;

        void Start()
        {
            
            // Get UI

            //startButtonScript = startButtonEnt.GetComponent<MainMenuButton>();
            //quitButtonScript = quitButtonEnt.GetComponent<MainMenuButton>();
            //startButtonComp = startButtonEnt.GetComponent<UI>();
            //quitButtonComp = quitButtonEnt.GetComponent<UI>();

            // White BG
            //whiteBGComp = whiteBGEnt.GetComponent<UI>();
            //Vector4 color = whiteBGComp.color;
            //color.w = 0.0f;
            //whiteBGComp.color = color;

            // SFX
            hoverSFXcomp = hoverSFXent.GetComponent<AudioSource>();
            clickSFXcomp = clickSFXent.GetComponent<AudioSource>();

            menuState = E_PAUSESTATE.IDLE;
            currentButton = E_BUTTONS.NONE;

            originalScale = panelSil.GetComponent<Transform>().localScale;

            smoothTime = entrySmoothTime + 1/60;
            growTime = entryGrowTime + 1/60;

            pbuttonBack.active = false;
            titleMainMenu.active = false;
            titleQuit.active = false;

            //buttonControls.GetComponent<PauseButtonScript>().active = false;
            //buttonSettings.GetComponent<PauseButtonScript>().active = false;
            //buttonReturn.GetComponent<PauseButtonScript>().active = false;
            //buttonMainMenu.GetComponent<PauseButtonScript>().active = false;
            //buttonQuit.GetComponent<PauseButtonScript>().active = false;

        }


        void Update()
        {
            // Update Fade in Fade Out
            //WhiteBGUpdate();
            MenuEntry();
            UpdateMenu();

            // Switch Cases for Main Menu State
            switch (menuState)
            {
                case E_PAUSESTATE.IDLE:
                    {
                        panelMain.active = true;
                        panelControls.active = false;
                        panelSettings.active = false;
                        panelConfirm.active = false;
                        pbuttonBack.active = false;
                    }
                    break;
                case E_PAUSESTATE.MAIN_MENU:
                    {
                        if (MenuExit())
                            Scene.ClearScenes("MainMenu");
                            //Scene.ChangeScene("MainMenu");
                    }
                    break;
                case E_PAUSESTATE.HOW_TO_PLAY_PANEL:
                    {
                        panelMain.active = false;
                        panelControls.active = true;
                        panelSettings.active = false;
                        panelConfirm.active = false;
                        pbuttonBack.active = true;
                    }
                    break;
                case E_PAUSESTATE.SETTINGS_PANEL:
                    {
                        panelMain.active = false;
                        panelControls.active = false;
                        panelSettings.active = true;
                        panelConfirm.active = false;
                        pbuttonBack.active = true;

                    }
                    break;
                case E_PAUSESTATE.RETURN_GAME:
                    {
                        if (MenuExit())
                            ResumeGame();
                    }
                    break;
                case E_PAUSESTATE.QUIT_GAME:
                    {
                        if (MenuExit())
                            Scene.Quit();
                    }
                    break;
                case E_PAUSESTATE.CONFIRM_MAIN_MENU:
                    {

                        panelMain.active = false;
                        panelControls.active = false;
                        panelSettings.active = false;
                        panelConfirm.active = true;
                        pbuttonBack.active = false;

                        titleMainMenu.active = true;
                        titleQuit.active = false;
                        textMainMenu.active = true;
                        textQuit.active = false;
                    }
                    break;
                case E_PAUSESTATE.CONFIRM_QUIT_GAME:
                    {
                        panelMain.active = false;
                        panelControls.active = false;
                        panelSettings.active = false;
                        panelConfirm.active = true;
                        pbuttonBack.active = false;

                        titleMainMenu.active = false;
                        titleQuit.active = true;
                        textMainMenu.active = false;
                        textQuit.active = true;
                    }
                    break;
                default:
                    break;


    }
        }

        //void ActivateButtons()
        //{
        //    buttonControls.GetComponent<PauseButtonScript>().active = true;
        //    buttonSettings.GetComponent<PauseButtonScript>().active = true;
        //    buttonReturn.GetComponent<PauseButtonScript>().active = true;
        //    buttonMainMenu.GetComponent<PauseButtonScript>().active = true;
        //    buttonQuit.GetComponent<PauseButtonScript>().active = true;
        //}

        void UpdateMenu()
        {
            // Mouse Hover Check
            if (UpdateMouse())
                return;

            // KeyBoard Check
            if (UpdateKBAndController())
                return;
        }

        bool UpdateMouse()
        {


            if (buttonControls.GetComponent<PauseButtonScript>().isClicked)
            {
                Console.WriteLine("controls clicked");
                buttonControls.GetComponent<PauseButtonScript>().SetUnHovered();
                menuState = E_PAUSESTATE.HOW_TO_PLAY_PANEL;
                return true;
            }

            if (buttonSettings.GetComponent<PauseButtonScript>().isClicked)
            {
                Console.WriteLine("settings clicked");
                buttonSettings.GetComponent<PauseButtonScript>().SetUnHovered();
                menuState = E_PAUSESTATE.SETTINGS_PANEL;

                return true;
            }

            if (buttonReturn.GetComponent<PauseButtonScript>().isClicked)
            {
                Console.WriteLine("return clicked");
                buttonReturn.GetComponent<PauseButtonScript>().SetUnHovered();
                menuState = E_PAUSESTATE.RETURN_GAME;
                return true;
            }

            if (buttonQuit.GetComponent<PauseButtonScript>().isClicked)
            {
                Console.WriteLine("quit clicked");
                buttonQuit.GetComponent<PauseButtonScript>().SetUnHovered();
                menuState = E_PAUSESTATE.CONFIRM_QUIT_GAME;
                return true;
            }

            if (buttonMainMenu.GetComponent<PauseButtonScript>().isClicked)
            {
                Console.WriteLine("menu clicked");
                buttonMainMenu.GetComponent<PauseButtonScript>().SetUnHovered();
                menuState = E_PAUSESTATE.CONFIRM_MAIN_MENU;
                return true;
            }

            if (pbuttonBack.GetComponent<PauseButtonScript>().isClicked)
            {
                menuState = E_PAUSESTATE.IDLE;
                pbuttonBack.GetComponent<PauseButtonScript>().isClicked = false;
                pbuttonBack.GetComponent<PauseButtonScript>().SetUnHovered();
                buttonControls.GetComponent<PauseButtonScript>().SetUnHovered();
                buttonSettings.GetComponent<PauseButtonScript>().SetUnHovered();

                return true;
            }

            if (pButtonYes.GetComponent<PauseButtonScript>().isClicked)
            {
                if (menuState == E_PAUSESTATE.CONFIRM_MAIN_MENU)
                {
                    menuState = E_PAUSESTATE.MAIN_MENU;
                }
                else if (menuState == E_PAUSESTATE.CONFIRM_QUIT_GAME)
                { 
                    menuState = E_PAUSESTATE.QUIT_GAME;
                }
                pButtonYes.GetComponent<PauseButtonScript>().SetUnHovered();
                return true;
            }

            if (pButtonNo.GetComponent<PauseButtonScript>().isClicked)
            {
                menuState = E_PAUSESTATE.IDLE;
                pButtonNo.GetComponent<PauseButtonScript>().SetUnHovered();
                return true;
            }


            return false;
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




        bool UpdateKBAndController()
        {
            E_BUTTONS prevButton = currentButton;

            switch (menuState)
            {
                case E_PAUSESTATE.IDLE:
                    { 
                        if (NavCheckEscape())
                        {
                            //currentButton = E_BUTTONS.NO;
                            //menuState = E_PAUSESTATE.CONFIRM_QUIT_GAME;
                            //return true;
                            if (MenuExit())
                            {
                                ResumeGame();
                                Audio.PlaySource(clickSFXcomp);
                                //Audio.PlaySource(clickSFXcomp);

                                return true;
                            }
                        }

                        if (NavCheckUp())
                        {
                            //Console.WriteLine("key up pressed! current btn = " + currentButton);
                            if ((int)currentButton > (int)E_BUTTONS.RETURN)
                            {
                                int newCurr = (int)currentButton - 1;
                                currentButton = (E_BUTTONS)(newCurr);
                            }
                            else
                            {
                                currentButton = E_BUTTONS.QUIT;
                            }
                        }
                        else if (NavCheckDown())
                        {
                            //Console.WriteLine("key down pressed! current btn = " + currentButton);
                            if ((int)currentButton < (int)E_BUTTONS.QUIT)
                            {
                                int newCurr = (int)currentButton + 1;
                                currentButton = (E_BUTTONS)(newCurr);
                                Console.WriteLine("a! " + (newCurr));
                            }
                            else
                            {
                                currentButton = E_BUTTONS.RETURN;
                            }
                        }
                    }
                    break;
                case E_PAUSESTATE.HOW_TO_PLAY_PANEL:
                    {
                        if (NavCheckEscape())
                        {
                            currentButton = E_BUTTONS.NONE;
                            menuState = E_PAUSESTATE.IDLE;
                            buttonControls.GetComponent<PauseButtonScript>().SetUnHovered();
                        }
                    }
                    break;
                case E_PAUSESTATE.SETTINGS_PANEL:
                    {
                        if (NavCheckEscape())
                        {
                            currentButton = E_BUTTONS.NONE;
                            menuState = E_PAUSESTATE.IDLE;
                            buttonSettings.GetComponent<PauseButtonScript>().SetUnHovered();
                        }
                    }
                    break;
                case E_PAUSESTATE.CONFIRM_QUIT_GAME:
                    {
                        if (NavCheckLeft() || NavCheckRight())
                        {
                            currentButton = (currentButton == E_BUTTONS.NO ? E_BUTTONS.YES : E_BUTTONS.NO);
                        }

                        if (NavCheckEscape())
                        {
                            currentButton = E_BUTTONS.NONE;
                            menuState = E_PAUSESTATE.IDLE;
                            buttonQuit.GetComponent<PauseButtonScript>().SetUnHovered();
                        }
                      
                    }
                    break;
                case E_PAUSESTATE.CONFIRM_MAIN_MENU:
                    {

                        if (NavCheckLeft() || NavCheckRight())
                        {
                            currentButton = (currentButton == E_BUTTONS.NO ? E_BUTTONS.YES : E_BUTTONS.NO);
                        }


                        if (NavCheckEscape())
                        {
                            currentButton = E_BUTTONS.NONE;
                            menuState = E_PAUSESTATE.IDLE;
                            buttonMainMenu.GetComponent<PauseButtonScript>().SetUnHovered();
                        }

                    }
                    break;
                default:
                    break;
            }

            if (NavCheckEnter())
            {
                Console.WriteLine("CurrentButton is " + currentButton);
                switch (currentButton)
                {
                    case E_BUTTONS.RETURN:
                        {
                            if (MenuExit())
                            {
                                ResumeGame();
                            }
                            return true;
                        }
                    case E_BUTTONS.MENU:
                        {
                            menuState = E_PAUSESTATE.CONFIRM_MAIN_MENU;
                            return true;
                        }
                    case E_BUTTONS.CONTROLS:
                        {
                            menuState = E_PAUSESTATE.HOW_TO_PLAY_PANEL;
                            return true;
                        }
                    case E_BUTTONS.SETTINGS:
                        {
                            menuState = E_PAUSESTATE.SETTINGS_PANEL;
                            return true;
                        }
                    case E_BUTTONS.QUIT:
                        {
                            menuState = E_PAUSESTATE.CONFIRM_QUIT_GAME;
                            return true;
                        }
                    case E_BUTTONS.BACK:
                        {
                            currentButton = E_BUTTONS.NONE;
                            menuState = E_PAUSESTATE.IDLE;
                            return true;
                        }
                    case E_BUTTONS.YES:
                        {
                            if (menuState == E_PAUSESTATE.CONFIRM_MAIN_MENU)
                            {
                                menuState = E_PAUSESTATE.MAIN_MENU;
                            }
                            else if (menuState == E_PAUSESTATE.CONFIRM_QUIT_GAME)
                            {
                                menuState = E_PAUSESTATE.QUIT_GAME;
                            }
                            currentButton = E_BUTTONS.NONE;
                            return true;
                        }
                    case E_BUTTONS.NO:
                        {
                            currentButton = E_BUTTONS.NONE;
                            menuState = E_PAUSESTATE.IDLE;
                            return true;
                        }

                    default:
                        break;
                }
            }

            if (prevButton != currentButton)
            {
                HighlightCurrentButton(prevButton);
                return true;
            }
            return false;
        }


        bool UpdateController()
        {
            return false;
        }

        void ResumeGame()
        {
            Scene.PopScene();
            //Scene.ChangeScene("Level 1");
            //musicPlayer.GetComponent<AudioController>().PauseAll();
        }

        void HighlightCurrentButton(E_BUTTONS prev)
        {

            if (prev == E_BUTTONS.RETURN)
            {
                buttonReturn.GetComponent<PauseButtonScript>().SetUnHovered();
            }
            else if (prev == E_BUTTONS.MENU)
            {
                buttonMainMenu.GetComponent<PauseButtonScript>().SetUnHovered();
            }
            else if (prev == E_BUTTONS.SETTINGS)
            {
                buttonSettings.GetComponent<PauseButtonScript>().SetUnHovered();
            }
            else if (prev == E_BUTTONS.CONTROLS)
            {
                buttonControls.GetComponent<PauseButtonScript>().SetUnHovered();
            }
            else if (prev == E_BUTTONS.QUIT)
            {
                buttonQuit.GetComponent<PauseButtonScript>().SetUnHovered();
            }
            else if (prev == E_BUTTONS.BACK)
            {
                pbuttonBack.GetComponent<PauseButtonScript>().SetUnHovered();
            }
            else if (prev == E_BUTTONS.YES)
            {
                pButtonYes.GetComponent<PauseButtonScript>().SetUnHovered();
            }
            else if (prev == E_BUTTONS.NO)
            {
                pButtonNo.GetComponent<PauseButtonScript>().SetUnHovered();
            }


            //Console.WriteLine("btn changed! prev is = " + prev + " and current is = " + currentButton);
            if (currentButton == E_BUTTONS.NONE)
            {
                
            }
            else if (currentButton == E_BUTTONS.RETURN)
            {
                //buttonReturn.GetComponent<UI>().SetHighlight();
                buttonReturn.GetComponent<PauseButtonScript>().GrowBig();
            }
            else if (currentButton == E_BUTTONS.MENU)
            {
                //buttonMainMenu.GetComponent<UI>().SetHighlight();
                buttonMainMenu.GetComponent<PauseButtonScript>().GrowBig();
            }
            else if (currentButton == E_BUTTONS.SETTINGS)
            {
                //buttonSettings.GetComponent<UI>().SetHighlight();
                buttonSettings.GetComponent<PauseButtonScript>().GrowBig();
            }
            else if (currentButton == E_BUTTONS.CONTROLS)
            {
                //buttonControls.GetComponent<UI>().SetHighlight();
                buttonControls.GetComponent<PauseButtonScript>().GrowBig();
            }
            else if (currentButton == E_BUTTONS.QUIT)
            {
                //buttonQuit.GetComponent<UI>().SetHighlight();
                buttonQuit.GetComponent<PauseButtonScript>().GrowBig();
            }
            else if (currentButton == E_BUTTONS.YES)
            {
                //pButtonYes.GetComponent<UI>().SetHighlight();
                pButtonYes.GetComponent<PauseButtonScript>().GrowBig();
            }
            else if (currentButton == E_BUTTONS.NO)
            {
                //pButtonNo.GetComponent<UI>().SetHighlight();
                pButtonNo.GetComponent<PauseButtonScript>().GrowBig();
            }
            //else if (currentButton == E_BUTTONS.BACK)
            //{
            //    pbuttonBack.GetComponent<UI>().SetHighlight(true);
            //    buttonReturn.GetComponent<PauseButtonScript>().GrowBig();
            //}
            //else if (currentButton == E_BUTTONS.SAVE)
            //{
            //    pbuttonSave.GetComponent<UI>().SetHighlight(true);
            //    buttonReturn.GetComponent<PauseButtonScript>().GrowBig();
            //}
            //else if (currentButton == E_BUTTONS.CANCEL)
            //{
            //    pbuttonCancel.GetComponent<UI>().SetHighlight(true);
            //    buttonReturn.GetComponent<PauseButtonScript>().GrowBig();
            //}
        }

        //void MenuEntry()
        //{

        //    if (isMenuInit)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        panelSil.active = true;
        //        Vector3 scale = panelSil.GetComponent<Transform>().localScale;
        //        if (timer < 0.25f)
        //        {
        //            scale.x *= 1.3f;
        //            scale.y *= 1.3f;
        //            //pos = Vector3.Lerp(pos, new Vector3(0.0f, 0.0f, -0.9f), 0);
        //            panelSil.GetComponent<Transform>().localScale = scale;
        //            timer += Time.fixedDeltaTime;
        //        }
        //        else
        //        {
        //            isMenuInit = true;
        //            timer = 0.0f;

        //            panelSil.active = false;
        //            currentButton = E_BUTTONS.NONE;
        //        }
        //    }

        //}


        void MenuEntry()
        {
            if (isMenuInit)
            {
                return;
            }
            else
            {
                
                Vector3 expandedScale = new Vector3(1.1f, 1.1f, 1.0f);
                Vector3 normalScale = new Vector3(1.0f, 1.0f, 1.0f);
                Transform trans = panelMain.GetComponent<Transform>();

                //float totalTime = entrySmoothTime + entryGrowTime;

                //trans.SmoothResize(expandedScale, normalScale, ref scaleVelocity, entryGrowTime, float.MaxValue, Time.deltaTime);
                timer += Time.deltaTime;

                if (entryControl == 0)
                {
                    //trans.SmoothDirectionalResize(trans.localPosition, Vector3.zero, ref posVelocity, trans.localScale,
                    //                                    expandedScale, ref scaleVelocity, smoothTime, float.MaxValue, Time.deltaTime);

                    trans.SmoothDirectionalResize(Vector3.zero, Vector3.zero, ref posVelocity, trans.localScale,
                                                        expandedScale, ref scaleVelocity, smoothTime, float.MaxValue, Time.deltaTime);
                    //Console.WriteLine("local pos is " + trans.localPosition.y);

                    smoothTime -= Time.deltaTime;
                    if (smoothTime <= 0.0f /*&& scaleVelocity.magnitude < 100f*/)
                    {
                        trans.localPosition = Vector3.zero;
                        entryControl = 1;
                    }


                }
                else if (entryControl == 1)
                {
                    trans.SmoothResize(trans.localScale, normalScale, ref scaleVelocity, growTime, float.MaxValue, Time.deltaTime);

                    growTime -= Time.deltaTime;
                    if (growTime <= 0.0f /*&& scaleVelocity.magnitude < 100f*/)
                    {
                        //trans.localPosition = Vector3.zero;
                        trans.localScale = normalScale;
                        isMenuInit = true;
                        entryControl = 0;
                        //ActivateButtons();
                        //musicPlayer.GetComponent<AudioController>().PauseAll();
                    }
                }
            }
        }

        bool MenuExit()
        {
            return true;
            if (isMenuExit)
            {
                return true;
            }
            else
            {
                //TODO
                isMenuExit = true;

                //panelSil.active = true;
                //Vector3 scale = panelSil.GetComponent<Transform>().localScale;
                //if (timer < 0.15f)
                //{
                //    scale.x *= 0.76f;
                //    scale.y *= 0.76f;
                //    //pos = Vector3.Lerp(pos, new Vector3(0.0f, 0.0f, -0.9f), 0);
                //    panelSil.GetComponent<Transform>().localScale = scale;
                //    timer += Time.fixedDeltaTime;
                //}
                //else
                //{
                    
                //    panelSil.GetComponent<Transform>().localScale = originalScale;
                //    timer = 0.0f;
                //}
                return false;
            }
        }

        //void WhiteBGUpdate()
        //{
        //    Vector4 color = whiteBGComp.color;
        //    color.w += Time.fixedDeltaTime;
        //    whiteBGComp.color = color;
        //}


    }
}
