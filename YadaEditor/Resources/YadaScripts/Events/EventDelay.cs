using System;
using YadaScriptsLib;

namespace YadaScripts
{
    public class EventDelay : Component 
    {
        public int listeningEventID;
        public int relayEventID;
        public float delay = 3;
        private EventResponse eventRes;
        private EventTrigger eventTrig;

        void Start()
        {
            eventRes = this.entity.GetComponent<EventResponse>();
            eventTrig = this.entity.GetComponent<EventTrigger>();
        }

        void Update()
        {
            if (delay > 0 && eventRes.fired == true)
            {
                delay -= Time.deltaTime;
                if (delay <= 0)
                {
                    eventTrig.SetTrigger(true);
                }
            }
        }
    }
}
