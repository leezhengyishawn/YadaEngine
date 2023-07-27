using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class TransitionLevelTrigger : Component
    {
        public Entity blackOverlayObject;
        public Entity whiteOverlayObject;
        public int currentLevel;
        public bool collideTransit = false;

        private float transitionTimer;
        private bool hasActivated = false;

        void Start()
        {
            if (this.entity.GetComponent<Renderer>() != null)
            {
                this.entity.GetComponent<Renderer>().active = false;
            }
            SceneController.isTransitioningLevel = false;
            SetAlpha(blackOverlayObject, 1.0f);
            blackOverlayObject.active = true;
        }

        void Update()
        {
            if (SceneController.isTransitioningLevel == true)
            {
                if (whiteOverlayObject != null)
                {
                    FadeIn(whiteOverlayObject, 0.5f);
                }
                else
                {
                    FadeIn(blackOverlayObject, 0.5f);
                }
                TransitionToLevel();
            }
            else
            {
                FadeOut(blackOverlayObject, 0.5f);
            }
            CheckActive();

            if (hasActivated == true && collideTransit == true)
            {
                //SceneController.mainPlayer1.GetComponent<PlayerBehaviour>().playerState = PlayerBehaviour.PlayerState.WALK;
                //SceneController.mainPlayer2.GetComponent<PlayerBehaviour>().playerState = PlayerBehaviour.PlayerState.WALK;

                SceneController.mainPlayer1.GetComponent<PlayerMovement>().inputDirection.y = 1;
                SceneController.mainPlayer1.GetComponent<PlayerMovement>().inputDirection.x = 0;

                SceneController.mainPlayer2.GetComponent<PlayerMovement>().inputDirection.y = 1;
                SceneController.mainPlayer2.GetComponent<PlayerMovement>().inputDirection.x = 0;

                //SceneController.mainPlayer2.GetComponent<PlayerBehaviour>().PlayerAnimation((int)PlayerBehaviour.AnimationState.WALK, -1, true);
                SceneController.mainPlayer1.GetComponent<PlayerMovement>().isMoving = true;
                SceneController.mainPlayer2.GetComponent<PlayerMovement>().isMoving = true;

                SceneController.mainPlayer1.GetComponent<PlayerMovement>().SetMovement(1.0f);
                SceneController.mainPlayer2.GetComponent<PlayerMovement>().SetMovement(1.0f);

                //SceneController.mainPlayer1.GetComponent<PlayerBehaviour>().PlayerAnimation((int)PlayerBehaviour.AnimationState.WALK, -1, true);
                //SceneController.mainPlayer2.GetComponent<PlayerBehaviour>().PlayerAnimation((int)PlayerBehaviour.AnimationState.WALK, -1, true);

            }
        }

        private void CheckActive()
        {
            if (blackOverlayObject.GetComponent<UI>().color.w > 0.0f)
            {
                blackOverlayObject.active = true;
            }
            else
            {
                blackOverlayObject.active = false;
            }
        }

        void OnTriggerEnter(Entity collider)
        {
            if (collideTransit)
            {
                if (hasActivated == false && (collider == SceneController.mainPlayer1 || collider == SceneController.mainPlayer2))
                {
                    SceneController.isTransitioningLevel = true;
                    hasActivated = true;

                    SceneController.mainPlayer1.GetComponent<PlayerBehaviour>().playerModel.GetComponent<Animator>().animationIndex = (int)PlayerBehaviour.AnimationState.WALK;
                    SceneController.mainPlayer1.GetComponent<PlayerBehaviour>().playerModel.GetComponent<Animator>().animateCount = -1;
                    SceneController.mainPlayer2.GetComponent<PlayerBehaviour>().playerModel.GetComponent<Animator>().animationIndex = (int)PlayerBehaviour.AnimationState.WALK;
                    SceneController.mainPlayer2.GetComponent<PlayerBehaviour>().playerModel.GetComponent<Animator>().animateCount = -1;

                    SceneController.mainPlayer1.GetComponent<PlayerBehaviour>().active = false;
                    SceneController.mainPlayer2.GetComponent<PlayerBehaviour>().active = false;
                }
            }
        }

        private void TransitionToLevel()
        {
            if (blackOverlayObject.GetComponent<UI>().color.w >= 1.0f || (whiteOverlayObject != null && whiteOverlayObject.GetComponent<UI>().color.w >= 1.0f))
            {
                if (currentLevel == 1)
                {
                    Scene.ChangeScene("Level 2");
                }
                else if (currentLevel == 2)
                {
                    Scene.ChangeScene("Level 3");
                }
                else if (currentLevel == 3)
                {
                    if (transitionTimer < 2.0f)
                    {
                        transitionTimer += Time.deltaTime;
                    }
                    else
                    {
                        Scene.ChangeScene("CinematicEpilogue");
                    }
                }
            }
        }

        private void FadeIn(Entity sprite, float speed)
        {
            UI myUI;
            myUI = sprite.GetComponent<UI>();
            Vector4 color = myUI.color;
            color.w += Time.deltaTime * speed;
            if (color.w > 1.0f) { color.w = 1.0f; }
            myUI.color = color;
        }

        private void FadeOut(Entity sprite, float speed)
        {
            UI myUI;
            myUI = sprite.GetComponent<UI>();
            Vector4 color = myUI.color;
            color.w -= Time.deltaTime * speed;
            if (color.w < 0.0f) { color.w = 0.0f; }
            myUI.color = color;
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