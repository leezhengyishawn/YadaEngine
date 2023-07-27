using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class EndScreen : Component
    {
        public Entity blackScreen;
        public Entity endPrompt;

        private UI ui;
        private bool startFade = false;
        private float currentTimer;

        void FixedUpdate()
        {
            if (startFade)
            {
                FadeIn(blackScreen, 2f);
                FadeIn(endPrompt, 0.8f);
                currentTimer += Time.fixedDeltaTime;
            }

            if(currentTimer >= 5f)
            {
                startFade = false;
                Scene.ChangeScene("SplashScreen");
            }
        }
        void OnTriggerEnter (Entity collider)
        {
            if (collider.GetComponent<PlayerBehaviour>() != null)
            {
                startFade = true;
            }
        }

        void FadeIn(Entity sprite, float fadeSpeed)
        {
            ui = sprite.GetComponent<UI>();
            Vector4 color = ui.color;
            color.w += Time.fixedDeltaTime * fadeSpeed;
            ui.color = color;
        }

    }
}
