using System;
using YadaScriptsLib;

namespace YadaScripts
{
	public class GroundCheck : Component
	{
		public PlayerBehaviour playerBehaviour;
		public Vector3 colliderOffset;
		public bool isTouchingGround;

		private Entity ownerObject;
		private Transform transform;
		private Collider collider;

		private void Start()
		{
			transform = this.entity.GetComponent<Transform>();
			collider = this.entity.GetComponent<Collider>();
			ActivateCollider(true);
		}

		private void FixedUpdate()
		{
			CheckMovement();
		}

		private void CheckMovement()
		{
			if (ownerObject != null)
			{
				transform.globalPosition = ownerObject.GetComponent<Transform>().globalPosition + colliderOffset;
				transform.globalRotation = ownerObject.GetComponent<Transform>().globalRotation;
			}
		}

		public void ActivateCollider(bool check)
		{
			collider.active = check;
		}

		public void SetOwner(Entity owner)
		{
			ownerObject = owner;
			playerBehaviour = owner.GetComponent<PlayerBehaviour>();
		}

		void OnTriggerStay(Entity collider)
		{
			if (collider != null && ownerObject != null && collider != ownerObject && collider.GetComponent<Collider>() != null && collider.GetComponent<Collider>().isTrigger == false)
			{
				isTouchingGround = true;
			}
		}

		void OnTriggerExit(Entity collider)
		{
			if (collider != null && ownerObject != null && collider != ownerObject && collider.GetComponent<Collider>() != null && collider.GetComponent<Collider>().isTrigger == false)
			{
				isTouchingGround = false;
			}
		}
	}
}