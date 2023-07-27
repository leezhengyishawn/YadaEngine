using System;
using YadaScriptsLib;


namespace YadaScripts
{
    class PlatformParent : Component
    {
        public Entity getCart;
        void Start()
        {
        }

        void OnTriggerStay(Entity collision)
        {
            Console.WriteLine("EnterLiao");
            if (collision.GetComponent<PlayerBehaviour>() != null)
            {
                collision.GetComponent<Transform>().parent = getCart.GetComponent<Transform>();
            }
        }

        void OnTriggerExit(Entity collision)
        {
            Console.WriteLine("ExitLiao");
            if (collision.GetComponent<PlayerBehaviour>() == null)
            {
                collision.GetComponent<Transform>().parent = null;
            }
        }
    }
}
