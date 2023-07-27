using System;
using YadaScriptsLib;

namespace YadaScripts
{
    public class Waddle : Component
    {

        private RigidBody rigidbody;
        public Entity childModel;

        // Use this for initialization
        void Start()
        {
            rigidbody = this.entity.GetComponent<RigidBody>();
        }

        // Update is called once per frame
        void Update()
        {
            if (rigidbody.linearVelocity.magnitude > 1.0f)
            {
                childModel.GetComponent<Transform>().globalEulerAngles += new Vector3(0.0f, 0.0f, 0.1f);
            }
        }
    }
}