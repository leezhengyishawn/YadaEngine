using System;
using System.Collections.Generic;
using YadaScriptsLib;

namespace YadaScripts
{
    class ParticleManager : Component
    {
        public Entity emitterEntity1;
        public Entity emitterEntity2;
        public Entity emitterEntity3;

        private ParticleEmitter m_emitter1 = null;
        private ParticleEmitter m_emitter2 = null;
        private ParticleEmitter m_emitter3 = null;

        void Start()
        {
            // Bind emitters
            if (emitterEntity1)
            {
                m_emitter1 = emitterEntity1.GetComponent<ParticleEmitter>();
                m_emitter1.InitEmitter();
            }

            if (emitterEntity2)
            {
                m_emitter2 = emitterEntity2.GetComponent<ParticleEmitter>();
                m_emitter2.InitEmitter();
            }

            if (emitterEntity3)
            {
                m_emitter3 = emitterEntity3.GetComponent<ParticleEmitter>();
                m_emitter3.InitEmitter();
            }
        }

        void Update()
        {
            // Update iff particle emitters exist
            if (emitterEntity1)
                m_emitter1.UpdateEmitter();

            if (emitterEntity2)
                m_emitter2.UpdateEmitter();

            if (emitterEntity3)
                m_emitter3.UpdateEmitter();
        }
    }
}
