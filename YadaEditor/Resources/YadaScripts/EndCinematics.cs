using System;
using YadaScriptsLib;

namespace YadaScripts
{
	public class EndCinematics : Component
	{
		public Entity epilogue1;
		public Entity epilogue2;
		public Entity epilogue3;
		public Entity credits;

		public Entity subtitle1;
		public Entity subtitle2;
		public Entity subtitle22;
		public Entity subtitle3;

		public Entity transitionScreen;
		public Entity blackScreen;
		public Entity whiteScreen;

		//epilogue1 
		public Entity e1BG;
		public Entity e1MG;
		public Entity e1Lighting;
		private Vector3 e1StartPos;

		//epilogue2 
		public Entity e2BG;
		public Entity e2FG;
		public Entity e2FGConfetti;
		public Entity e2MG;
		private Vector3 e2FGStartPos;
		private Vector3 e2FGStartScale;
		private Vector3 e2MGStartPos;
		private Vector3 e2FGConfettiStartPos;
		private Vector3 e2MGStartScale;
		private Vector3 e2FGConfettiStartScale;

		//credits
		public Entity credBG;
		public Entity cred01;
		public Entity cred02;
		public Entity cred1;
		public Entity cred2;
		public Entity cred3;
		public Entity cred4;
		public Entity cred5;
		public Entity cred61;
		public Entity cred62;

		private AudioSource e1VO;
		private AudioSource e2VO;
		private AudioSource e3VO;

		private AudioSource e1BGM;
		private AudioSource e2BGM;
		private AudioSource e3BGM;
		private AudioSource creditsBGM;

		private UI ui1;
		private UI ui2;
		private UI ui3;

        public Entity e1Particle;
        public Entity e2Particle;
        public Entity e3Particle;

		private int scene = 1;
		private int swap = 0;
		private int credSequence = 0;
		private float swapTimer = 0f;
		private float cTimer = 0f;
		private float maxTimer = 1f;
		private float skipTimer = 0f;
		private bool transition = false;

		private Vector3 velocity = Vector3.zero;

		public Entity blackScreenUI;

		//Skip and Next Button Timer
		private float delayDurationMaximum;
		private float delayDurationCurrent;

		//Timer Checking
		private bool player1ReadySkip;
		private bool player2ReadySkip;
		private bool playedSkip;

		private AudioSource rainBGM;

		void Start()
		{
			e1VO = subtitle1.GetComponent<AudioSource>();
			e2VO = subtitle2.GetComponent<AudioSource>();
			e3VO = subtitle3.GetComponent<AudioSource>();

			e1BGM = epilogue1.GetComponent<AudioSource>();
			e2BGM = epilogue2.GetComponent<AudioSource>();
			e3BGM = epilogue3.GetComponent<AudioSource>();
			creditsBGM = credits.GetComponent<AudioSource>();

            e1Particle.GetComponent<ParticleEmitter>().Pause();
            e2Particle.GetComponent<ParticleEmitter>().Pause();
            e3Particle.GetComponent<ParticleEmitter>().Pause();

			rainBGM = e1Particle.GetComponent<AudioSource>();
			rainBGM.volume = 0.0f;

			delayDurationMaximum = 0.5f;
			CutsceneButton.isShowingSkip = true;

			SetAlpha(blackScreen, 0.0f);
			SetAlpha(transitionScreen, 1.0f);
			skipTimer = 0.0f;
		}

		void Update()
        {
			if (skipTimer < 2.0f)
			{
				skipTimer += Time.deltaTime;
			}
			else
			{
				skipTimer = 2.0f;
				//skip button
				CutsceneButton.forceFadeIn = true;
				CutsceneButton.allowButtonFadeIn = true;
				CheckSkipButton();
				CheckButtonStatus();
			}
		}

		void FixedUpdate()
		{
			if (player1ReadySkip && player2ReadySkip)
            {
				subtitle1.active = false;
				subtitle2.active = false;
				subtitle22.active = false;
				subtitle3.active = false;
			}
			else
            {
				switch (scene)
				{
					case 1:
						EpilogueOne();
						break;
					case 2:
						EpilogueTwo();
						break;
					case 3:
						EpilogueThree();
						break;
					case 4:
						EndCredits();
						break;
					default:
						break;
				}
			}
		}

		private void CheckSkipButton()
		{
			//Player 1 Skip
			if (Input.GetGamepadButtonPress(GAMEPADCODE.GAMEPAD_B, 0) || Input.GetKeyPress(KEYCODE.KEY_V))
			{
				if (player1ReadySkip == false)
				{
					AudioController.PlaySFX("SFX Button Cutscene Press L");
					CutsceneButton.CutsceneButtonPress(1);
					player1ReadySkip = true;
				}
			}
			//Player 2 Skip
			if (Input.GetGamepadButtonPress(GAMEPADCODE.GAMEPAD_B, 1) || Input.GetKeyPress(KEYCODE.KEY_P))
			{
				if (player2ReadySkip == false)
				{
					AudioController.PlaySFX("SFX Button Cutscene Press L");
					CutsceneButton.CutsceneButtonPress(2);
					player2ReadySkip = true;
				}
			}
		}

		private void CheckButtonStatus()
		{
			if (player1ReadySkip && player2ReadySkip)
			{
				if (delayDurationCurrent < delayDurationMaximum)
				{
					delayDurationCurrent += Time.fixedDeltaTime;
				}
				else
				{
					ExitCutscene();
				}
			}
		}

		private void ExitCutscene()
		{
			if (playedSkip == false)
			{
				AudioController.PlaySFX("SFX Button End page Fast");
				playedSkip = true;
			}
			if (blackScreenUI.GetComponent<UI>().color.w < 1)
			{
				FadeIn(blackScreenUI, 1f, ui1);
			}
			else
			{
				EndGame();
			}
		}

		private void EndGame()
        {
			Scene.ChangeScene("MainMenu");
		}

		void EpilogueOne()
		{
			epilogue1.active = true;

			//if (blackScreen.GetComponent<UI>().color.w >= 0 && !transition)
			//{
			//	FadeOut(blackScreen, 0.5f, ui1);
			//}

			if (transitionScreen.GetComponent<UI>().color.w >= 0 && !transition)
			{
				FadeOut(transitionScreen, 0.5f, ui1);
			}

			if (!Audio.IsSourcePlaying(e1VO.channel))
			{
				Audio.PlaySource(e1VO);
				Audio.PlaySource(e1BGM);
			}

			if (cTimer < 5.7f && !transition)
			{
				cTimer += Time.fixedDeltaTime;
			}
			else if (cTimer >= 5.7f)
			{
				cTimer = 0f;
				transition = true;
				subtitle1.active = false;
			}

            if (cTimer >= 0.2f)
            {
                subtitle1.active = true;
                e1Particle.GetComponent<ParticleEmitter>().Play();
            }

            if (cTimer >= 0.6f)
			{
				FadeIn(e1Lighting, 0.3f, ui2);
				FadeOut(e1MG, 0.3f, ui1);
            }

            if(cTimer >= 4f)
                e1Particle.GetComponent<ParticleEmitter>().Stop();

			if (cTimer > 0.1f && cTimer < 4.1f)
			{
				if (rainBGM.volume < 0.9f)
				{
					rainBGM.volume += 1.0f * Time.deltaTime;
				}
				else
				{
					rainBGM.volume = 0.9f;

				}
			}
			else
			{
				if (rainBGM.volume > 0.0f)
				{
					rainBGM.volume -= 0.5f * Time.deltaTime;
				}
				else
				{
					rainBGM.volume = 0.0f;
				}
			}

            e1StartPos = epilogue1.GetComponent<Transform>().localPosition;
			epilogue1.GetComponent<Transform>().localPosition = Vector3.SmoothDamp(e1StartPos, new Vector3(160, 0, 0), ref velocity, 0.3f, 2f, 1f);

			if (blackScreen.GetComponent<UI>().color.w < 1 && transition)
			{
				FadeIn(blackScreen, 2f, ui1);
			}

			else if (blackScreen.GetComponent<UI>().color.w > 1 && transition)
			{
				transition = false;
				epilogue1.active = false;
				epilogue2.active = true;
				scene = 2;
				Audio.StopSource(e1BGM.channel);
			}
		}

		void EpilogueTwo()
		{
			if (rainBGM.volume > 0.0f)
			{
				rainBGM.volume -= 0.5f * Time.deltaTime;
			}
			else
			{
				rainBGM.volume = 0.0f;
			}

			if (blackScreen.GetComponent<UI>().color.w >= 0 && !transition)
			{
				FadeOut(blackScreen, 3f, ui1);
			}

			if (!Audio.IsSourcePlaying(e2BGM.channel))
			{
				Audio.PlaySource(e2BGM);
				Audio.PlaySource(e2VO);
			}

			if (cTimer < 7f && !transition)
			{
				cTimer += Time.fixedDeltaTime;
			}
			else if (cTimer >= 7f)
			{
				cTimer = 0f;
				transition = true;
				subtitle22.active = false;
            }

            if (cTimer >= 0.2f)
            {
                subtitle2.active = true;
                e2Particle.GetComponent<ParticleEmitter>().Play();
            }
            if (cTimer >= 4.8f)
			{
                e2Particle.GetComponent<ParticleEmitter>().Stop();
                subtitle2.active = false;
				if (cTimer >= 5f) subtitle22.active = true;
			}

			e2FGStartPos = e2FG.GetComponent<Transform>().localPosition;
			e2FGStartScale = e2FG.GetComponent<Transform>().localScale;
			e2MGStartPos = e2MG.GetComponent<Transform>().localPosition;
			e2MGStartScale = e2MG.GetComponent<Transform>().localScale;
			e2FGConfettiStartPos = e2FGConfetti.GetComponent<Transform>().localPosition;
			e2FGConfettiStartScale = e2FGConfetti.GetComponent<Transform>().localScale;

			if (cTimer > 0.3f)
			{
				e2FG.GetComponent<Transform>().SmoothDirectionalResize(e2FGStartPos, new Vector3(0, 0, -0.001f), ref velocity, e2FGStartScale, new Vector3(1920, 1080, 1), ref velocity, 0.2f, 25f, 1f);
				e2MG.GetComponent<Transform>().SmoothDirectionalResize(e2MGStartPos, new Vector3(0, 0, -0.003f), ref velocity, e2MGStartScale, new Vector3(1920, 1080, 1), ref velocity, 0.2f, 1f, 1f);
				e2FGConfetti.GetComponent<Transform>().SmoothDirectionalResize(e2FGConfettiStartPos, new Vector3(0, 0, -0.002f), ref velocity, e2FGConfettiStartScale, new Vector3(1920, 1080, 1), ref velocity, 0.2f, 2f, 1f);
			}

			if (blackScreen.GetComponent<UI>().color.w < 1 && transition)
			{
				FadeIn(blackScreen, 2f, ui1);
            }

            else if (blackScreen.GetComponent<UI>().color.w >= 1f && transition)
			{
				transition = false;
				epilogue2.active = false;
				epilogue3.active = true;
				scene = 3;
				Audio.StopSource(e2BGM.channel);

			}
		}

		void EpilogueThree()
		{
			if (blackScreen.GetComponent<UI>().color.w >= 0f && !transition)
			{
				FadeOut(blackScreen, 0.3f, ui1);
			}

			if (!Audio.IsSourcePlaying(e3VO.channel) && cTimer > 0.3f && cTimer < 4.5f)
			{
				Audio.PlaySource(e3VO);
				Audio.PlaySource(e3BGM);
			}

			if (cTimer < 6.5f && !transition)
			{
				cTimer += Time.fixedDeltaTime;
			}

			else if (cTimer >= 6.5f)
			{
				transition = true;
				cTimer = 0f;
			}

            if (cTimer >= 0.2f && !transition)
            {
                subtitle3.active = true;
                e3Particle.GetComponent<ParticleEmitter>().Play();
            }
            if (cTimer >= 4.4f)
            {
                subtitle3.active = false;
                e3Particle.GetComponent<ParticleEmitter>().Stop();
            }

                if (whiteScreen.GetComponent<UI>().color.w < 1 && cTimer >= 4f)
			{
                FadeOut(blackScreen, 3f, ui3);
				FadeIn(whiteScreen, 0.5f, ui1);
            }

            else if (whiteScreen.GetComponent<UI>().color.w > 1f && transition)
			{
				transition = false;
				epilogue3.active = false;
				credits.active = true;
				scene = 4;
				Audio.StopSource(e3BGM.channel);
			}

		}

		void EndCredits()
		{
			if (whiteScreen.GetComponent<UI>().color.w >= 0f && !transition)
			{
				FadeOut(whiteScreen, 1f, ui1);
			}

			if (!Audio.IsSourcePlaying(creditsBGM.channel) && cTimer > 0.3f)
			{
				Audio.PlaySource(creditsBGM);
			}

			if (cTimer < maxTimer && !transition)
			{
				cTimer += Time.fixedDeltaTime;
			}
			else if (cTimer >= maxTimer)
			{
				cTimer = 0f;
				credSequence++;
			}

			if (transition) EndGame();

			if (credSequence == 0 && !transition)
			{
				credSequence = 1;
			}
			switch (credSequence)
			{
				case 1:
					FadeIn(cred01, 1f, ui1);
					FadeIn(cred02, 1f, ui1);
					SwapSprite(cred01, cred02);
					maxTimer = 4f;
					break;
				case 2:
					FadeOut(cred01, 4f, ui1);
					FadeOut(cred02, 4f, ui1);
					FadeIn(cred1, 1f, ui2);
					maxTimer = 5f;
					break;
				case 3:
					FadeOut(cred1, 4f, ui1);
					FadeIn(cred2, 1f, ui2);
					break;
				case 4:
					FadeOut(cred2, 4f, ui1);
					FadeIn(cred3, 1f, ui2);
					break;
				case 5:
					FadeOut(cred3, 4f, ui1);
					FadeIn(cred4, 1f, ui2);
					break;
				case 6:
					FadeOut(cred4, 4f, ui1);
					FadeIn(cred5, 0.5f, ui2);
					maxTimer = 6f;
					break;
				case 7:
					FadeOut(cred5, 4f, ui1);
					FadeIn(cred61, 1f, ui2);
					FadeIn(cred62, 1f, ui2);
					SwapSprite(cred61, cred62);
					maxTimer = 3f;
					break;
				case 8:
					transition = true;
					break;
				default:
					break;
			}
		}

		void SwapSprite(Entity sprite1, Entity sprite2)
		{
			swapTimer += Time.fixedDeltaTime;
			if (swapTimer >= 0.5f && !transition)
			{
				swapTimer = 0f;
				if (swap != 1) swap = 1;
				else swap = 2;
			}
			else if (transition) swap = 0;

			switch (swap)
			{
				case 1:
					sprite1.active = true;
					sprite2.active = false;
					break;
				case 2:
					sprite1.active = false;
					sprite2.active = true;
					break;
				default:
					break;
			}
		}

		void FadeIn(Entity sprite, float speed, UI ui)
		{
			ui = sprite.GetComponent<UI>();
			Vector4 color = ui.color;
			color.w += Time.fixedDeltaTime * speed;
			ui.color = color;
		}

		void FadeOut(Entity sprite, float speed, UI ui)
		{
			ui = sprite.GetComponent<UI>();
			Vector4 color = ui.color;
			color.w -= Time.fixedDeltaTime * speed;
			ui.color = color;
		}

		private void SetAlpha(Entity sprite, float value)
		{
			UI myUI;
			myUI = sprite.GetComponent<UI>();
			Vector4 color = myUI.color;
			color.w = value;
			myUI.color = color;
		}
	}
}