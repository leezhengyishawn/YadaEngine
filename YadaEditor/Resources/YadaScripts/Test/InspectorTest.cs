using System;
using System.Data.SqlTypes;
using YadaScriptsLib;

namespace YadaScripts
{
    public class InspectorTest : Component
    {
        public bool a = false;
        public char b = 'd';
        public short c = -20;
        public UInt16 d = 500;
        public int e = -12305;
        public UInt32 f = 1111111;
        public long g = -555555555;
        public UInt64 h = 999999999;
        public float i = 1335.1232f;
        public double j = 53501231250.1231251231;
        public Vector2 k = new Vector2(1.0f, 2.0f);
        public Vector3 l = new Vector3(10.0f, 20.0f, 30.0f);
        public Vector4 m = new Vector4(100.0f, 200.0f, 300.0f, 400.0f);
        public Entity n;

        Test testScript;

        void Start()
        {
        }

        void Update()
        {
            if (n != null)
            {
                //    Console.WriteLine(n.active);
                //    Console.WriteLine(n.id);
                //    Console.WriteLine(n.layer);
                //    Console.WriteLine(n.tag);
                //    Console.WriteLine(n.name);

                //    Transform trans = n.GetComponent<Transform>();
                //    Console.WriteLine(trans.globalPosition.x.ToString() + ", " + trans.globalPosition.y.ToString() + ", " + trans.globalPosition.z.ToString());
                //    Console.WriteLine(trans.globalEulerAngles.x.ToString() + ", " + trans.globalEulerAngles.y.ToString() + ", " + trans.globalEulerAngles.z.ToString());
                //    Console.WriteLine(trans.globalScale.x.ToString() + ", " + trans.globalScale.y.ToString() + ", " + trans.globalScale.z.ToString());
                //    Console.WriteLine(trans.localPosition.x.ToString() + ", " + trans.localPosition.y.ToString() + ", " + trans.localPosition.z.ToString());
                //    Console.WriteLine(trans.localEulerAngles.x.ToString() + ", " + trans.localEulerAngles.y.ToString() + ", " + trans.localEulerAngles.z.ToString());
                //    Console.WriteLine(trans.localScale.x.ToString() + ", " + trans.localScale.y.ToString() + ", " + trans.localScale.z.ToString());

                //Entity test = this.entity;
                //Console.WriteLine(test.active);
                //Console.WriteLine(test.id);
                //Console.WriteLine(test.layer);
                //Console.WriteLine(test.tag);
                //Console.WriteLine(test.name);

                //RigidBody rigid = this.entity.GetComponent<RigidBody>();
                //rigid.mass = 50.0f;
                //rigid.sleepTime = 1.0f;
                //rigid.type = RIGIDBODY_TYPE.DYNAMIC;
                //rigid.useGravity = true;

                //Console.WriteLine(rigid.mass);
                //Console.WriteLine(rigid.sleepTime);
                //Console.WriteLine(rigid.linearVelocity.x.ToString() + ", " + rigid.linearVelocity.y.ToString() + ", " + rigid.linearVelocity.z.ToString());
                //Console.WriteLine(rigid.angularVelocity.x.ToString() + ", " + rigid.angularVelocity.y.ToString() + ", " + rigid.angularVelocity.z.ToString());
                //Console.WriteLine(rigid.type);
                //Console.WriteLine(rigid.isSleeping);
                //Console.WriteLine(rigid.useGravity);

                ////rigid.AddForce(10.0f, 10.0f, 10.0f);
                //rigid.AddTorque(10.0f, 10.0f, 10.0f);

                //Collider collider = this.entity.GetComponent<Collider>();
                //collider.isTrigger = true;
                //Console.WriteLine(collider.shape);
                //collider.shape = COLLIDER_SHAPE.BOX;
                //collider.globalCenter = new Vector3(0.0f, 5.0f, 0.0f);
                if(!a)
                {
                    n.AddComponent<InspectorTest>();
                    a = true;


                }

            }
    
            if(testScript == null)
            {
                Entity test = new Entity("Test");
                testScript = test.AddComponent<Test>();
            }
            else
                testScript.temp();
        }

        void FixedUpdate()
        {

        }

        void LateUpdate()
        {
        }

        void OnCollisionEnter(Entity collider)
        {
            Console.WriteLine("Collision Enter");
        }

        void OnCollisionStay(Entity collider)
        {
            //Console.WriteLine("Collision Stay");
        }

        void OnCollisionExit(Entity collider)
        {
            Console.WriteLine("Collision Exit");
        }

        void OnTriggerEnter(Entity collider)
        {
            Console.WriteLine("Trigger Enter, this: " + this.entity.name + " vs " + collider.name);
        }

        void OnTriggerStay(Entity collider)
        {
            //Console.WriteLine("Trigger Stay");
        }

        void OnTriggerExit(Entity collider)
        {
            Console.WriteLine("Trigger Exit");
        }
    }
}
