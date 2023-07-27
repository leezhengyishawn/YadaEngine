using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class TwoChooseOneScript : Component
    {
        public bool isToggledOn = false;


        public Entity otherButton;
        public Entity clickSFXent;
        
        public AudioSource clickSFXcomp;
        // private Vector3 scale;

        void Start()
        {
            clickSFXcomp = clickSFXent.GetComponent<AudioSource>();
        }

        void Update()
        {
            if (isToggledOn)
                this.entity.GetComponent<UI>().SetHighlight();
        }

        void FixedUpdate()
        {
        }

        public void Toggle(bool flag)
        {
            if (!flag)
            {
                isToggledOn = false;
            }
            else
            {
                otherButton.GetComponent<TwoChooseOneScript>().Toggle(false);
                isToggledOn = true;
            }
            
        }

        public bool GetToggledOn()
        {
            return isToggledOn;
        }

        void OnPointerClick()
        {
            Toggle(true);
            Audio.PlaySource(clickSFXcomp);
        }

        void OnPointerDeselect()
        {
        }

        void OnPointerEnter()
        {
        }

        void OnPointerExit()
        {
        }

    }
}
