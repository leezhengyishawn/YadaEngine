using YadaScriptsLib;

namespace YadaScripts
{
    public class InputTest : Component
    {
        bool init = false;
        Transform trans;
        Transform trans2;

        void Start()
        {
        }

        void Update()
        {
            if(!init)
            {
                Entity entity = new Entity("PlayerCharacter1");
                entity.tag = "Player";
                trans = entity.AddComponent<Transform>();
                entity.AddComponent<Collider>();
                entity.AddComponent<Renderer>();
                entity.AddComponent<RigidBody>();

                Entity entity2 = new Entity("PlayerCharacter2");
                entity2.tag = "Player";
                trans2 = entity2.AddComponent<Transform>();
                entity2.AddComponent<Collider>();
                entity2.AddComponent<Renderer>();
                entity2.AddComponent<RigidBody>();
                init = true;
            }

            if (trans != null)
            {
                Vector3 pos = trans.globalPosition;
                Vector3 pos2 = trans2.globalPosition;
                //if (Input.GetKey(KEYCODE.KEY_W))
                // pos = new Vector3(pos.x, pos.y + 0.1f, pos.z);
                //if (Input.GetKey(KEYCODE.KEY_S))
                // pos = new Vector3(pos.x, pos.y - 0.1f, pos.z);
                //if (Input.GetKey(KEYCODE.KEY_D))
                // pos = new Vector3(pos.x + 0.1f, pos.y, pos.z);
                //if (Input.GetKey(KEYCODE.KEY_A))
                // pos = new Vector3(pos.x - 0.1f, pos.y, pos.z);

                if (Input.GetKey(KEYCODE.KEY_W))
                    pos = new Vector3(pos.x, pos.y + 0.1f, pos.z);
                if (Input.GetKey(KEYCODE.KEY_S))
                    pos = new Vector3(pos.x, pos.y - 0.1f, pos.z);
                if (Input.GetKey(KEYCODE.KEY_D))
                    pos = new Vector3(pos.x + 0.1f, pos.y, pos.z);
                if (Input.GetKey(KEYCODE.KEY_A))
                    pos = new Vector3(pos.x - 0.1f, pos.y, pos.z);

                trans.globalPosition = pos;

                if (Input.GetKey(KEYCODE.KEY_UP))
                    pos2 = new Vector3(pos2.x, pos2.y + 0.1f, pos2.z);
                if (Input.GetKey(KEYCODE.KEY_DOWN))
                    pos2 = new Vector3(pos2.x, pos2.y - 0.1f, pos2.z);
                if (Input.GetKey(KEYCODE.KEY_LEFT))
                    pos2 = new Vector3(pos2.x + 0.1f, pos2.y, pos2.z);
                if (Input.GetKey(KEYCODE.KEY_RIGHT))
                    pos2 = new Vector3(pos2.x - 0.1f, pos2.y, pos2.z);

                trans2.globalPosition = pos2;
            }
        }
    }
}
