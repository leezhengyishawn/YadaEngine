using System;
using YadaScriptsLib;

namespace YadaScripts
{
    public class PopUp : Component
    {
        private Entity popupSprite;
        private Vector3 spriteStartPos;
        private Vector3 spriteStartScale;

        private Vector3 velocity = Vector3.zero;
        private Vector3 scaleVelocity = Vector3.zero;
        private Vector3 endPos;
        private Vector3 endScale;

        private Transform trans;

        private UI ui;
        private float speed;
        private int popUpState;

        private float threshold = 0.00001f;
        private float timeTaken = 0.1f; // Feel free to public this
        private float hangTime;

        void Start()
        {
            popupSprite = this.entity;
            spriteStartPos = popupSprite.GetComponent<Transform>().localPosition;
            spriteStartScale = popupSprite.GetComponent<Transform>().localScale;

            trans = popupSprite.GetComponent<Transform>();
            ui = popupSprite.GetComponent<UI>();
            popUpState = 0;
        }

        void FixedUpdate()
        {
            switch(popUpState)
            {
                case 1:
                    PopUpUpdate();
                    break;
                case 2:
                    HangUpdate();
                    break;
                case 3:
                    SpriteFadeout(2.0f);
                    break;
            }
        }

        private void PopUpUpdate()
        {
            trans.SmoothDirectionalResize(trans.localPosition, endPos, ref velocity, trans.localScale, endScale, ref scaleVelocity, timeTaken, 5f, Time.fixedDeltaTime);
            Vector3 scaleDiff = trans.localScale - endScale;
            if (scaleDiff.magnitudeSq < threshold)
            {
                popUpState = 2; // Fade out time
                
            }
        }

        public void SpritePopUp(Vector3 endpos, Vector3 endscale, float duration)
        {
            popUpState = 1;
            endScale = endscale;
            endPos = endpos;

            Vector4 color = ui.color;
            color.w = 1.0f;
            ui.color = color;

            trans.localScale = spriteStartScale;
            trans.localPosition = spriteStartPos;
            hangTime = duration;
        }

        private void HangUpdate()
        {
            hangTime -= Time.fixedDeltaTime;
            if (hangTime <= 3.0f)
                popUpState = 3;
        }

        public void SpriteFadeout(float speed)
        {
            ui = popupSprite.GetComponent<UI>();
            Vector4 color = ui.color;
            color.w -= Time.fixedDeltaTime * speed;
            ui.color = color;
        }

    }
}
