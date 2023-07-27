using System;
using YadaScriptsLib;

namespace YadaScripts
{
	public class EnemyProjectileBehaviour : Component
	{
		public Entity myMesh;
		public float speed;
		public float rotateSpeed;
		public float lifetime;
		public Transform enemyTransform;

		private Entity targetPlayer;
		private Vector3 targetPosition;
		private Vector3 dir;
		private float currRot;

		void Start()
        {
			myMesh = this.entity.GetComponent<Transform>().GetChildByIndex(0).entity;
			currRot = 0;
		}

		void Update()
		{
			if (targetPlayer != null)
            {
				if (lifetime > 0)
				{
					this.entity.GetComponent<Transform>().globalPosition += dir * Time.deltaTime * speed;
					projectileAnim();
					lifetime -= Time.deltaTime;
				}
				else
				{
					//this.entity.active = false;
					Entity.DestroyEntity(this.entity);
				}
			}
		}

		public void SetTargetDirection(Entity target)
        {
			targetPlayer = target;
			dir = (targetPlayer.GetComponent<Transform>().globalPosition - this.entity.GetComponent<Transform>().globalPosition).normalized;
			targetPosition = targetPlayer.GetComponent<Transform>().globalPosition;
		}

		void OnTriggerStay(Entity collider)
		{
			if (collider.GetComponent<PlayerBehaviour>() != null)
			{
				Vector3 punchDirection = (collider.GetComponent<Transform>().globalPosition - this.entity.GetComponent<Transform>().globalPosition).normalized;
				collider.GetComponent<PlayerBehaviour>().ReceivePunch(1, punchDirection, SceneController.punchForceNormal, SceneController.punchForceNormal, false);
				//Entity.DestroyEntity(this.entity);
			}
			else if (collider.GetComponent<Collider>().isTrigger == true || collider.GetComponent<CommonEnemyBehaviour>() != null ||
				collider.GetComponent<PunchCheck>() != null)
			{
				//The projectile can pass through these obeject
				return;
			}
			Entity deathParticle = Entity.InstantiatePrefab("eneDeath");
			deathParticle.GetComponent<Transform>().globalPosition = entity.GetComponent<Transform>().globalPosition;
			//this.entity.active = false;
			Entity.DestroyEntity(this.entity);
		}

		private void projectileAnim()
        {
			//Quaternion newRotation = Quaternion.RotateTowards(cameraTransform.localRotation, quat, turningSpeed);

			Transform playerTransform = targetPlayer.GetComponent<Transform>();
			currRot += rotateSpeed * Time.deltaTime;

			Quaternion quat1 = Quaternion.CreateFromAxisAngle(new Vector3(0f, 1f, 0f), currRot); //rotate around y axis
			Quaternion quat3 = Quaternion.CreateFromAxisAngle(new Vector3(1f, 0f, 0f), (float)(Math.PI * 0.5)); //rotate around x axis
			Quaternion quat2 = Quaternion.LookAt(enemyTransform.globalPosition, targetPosition);
			Quaternion quat = quat2 * quat3 * quat1;

			this.entity.GetComponent<Transform>().localRotation = quat;
		}
	}
}