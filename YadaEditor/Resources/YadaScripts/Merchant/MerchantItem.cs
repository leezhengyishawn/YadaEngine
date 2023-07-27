using System;
using YadaScriptsLib;

namespace YadaScripts
{
	public class MerchantItem : Component
	{
		public Entity merchantStore;
		public Entity merchantdiag;
		public bool isPlayerInfront;
		public int upgradeValue = 0;

		private MerchantBehaviour merchantBehaviour;
		private UI ui;
		private Transform myTransform;
		private Vector3 startingPos;
		private Vector3 endingPos;
		private float floatUpAmount;
		private bool isFloatingUp;
		private bool isAwake;

		void Start()
		{
			myTransform = this.entity.GetComponent<Transform>();
			merchantBehaviour = merchantStore.GetComponent<MerchantBehaviour>();
			startingPos = myTransform.localPosition;
			floatUpAmount = 0.2f;
			endingPos = startingPos + (Vector3.up * floatUpAmount);
			merchantdiag.active = false;
			isAwake = false;
			isFloatingUp = true;
			isPlayerInfront = false;
		}

		void Update()
		{
			CheckMovement();
			CheckUI();
		}

		private void CheckUI()
        {
			if (isPlayerInfront && merchantdiag.GetComponent<UI>().color.w < 1)
			{
				ui = merchantdiag.GetComponent<UI>();
				Vector4 color = ui.color;
				color.w += Time.deltaTime * 1.5f;
				ui.color = color;
			}
			if (!isPlayerInfront && merchantdiag.GetComponent<UI>().color.w > 0 && merchantdiag.active)
			{
				ui = merchantdiag.GetComponent<UI>();
				Vector4 color = ui.color;
				color.w -= Time.deltaTime * 2f;
				ui.color = color;
			}
			else if (!isPlayerInfront && merchantdiag.GetComponent<UI>().color.w <= 0)
			{
				merchantdiag.active = false;
			}
		}

		private void CheckMovement()
        {
			if (isAwake == true)
            {
				if (isFloatingUp == true)
				{
					myTransform.localPosition = Vector3.MoveTowards(myTransform.localPosition, endingPos, 1.0f * Time.deltaTime);
					if (GetDistance(myTransform.localPosition, endingPos) < 0.01f)
                    {
						isFloatingUp = false;
                    }
				}
				else
				{
					myTransform.localPosition = Vector3.MoveTowards(myTransform.localPosition, startingPos, 0.25f * Time.deltaTime);
					if (GetDistance(myTransform.localPosition, startingPos) < 0.01f)
					{
						isFloatingUp = true;
					}
				}
			}
			else
            {
				myTransform.localPosition = Vector3.MoveTowards(myTransform.localPosition, startingPos, 2.0f * Time.deltaTime);
			}
		}

		public void Wake()
		{
			isAwake = true;
			isFloatingUp = true;
		}

		public void Sleep()
		{
			isAwake = false;
		}

		void OnTriggerStay(Entity collider)
		{
			PunchCheck punchCheckCollider = collider.GetComponent<PunchCheck>();
            if (isAwake == true && punchCheckCollider != null && punchCheckCollider.GetUpgradeStatus() == false)
            {
				isPlayerInfront = true;
				if (punchCheckCollider.GetOwner().playerState == PlayerBehaviour.PlayerState.PUNCHING && merchantBehaviour.purchasedItem == false)
                {
					Entity purchaseParticle = Entity.InstantiatePrefab("sparkle");
					AudioController.PlaySFX("SFX Item Collect", AudioController.AudioVolume.VOLUME_200);
					purchaseParticle.GetComponent<Transform>().globalPosition = this.entity.GetComponent<Transform>().globalPosition;
					ApplyUpgradeEffects(punchCheckCollider, false);
					merchantdiag.active = false;
					this.entity.active = false;
				}
				else
                {
					merchantdiag.active = true;
				}
            }
		}

		void OnTriggerExit(Entity collider)
		{
			if (collider.GetComponent<PunchCheck>() != null)
			{
				isPlayerInfront = false;
			}
		}

		void ApplyUpgradeEffects(PunchCheck touchedPunchChecker, bool affectsBothPlayers)
		{
			if (affectsBothPlayers == true)
			{
				merchantBehaviour.HasPurchaseItem();
				SceneController.SetPunchUpgrade(upgradeValue);
			}
			else
            {
				touchedPunchChecker.SetUpgradeStatus(true);
				touchedPunchChecker.SetPunchUpgrade(upgradeValue);
			}
		}

		private float GetDistance(Vector3 point1, Vector3 point2)
		{
			return (point1 - point2).magnitude;
		}
	}
}