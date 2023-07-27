using System;
using YadaScriptsLib;

namespace YadaScripts
{

    public class BossSpawnTriggerResponse : Component
    {
        //public Entity spawnedEnemies; //The batch of enemies the boss spawns

        private EventResponse eventRes;
        bool hasPlayedOnce = false;
        public Entity camRef;

        public int spawnWave = 0;
        public float bossAttackRange = 15.0f;
        public float bossAttackFreq = 5.0f; //How many sec before boss next attack
        public float bossAttackForce = 10000f;
        public void Start()
        {
            eventRes = this.entity.GetComponent<EventResponse>();
            this.entity.GetComponent<Renderer>().active = false;
        }

        public void Update()
        {
            if (eventRes.fired == true)
            {
                if (hasPlayedOnce == false)
                {
                    Entity boss = Entity.InstantiatePrefab("Boss");
                    boss.GetComponent<Transform>().globalPosition = this.entity.GetComponent<Transform>().globalPosition;
                    boss.GetComponent<Transform>().globalRotation = this.entity.GetComponent<Transform>().globalRotation;

                    //if (spawnWave == 2) //Special case for final encounter
                    //    boss.GetComponent<Transform>().GetChildByIndex(0).localRotation = new Vector3.


                    boss.GetComponent<BossBehaviour>().playBossBurrowSpawn = true;
                    boss.GetComponent<BossBehaviour>().camRef = camRef;
                    boss.GetComponent<BossBehaviour>().spawnWave = spawnWave;

                    boss.GetComponent<BossBehaviour>().attackRange = bossAttackRange;
                    boss.GetComponent<BossBehaviour>().attackCooldownLength = bossAttackFreq;
                    boss.GetComponent<BossBehaviour>().attackForce = bossAttackForce;

                    SceneController.bossMinionWave = eventRes.eventID - 11;
                    hasPlayedOnce = true;
                }
            }
        }
    }
}
