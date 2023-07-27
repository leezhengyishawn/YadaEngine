using System;
using YadaScriptsLib;

namespace YadaScripts
{
    public class EventTrigger : Component
    {
        public int eventID = 0;
        private bool triggerBool = false;

        public EventListener listener;

        void Start()
        {
            //There should only be one event listener
            listener = Entity.GetEntitiesWithComponent<EventListener>()[0].GetComponent<EventListener>();
        }

        public bool GetTrigger()
        {
            return triggerBool;
        }
        
        public void SetTrigger(bool set)
        {
            triggerBool = set;

            if (set)
                listener.TriggerUpdate(eventID);
            else
                listener.TriggerReverseUpdate(eventID);
        }
    }
}
