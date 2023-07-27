using System;
using YadaScriptsLib;

namespace YadaScripts
{
    public class BossSpawnTriggerZone : Component
    {
        private EventTrigger eventTrigger;
        private float overlappingCount = 0;

        public void Start()
        {
            eventTrigger = this.entity.GetComponent<EventTrigger>();
        }


        void OnTriggerEnter(Entity collider)
        {
            if (collider.GetComponent<PlayerBehaviour>() != null)
            {
                //eventTrigger.SetTrigger(true);
                //this.active = false;
                //collider.GetComponent<PlayerMovement>().isMoving = false;
                //collider.GetComponent<PlayerMovement>().active = false;
                ++overlappingCount;

                if (overlappingCount >= 1)
                {
                    eventTrigger.SetTrigger(true);
                    this.entity.GetComponent<Collider>().active = false;
                    this.active = false;
                }
            }
        }
        void OnTriggerExit(Entity collider)
        {
            if (collider.GetComponent<PlayerBehaviour>() != null)
            {
                //eventTrigger.SetTrigger(true);
                //this.active = false;
                //collider.GetComponent<PlayerMovement>().isMoving = false;
                //collider.GetComponent<PlayerMovement>().active = false;
                --overlappingCount;
            }
        }

    }
}
