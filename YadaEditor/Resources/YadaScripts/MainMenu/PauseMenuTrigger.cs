using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class PauseMenuTrigger : Component
    {

        public Entity soundControl;
        public AudioController ac;

        private int count;

        void Start()
        {
        }

        void Update()
        {
            if (Input.GetKeyPress(KEYCODE.KEY_ESCAPE) || Input.GetGamepadButtonPress(GAMEPADCODE.GAMEPAD_START, 0))
            {
                Scene.PushScene("PauseScene");
                //soundControl.GetComponent<AudioController>().PauseAll();


            }


            //if (Input.GetKeyPress(KEYCODE.KEY_F))
            //{
            //    soundControl.GetComponent<AudioController>().PauseAll();


            //}

            //if (count > (int)Scene.sceneCount)
            //{
            //    soundControl.GetComponent<AudioController>().PauseAll();
            //}

            count = (int)Scene.sceneCount;
        }
    }
}
