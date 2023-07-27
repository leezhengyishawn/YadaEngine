using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class PauseButtonScript : Component
    {
        public bool isGrowBig = true;
        public bool isHovered = false;
        public bool isClicked = false;
        private bool framePassed = false;


        private Vector3 originalScale;
        public Entity hoverSFXent;
        public Entity clickSFXent;
        
        public AudioSource hoverSFXcomp;
        public AudioSource clickSFXcomp;
        // private Vector3 scale;

        void Start()
        {
            originalScale = this.entity.GetComponent<Transform>().localScale;
            hoverSFXcomp = hoverSFXent.GetComponent<AudioSource>();
            clickSFXcomp = clickSFXent.GetComponent<AudioSource>();

            // Load the master volume, override the scene's master volume (if available)
            File.ReadJsonFile("tempSave");
            if (File.CheckDataExists("MasterVolume"))
                Audio.masterVolume = File.ReadDataAsFloat("MasterVolume");
        }

        void Update()
        {
            if (framePassed)
            {
                isClicked = false;
            }

            framePassed = isClicked;
        }

        void FixedUpdate()
        {
            if (framePassed)
            {
                isClicked = false;
            }

            framePassed = isClicked;
        }

        //public void Reset()
        //{
        //    isHovered = false;
        //    isClicked = false;
        //    framePassed = false;
        //}

        public void SetUnHovered()
        {
            isHovered = false;
            isClicked = false;
            GrowSmall();
            //this.entity.GetComponent<UI>().SetHighlight(false);
        }

        void OnPointerClick()
        {
            isClicked = true;
            Audio.PlaySource(clickSFXcomp);
        }

        void OnPointerDeselect()
        {
            isClicked = false;
        }

        void OnPointerEnter()
        {
            if (this.active)
            {
                isHovered = true;
                GrowBig();
                Audio.PlaySource(hoverSFXcomp);
            }
        }

        void OnPointerExit()
        {
            isHovered = false;
            GrowSmall();
            // Mouse.SetCursor("Normal_Cursor", true);
        }

        public void GrowBig()
        {
            if (isGrowBig)
            {
                Vector3 newScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 1.2f, originalScale.z * 1.2f);
                this.entity.GetComponent<Transform>().localScale = newScale;
            }
            
        }

        public void GrowSmall()
        {
            if (isGrowBig)
            {
                this.entity.GetComponent<Transform>().localScale = originalScale;
            }
        }
    }
}
