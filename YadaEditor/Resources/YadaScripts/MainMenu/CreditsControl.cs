using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class CreditsControl : Component
    {


        public Entity buttonLeftEnt;
        public Entity buttonRightEnt;

        public Entity page0Ent;
        public Entity page1Ent;
        public Entity page2Ent;
        public Entity page3Ent;

        int creditsPage; //0 to 3

        //Analog Stick
        private bool analogLeft = false;
        private bool analogRight = false;
        private bool analogUp = false;
        private bool analogDown = false;
        private float analogTimer = 0.0f;

        //KB Controller


        //Settings value


        void Start()
        {

        }

        void Update()
        {


            if (CaptureMouse())
                return;

            if (CaptureKBController())
                return;

            switch (creditsPage)
            {
                case 0:
                    {
                        page0Ent.active = true;
                        page1Ent.active = false;
                        page2Ent.active = false;
                        page3Ent.active = false;

                        buttonLeftEnt.active = false;
                        buttonRightEnt.active = true;
                    }
                    break;
                case 1:
                    {
                        page0Ent.active = false;
                        page1Ent.active = true;
                        page2Ent.active = false;
                        page3Ent.active = false;

                        buttonLeftEnt.active = true;
                        buttonRightEnt.active = true;
                    }
                    break;
                case 2:
                    {
                        page0Ent.active = false;
                        page1Ent.active = false;
                        page2Ent.active = true;
                        page3Ent.active = false;

                        buttonLeftEnt.active = true;
                        buttonRightEnt.active = true;
                    }
                    break;
                case 3:
                    {
                        page0Ent.active = false;
                        page1Ent.active = false;
                        page2Ent.active = false;
                        page3Ent.active = true;

                        buttonLeftEnt.active = true;
                        buttonRightEnt.active = false;
                    }
                    break;
                default:
                    creditsPage = 0;
                    break;
            }

        }

        bool CaptureMouse()
        {

            if (buttonLeftEnt.GetComponent<PauseButtonScript>().isClicked)
            {
                if (creditsPage > 0)
                    creditsPage -= 1;
                return true;
            }

            if (buttonRightEnt.GetComponent<PauseButtonScript>().isClicked)
            {
                if (creditsPage < 3)
                    creditsPage += 1;
                return true;
            }

            return false;
        }

        bool CaptureKBController()
        {
            if (NavCheckLeft())
            {
                if (creditsPage > 0)
                    creditsPage -= 1;
                return true;
            }

            if (NavCheckRight())
            {
                if (creditsPage < 3)
                    creditsPage += 1;
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
                Input.GetGamepadButtonPress(GAMEPADCODE.GAMEPAD_A, 0))
            {
                return true;
            }
            return false;
        }

    }
}
