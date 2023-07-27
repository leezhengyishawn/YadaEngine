using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class CheckboxButton : Component
    {
        public bool isToggledOn = false;
        public bool isHovered = false;
        public bool isClicked = false;
        private bool framePassed = false;


        public Entity clickSFXent;
        
        public AudioSource clickSFXcomp;
        // private Vector3 scale;

        void Start()
        {
            clickSFXcomp = clickSFXent.GetComponent<AudioSource>();
        }

        void Update()
        {
            if (isHovered)
                this.entity.GetComponent<UI>().SetHighlight();
            

            if (isToggledOn)
                this.entity.GetComponent<UI>().SetHighlight();

            if (framePassed)
                isClicked = false;
            framePassed = isClicked;
        }

        void FixedUpdate()
        {
        }

        public void Toggle(bool flag)
        {
            isToggledOn = flag;
        }


        void OnPointerClick()
        {
            isClicked = true;
            Audio.PlaySource(clickSFXcomp);
        }

        void OnPointerDeselect()
        {
        }

        void OnPointerEnter()
        {
            isHovered = true;
        }

        void OnPointerExit()
        {
            isHovered = false;
        }

    }
}
