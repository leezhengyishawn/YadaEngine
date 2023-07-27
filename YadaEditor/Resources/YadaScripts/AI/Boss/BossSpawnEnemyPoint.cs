using System;
using YadaScriptsLib;

namespace YadaScripts
{
    public class BossSpawnEnemyPoint : Component
    {
        public int spawnWave = 0; //Which batch of minions should be spawned at the same time,

        public int minionSpawnType = 0;

        Transform transform;
        Entity burrow;
        Entity enemy;

        public float sinkIntoGroundAmount = -0.5f;

        bool spawned = false;

        enum MINIONTYPE    
        {
            MELEE = 0,
            RANGED,
            TANK
        }

        void Start()
        {
            transform = this.entity.GetComponent<Transform>();
            //SpawnFromGround();
        }

        void Update()
        {
            if (spawned)
            {
                // Line 38 potentially returns a null entity
                burrow = enemy.GetComponent<CommonEnemyBehaviour>().linkedBurrow;

                sinkIntoGroundAmount += 0.5f * Time.deltaTime; //Raise to surface in one second
                if (sinkIntoGroundAmount >= 0.0f)
                {
                    sinkIntoGroundAmount = 0.0f;
                    //enemy.GetComponent<CommonEnemyBehaviour>().active = true;
                    enemy.GetComponent<CommonEnemyBehaviour>().aiState = CommonEnemyBehaviour.AIState.ALERTED;
                }
                enemy.GetComponent<Transform>().GetChildByIndex(0).localPosition = new Vector3(0, sinkIntoGroundAmount, 0);

                // Guard against null burrow
                if (burrow)
                    burrow.GetComponent<Transform>().localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + sinkIntoGroundAmount, transform.localPosition.z);
            }
        }

        //Push the models to y = -0.5 and rise them up. Then make them alert
        public void SpawnFromGround()
        {
            // Line 61 is redundant
            burrow = Entity.InstantiatePrefab("Burrow");

            if (minionSpawnType == (int)MINIONTYPE.MELEE)
                enemy = Entity.InstantiatePrefab("NewEnemyMelee");
            else if (minionSpawnType == (int)MINIONTYPE.RANGED)
                enemy = Entity.InstantiatePrefab("NewEnemyRanged");
            else if (minionSpawnType == (int)MINIONTYPE.TANK)
                enemy = Entity.InstantiatePrefab("NewEnemyTank");


            //burrow.GetComponent<Transform>().localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - sinkIntoGroundAmount, transform.localPosition.z);
            enemy.GetComponent<Transform>().localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + sinkIntoGroundAmount, transform.localPosition.z);
            enemy.GetComponent<Transform>().GetChildByIndex(0).localPosition = new Vector3(0, sinkIntoGroundAmount, 0);
            //enemy.GetComponent<CommonEnemyBehaviour>().active = false;
            spawned = true;
        }
    }
}

