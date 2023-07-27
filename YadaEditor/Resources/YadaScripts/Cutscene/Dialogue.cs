using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class Dialogue : Component
    {
        public Entity speakerSprite;
        public Entity dialogueSprite;
        public Entity text1 = null;
        public Entity text2 = null;
        public Entity text3 = null;

        private Entity currentText;

        public Vector3 speakerSpriteStartPos;
        private Vector3 speakerSpriteEndPos;

        private UI ui1;
        private UI ui2;
        private AudioSource speakerSFX1;
        private AudioSource speakerSFX2;
        private AudioSource speakerSFX3;
        
        private float cTimer;

        private bool hasPlayedSpeakerAudio;
        private bool hasUnlock;

        public bool dialogueIsShowing;
        private int currentTextNumber;

        void Start()
        {
            if (text1 != null)
                speakerSFX1 = text1.GetComponent<AudioSource>();
            if (text2 != null)
                speakerSFX2 = text2.GetComponent<AudioSource>();
            if (text3 != null)
                speakerSFX3 = text3.GetComponent<AudioSource>();
            speakerSpriteEndPos = speakerSprite.GetComponent<Transform>().localPosition;
            ui1 = dialogueSprite.GetComponent<UI>();
            ui2 = speakerSprite.GetComponent<UI>();
            currentText = text1;
            
        }

        void FixedUpdate()
        {
            CheckSpeakerAudio();
            CheckSpeakerLerp();
            CheckUnderlayFade();
            if (text1 !=null)
                CheckText();
        }

        private void CheckSpeakerAudio()
        {
            //Play Speaker Audio
            if (dialogueIsShowing == true && hasPlayedSpeakerAudio == false && currentText.active == true)
            {
                //Console.WriteLine(currentTextNumber);
                switch (currentTextNumber)
                {
                    case 0:
                        if (speakerSFX1 != null)
                            Audio.PlaySource(speakerSFX1);
                        break;
                    case 1:
                        if (speakerSFX2 != null)
                            Audio.PlaySource(speakerSFX2);
                        break;
                    case 2:
                        if (speakerSFX3 != null)
                            Audio.PlaySource(speakerSFX3);
                        break;
                    default:
                        if (speakerSFX1 != null)
                            Audio.PlaySource(speakerSFX1);
                        break;
                }
                hasPlayedSpeakerAudio = true;
            }
        }

        private void CheckSpeakerLerp()
        {
            if (dialogueIsShowing == true)
            {
                if (cTimer < 1.0f)
                {
                    cTimer += Time.fixedDeltaTime;
                }
                else
                {
                    cTimer = 1.0f;
                }
            }
            else
            {
                if (cTimer > 0.0f)
                {
                    cTimer -= Time.fixedDeltaTime;
                }
                else
                {
                    cTimer = 0.0f;
                }
            }
            speakerSprite.GetComponent<Transform>().localPosition = Vector3.Lerp(speakerSpriteStartPos, speakerSpriteEndPos, cTimer);
        }

        private void CheckUnderlayFade()
        {
            if (dialogueIsShowing == true && cTimer > 0.8f)
            {
                //Fading In Dialogue Image
                Vector4 color = ui1.color;
                color.w += Time.fixedDeltaTime * 1.5f;
                if (color.w > 1.0f) { color.w = 1.0f; }
                ui1.color = color;

                //Fading In Speaker Image
                Vector4 color2 = ui2.color;
                color2.w += Time.fixedDeltaTime * 2.0f;
                if (color2.w > 1.0f) { color2.w = 1.0f; }
                ui2.color = color2;
            }
            else
            {
                //Fading Out Dialogue Image
                Vector4 color = ui1.color;
                color.w -= Time.fixedDeltaTime * 10.0f;
                if (color.w < 0.0f) { color.w = 0.0f; }
                ui1.color = color;

                //Fading Out Speaker Image
                Vector4 color2 = ui2.color;
                color2.w -= Time.fixedDeltaTime * 10.0f;
                if (color2.w < 0.0f) { color2.w = 0.0f; }
                ui2.color = color2;
            }
        }

        private void CheckText()
        {
            if (dialogueIsShowing == true)
            {
                if (ui1.color.w >= 0.5f)
                {
                    currentText.active = true;
                }
            }
            else
            {
                currentText.active = false;
            }
        }

        public void SetDialogueText(int numberOrder)
        {
            if (speakerSFX1 != null)
                Audio.StopSource(speakerSFX1.channel);
            if (speakerSFX2 != null)
                Audio.StopSource(speakerSFX2.channel);
            if (speakerSFX3 != null)
                Audio.StopSource(speakerSFX3.channel);
            currentTextNumber = numberOrder;
            currentText.active = false;
            switch (numberOrder)
            {
                case 0:
                    currentText = text1;
                    break;
                case 1:
                    currentText = text2;
                    break;
                case 2:
                    currentText = text3;
                    break;
                default:
                    currentText = text1;
                    break;
            }
            hasPlayedSpeakerAudio = false;
        }
    }
}