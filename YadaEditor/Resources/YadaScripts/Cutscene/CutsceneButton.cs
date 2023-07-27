using System;
using System.Collections.Generic;
using YadaScriptsLib;

namespace YadaScripts
{
    public class CutsceneButton : Component
    {
        private static Entity skipText;
        private static Entity nextText;
        private static Entity buttonB;
        private static Entity buttonV;
        private static Entity buttonP;
        private static Entity iconP1;
        private static Entity iconP2;
        private static Entity blackBarTop;
        private static Entity blackBarBot;

        private static Transform buttonVTransform;
        private static Transform buttonPTransform;
        private static Transform iconP1Transform;
        private static Transform iconP2Transform;
        private static Transform blackBarTopTransform;
        private static Transform blackBarBotTransform;

        private static Vector3 buttonVStartScale;
        private static Vector3 buttonPStartScale;
        private static Vector3 iconP1StartScale;
        private static Vector3 iconP2StartScale;
        private static Vector3 blackBarTopStartPos;
        private static Vector3 blackBarBotStartPos;

        private float transitionSpeed;
        private float blackBarHideOffset;

        public static bool forceFadeIn;
        public static bool allowButtonFadeIn;
        public static bool isShowingSkip;
        private static bool player1HasPressed;
        private static bool player2HasPressed;

        public bool isNotInGameplay;

        private void Start()
        {
            forceFadeIn = false;
            allowButtonFadeIn = false;
            isShowingSkip = false;
            player1HasPressed = false;
            player2HasPressed = false;

            skipText = this.entity.GetComponent<Transform>().GetChildByIndex(0).entity;
            nextText = this.entity.GetComponent<Transform>().GetChildByIndex(1).entity;
            buttonB = this.entity.GetComponent<Transform>().GetChildByIndex(2).entity;
            buttonV = this.entity.GetComponent<Transform>().GetChildByIndex(3).entity;
            buttonP = this.entity.GetComponent<Transform>().GetChildByIndex(4).entity;
            iconP1 = this.entity.GetComponent<Transform>().GetChildByIndex(5).entity;
            iconP2 = this.entity.GetComponent<Transform>().GetChildByIndex(6).entity;
            blackBarTop = this.entity.GetComponent<Transform>().GetChildByIndex(7).entity;
            blackBarBot = this.entity.GetComponent<Transform>().GetChildByIndex(8).entity;

            buttonVTransform = buttonV.GetComponent<Transform>();
            buttonPTransform = buttonP.GetComponent<Transform>();
            iconP1Transform = iconP1.GetComponent<Transform>();
            iconP2Transform = iconP2.GetComponent<Transform>();
            blackBarTopTransform = blackBarTop.GetComponent<Transform>();
            blackBarBotTransform = blackBarBot.GetComponent<Transform>();

            buttonVStartScale = buttonVTransform.localScale;
            buttonPStartScale = buttonPTransform.localScale;
            iconP1StartScale = iconP1Transform.localScale;
            iconP2StartScale = iconP2Transform.localScale;
            blackBarTopStartPos = blackBarTopTransform.localPosition;
            blackBarBotStartPos = blackBarBotTransform.localPosition;

            iconP1.GetComponent<Transform>().localScale = Vector3.zero;
            iconP2.GetComponent<Transform>().localScale = Vector3.zero;

            transitionSpeed = 10.0f;

            if (isNotInGameplay == false)
            {
                blackBarHideOffset = 100.0f;
                blackBarTopTransform.localPosition = blackBarTopStartPos + (Vector3.up * blackBarHideOffset);
                blackBarBotTransform.localPosition = blackBarBotStartPos - (Vector3.up * blackBarHideOffset);
            }

            SetAlpha(skipText, 0.0f);
            SetAlpha(nextText, 0.0f);
            SetAlpha(buttonB, 0.0f);
            SetAlpha(buttonV, 0.0f);
            SetAlpha(buttonP, 0.0f);
            SetAlpha(iconP1, 0.0f);
            SetAlpha(iconP2, 0.0f);
        }
        private void Update()
        {
            if (allowButtonFadeIn == true && (SceneController.isInCutscene == true || forceFadeIn == true))
            {
                CheckBaseFadeIn();
                CheckIconStatus();
            }
            else
            {
                CheckBaseFadeOut();
            }
            CheckBlackBar();
        }

        private void CheckBaseFadeIn()
        {
            if (blackBarBotTransform.localPosition.y > blackBarBotStartPos.y - 10.0f)
            {
                if (isShowingSkip == true)
                {
                    FadeIn(skipText, 1.5f);
                    FadeOut(nextText, 1.5f);
                }
                else
                {
                    FadeIn(nextText, 1.5f);
                    FadeOut(skipText, 1.5f);
                }
                FadeIn(buttonB, 1.5f);
                FadeIn(buttonV, 1.0f);
                FadeIn(buttonP, 1.0f);
                FadeIn(iconP1, 1.0f);
                FadeIn(iconP2, 1.0f);
            }
        }

        private void CheckBaseFadeOut()
        {
            FadeOut(skipText, 2.0f * transitionSpeed);
            FadeOut(nextText, 2.0f * transitionSpeed);
            FadeOut(buttonB, 2.0f * transitionSpeed);
            FadeOut(buttonV, 1.5f * transitionSpeed);
            FadeOut(buttonP, 1.5f * transitionSpeed);
            FadeOut(iconP1, 1.5f * transitionSpeed);
            FadeOut(iconP2, 1.5f * transitionSpeed);
        }

        private void CheckIconStatus()
        {
            //Player 1 icon check
            if (player1HasPressed)
            {
                buttonVTransform.localScale = Vector3.Lerp(buttonVTransform.localScale, Vector3.zero, transitionSpeed * 2.0f * Time.deltaTime);
                iconP1Transform.localScale = Vector3.Lerp(iconP1Transform.localScale, iconP1StartScale, transitionSpeed * Time.deltaTime);
            }
            else
            {
                buttonVTransform.localScale = Vector3.Lerp(buttonVTransform.localScale, buttonVStartScale, transitionSpeed * Time.deltaTime);
                iconP1Transform.localScale = Vector3.Lerp(iconP1Transform.localScale, Vector3.zero, transitionSpeed * 2.0f * Time.deltaTime);
            }

            //Player 2 icon check
            if (player2HasPressed)
            {
                buttonPTransform.localScale = Vector3.Lerp(buttonPTransform.localScale, Vector3.zero, transitionSpeed * 2.0f * Time.deltaTime);
                iconP2Transform.localScale = Vector3.Lerp(iconP2Transform.localScale, iconP2StartScale, transitionSpeed * Time.deltaTime);
            }
            else
            {
                buttonPTransform.localScale = Vector3.Lerp(buttonPTransform.localScale, buttonPStartScale, transitionSpeed * Time.deltaTime);
                iconP2Transform.localScale = Vector3.Lerp(iconP2Transform.localScale, Vector3.zero, transitionSpeed * 2.0f * Time.deltaTime);
            }
        }

        private void CheckBlackBar()
        {
            if (SceneController.isInCutscene == true || forceFadeIn == true)
            {
                blackBarTopTransform.localPosition = Vector3.Lerp(blackBarTopTransform.localPosition, blackBarTopStartPos, transitionSpeed * Time.deltaTime);
                blackBarBotTransform.localPosition = Vector3.Lerp(blackBarBotTransform.localPosition, blackBarBotStartPos, transitionSpeed * Time.deltaTime);
            }
            else
            {
                blackBarTopTransform.localPosition = Vector3.Lerp(blackBarTopTransform.localPosition, blackBarTopStartPos + (Vector3.up * blackBarHideOffset), transitionSpeed * Time.deltaTime);
                blackBarBotTransform.localPosition = Vector3.Lerp(blackBarBotTransform.localPosition, blackBarBotStartPos - (Vector3.up * blackBarHideOffset), transitionSpeed * Time.deltaTime);
            }
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

        public static void CutsceneButtonPress(int value)
        {
            switch(value)
            {
                case 1:
                    player1HasPressed = true;
                    break;

                case 2:
                    player2HasPressed = true;
                    break;

                default:
                    player1HasPressed = false;
                    player2HasPressed = false;
                    break;
            }
        }

        public static void ResetButtons()
        {
            CutsceneButtonPress(0);
            buttonVTransform.localScale = buttonVStartScale;
            buttonPTransform.localScale = buttonPStartScale;
            iconP1Transform.localScale = Vector3.zero;
            iconP2Transform.localScale = Vector3.zero;
        }
    }
}