using System;
using YadaScriptsLib;

namespace YadaScripts
{
	public class EnemyAlertExcalamtion : Component
	{
		public float lifespan = 1.5f;

		private Transform myTransform;
		private Transform followTarget;

		void Start()
		{
			myTransform = this.entity.GetComponent<Transform>();
			//Put sounds here
		}

		void Update()
		{
			FollowTarget();
			CheckLifespan();
		}

		void FollowTarget()
		{
			if (followTarget != null)
			{
				myTransform.globalPosition = new Vector3(followTarget.globalPosition.x, followTarget.globalPosition.y + 1, followTarget.globalPosition.z);
			}
		}

		void CheckLifespan()
		{
			lifespan -= Time.deltaTime;
			if (lifespan > 0)
			{
				lifespan -= Time.deltaTime;
			}
			else
			{
				Entity.DestroyEntity(entity);
			}
		}

		public void SetFollowTarget(Transform target)
		{
			followTarget = target;

		}
	}
}