using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class SettingsControl : Component
    {
        enum E_SETTINGS
        {
            VOLUME,
            SCREEN_MODE
        }


        public Entity buttonFullScreen;
        public Entity buttonWindowed;

        public Entity[] volumeArray;

        public Entity volBubble0;
        public Entity volBubble1;
        public Entity volBubble2;
        public Entity volBubble3;
        public Entity volBubble4;
        public Entity volBubble5;
        public Entity volBubble6;
        public Entity volBubble7;
        public Entity volBubble8;
        public Entity volBubble9;
        public Entity volBubble10;

        public Entity selectorDisplay;

        E_SETTINGS settingsState;

        //Analog Stick
        private bool analogLeft = false;
        private bool analogRight = false;
        private bool analogUp = false;
        private bool analogDown = false;
        private float analogTimer = 0.0f;

        //KB Controller;
        public int currentVol = 0;
        bool currentScr = true;

        //Settings value
        bool isFullscreen = true;
        public int volPercent = 5;

        void Start()
        {
            volumeArray = new Entity[11];
            volumeArray[0] = volBubble0;
            volumeArray[1] = volBubble1;
            volumeArray[2] = volBubble2;
            volumeArray[3] = volBubble3;
            volumeArray[4] = volBubble4;
            volumeArray[5] = volBubble5;
            volumeArray[6] = volBubble6;
            volumeArray[7] = volBubble7;
            volumeArray[8] = volBubble8;
            volumeArray[9] = volBubble9;
            volumeArray[10] = volBubble10;

            //TODO: LINK VOL AND FULLSCREEN
            if (Window.GetMode() == Window.Mode.Fullscreen)
                isFullscreen = true;
            else
                isFullscreen = false;

            volPercent = (int)(Audio.masterVolume * 10);

            for (int i = 0; i <= volPercent; ++i)
            {
                volumeArray[i].GetComponent<CheckboxButton>().Toggle(true);
            }

            settingsState = E_SETTINGS.VOLUME;

            buttonFullScreen.GetComponent<PauseButtonScript>().isGrowBig = false;
            buttonWindowed.GetComponent<PauseButtonScript>().isGrowBig = false;

        }

        void Update()
        {

            if (isFullscreen)
            {
                buttonFullScreen.GetComponent<TwoChooseOneScript>().Toggle(true);
            }
            else
            {
                buttonWindowed.GetComponent<TwoChooseOneScript>().Toggle(true);
            }
            selectorDisplay.GetComponent<Selector>().index = (int)settingsState;

            UpdateSlider();

            if (CaptureMouse())
                return;

            if (CaptureKBController())
                return;

        }

        void UpdateSlider()
        {
            //volPercent = (int)(Audio.masterVolume * 10);
            for (int i = 0; i < 11; ++i)
            {
                if (i <= volPercent)
                    volumeArray[i].GetComponent<CheckboxButton>().Toggle(true);
                else
                    volumeArray[i].GetComponent<CheckboxButton>().Toggle(false);
            }
        }


        bool CaptureMouse()
        {
            for (int i = 0; i < 11; ++i)
            {
                if (volumeArray[i].GetComponent<CheckboxButton>().isHovered)
                {
                    //UpdateSlider();
                    for (int j = 0; j < 11; ++j)
                    {
                        if (j <= i)
                            volumeArray[j].GetComponent<CheckboxButton>().Toggle(true);
                        else
                            volumeArray[j].GetComponent<CheckboxButton>().Toggle(false);
                    }
                }
                
                if (volumeArray[i].GetComponent<CheckboxButton>().isClicked)
                {
                    volPercent = i;
                    Audio.masterVolume = ((float)volPercent) / 10;

                    // Save the master volume
                    File.WriteJsonFile("tempSave");
                    File.WriteDataAsFloat("MasterVolume", Audio.masterVolume);

                    return true;
                }


                if (buttonFullScreen.GetComponent<PauseButtonScript>().isClicked)
                {
                    isFullscreen = true;
                    Window.SetMode(Window.Mode.Fullscreen);
                    buttonFullScreen.GetComponent<TwoChooseOneScript>().Toggle(true);
                }
                if (buttonWindowed.GetComponent<PauseButtonScript>().isClicked)
                {
                    isFullscreen = false;
                    Window.SetMode(Window.Mode.Windowed);
                    buttonWindowed.GetComponent<TwoChooseOneScript>().Toggle(true);
                }


            }
            return false;
        }

        bool CaptureKBController()
        {
            switch (settingsState)
            {
                case E_SETTINGS.VOLUME:
                    {
                        currentVol = volPercent;
                        if (NavCheckUp())
                        {
                            settingsState = E_SETTINGS.SCREEN_MODE;
                            return true;
                        }
                        if (NavCheckDown())
                        {
                            settingsState = E_SETTINGS.SCREEN_MODE;
                            return true;
                        }
                        if (NavCheckLeft())
                        {
                            if (currentVol > 0)
                            {
                                volumeArray[currentVol].GetComponent<CheckboxButton>().Toggle(false);
                                volPercent = currentVol - 1;
                                Audio.masterVolume = (float)volPercent / 10;


                                // Save the master volume
                                File.WriteJsonFile("tempSave");
                                File.WriteDataAsFloat("MasterVolume", Audio.masterVolume);
                            }
                            return true;
                        }

                        if (NavCheckRight())
                        {
                            if (currentVol < 10)
                            {
                                volumeArray[currentVol].GetComponent<CheckboxButton>().Toggle(true);
                                volPercent = currentVol + 1;
                                Audio.masterVolume = (float)volPercent / 10;


                                // Save the master volume
                                File.WriteJsonFile("tempSave");
                                File.WriteDataAsFloat("MasterVolume", Audio.masterVolume);
                            }
                            return true;

                        }
                    }
                    break;
                case E_SETTINGS.SCREEN_MODE:
                    {
                        if (NavCheckUp())
                        {
                            settingsState = E_SETTINGS.VOLUME;
                            return true;
                        }
                        if (NavCheckDown())
                        {
                            settingsState = E_SETTINGS.VOLUME;
                            return true;
                        }
                        if (NavCheckLeft())
                        {
                            isFullscreen = !isFullscreen;
                            if (isFullscreen)
                                Window.SetMode(Window.Mode.Fullscreen);
                            else
                                Window.SetMode(Window.Mode.Windowed);
                            return true;
                        }
                        if (NavCheckRight())
                        {
                            isFullscreen = !isFullscreen;
                            if (isFullscreen)
                                Window.SetMode(Window.Mode.Fullscreen);
                            else
                                Window.SetMode(Window.Mode.Windowed);
                            return true;
                        }
                    }
                    break;
                default:
                    return false;
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
                Input.GetGamepadButtonPress(GAMEPADCODE.GAMEPAD_A, 0))
            {
                return true;
            }
            return false;
        }

    }
}
