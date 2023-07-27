using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class LemonBehaviour : Component
    {
        public Entity whiteScreenOverlay;
        public float explosionTimerMaximum;
        public float delaySpawnTimerMaximum;
        public float fadeoutTimerMaximum;
        public float movementSpeed;

        private Transform myTransform;
        private Animator myAnim;
        private Vector3 startPosition;
        private Vector3 endPosition;
        private float explosionTimerCurrent;
        private float delaySpawnTimerCurrent;
        private float fadeoutTimerCurrent;
        private float transitionTimerCurrent;
        private bool startAnimation;
        private bool hasSpawnExplosion;
        private bool hasSpawnSound;

        void Start()
        {
            myTransform = this.entity.GetComponent<Transform>();
            myAnim = this.entity.GetComponent<Animator>();
            startPosition = myTransform.globalPosition;
            endPosition = myTransform.globalPosition + (Vector3.up * 20.0f);
            myTransform.globalPosition = startPosition - (Vector3.up * 10.0f);
            myAnim.animate = false;
            SetAlpha(whiteScreenOverlay, 0.0f);
        }

        void Update()
        {
            if (startAnimation == true)
            {
                if (delaySpawnTimerCurrent < delaySpawnTimerMaximum)
                {
                    delaySpawnTimerCurrent += Time.deltaTime;
                }
                else
                {
                    if (hasSpawnSound == false)
                    {
                        AudioController.PlaySFX("SFX Lemon Explode");
                        hasSpawnSound = true;
                    }

                    if (myTransform.globalPosition.y < endPosition.y)
                    {
                        myTransform.globalPosition += Vector3.up * movementSpeed * Time.deltaTime;
                    }

                    if (explosionTimerCurrent < explosionTimerMaximum)
                    {
                        explosionTimerCurrent += Time.deltaTime;
                    }
                    else
                    {
                        if (hasSpawnExplosion == false)
                        {
                            SceneController.mainCamera.GetComponent<CameraBehaviour>().ShakeCamera(true, 2.0f);
                            Entity spawnParticle = Entity.InstantiatePrefab("LemonParticle");
                            spawnParticle.GetComponent<Transform>().globalPosition = myTransform.globalPosition + (Vector3.up * 0.5f);
                            hasSpawnExplosion = true;
                        }
                        else
                        {
                            if (fadeoutTimerCurrent < fadeoutTimerMaximum)
                            {
                                fadeoutTimerCurrent += Time.deltaTime;
                            }
                            else
                            {
                                FadeIn(whiteScreenOverlay, 5.0f);
                                if (transitionTimerCurrent < 1.0f)
                                {
                                    transitionTimerCurrent += Time.deltaTime;
                                }
                                else if (whiteScreenOverlay.GetComponent<UI>().color.w >= 1.0f)
                                {
                                    SceneController.isTransitioningLevel = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void PlayAnimation()
        {
            myTransform.globalPosition = startPosition;
            startAnimation = true;
            myAnim.animate = true;
        }

        private void FadeIn(Entity sprite, float speed)
        {
            UI myUI;
            myUI = sprite.GetComponent<UI>();
            Vector4 color = myUI.color;
            color.w += Time.fixedDeltaTime * speed;
            if (color.w > 1.0f) { color.w = 1.0f; }
            myUI.color = color;
        }

        private void FadeOut(Entity sprite, float speed)
        {
            UI myUI;
            myUI = sprite.GetComponent<UI>();
            Vector4 color = myUI.color;
            color.w -= Time.fixedDeltaTime * speed;
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