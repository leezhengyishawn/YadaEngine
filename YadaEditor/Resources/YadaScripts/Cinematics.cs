using System;
using YadaScriptsLib;

namespace YadaScripts
{
	public class Cinematics : Component
	{
		public Entity prologue1;
		public Entity prologue2;
		public Entity prologue3;
		public Entity prologue4;

		public Entity subtitle1;
		public Entity subtitle2;
		public Entity subtitle3;
		public Entity subtitle4;
		public Entity subtitle5;
		public Entity subtitle6;

		public Entity blackScreen;
		public Entity whiteScreen;

		//prologue1 
		public Entity p1Tree;
		public Entity p1FG;
		private Vector3 p1StartPos;
		private Vector3 p1TreeStartPos;
		private Vector3 p1FGStartPos;

		//prologue2 
		public Entity p2Glow;
		public Entity p2Vignette;

		//prologue3
		public Entity p3Vegs;
		public Entity p3Fruits;
		private Vector3 p3VegStartPos;
		private Vector3 p3VegStartScale;
		private Vector3 p3FruitStartPos;
		private Vector3 p3FruitStartScale;

		//prologue4
		public Entity p4Lighting;
		private Vector3 p4StartPos;

		private AudioSource p1VO;
		private AudioSource p2VO;
		private AudioSource p3VO;
		private AudioSource p4VO;

		private AudioSource p1BGM;
		private AudioSource p2BGM;
		private AudioSource p3BGM;
		private AudioSource p4BGM;

		public Entity p1Particle;
		public Entity p2Particle;
		public Entity p4Particle;

		private UI ui1;
		private UI ui2;
		private UI ui3;

		private int scene = 1;
		private int glow = 0;
		private float glowTimer = 0f;
		private float cTimer = 0f;
		private float skipTimer = 0f;
		private bool transition = false;
		private bool thunder = false;

		private Vector3 velocity = Vector3.zero;

		public Entity blackScreenUI;

		//Skip and Next Button Timer
		private float delayDurationMaximum;
		private float delayDurationCurrent;

		//Timer Checking
		private bool player1ReadySkip;
		private bool player2ReadySkip;
		private bool playedSkip;

		void Start()
		{
			p1VO = subtitle1.GetComponent<AudioSource>();
			p2VO = subtitle3.GetComponent<AudioSource>();
			p3VO = subtitle4.GetComponent<AudioSource>();
			p4VO = subtitle5.GetComponent<AudioSource>();

			p1BGM = prologue1.GetComponent<AudioSource>();
			p2BGM = prologue2.GetComponent<AudioSource>();
			p3BGM = prologue3.GetComponent<AudioSource>();
			p4BGM = prologue4.GetComponent<AudioSource>();

            p1Particle.GetComponent<ParticleEmitter>().Pause();
            p2Particle.GetComponent<ParticleEmitter>().Pause();
            p4Particle.GetComponent<ParticleEmitter>().Pause();

			delayDurationMaximum = 0.5f;
			CutsceneButton.isShowingSkip = true;

			SetAlpha(blackScreen, 1.0f);
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
				subtitle3.active = false;
				subtitle4.active = false;
				subtitle5.active = false;
				subtitle6.active = false;
			}
			else
            {
				switch (scene)
				{
					case 1:
						PrologueOne();
						break;
					case 2:
						PrologueTwo();
						break;
					case 3:
						PrologueThree();
						break;
					case 4:
						PrologueFour();
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
					delayDurationCurrent += Time.deltaTime;
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
                blackScreenUI.active = true;
				FadeIn(blackScreenUI, 1f, ui1);
			}
			else
			{
				StartGame();
			}
		}

		void PrologueOne()
		{
			prologue1.active = true;

			if (blackScreen.GetComponent<UI>().color.w >= 0 && !transition)
			{
				FadeOut(blackScreen, 0.5f, ui1);
			}

			p1StartPos = prologue1.GetComponent<Transform>().localPosition;
			prologue1.GetComponent<Transform>().localPosition = Vector3.SmoothDamp(p1StartPos, new Vector3(-230, 0, 0), ref velocity, 0.5f, 3.5f, 1f);

			p1TreeStartPos = p1Tree.GetComponent<Transform>().localPosition;
			p1Tree.GetComponent<Transform>().localPosition = Vector3.SmoothDamp(p1TreeStartPos, new Vector3(-20, 0, -0.004f), ref velocity, 15f, 3f, 0.2f);

			p1FGStartPos = p1FG.GetComponent<Transform>().localPosition;
			p1FG.GetComponent<Transform>().localPosition = Vector3.SmoothDamp(p1FGStartPos, new Vector3(15, 0, -0.002f), ref velocity, 10f, 10f, 0.1f);

			if (!Audio.IsSourcePlaying(p1VO.channel))
			{
				Audio.PlaySource(p1VO);
				Audio.PlaySource(p1BGM);
			}

			if (cTimer < 5.5f && !transition)
			{
				cTimer += Time.fixedDeltaTime;
			}
			else if (cTimer >= 5.5f)
			{
				cTimer = 0f;
				transition = true;
				subtitle2.active = false;
                p1Particle.GetComponent<ParticleEmitter>().Stop();
            }

            if (cTimer >= 0.2f)
			{
                blackScreenUI.active = false;
				subtitle1.active = true;
				if (cTimer >= 2.8f)
				{
					subtitle1.active = false;
					if (cTimer >= 3f) subtitle2.active = true;
				}
                p1Particle.GetComponent<ParticleEmitter>().Play();
			}

			if (blackScreen.GetComponent<UI>().color.w < 1 && transition)
			{
				FadeIn(blackScreen, 2f, ui1);
			}
			else if (blackScreen.GetComponent<UI>().color.w >= 1 && transition)
			{
				transition = false;
				prologue1.active = false;
				prologue2.active = true;
				scene = 2;
			}
		}

		void PrologueTwo()
		{
			if (blackScreen.GetComponent<UI>().color.w >= 0 && !transition)
			{
				FadeOut(blackScreen, 3f, ui1);
			}

			if (!Audio.IsSourcePlaying(p2BGM.channel) && cTimer > 0.3f && !thunder)
			{
				Audio.PlaySource(p2BGM);
			}

			if (cTimer < 5.8f && !transition)
			{
				cTimer += Time.fixedDeltaTime;
				glowTimer += Time.fixedDeltaTime;
			}
			else if (cTimer >= 5.8f)
			{
				cTimer = 0f;
				transition = true;
				subtitle3.active = false;
                p2Particle.GetComponent<ParticleEmitter>().Stop();

            }

            if (cTimer >= 0.2f) subtitle3.active = true;

			if (cTimer > 0.2f && cTimer < 0.4f && whiteScreen.GetComponent<UI>().color.w < 1 & !thunder)
			{
				FadeIn(whiteScreen, 15f, ui1);
                p2Particle.GetComponent<ParticleEmitter>().Play();
			}
			else if (cTimer > 0.4f && whiteScreen.GetComponent<UI>().color.w >= 0 && !thunder)
			{
				FadeOut(whiteScreen, 3f, ui1);
			}
			else if (cTimer > 0.5f && whiteScreen.GetComponent<UI>().color.w <= 0)
			{
				thunder = true;
			}

			if (!Audio.IsSourcePlaying(p2VO.channel) && cTimer > 1.4f)
			{
				Audio.PlaySource(p2VO);
			}

			if (glowTimer >= 0.5f && !transition)
			{
				glowTimer = 0f;
				if (glow != 1) glow = 1;
				else glow = 2;
			}
			else if (transition)
			{
				glow = 0;
			}

			switch (glow)
			{
				case 1:
					FadeIn(p2Glow, 2f, ui2);
					FadeIn(p2Vignette, 1f, ui3);
					break;
				case 2:
					FadeOut(p2Glow, 2f, ui2);
					FadeOut(p2Vignette, 0.6f, ui3);
					break;
				default:
					break;
			}

			if (blackScreen.GetComponent<UI>().color.w < 1 && transition)
			{
				FadeIn(blackScreen, 2f, ui1);
			}
			else if (blackScreen.GetComponent<UI>().color.w >= 1f && transition)
			{
				transition = false;
				prologue2.active = false;
				prologue3.active = true;
				scene = 3;
			}
		}

		void PrologueThree()
		{
			if (blackScreen.GetComponent<UI>().color.w >= 0f && !transition)
			{
				FadeOut(blackScreen, 0.5f, ui1);
			}

			if (!Audio.IsSourcePlaying(p3VO.channel) && cTimer > 0.3f)
			{
				Audio.PlaySource(p3VO);
				Audio.PlaySource(p3BGM);
			}

			if (cTimer < 3.5f && !transition)
			{
				cTimer += Time.fixedDeltaTime;
			}
			else if (cTimer >= 3.5f)
			{
				cTimer = 0f;
				transition = true;
				subtitle4.active = false;
			}

			if (cTimer >= 0.2f) subtitle4.active = true;

			p3VegStartPos = p3Vegs.GetComponent<Transform>().localPosition;
			p3VegStartScale = p3Vegs.GetComponent<Transform>().localScale;
			p3FruitStartPos = p3Fruits.GetComponent<Transform>().localPosition;
			p3FruitStartScale = p3Fruits.GetComponent<Transform>().localScale;

			if (cTimer > 1f)
			{
				p3Vegs.GetComponent<Transform>().SmoothDirectionalResize(p3VegStartPos, new Vector3(0, 0, -0.004f), ref velocity, p3VegStartScale, new Vector3(1920, 1080, 1), ref velocity, 3f, 25f, 1f);
				p3Fruits.GetComponent<Transform>().SmoothDirectionalResize(p3FruitStartPos, new Vector3(0, -50, -0.001f), ref velocity, p3FruitStartScale, new Vector3(1850, 980, 1), ref velocity, 0.2f, 6f, 1f);
			}

			if (blackScreen.GetComponent<UI>().color.w < 1 && transition)
			{
				FadeIn(blackScreen, 1f, ui1);
			}
			else if (blackScreen.GetComponent<UI>().color.w >= 1 && transition)
			{
				transition = false;
				prologue3.active = false;
				prologue4.active = true;
				scene = 4;
			}
		}

		void PrologueFour()
		{
			if (blackScreen.GetComponent<UI>().color.w >= 0f && !transition)
			{
				FadeOut(blackScreen, 3f, ui1);
			}

			if (!Audio.IsSourcePlaying(p4VO.channel) && cTimer > 0.3f)
			{
				Audio.PlaySource(p4VO);
				Audio.PlaySource(p4BGM);
			}

			if (cTimer < 7f && !transition)
			{
				cTimer += Time.fixedDeltaTime;
			}
			else if (cTimer >= 7f)
			{
				cTimer = 0f;
				transition = true;
				subtitle6.active = false;
                p4Particle.GetComponent<ParticleEmitter>().Stop();

            }

            if (cTimer > 0.2f)
			{
                p4Particle.GetComponent<ParticleEmitter>().Play();
				subtitle5.active = true;
				FadeIn(p4Lighting, 0.3f, ui3);
			}
			if (cTimer > 2.5f) subtitle5.active = false;
			if (cTimer > 2.7f) subtitle6.active = true;

			p4StartPos = prologue4.GetComponent<Transform>().localPosition;
			prologue4.GetComponent<Transform>().localPosition = Vector3.SmoothDamp(p4StartPos, new Vector3(0, -210, 0), ref velocity, 0.5f, 2f, 1f);

			if (blackScreenUI.GetComponent<UI>().color.w < 1 && transition)
			{
				FadeIn(blackScreenUI, 1f, ui1);
			}
			else if (blackScreenUI.GetComponent<UI>().color.w >= 1 && transition)
			{
				transition = false;
				StartGame();
			}
		}

		void StartGame()
		{
			Scene.ChangeScene("Level 1");
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