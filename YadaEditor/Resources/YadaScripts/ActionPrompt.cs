using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class ActionPrompt : Component
    {
        public Entity cPromptSprite;
        public Entity kPromptSprite;

        private UI ui;
        private AudioSource promptSFX;

        private bool triggeredOnce = false;
        private bool promptOnQueue = false;
        private float cTimer;


        void Start()
        {
            promptSFX = this.entity.GetComponent<AudioSource>();

        }

        void FixedUpdate()
        {

            if (!SceneController.isPromptOpen && triggeredOnce)
                promptOnQueue = false;
            if (!promptOnQueue)
            {
                ActivatePrompt(cPromptSprite);
                if(kPromptSprite != null)
                    ActivatePrompt(kPromptSprite);
            }
        }

        void ActivatePrompt(Entity promptSprite)
        {
            if (triggeredOnce && promptSprite.GetComponent<UI>().color.w < 1 && cTimer <= 0f)
            {
                ui = promptSprite.GetComponent<UI>();
                Vector4 color = ui.color;
                color.w += Time.fixedDeltaTime * 1.5f;
                ui.color = color;

                if (!Audio.IsSourcePlaying(promptSFX.channel) && promptSprite.GetComponent<UI>().color.w < 0.5)
                    Audio.PlaySource(promptSFX);
            }

            else if (triggeredOnce && promptSprite.GetComponent<UI>().color.w >= 1)
            {
                cTimer += Time.fixedDeltaTime *0.5f;
            }

            if (cTimer >= 4.5f && promptSprite.GetComponent<UI>().color.w >= 0)
            {
                ui = promptSprite.GetComponent<UI>();
                Vector4 color = ui.color;
                color.w -= Time.fixedDeltaTime * 1f;
                ui.color = color;

                SceneController.isPromptOpen = false;
            }
        }

        void OnTriggerEnter(Entity collider)
        {
            if (!triggeredOnce && (collider.name == "PlayerCharacter1" || collider.name == "PlayerCharacter2" ))
            {
                triggeredOnce = true;

                if (SceneController.isPromptOpen)
                    promptOnQueue = true;
                else 
                    SceneController.isPromptOpen = true;
            }
        }
    }
}
