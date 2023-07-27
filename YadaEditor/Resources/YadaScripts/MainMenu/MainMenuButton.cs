using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class MainMenuButton : Component
    {
        public bool hover = false;
        public bool click = false;
        private bool framePassed = false;

        public Entity hoverSFXent;
        public Entity clickSFXent;     
        
        public AudioSource hoverSFXcomp;
        public AudioSource clickSFXcomp;
        // private Vector3 scale;

        void Start()
        {
            hoverSFXcomp = hoverSFXent.GetComponent<AudioSource>();
            clickSFXcomp = clickSFXent.GetComponent<AudioSource>();
        }

        void Update()
        {
            if (framePassed)
                click = false;
            framePassed = click;
        }

        public void ResetButton()
        {
            hover = false;
            click = false;
            framePassed = false;
        }

        void OnPointerClick()
        {
            click = true;
            Audio.PlaySource(clickSFXcomp);
        }

        void OnPointerDeselect()
        {
            click = false;
        }

        void OnPointerEnter()
        {
            if (this.active)
            {
                hover = true;
                Audio.PlaySource(hoverSFXcomp);
            }
        }

        void OnPointerExit()
        {
            hover = false;
            // Mouse.SetCursor("Normal_Cursor", true);
        }
    }
}
