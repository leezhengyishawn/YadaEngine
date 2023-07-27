using System;
using YadaScriptsLib;

namespace YadaScripts
{
    public class Spawner : Component
    {
        private bool playOnce = true;
        private EventResponse eventRes;
        public Entity enemyToSpawn;

        //set enemy entity active to false, spawner spawns by setting the enemy's entity to true

        void Start()
        {
            eventRes = this.entity.GetComponent<EventResponse>();
        }

        void Update()
        {
            if (eventRes.fired == true)
            {
                //spawn enemy here
                enemyToSpawn.active = true;
            }
        }
    }
}