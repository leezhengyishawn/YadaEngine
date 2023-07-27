using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class ParticleDestroy : Component
    {
        public ParticleEmitter emitter;
        void Start()
        {
            emitter = this.entity.GetComponent<ParticleEmitter>();
        }

        void FixedUpdate()
        {
            if (!emitter.isPlaying)
                Entity.DestroyEntity(this.entity);
        }
    }
}
