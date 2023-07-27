using System;
using YadaScriptsLib;

namespace YadaScripts
{
	public class BurrowBehaviour : Component
	{
		//During the Awake() of the enemy, it will spawn this object.
		public CommonEnemyBehaviour linkedEnemy;
		private float animationTime = 0.0f;
		private Vector3 finalScale;
		private float incrementalScale;

		void Update()
        {
			if (animationTime > 0.0f)
            {
				this.entity.GetComponent<Transform>().localScale += new Vector3(incrementalScale * Time.deltaTime, incrementalScale * Time.deltaTime, incrementalScale * Time.deltaTime);
				animationTime -= Time.deltaTime;

				if (animationTime <= 0.0f)
                {
					animationTime = 0.0f;
					this.entity.GetComponent<Transform>().localScale = finalScale;

					if (finalScale.x == 0 && finalScale.y == 0 && finalScale.z == 0)
					{
						//Entity.DestroyEntity(this.entity);

						// This fix the issue of "first encounter with boss in lvl 3
						// (after jumping up onto the platform where the boss is) crashes the game"
						this.entity.active = false;
					}
                }
			}
        }

		//Assume the burrow will scale uniformly. (x,y,z) all at the same rate.
		public void ScaleTo(float scaleTo = 1.0f, float timeToAnimate = 0.5f)
        {
			animationTime = timeToAnimate;
			finalScale = new Vector3(scaleTo, scaleTo, scaleTo);
			incrementalScale = (scaleTo - this.entity.GetComponent<Transform>().localScale.x) / timeToAnimate;
		}
	}
}