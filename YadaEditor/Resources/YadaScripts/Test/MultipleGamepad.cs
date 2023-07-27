using System;
using YadaScriptsLib;

namespace YadaScripts
{
    public class MultipleGamepad : Component
    {
        public uint playerID = 0;
        private Transform transform;
        void Start()
        {
            transform = this.entity.GetComponent<Transform>();
        }

        void Update()
        {
            if (Input.GetGamepadButtonPress(GAMEPADCODE.GAMEPAD_A, playerID))
            {
                //this.entity.GetComponent<Transform>().globalPosition = new Vector3(this.entity.GetComponent<Transform>().globalPosition.x + 1,
                //    this.entity.GetComponent<Transform>().globalPosition.y,
                //    this.entity.GetComponent<Transform>().globalPosition.z);

                transform.globalPosition = new Vector3(transform.globalPosition.x + 1,
                                                       transform.globalPosition.y,
                                                       transform.globalPosition.z);
            }

        }
    }
}
