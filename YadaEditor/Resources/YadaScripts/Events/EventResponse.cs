using System;
using YadaScriptsLib;

namespace YadaScripts
{
    public class EventResponse : Component
    {
        public int eventID;
        public Entity self;
        public bool fired;
        public bool intermediateFired; //for intermediate triggers

        //interface for various response types


        public void Start()
        {
            self = this.entity;
            fired = false;
            intermediateFired = false;
        }


        public virtual void SetResponseEvent(bool eventBool)
        {
            fired = eventBool;
        }

        public virtual void FireResponseEvent()
        {
            fired = true;
        }

        public virtual void FireIntermediateEvent()
        {
            intermediateFired = true;
        }

        public virtual void SetIntermediateEvent(bool eventBool)
        {
            intermediateFired = eventBool;
        }
    }
}
