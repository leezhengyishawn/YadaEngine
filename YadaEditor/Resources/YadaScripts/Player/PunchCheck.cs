using System;
using System.Collections.Generic;
using YadaScriptsLib;

namespace YadaScripts
{
	public class PunchCheck : Component
	{
		private Entity ownerObject;
		private Collider collider;
		private int savedDamageCount;
		private bool isActive;
		private bool isPlayerColliding;
		private bool isUpgraded;

		private bool isPlayer1;
		private bool hasHit;
		//private bool hasAttacked;

		public bool isOverlappingPartner;
		public int savedUpgradeValue;

		private List<Entity> colliderList;

		//variables for punch wave to use
		public bool punchWave;
		public bool punchWaveUpgrade;
		public int punchWaveUpgradeVal;
		public bool punchWaveCharging;

		public void setActive(bool act)
		{
			isActive = act;
			if (act == false)
			{
				isOverlappingPartner = false;
				hasHit = false;
				//if (colliderList.Count != 0)
				colliderList.Clear();
			}
			else
				punchWave = true;
		}

		void Update()
        {
			if(Input.GetKeyPress(KEYCODE.KEY_F12))
            {
				SetPunchUpgrade(1);
            }
			if (Input.GetKeyPress(KEYCODE.KEY_F11))
			{
				SetPunchUpgrade(2);
			}
		}

		public void SetPunchUpgrade(int upgradeValue)
		{
			if (upgradeValue == 1 || upgradeValue == 2 || upgradeValue/10 == 1 || upgradeValue/10 == 2) //for punchwave
            {
				punchWaveUpgradeVal = upgradeValue > 9 ? upgradeValue/10 : upgradeValue;
				punchWaveUpgrade = true;
			}

			Vector3 newOffset = SceneController.punchDefaultOffset;
			Vector3 newSize = SceneController.punchDefaultSize;

			if (upgradeValue > 4)
				savedUpgradeValue = upgradeValue%10; //only for second stage upgrades
			else
				savedUpgradeValue = upgradeValue;

			if (upgradeValue == 3 || upgradeValue == 4) //for second stage upgrade
            {
				int currentUpgrade = Save.getUpgradeVal(isPlayer1);
				
				if (currentUpgrade/10 == (int)SceneController.PunchUpgrade.LONG_PUNCH) //long
                {
					newOffset = SceneController.punchLongOffset;
					newSize = SceneController.punchLongSize;
				}
				else if (currentUpgrade/10 == (int)SceneController.PunchUpgrade.WIDE_PUNCH) //wide
                {
					newOffset = SceneController.punchWideOffset;
					newSize = SceneController.punchWideSize;
				}
            }

            switch (punchWaveUpgradeVal)
            {
                case (int)SceneController.PunchUpgrade.DEFAULT_PUNCH:
                    collider.offset = SceneController.punchDefaultOffset;
                    collider.halfExtents = SceneController.punchDefaultSize;
                    break;
                case (int)SceneController.PunchUpgrade.LONG_PUNCH:
                    collider.offset = SceneController.punchLongOffset;
                    collider.halfExtents = SceneController.punchLongSize;
                    break;
                case (int)SceneController.PunchUpgrade.WIDE_PUNCH:
                    collider.offset = SceneController.punchWideOffset;
                    collider.halfExtents = SceneController.punchWideSize;
                    break;
                case (int)SceneController.PunchUpgrade.SUPER_DASH:
                    collider.offset = newOffset;
                    collider.halfExtents = newSize;
                    break;
                case (int)SceneController.PunchUpgrade.AUTO_CHARGED:
                    collider.offset = newOffset;
                    collider.halfExtents = newSize;
                    break;
            }

            Save.setUpgrade(upgradeValue, isPlayer1);
		}

		private void Start()
		{
			collider = this.entity.GetComponent<Collider>();
			collider.active = true;
			colliderList = new List<Entity>();
			punchWave = false;
			punchWaveUpgrade = false;
		}

		public void reInit()
		{
			//set punch upgrade from save file
			int upgradeVal = Save.getUpgradeVal(isPlayer1);
			SetPunchUpgrade(upgradeVal);
		}

		private void CheckPunchCollision(Entity touchedObject)
		{
            if (colliderList.Contains(touchedObject))
            {
                hasHit = true;
            }
            else
            {
				if (touchedObject.GetComponent<EnemyProjectileBehaviour>() != null || touchedObject.GetComponent<RollingEnemyBehaviour>() != null ||
					touchedObject.GetComponent<BossProjectile>() != null || touchedObject.GetComponent<BossProjectileLandingPredictor>() != null)
						return;
				colliderList.Add(touchedObject);
            }

            if (ownerObject != null && touchedObject != ownerObject && touchedObject.GetComponent<Collider>().isTrigger == false && hasHit == false)
			{
				bool hasHitInteractive = false;

				Vector3 punchDirection = (touchedObject.GetComponent<Transform>().globalPosition - ownerObject.GetComponent<Transform>().globalPosition).normalized;

				CommonEnemyBehaviour touchedEnemyBehaviour = touchedObject.GetComponent<CommonEnemyBehaviour>();
				if (touchedEnemyBehaviour != null)
				{
					touchedEnemyBehaviour.ReceivePunch(savedDamageCount, punchDirection, SceneController.punchForceNormal, SceneController.punchForceCharged);
					AudioController.PlaySFX(SFXType.PLAYER_PUNCH, AudioController.AudioVolume.VOLUME_100);
					hasHitInteractive = true;
				}

				PlayerBehaviour touchedPlayerBehaviour = touchedObject.GetComponent<PlayerBehaviour>();
				if (touchedPlayerBehaviour != null)
				{
					isOverlappingPartner = true;
					if (touchedPlayerBehaviour.playerState == PlayerBehaviour.PlayerState.CHARGING && savedDamageCount == SceneController.chargeCountMaximum)
					{
						ownerObject.GetComponent<PlayerBehaviour>().ReceivePunch(savedDamageCount, -punchDirection, SceneController.punchForceNormal, SceneController.punchForceCharged, true);
					}
					touchedPlayerBehaviour.ReceivePunch(savedDamageCount, punchDirection, SceneController.punchForceNormal, SceneController.punchForceCharged, true);

					//Only do a minor jump if one person punching the other
					if (ownerObject.GetComponent<PlayerBehaviour>().chargeCountCurrent >= SceneController.chargeCountMaximum && touchedPlayerBehaviour.chargeCountCurrent == 0)
					{
						ownerObject.GetComponent<PlayerBehaviour>().PlayerAnimation((int)PlayerBehaviour.AnimationState.PUNCH_UPPERCUT, 1, true);
						ownerObject.GetComponent<PlayerBehaviour>().PlayerJump(Vector3.up, SceneController.punchForceCharged * 0.5f);
					}
					hasHitInteractive = true;
				}

				Breakable breakableObject = touchedObject.GetComponent<Breakable>();
				if (breakableObject != null)
				{
					breakableObject.ActivateObject();
					hasHitInteractive = true;
				}

				PlankBehaviour plankObject = touchedObject.GetComponent<PlankBehaviour>();
				if (plankObject != null)
				{
					plankObject.PushPlank(punchDirection);
					hasHitInteractive = true;
				}

				MerchantItem merchantItem = touchedObject.GetComponent<MerchantItem>();
				if (merchantItem != null)
                {
					if (isUpgraded == false)
					{
						AudioController.PlaySFX("SFX Merchant Item Purchase");
						hasHitInteractive = true;
					}
                }

				BossBehaviour punchedBoss = touchedObject.GetComponent<BossBehaviour>();
				if (punchedBoss != null)
				{
					punchedBoss.ReceivePunch();
					hasHitInteractive = true;
				}

				ownerObject.GetComponent<PlayerBehaviour>().SetPunchEffects(hasHitInteractive);
			}
		}

		private void CheckRolling(Entity obj, bool isEnter)
		{
			if (ownerObject != null && obj != ownerObject)
			{
				PlayerBehaviour touchedPlayerBehaviour = obj.GetComponent<PlayerBehaviour>();
				if (touchedPlayerBehaviour != null)
				{
					isPlayerColliding = isEnter ? true : false;
				}
			}
		}

		void OnTriggerStay(Entity collider)
		{
			if (isActive)
			{
				CheckPunchCollision(collider);
			}
			else
			{
				CheckRolling(collider, true);
			}
		}

		void OnTriggerEnter(Entity collider)
		{
			if (isActive)
				CheckPunchCollision(collider);
			else
			{
				CheckRolling(collider, true);
			}
		}

		void OnTriggerExit(Entity collider)
		{
			CheckRolling(collider, false);
		}

		public bool getPunchColliding()
		{
			return isPlayerColliding;
		}

		public void SetOwner(Entity owner, bool player1)
		{
			ownerObject = owner;
			isPlayer1 = player1;
		}

		public PlayerBehaviour GetOwner()
		{
			return ownerObject.GetComponent<PlayerBehaviour>();
		}

		public void SetUpgradeStatus(bool check)
		{
			isUpgraded = check;
		}

		public bool GetUpgradeStatus()
		{
			return isUpgraded;
		}

		public void ActivatePunch(int damageCount)
		{
			savedDamageCount = damageCount;
		}
	}
}