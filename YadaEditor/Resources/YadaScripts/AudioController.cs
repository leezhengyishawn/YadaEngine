using System;
using System.Collections.Generic;
using YadaScriptsLib;

namespace YadaScripts
{
    public enum SFXType
    {
        MELON_DAMAGED = 0,
        MELON_FARFROMPLAYER,
        MELON_FRIENDLYDAMAGED,
        MELON_OI,
        PINEAPPLE_DAMAGED,
        PINEAPPLE_FARFROMPLAYER,
        PINEAPPLE_FRIENDLYDAMAGED,
        PINEAPPLE_OI,
        PLAYER_CHARGEDPUNCH,
        EGGPLANT_DAMAGED,
        ENEMY_DAMAGED,
        ONION_FART,
        PLAYER_PUNCH,
    }

    public class AudioController : Component
    {
        private static Random randomizer;

        public Entity normalLevelBGMObject;
        public Entity combatLevelBGMObject;
        public Entity shopBGMObject;
        public Entity cutsceneBGMObject;
        public Entity transitionBGMObject;

        public static AudioSource normalLevelBGM;
        public static AudioSource combatLevelBGM;
        public static AudioSource shopBGM;
        public static AudioSource cutsceneBGM;
        public static AudioSource transitionBGM;

        public static int enemyDetectCount;

        private List<AudioSource> bgmAudioList = new List<AudioSource>();
        private bool hasStartedCombatBGM;
        public static bool isPlayingTransitionBGM;

        private int bgmStateValue;
        private float bgmMaxVolume;
        private float bgmFadeInSpeed;
        private float bgmFadeOutSpeed;
        public bool isNotInGameplay;
        public bool isTestingBGM;

        private bool fadeAllBGM;


        public enum AudioVolume
        {
            VOLUME_25,
            VOLUME_50,
            VOLUME_75,
            VOLUME_100,
            VOLUME_150,
            VOLUME_200,
            VOLUME_250,
            VOLUME_300,
        }

        private static AudioSource audioSource25;
        private static AudioSource audioSource50;
        private static AudioSource audioSource75;
        private static AudioSource audioSource100;
        private static AudioSource audioSource150;
        private static AudioSource audioSource200;
        private static AudioSource audioSource250;
        private static AudioSource audioSource300;

        public enum BGMState
        {
            NORMAL,
            COMBAT,
            SHOP,
            CUTSCENE,
            TRANSITION
        }

        void Start()
        {
            randomizer = new Random();

            enemyDetectCount = 0;

            normalLevelBGM = normalLevelBGMObject.GetComponent<AudioSource>();
            combatLevelBGM = combatLevelBGMObject.GetComponent<AudioSource>();
            shopBGM = shopBGMObject.GetComponent<AudioSource>();
            cutsceneBGM = cutsceneBGMObject.GetComponent<AudioSource>();
            transitionBGM = transitionBGMObject.GetComponent<AudioSource>();

            bgmAudioList.Add(normalLevelBGM); //0
            bgmAudioList.Add(combatLevelBGM); //1
            bgmAudioList.Add(shopBGM); //2
            bgmAudioList.Add(cutsceneBGM); //3
            bgmAudioList.Add(transitionBGM); //4

            bgmMaxVolume = 1.3f;

            audioSource25 = this.entity.AddComponent<AudioSource>();
            audioSource50 = this.entity.AddComponent<AudioSource>();
            audioSource75 = this.entity.AddComponent<AudioSource>();
            audioSource100 = this.entity.AddComponent<AudioSource>();
            audioSource150 = this.entity.AddComponent<AudioSource>();
            audioSource200 = this.entity.AddComponent<AudioSource>();
            audioSource250 = this.entity.AddComponent<AudioSource>();
            audioSource300 = this.entity.AddComponent<AudioSource>();

            audioSource25.isPlayedOnStart = false;
            audioSource50.isPlayedOnStart = false;
            audioSource75.isPlayedOnStart = false;
            audioSource100.isPlayedOnStart = false;
            audioSource150.isPlayedOnStart = false;
            audioSource200.isPlayedOnStart = false;
            audioSource250.isPlayedOnStart = false;
            audioSource300.isPlayedOnStart = false;

            isPlayingTransitionBGM = false;

            bgmFadeInSpeed = 0.5f; // halfs the speed       (Sound) x bgmfadeinspeed
            bgmFadeOutSpeed = 0.5f;

            // Load the master volume, override the scene's master volume (if available)
            File.ReadJsonFile("tempSave");
            if (File.CheckDataExists("MasterVolume"))
                Audio.masterVolume = File.ReadDataAsFloat("MasterVolume");
        }

        void Update()
        {
            //BGM Test
            //if (Input.GetKeyPress(KEYCODE.KEY_CONTROL))
            //{
            //    isTestingBGM = !isTestingBGM;
            //}

            if (isTestingBGM == true)
            {
                TestInput();
            }
            else
            {
                CheckBGMState();
            }
            CheckBGMTransition();
        }

        private void TestInput()
        {
            if (Input.GetKeyPress(KEYCODE.KEY_6))
            {
                SetBGMState(BGMState.NORMAL);
            }
            else if (Input.GetKeyPress(KEYCODE.KEY_7))
            {
                SetBGMState(BGMState.COMBAT);
            }
            else if (Input.GetKeyPress(KEYCODE.KEY_8))
            {
                SetBGMState(BGMState.SHOP);
            }
            else if (Input.GetKeyPress(KEYCODE.KEY_9))
            {
                SetBGMState(BGMState.CUTSCENE);
            }
        }

        private void CheckBGMState()
        {
            if (SceneController.isTransitioningLevel == true)
            {
                fadeAllBGM = true;
            }
            else if (SceneController.isInMerchant == true)
            {
                SetBGMState(BGMState.SHOP);
            }
            else if (SceneController.isInCutscene == true && SceneController.allowCutsceneBGM == true)
            {
                SetBGMState(BGMState.CUTSCENE);
            }
            else if (enemyDetectCount > 0)
            {
                SceneController.currentLightColour = SceneController.combatLightColour;
                SetBGMState(BGMState.COMBAT);
                if (hasStartedCombatBGM == false)
                {
                    PlaySFX("SFX Combat Start");
                    hasStartedCombatBGM = true;
                }
            }
            else
            {
                SceneController.currentLightColour = SceneController.defaultLightColour;
                if (isPlayingTransitionBGM == true)
                {
                    SetBGMState(BGMState.TRANSITION);
                }
                else
                {
                    SetBGMState(BGMState.NORMAL);
                }
                if (hasStartedCombatBGM == true)
                {
                    PlaySFX("SFX Combat End", AudioVolume.VOLUME_200);
                    hasStartedCombatBGM = false;
                }
            }
        }

        private void CheckBGMTransition()
        {
            for (int i = 0; i < bgmAudioList.Count; i++)
            {
                if ((i == bgmStateValue && isNotInGameplay == false) && (fadeAllBGM == false))
                {
                    if (bgmAudioList[i].volume < (i == (int)BGMState.COMBAT ? bgmMaxVolume / 1.1f : bgmMaxVolume))
                    {
                        bgmAudioList[i].volume += bgmFadeInSpeed * (i == (int)BGMState.COMBAT ? bgmMaxVolume / 1.1f : bgmMaxVolume) * Time.deltaTime;
                    }
                    else
                    {
                        bgmAudioList[i].volume = (i == (int)BGMState.COMBAT ? bgmMaxVolume / 1.1f : bgmMaxVolume);
                    }
                }
                else
                {
                    if (bgmAudioList[i].volume > 0.0f)
                    {
                        bgmAudioList[i].volume -= bgmFadeOutSpeed * bgmMaxVolume * Time.deltaTime;
                    }
                    else
                    {
                        bgmAudioList[i].volume = 0.0f;
                    }
                }
            }
        }

        private void SetBGMState(BGMState bgm)
        {
            bgmStateValue = (int)bgm;
        }

        public static void PlaySFX(string fileName, AudioVolume volume = AudioVolume.VOLUME_100)
        {
            SetAudioVolume(fileName, volume);
        }

        public static void PlaySFX(SFXType audioType, AudioVolume volume = AudioVolume.VOLUME_100)
        {
            string SFXFile = " ";
            int randNum = 0;
            switch (audioType)
            {
                case SFXType.MELON_DAMAGED:
                    SFXFile = "Melon_Damaged_";
                    randNum = randomizer.Next(1, 4); //1 - 3
                    break;
                case SFXType.MELON_FARFROMPLAYER:
                    SFXFile = "Melon_FarfromPlayer_";
                    randNum = randomizer.Next(1, 3); //1 - 2
                    break;
                case SFXType.MELON_FRIENDLYDAMAGED:
                    SFXFile = "Melon_FriendlyDamaged_";
                    randNum = randomizer.Next(1, 3); //1 - 2
                    break;
                case SFXType.MELON_OI:
                    SFXFile = "Melon_Oi_";
                    randNum = randomizer.Next(1, 3); //1 - 2
                    break;
                case SFXType.PINEAPPLE_DAMAGED:
                    SFXFile = "Pineapple_Damaged_";
                    randNum = randomizer.Next(1, 4); //1 - 3
                    break;
                case SFXType.PINEAPPLE_FARFROMPLAYER:
                    SFXFile = "Pineapple_FarfromPlayer_";
                    randNum = randomizer.Next(1, 3); //1 - 2
                    break;
                case SFXType.PINEAPPLE_FRIENDLYDAMAGED:
                    SFXFile = "Pineapple_FriendlyDamaged_";
                    randNum = randomizer.Next(1, 4); //1 - 3
                    break;
                case SFXType.PINEAPPLE_OI:
                    SFXFile = "Pineapple_Oi_";
                    randNum = randomizer.Next(1, 3); //1 - 2
                    break;
                case SFXType.PLAYER_CHARGEDPUNCH:
                    SFXFile = "SFX Player Charge Punch ";
                    randNum = randomizer.Next(1, 5); //1 - 4
                    break;
                case SFXType.EGGPLANT_DAMAGED:
                    SFXFile = "Eggplant_Damaged";
                    randNum = randomizer.Next(1, 3); //1 - 2
                    break;
                case SFXType.ENEMY_DAMAGED:
                    SFXFile = "SFX Enemy Injure ";
                    randNum = randomizer.Next(1, 4); //1 - 3
                    break;
                case SFXType.ONION_FART:
                    SFXFile = "SFX Fart ";
                    randNum = randomizer.Next(1, 4); //1 - 3
                    break;
                case SFXType.PLAYER_PUNCH:
                    SFXFile = "SFX Player Punch ";
                    randNum = randomizer.Next(1, 4); //1 - 3
                    break;
                    
            }

            SFXFile += randNum.ToString();
            SetAudioVolume(SFXFile, volume);
        }

        private static void SetAudioVolume(string fileName, AudioVolume volume)
        {
            switch (volume)
            {
                case AudioVolume.VOLUME_25:
                    AssignAudioSource(fileName, audioSource25, 0.25f);
                    break;

                case AudioVolume.VOLUME_50:
                    AssignAudioSource(fileName, audioSource50, 0.5f);
                    break;

                case AudioVolume.VOLUME_75:
                    AssignAudioSource(fileName, audioSource75, 0.75f);
                    break;

                case AudioVolume.VOLUME_100:
                    AssignAudioSource(fileName, audioSource100, 1.0f);
                    break;

                case AudioVolume.VOLUME_150:
                    AssignAudioSource(fileName, audioSource150, 1.5f);
                    break;

                case AudioVolume.VOLUME_200:
                    AssignAudioSource(fileName, audioSource200, 2.0f);
                    break;

                case AudioVolume.VOLUME_250:
                    AssignAudioSource(fileName, audioSource250, 2.5f);
                    break;

                case AudioVolume.VOLUME_300:
                    AssignAudioSource(fileName, audioSource300, 3.0f);
                    break;
            }
        }

        private static void AssignAudioSource(string fileName, AudioSource audioSource, float volume)
        {
            audioSource.sourceName = fileName;
            audioSource.volume = volume;
            Audio.PlaySource(audioSource);
        }
    }
}