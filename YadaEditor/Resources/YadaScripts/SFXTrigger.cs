using System;
using System.Collections.Generic;
using YadaScriptsLib;

namespace YadaScripts
{
    class SFXTrigger : Component
    {
        public bool b_useSFXEntity; // Refers to an entity in the scene with an audioSource (be it 2D or 3D)
        public bool b_usePrefabSFXEntity; // Prefab will get deleted right after it plays (so dont play bgm here cuz u cant pause it)

        public bool b_useTriggerEnter;
        public bool b_useTriggerExit;

        public bool b_triggerOnce;
        public bool b_triggerOncePerPlayer;
        public bool b_stopSFX; // Use this to stop the referenced entity 

        public Entity sfxEntity;
        public int i_PrefabIndex;
        public List<string> prefabName = new List<string>();

        private AudioSource m_sfx;
        private bool b_player1Trigger = false;
        private bool b_player2Trigger = false;

        void Start()
        {
            if (b_useSFXEntity && sfxEntity) // Check for valid entity
                m_sfx = sfxEntity.GetComponent<AudioSource>(); // No need to init as settings are in the entity

            // Append the list of prefab names here
            // Change the index in the editor
            //prefabName.Add("TestAudioPrefab");
        }

        void OnTriggerEnter(Entity collider)
        {
            if (b_useTriggerEnter)
                TriggerSFX(collider);
        }

        void OnTriggerExit(Entity collider)
        {
            if (b_useTriggerExit)
                TriggerSFX(collider);
        }
        private void TriggerSFX(Entity collider)
        {
            if (b_triggerOnce)
            {
                if (b_useSFXEntity)
                {
                    if (b_stopSFX)
                        Audio.StopSource(m_sfx.channel);
                    else
                        Audio.PlaySource(m_sfx);
                }

                if (b_usePrefabSFXEntity)
                    SpawnSFXPrefab();

                this.active = false;
            }
            else if (b_triggerOncePerPlayer)
            {
                if (collider.name == "PlayerCharacter1")
                {
                    if (b_usePrefabSFXEntity)
                        SpawnSFXPrefab();

                    b_player1Trigger = true;
                }

                if (collider.name == "PlayerCharacter2")
                {
                    if (b_usePrefabSFXEntity)
                        SpawnSFXPrefab();

                    b_player2Trigger = true;
                }

                if (b_player1Trigger && b_player2Trigger)
                    this.active = false;
            }
            else // Can be triggerred multiple times
            {
                if (b_useSFXEntity)
                {
                    if (b_stopSFX)
                        Audio.StopSource(m_sfx.channel);
                    else
                        Audio.PlaySource(m_sfx);
                }

                if (b_usePrefabSFXEntity)
                    SpawnSFXPrefab();
            }
        }

        private void SpawnSFXPrefab()
        {
            sfxEntity = Entity.InstantiatePrefab(prefabName[i_PrefabIndex]);
            m_sfx = sfxEntity.GetComponent<AudioSource>();
            Audio.PlaySource(m_sfx);

            // Dont overpopulate the hierarchy
            Entity.DestroyEntity(sfxEntity);
        }
    }
}
