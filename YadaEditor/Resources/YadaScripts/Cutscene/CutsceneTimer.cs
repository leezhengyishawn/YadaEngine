using System;
using System.Collections.Generic;
using YadaScriptsLib;

namespace YadaScripts
{
    class CutsceneTimer : Component
    {
        //Inside the station child object to detect the time to stay in that shot
        public float shotTiming = 1.0f;
        public bool freezeCamAngle;
        public bool freezeCamMovement;
        public bool teleportShot;

        public Entity specialObjectTriggerCondition;
    }
}