using System;
using YadaScriptsLib;

namespace YadaScripts
{
    public class Test : Component
    {
        bool trigger = false;
        UInt16 a;
        UInt32 b;
        UInt64 c;
        Vector2 d;
        Vector3 e;
        Vector4 f;
        Entity test1;
        
        void Start()
        {

        }

        void Update()
        {
            if (!trigger)
            {
                test1 = new Entity("TransformTest");
                Transform trans = test1.AddComponent<Transform>();
                //Console.WriteLine(trans.active);
                //trans.active = false;
                trans.globalPosition = new Vector3(1.0f, 2.0f, 3.0f);
                trans.globalScale = new Vector3(2.0f, 2.0f, 2.0f);
                //Console.WriteLine(trans.active);
                //Console.WriteLine(trans.name);
                Console.WriteLine(trans.globalPosition.x.ToString() + ", " + trans.globalPosition.y.ToString() + ", " + trans.globalPosition.z.ToString());
                Console.WriteLine(trans.globalScale.x.ToString() + ", " + trans.globalScale.y.ToString() + ", " + trans.globalScale.z.ToString());

                Transform transform = trans;
                Console.WriteLine("\n");
                //Console.WriteLine(transform.active);
                //Console.WriteLine(transform.name);
                Console.WriteLine(transform.globalPosition.x.ToString() + ", " + transform.globalPosition.y.ToString() + ", " + transform.globalPosition.z.ToString());
                Console.WriteLine(transform.globalScale.x.ToString() + ", " + transform.globalScale.y.ToString() + ", " + transform.globalScale.z.ToString());

                transform = test1.AddComponent<Transform>();
                Console.WriteLine("\n");
                //Console.WriteLine(transform.active);
                //Console.WriteLine(transform.name);
                Console.WriteLine(transform.globalPosition.x.ToString() + ", " + transform.globalPosition.y.ToString() + ", " + transform.globalPosition.z.ToString());
                Console.WriteLine(transform.globalScale.x.ToString() + ", " + transform.globalScale.y.ToString() + ", " + transform.globalScale.z.ToString());



                //trans.active = true;
                trans.globalPosition = new Vector3(10.0f, 20.0f, 30.0f);
                trans.globalEulerAngles = new Vector3(45.0f, 90.0f, 180.0f);
                trans.globalScale = new Vector3(5.0f, 5.0f, 5.0f);
                Console.WriteLine("\n");
                //Console.WriteLine(trans.active);
                //Console.WriteLine(trans.name);
                Console.WriteLine(trans.globalPosition.x.ToString() + ", " + trans.globalPosition.y.ToString() + ", " + trans.globalPosition.z.ToString());
                Console.WriteLine(trans.globalEulerAngles.x.ToString() + ", " + trans.globalEulerAngles.y.ToString() + ", " + trans.globalEulerAngles.z.ToString());
                Console.WriteLine(trans.globalScale.x.ToString() + ", " + trans.globalScale.y.ToString() + ", " + trans.globalScale.z.ToString());
                Console.WriteLine(trans.localPosition.x.ToString() + ", " + trans.localPosition.y.ToString() + ", " + trans.localPosition.z.ToString());
                Console.WriteLine(trans.localEulerAngles.x.ToString() + ", " + trans.localEulerAngles.y.ToString() + ", " + trans.localEulerAngles.z.ToString());
                Console.WriteLine(trans.localScale.x.ToString() + ", " + trans.localScale.y.ToString() + ", " + trans.localScale.z.ToString());


                trans.localPosition = new Vector3(100.0f, 200.0f, 300.0f);
                trans.localEulerAngles = new Vector3(1.0f, 2.0f, 3.0f);
                trans.localScale = new Vector3(10.0f, 10.0f, 10.0f);
                Console.WriteLine("\n");
                Console.WriteLine(trans.globalPosition.x.ToString() + ", " + trans.globalPosition.y.ToString() + ", " + trans.globalPosition.z.ToString());
                Console.WriteLine(trans.globalEulerAngles.x.ToString() + ", " + trans.globalEulerAngles.y.ToString() + ", " + trans.globalEulerAngles.z.ToString());
                Console.WriteLine(trans.globalScale.x.ToString() + ", " + trans.globalScale.y.ToString() + ", " + trans.globalScale.z.ToString());
                Console.WriteLine(trans.localPosition.x.ToString() + ", " + trans.localPosition.y.ToString() + ", " + trans.localPosition.z.ToString());
                Console.WriteLine(trans.localEulerAngles.x.ToString() + ", " + trans.localEulerAngles.y.ToString() + ", " + trans.localEulerAngles.z.ToString());
                Console.WriteLine(trans.localScale.x.ToString() + ", " + trans.localScale.y.ToString() + ", " + trans.localScale.z.ToString());


                trigger = true;
            }
        }

        void FixedUpdate()
        {

        }
        void LateUpdate()
        {

        }

        public void temp()
        {
            //Console.WriteLine("Haha");
        }
    }
}
