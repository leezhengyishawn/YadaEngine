using System;
using YadaScriptsLib;

namespace YadaScripts
{
    public class Move : Component
    {
        public float movementSpeed = 10.0f;
        private RigidBody rigidBody;
        private float inputForward = 0.0f;
        private float inputSide = 0.0f;
        public Entity model;
        
        void Start()
        {
            rigidBody = this.entity.GetComponent<RigidBody>();
        }

        void Update()
        {
            if (Input.GetKey(KEYCODE.KEY_W) && Input.GetKey(KEYCODE.KEY_S))
            { 
                inputForward = 0.0f; 
            }
            else if (Input.GetKey(KEYCODE.KEY_W))
            { 
                inputForward = -1.0f;
            }
            else if (Input.GetKey(KEYCODE.KEY_S))
            { 
                inputForward = 1.0f;
            }
            else
            { 
                inputForward = 0.0f; 
            }

            //if (Input.GetKey(KEYCODE.KEY_A) && Input.GetKey(KEYCODE.KEY_D))
            //{ 
            //    inputSide = 0.0f; 
            //}
            //else if (Input.GetKey(KEYCODE.KEY_D))
            //{ 
            //    inputSide = 1.0f;
            //}
            //else if (Input.GetKey(KEYCODE.KEY_A))
            //{ 
            //    inputSide = -1.0f;
            //}
            //else
            //{ 
            //    inputSide = 0.0f; 
            //}

            rigidBody.AddForce(new Vector3(inputSide, 0, inputForward) * movementSpeed);
        }
    }
}