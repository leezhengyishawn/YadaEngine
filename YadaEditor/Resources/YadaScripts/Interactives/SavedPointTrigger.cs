using System;
using YadaScriptsLib;

namespace YadaScripts
{
    public class SavedPointTrigger : Component
    {
        public Entity spawnPointPlayer1;
        public Entity spawnPointPlayer2;

        //private Light light1; //player 1 light
        //private Light light2; //player 1 light
        //private Light light3; //player 1 light
        private bool player1Crossed;
        private bool player2Crossed;
        private bool hasActivated;

        void Start()
        {
            if (spawnPointPlayer1 != null && spawnPointPlayer1.GetComponent<Renderer>() != null)
                spawnPointPlayer1.GetComponent<Renderer>().active = false;
            if (spawnPointPlayer2 != null && spawnPointPlayer2.GetComponent<Renderer>() != null)
                spawnPointPlayer2.GetComponent<Renderer>().active = false;

            this.entity.GetComponent<Renderer>().active = false;

            //light1 = this.entity.GetComponent<Transform>().GetChildByIndex(0).entity.GetComponent<Light>();
            //light2 = this.entity.GetComponent<Transform>().GetChildByIndex(1).entity.GetComponent<Light>();
            //light3 = this.entity.GetComponent<Transform>().GetChildByIndex(2).entity.GetComponent<Light>();
            //light1.intensity = light2.intensity = light3.intensity = 0.0f;
        }

        void Update()
        {
            if (hasActivated == false && player1Crossed && player2Crossed)
            {
                SceneController.mainPlayer1.GetComponent<PlayerBehaviour>().SetSavePoint(spawnPointPlayer1.GetComponent<Transform>().globalPosition + (Vector3.up * SceneController.clawMachineHeight));
                SceneController.mainPlayer2.GetComponent<PlayerBehaviour>().SetSavePoint(spawnPointPlayer2.GetComponent<Transform>().globalPosition + (Vector3.up * SceneController.clawMachineHeight));
                //light1.intensity = light2.intensity = 0.0f;
                //light3.intensity = 5.0f;
                hasActivated = true;
            }
        }

        void OnTriggerEnter(Entity collider)
        {
            if (hasActivated == false && (collider == SceneController.mainPlayer1 || collider == SceneController.mainPlayer2))
            {
                if (collider == SceneController.mainPlayer1)
                {
                    //light1.intensity = 2.0f;
                    player1Crossed = true;
                }
                else if (collider = SceneController.mainPlayer2)
                {
                    //light2.intensity = 2.0f;
                    player2Crossed = true;
                }
            }
        }
    }
}