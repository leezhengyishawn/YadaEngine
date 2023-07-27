using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using YadaScriptsLib;

namespace YadaScripts
{
    public class EventListener : Component
    {
        
        public Entity[] triggerEntities;
        public Entity[] responseEntities;
        //public int[] triggerResponseCount; //How many triggers are needed to trigger this response, the array index is equal to the responseID
        public ConcurrentDictionary<int, int> triggerResponseCount;

        public int TEs;
        public int REs;

        void Start()
        {
            //get all entities with trigger and responses
            triggerEntities = Entity.GetEntitiesWithComponent<EventTrigger>();
            responseEntities = Entity.GetEntitiesWithComponent<EventResponse>();
            triggerResponseCount = new ConcurrentDictionary<int, int>(); //(eventID, triggerCount)

            TEs = triggerEntities.Length;
            REs = responseEntities.Length;

            //get all response event IDs - set to key
            for (int i = 0; i < responseEntities.Length; ++i)
            {
                triggerResponseCount.TryAdd(responseEntities[i].GetComponent<EventResponse>().eventID, 0);
            }

            //get all trigger event IDs - count in value
            foreach (KeyValuePair<int, int> res in triggerResponseCount)
            {
                for (int j = 0; j < triggerEntities.Length; ++j)
                {
                    if (triggerEntities[j].GetComponent<EventTrigger>().eventID == res.Key)
                        triggerResponseCount[res.Key]++;
                }
            }
        }

        public void TriggerUpdate(int id)
        {
            if (triggerResponseCount.ContainsKey(id))
            {
                triggerResponseCount[id]--;
                
                if (triggerResponseCount[id] > 0)
                {
                    for (int j = 0; j < responseEntities.Length; ++j)
                    {
                        if (responseEntities[j].GetComponent<EventResponse>().eventID == id)
                        {
                            responseEntities[j].GetComponent<EventResponse>().FireIntermediateEvent();
                        }
                    }
                }
            }

            foreach (KeyValuePair<int, int> res in triggerResponseCount)
            {
                if (res.Value == 0)
                {
                    for (int j = 0; j < responseEntities.Length; ++j)
                    {
                        if (responseEntities[j].GetComponent<EventResponse>().eventID == res.Key)
                        {
                            responseEntities[j].GetComponent<EventResponse>().FireResponseEvent();
                            triggerResponseCount[res.Key]--; //may change this
                        }
                    }
                }
                
            }
        }

        public void TriggerReverseUpdate(int id)
        {
            if (triggerResponseCount.ContainsKey(id))
            {
                triggerResponseCount[id]++;
            }
        }
    }
}
