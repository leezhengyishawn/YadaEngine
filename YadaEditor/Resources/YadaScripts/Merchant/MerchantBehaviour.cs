using System;
using YadaScriptsLib;

namespace YadaScripts
{
	public class MerchantBehaviour : Component
	{
		//Game Object References
		public Entity merchItem1;
		public Entity merchItem2;
		public Entity shopPrompt;
		public Entity merchModel;
		public Animator merchModelAnim;
		public Entity merchModelRunToPoint;
		public bool purchasedItem;
		public bool enteredTrigger;
        public bool allowMerchantMovement;

		//Animation control variables
		private bool triggeredYet = false;
		private bool atCounter = false;
		public bool startTalk = false;
		private float runToDistance = 0.0f; //How far the merchant model must run to the counter. We will always run to the counter in 1 sec

		private bool boughtOnce;
		private bool enteredOnce;

		private CutsceneTrigger cutsceneTrigger;
		private CameraBehaviour mainCamera;
		private Transform transform;

		private enum MerchantAnimState
        {
			UNTRIGGERED = 0,
			TRIGGERED,
			RUN,
			IDLE,
			TALK,
			WAVE
        }

		private void MerchantAnimation(int index, int count, bool canAnimate)
		{
			if (merchModelAnim.animationIndex != index)
			{
				merchModelAnim.animationIndex = index;
				merchModelAnim.animateCount = count;
				merchModelAnim.animate = canAnimate;
			}
		}

		void Start()
		{
			mainCamera = SceneController.mainCamera.GetComponent<CameraBehaviour>();
			transform = this.entity.GetComponent<Transform>();
			merchModelAnim = merchModel.GetComponent<Animator>();
			runToDistance = merchModelRunToPoint.GetComponent<Transform>().localPosition.z - merchModel.GetComponent<Transform>().localPosition.z;
			cutsceneTrigger = this.entity.GetComponent<CutsceneTrigger>();
		}

		void Update()
		{
			CheckPurchase();
			CheckMerchantMovement();
			CheckUI();
			CheckAnimation();
		}

		private void CheckMerchantMovement()
        {
			//First time entering shop, trigger animation
			if (allowMerchantMovement == true)
			{
				if (triggeredYet == false)
				{
					MerchantAnimation((int)MerchantAnimState.TRIGGERED, 1, true);
					triggeredYet = true;
				}
				if (merchModelAnim.animateCount == 0 && atCounter == false)
				{
					MerchantAnimation((int)MerchantAnimState.RUN, -1, true);
				}

				if (triggeredYet == true && atCounter == false && merchModelAnim.animationIndex == (int)MerchantAnimState.RUN)
				{
					merchModel.GetComponent<Transform>().localPosition += new Vector3(0, 0, runToDistance * Time.deltaTime); //Run towards counter at this speed

					//We are running along the z axis
					if (merchModel.GetComponent<Transform>().localPosition.z >= merchModelRunToPoint.GetComponent<Transform>().localPosition.z)
					{
						merchModel.GetComponent<Transform>().localPosition = merchModelRunToPoint.GetComponent<Transform>().localPosition;
						atCounter = true;
						MerchantAnimation((int)MerchantAnimState.TALK, -1, true);
						cutsceneTrigger.SetCanPressButtons();
						cutsceneTrigger.CheckDialogue(true);
						startTalk = true;
					}
				}
			}
		}

		private void CheckUI()
		{
			UI shopPromptUI = shopPrompt.GetComponent<UI>();
			if (!boughtOnce && enteredTrigger && shopPromptUI.color.w < 1.0f && startTalk)
            {
                Vector4 color = shopPromptUI.color;
                color.w += Time.fixedDeltaTime * 1.5f;
				if (color.w > 1.0f) { color.w = 1.0f; }
				shopPromptUI.color = color;
            }
            else if ((boughtOnce || !enteredTrigger) && shopPromptUI.color.w > 0.0f)
            {
				Vector4 color = shopPromptUI.color;
				color.w -= Time.fixedDeltaTime * 2.0f;
				if (color.w < 0.0f) { color.w = 0.0f; }
				shopPromptUI.color = color;
            }
        }

		private void CheckPurchase()
		{
			if (merchItem2.active == false || merchItem1 == false)
			{
				boughtOnce = true;
			}
		}

		private void CheckAnimation()
		{
			//Check if finish waving animation. If so, set to idle
			if (merchModelAnim.animationIndex == (int)MerchantAnimState.WAVE && merchModelAnim.animateCount == 0)
            {
				MerchantAnimation((int)MerchantAnimState.IDLE, -1, true);
			}
		}

		public void HasPurchaseItem()
		{
			//Audio.PlaySource(AudioController.purchaseSFX);
			purchasedItem = true;
		}

		void OnTriggerStay(Entity collider)
		{
			if (collider.GetComponent<CameraPoint>() != null && cutsceneTrigger.hasEndedDialogue == true)
			{
				if (SceneController.GetDistanceBetweenPlayers() < 5.0f)
                {
					WakeUpItems();
				}
				else
                {
					SleepItems();
                    
                }
			}
		}

		void OnTriggerExit(Entity collider)
		{
			if (collider.GetComponent<CameraPoint>() != null && cutsceneTrigger.hasEndedDialogue == true)
			{
				SleepItems();
			}
		}

		private void PlayWaveAnimation()
        {
			if (enteredOnce)
			{
				//We loop the animation thrice before going to idle
				MerchantAnimation((int)MerchantAnimState.WAVE, 1, true);
			}
		}

		void WakeUpItems()
		{
			if (enteredTrigger == false)
            {
				AudioController.PlaySFX("SFX Merchant Open");
				if (merchItem1)
				{
					merchItem1.GetComponent<MerchantItem>().Wake();
				}
				if (merchItem2)
				{
					merchItem2.GetComponent<MerchantItem>().Wake();
				}
				Vector3 tempCameraPos = (transform.forward * 10.0f) + (new Vector3(0, 1, 0));
				SceneController.SetMerchant(true);
				mainCamera.EnterCutscenePosition(transform.globalPosition, transform.globalPosition, tempCameraPos, false, false, false);
				enteredTrigger = true;
			}
		}

		void SleepItems()
		{
			if (enteredTrigger == true)
            {
				AudioController.PlaySFX("SFX Merchant Open");
				if (merchItem1)
				{
					merchItem1.GetComponent<MerchantItem>().Sleep();
				}
				if (merchItem2)
				{
					merchItem2.GetComponent<MerchantItem>().Sleep();
				}
				SceneController.SetMerchant(false);
				mainCamera.ExitCutscenePosition();
				enteredTrigger = false;
				enteredOnce = true;
				PlayWaveAnimation();
                AudioController.PlaySFX("LadyFinger_Greeting");
            }
		}
	}
}