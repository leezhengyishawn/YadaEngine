using System;
using System.Collections.Generic;
using YadaScriptsLib;

namespace YadaScripts
{
    class CutsceneTrigger : Component
    {
        public Entity cameraPosFollow;
        public bool hasEndedDialogue;

        //Check dialogue for NPCs
        public MerchantBehaviour merchantBehaviour;
        public TransitionLevelTrigger transitionLevelTrigger;
        public Dialogue dialogueBehaviour;
        public Unlockable unlockableBehaviour;
        public CutsceneLevelTransition cutsceneLevelTransition;

        //Getting Player
        private PlayerBehaviour player1Behaviour;
        private PlayerBehaviour player2Behaviour;

        //Testing with timer to get out of dialogue
        public bool controlledByTime;
        private float cutsceneDurationCurrent;
        private float cutsceneDurationMaximum;
        private bool startTimer;

        //Skip and Next Button Timer
        private float delayDurationMaximum;
        private float delayDurationCurrent;
        private float showDurationMaximum;
        private float showDurationCurrent;

        //Timer Checking
        private bool player1ReadyNext;
        private bool player2ReadyNext;
        private bool player1ReadySkip;
        private bool player2ReadySkip;

        //List of child
        private List<Entity> listOfLookObjects = new List<Entity>();
        private List<Entity> listOfStationObjects = new List<Entity>();
        private CameraBehaviour mainCamera;
        private int shotValue;
        private bool hasBeenTriggered;

        //Check Cutscene
        private bool cutsceneActivated;
        private bool hasResetButtons;

        private Quaternion startingRotation;
        private Entity enteredPlayer;
        private bool allowButtonPress;
        private bool isFrozenCam;

        public bool ignoreThisTrigger;

        private void Start()
        {
            delayDurationMaximum = 0.5f;
            showDurationMaximum = 1.0f;
            if (this.entity.GetComponent<Renderer>() != null)
            {
                this.entity.GetComponent<Renderer>().active = false;
            }
            for (int i = 0; i < (int)this.entity.GetComponent<Transform>().childCount; i++)
            {
                if (this.entity.GetComponent<Transform>().GetChildByIndex((ulong)i).entity != cameraPosFollow)
                {
                    if (this.entity.GetComponent<Transform>().GetChildByIndex((ulong)i).entity.GetComponent<Renderer>() != null)
                    {
                        this.entity.GetComponent<Transform>().GetChildByIndex((ulong)i).entity.GetComponent<Renderer>().active = false;
                    }
                    //Odd = Look. Even = Station.
                    if (i % 2 == 0) //Check if even number
                    {
                        //Add to look list
                        listOfStationObjects.Add(this.entity.GetComponent<Transform>().GetChildByIndex((ulong)i).entity);
                    }
                    else
                    {
                        //Add to station list
                        listOfLookObjects.Add(this.entity.GetComponent<Transform>().GetChildByIndex((ulong)i).entity);
                    }
                }
            }
            merchantBehaviour = this.entity.GetComponent<MerchantBehaviour>();
            transitionLevelTrigger = this.entity.GetComponent<TransitionLevelTrigger>();
            dialogueBehaviour = this.entity.GetComponent<Dialogue>();
            unlockableBehaviour = this.entity.GetComponent<Unlockable>();
            startingRotation = cameraPosFollow.GetComponent<Transform>().localRotation;
            cutsceneLevelTransition = this.entity.GetComponent<CutsceneLevelTransition>();
        }

        private void Update()
        {
            GetSceneObjects();
            if (cutsceneActivated)
            {
                CheckFadeTimer();
                if (hasResetButtons == false)
                {
                    CutsceneButton.ResetButtons();
                    hasResetButtons = true;
                }
                if (controlledByTime == true)
                {
                    CutsceneButton.isShowingSkip = true;
                    CheckSkipButton();
                    CheckTimer();
                }
                else
                {
                    CutsceneButton.isShowingSkip = false;
                    CheckNextButton();
                }
                CheckButtonStatus();
            }
            MoveObjectRotation();
        }

        private void CheckFadeTimer()
        {
            if (merchantBehaviour == null)
            {
                if (showDurationCurrent < showDurationMaximum)
                {
                    showDurationCurrent += Time.deltaTime;
                }
                else
                {
                    SetCanPressButtons();
                }
            }
        }

        private void MoveObjectRotation()
        {
            if (cutsceneActivated && enteredPlayer != null)
            {
                Transform npcTransform = cameraPosFollow.GetComponent<Transform>();
                Quaternion quat = Quaternion.LookAt(npcTransform.globalPosition, enteredPlayer.GetComponent<Transform>().globalPosition);
                Quaternion newRotation = Quaternion.RotateTowards(npcTransform.localRotation, quat, 5.0f);
                npcTransform.localRotation = newRotation;
            }
            else
            {
                Transform npcTransform = cameraPosFollow.GetComponent<Transform>();
                Quaternion newRotation = Quaternion.RotateTowards(npcTransform.localRotation, startingRotation, 5.0f);
                npcTransform.localRotation = newRotation;
            }
        }

        private void GetSceneObjects()
        {
            if (mainCamera == null)
            {
                mainCamera = SceneController.mainCamera.GetComponent<CameraBehaviour>();
                player1Behaviour = SceneController.mainPlayer1.GetComponent<PlayerBehaviour>();
                player2Behaviour = SceneController.mainPlayer2.GetComponent<PlayerBehaviour>();
            }
        }

        private void CheckNextButton()
        {
            if (allowButtonPress)
            {
                //Player 1 Next
                if (Input.GetGamepadButtonPress(GAMEPADCODE.GAMEPAD_B, player1Behaviour.gameID) || Input.GetKeyPress(KEYCODE.KEY_V))
                {
                    if (player1ReadyNext == false)
                    {
                        AudioController.PlaySFX("SFX Button Cutscene Press L");
                        CutsceneButton.CutsceneButtonPress(1);
                        player1ReadyNext = true;
                    }

                }
                //Player 2 Next
                if (Input.GetGamepadButtonPress(GAMEPADCODE.GAMEPAD_B, player2Behaviour.gameID) || Input.GetKeyPress(KEYCODE.KEY_P))
                {
                    if (player2ReadyNext == false)
                    {
                        AudioController.PlaySFX("SFX Button Cutscene Press L");
                        CutsceneButton.CutsceneButtonPress(2);
                        player2ReadyNext = true;
                    }
                }
            }
        }

        private void CheckSkipButton()
        {
            if (allowButtonPress)
            {
                //Player 1 Skip
                if (Input.GetGamepadButtonPress(GAMEPADCODE.GAMEPAD_B, player1Behaviour.gameID) || Input.GetKeyPress(KEYCODE.KEY_V))
                {
                    if (player1ReadySkip == false)
                    {
                        AudioController.PlaySFX("SFX Button Cutscene Press L");
                        CutsceneButton.CutsceneButtonPress(1);
                        player1ReadySkip = true;
                    }
                }
                //Player 2 Skip
                if (Input.GetGamepadButtonPress(GAMEPADCODE.GAMEPAD_B, player2Behaviour.gameID) || Input.GetKeyPress(KEYCODE.KEY_P))
                {
                    if (player2ReadySkip == false)
                    {
                        AudioController.PlaySFX("SFX Button Cutscene Press L");
                        CutsceneButton.CutsceneButtonPress(2);
                        player2ReadySkip = true;
                    }
                }
            }
        }

        private void CheckButtonStatus()
        {
            //Activate
            if (player1ReadyNext && player2ReadyNext)
            {
                if (delayDurationCurrent < delayDurationMaximum)
                {
                    delayDurationCurrent += Time.deltaTime;
                }
                else
                {
                    if (shotValue < listOfLookObjects.Count)
                    {
                        AudioController.PlaySFX("SFX Button Next page Fast");
                        SetNextShot();
                    }
                    else
                    {
                        AudioController.PlaySFX("SFX Button End page Fast");
                        ExitCutscene();
                    }
                }
            }
            //Activate
            if (player1ReadySkip && player2ReadySkip)
            {
                if (delayDurationCurrent < delayDurationMaximum)
                {
                    delayDurationCurrent += Time.deltaTime;
                }
                else
                {
                    AudioController.PlaySFX("SFX Button End page Fast");
                    ExitCutscene();
                }
            }
        }

        private void CheckTimer()
        {
            if (startTimer == true && (mainCamera.HasReachedStation() == true || isFrozenCam == true))
            {
                if (cutsceneDurationCurrent < cutsceneDurationMaximum)
                {
                    cutsceneDurationCurrent += Time.deltaTime;
                }
                else
                {
                    if (shotValue < listOfLookObjects.Count)
                    {
                        SetNextShot();
                    }
                    else
                    {
                        ExitCutscene();
                    }
                }
            }
        }

        private void SetNextShot()
        {
            if (controlledByTime == false)
            {
                CutsceneButton.CutsceneButtonPress(0);
            }
            CutsceneTimer currentCutsceneShot = listOfStationObjects[shotValue].GetComponent<CutsceneTimer>();
            if (currentCutsceneShot != null)
            {
                mainCamera.EnterCutscenePosition(cameraPosFollow.GetComponent<Transform>().globalPosition, 
                                                 listOfLookObjects[shotValue].GetComponent<Transform>().globalPosition, 
                                                 listOfStationObjects[shotValue].GetComponent<Transform>().localPosition, 
                                                 currentCutsceneShot.freezeCamAngle, currentCutsceneShot.freezeCamMovement, 
                                                 currentCutsceneShot.teleportShot);
                cutsceneDurationMaximum = currentCutsceneShot.shotTiming;
                isFrozenCam = currentCutsceneShot.freezeCamMovement;

                if (currentCutsceneShot.specialObjectTriggerCondition != null)
                {
                    if (currentCutsceneShot.specialObjectTriggerCondition.GetComponent<LemonBehaviour>() != null)
                    {
                        currentCutsceneShot.specialObjectTriggerCondition.GetComponent<LemonBehaviour>().PlayAnimation();
                    }
                }
            }
            else
            {
                mainCamera.EnterCutscenePosition(cameraPosFollow.GetComponent<Transform>().globalPosition, 
                                                 listOfLookObjects[shotValue].GetComponent<Transform>().globalPosition, 
                                                 listOfStationObjects[shotValue].GetComponent<Transform>().localPosition, false, false, false);
                cutsceneDurationMaximum = 1.0f;
                isFrozenCam = false;
            }
            player1ReadyNext = false;
            player2ReadyNext = false;
            delayDurationCurrent = 0.0f;
            cutsceneDurationCurrent = 0.0f;
            if (merchantBehaviour == null)
            {
                CheckDialogue(true);
            }
        }

        private void ExitCutscene()
        {
            if (SceneController.isTransitioningLevel == false)
            {
                if (transitionLevelTrigger != null || cutsceneLevelTransition != null)
                {
                    SceneController.isTransitioningLevel = true;
                }
                CheckDialogue(false);
                SceneController.mainPlayer1.GetComponent<PlayerBehaviour>().SetLookAt(null);
                SceneController.mainPlayer2.GetComponent<PlayerBehaviour>().SetLookAt(null);
                cutsceneDurationCurrent = 0.0f;
                SceneController.SetCinematics(false, false, true);
                mainCamera.ExitCutscenePosition();
                startTimer = false;
                cutsceneActivated = false;
                CutsceneButton.allowButtonFadeIn = false;
                isFrozenCam = false;
                hasEndedDialogue = true;
            }
        }

        public void CheckDialogue(bool turnOn)
        {
            if (dialogueBehaviour != null)
            {
                if (turnOn == true)
                {
                    SceneController.isPromptOpen = true;
                    dialogueBehaviour.dialogueIsShowing = true;
                    dialogueBehaviour.SetDialogueText(shotValue);
                }
                else
                {
                    dialogueBehaviour.dialogueIsShowing = false;
                    if (unlockableBehaviour == null)
                    {
                        SceneController.isPromptOpen = false;
                    }
                    else
                    {
                        unlockableBehaviour.triggered = true;
                    }
                }
            }
            ++shotValue;
        }

        void OnTriggerEnter(Entity collider)
        {
            if (ignoreThisTrigger == false)
            {
                PlayerBehaviour playerBehaviour = collider.GetComponent<PlayerBehaviour>();
                if (hasBeenTriggered == false)
                {
                    if ((merchantBehaviour == null && playerBehaviour != null && playerBehaviour.isInRespawnSequence == false) || (merchantBehaviour != null && collider.GetComponent<CameraPoint>() != null))
                    {
                        if (merchantBehaviour != null)
                        {
                            merchantBehaviour.allowMerchantMovement = true;
                        }
                        enteredPlayer = collider;
                        cutsceneActivated = true;
                        SetNextShot();
                        if (controlledByTime == true)
                        {
                            SceneController.SetCinematics(true, false, false);
                            SceneController.mainPlayer1.GetComponent<PlayerBehaviour>().SetLookAt(null);
                            SceneController.mainPlayer2.GetComponent<PlayerBehaviour>().SetLookAt(null);
                        }
                        else
                        {
                            SceneController.SetCinematics(true, true, false);
                            SceneController.mainPlayer1.GetComponent<PlayerBehaviour>().SetLookAt(cameraPosFollow.GetComponent<Transform>());
                            SceneController.mainPlayer2.GetComponent<PlayerBehaviour>().SetLookAt(cameraPosFollow.GetComponent<Transform>());
                        }
                        hasBeenTriggered = true;
                        startTimer = true;
                    }
                }
            }
        }

        //This triggers the same cutscene code as though the player has stepped in it.
        public void ManuallyTriggerCutscene()
        {
            cutsceneActivated = true;
            SetNextShot();
            if (controlledByTime == true)
            {
                SceneController.SetCinematics(true, false, false);
                SceneController.mainPlayer1.GetComponent<PlayerBehaviour>().SetLookAt(null);
                SceneController.mainPlayer2.GetComponent<PlayerBehaviour>().SetLookAt(null);
            }
            else
            {
                SceneController.SetCinematics(true, true, false);
                SceneController.mainPlayer1.GetComponent<PlayerBehaviour>().SetLookAt(null);
                SceneController.mainPlayer2.GetComponent<PlayerBehaviour>().SetLookAt(null);
            }
            startTimer = true;
        }

        private float GetDistance(Vector3 point1, Vector3 point2)
        {
            return (point1 - point2).magnitudeSq;
        }

        public void SetCanPressButtons()
        {
            allowButtonPress = true;
            CutsceneButton.allowButtonFadeIn = true;
        }
    }
}