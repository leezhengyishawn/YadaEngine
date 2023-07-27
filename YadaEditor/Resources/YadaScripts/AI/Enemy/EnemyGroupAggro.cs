using System;
using YadaScriptsLib;

namespace YadaScripts
{
	public class EnemyGroupAggro : Component
	{
		private CommonEnemyBehaviour enemyScript;
		public bool isGroupParent;
		public Entity parentEnemy;

		public Entity childEnemy1;
		public Entity childEnemy2;
		public Entity childEnemy3;
		public Entity childEnemy4;

		void Start()
		{
			enemyScript = this.entity.GetComponent<CommonEnemyBehaviour>();
		}


		void Update()
		{
			if (CheckEnemyAlert())
			{
				if (isGroupParent)
				{
					AlertAllInGroup();
				}
				else
				{
					parentEnemy.GetComponent<EnemyGroupAggro>().AlertAllInGroup();
				}
			}
		}

		public bool CheckEnemyAlert()
		{
			return (enemyScript.aiState == CommonEnemyBehaviour.AIState.ALERTED);

		}

		public void AlertThisEnemy()
        {
			//this.entity.GetComponent<CommonEnemyBehaviour>().aiState = CommonEnemyBehaviour.AIState.ALERTED;
			enemyScript.aiState = CommonEnemyBehaviour.AIState.ALERTED;
			enemyScript.isAwake = true;
		}

		public void AlertAllInGroup()
		{
			if (childEnemy1 != null)
            {
				//Console.WriteLine("test!");
				//childEnemy1.GetComponent<CommonEnemyBehaviour>().aiState = CommonEnemyBehaviour.AIState.ALERTED;
				//Console.WriteLine("test!" + childEnemy1.GetComponent<CommonEnemyBehaviour>().aiState);
				//Console.WriteLine("test!" + childEnemy1.name);
				childEnemy1.GetComponent<EnemyGroupAggro>().AlertThisEnemy();
			}
				

			if (childEnemy2 != null)
				childEnemy2.GetComponent<EnemyGroupAggro>().AlertThisEnemy();

			if (childEnemy3 != null)
				childEnemy3.GetComponent<EnemyGroupAggro>().AlertThisEnemy();

			if (childEnemy4 != null)
				childEnemy4.GetComponent<EnemyGroupAggro>().AlertThisEnemy();
		}
	}
}