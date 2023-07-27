using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class Scatter : Component
    {
        private float timerLifeTimerCurrent;
        private float timerLifetimeMaximum;

        void Start()
        {
            timerLifetimeMaximum = 1.0f;
        }

        void Update()
        {
            if (timerLifeTimerCurrent < timerLifetimeMaximum)
            {
                timerLifeTimerCurrent += Time.deltaTime;
            }
            else
            {
                Entity.DestroyEntity(this.entity);
            }
        }
    }
}