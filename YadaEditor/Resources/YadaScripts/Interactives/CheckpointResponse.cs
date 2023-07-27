using System;
using System.Collections.Generic;
using YadaScriptsLib;

namespace YadaScripts
{
    class CheckpointResponse : Component
    {
        public Entity[] triggerEntities;
        public List<Entity> triggerEntitiesList;

        private EventResponse eventRes;

        void Start()
        {
            eventRes = this.entity.GetComponent<EventResponse>();
            triggerEntities = Entity.GetEntitiesWithComponent<EventTrigger>();
        }

        void Update()
        {
            if (eventRes.fired == true)
            {
                List<Entity> listOfTriggers = GetTrigger(eventRes.eventID);
                if (listOfTriggers.Count > 1)
                {
                    SceneController.mainPlayer1.GetComponent<PlayerBehaviour>().SetSavePoint(listOfTriggers[0].GetComponent<Transform>().globalPosition + (Vector3.up * SceneController.clawMachineHeight));
                    SceneController.mainPlayer2.GetComponent<PlayerBehaviour>().SetSavePoint(listOfTriggers[1].GetComponent<Transform>().globalPosition + (Vector3.up * SceneController.clawMachineHeight));
                }
                this.active = false;
            }
        }

        public List<Entity> GetTrigger(int id)
        {
            List<Entity> triggerEntitiesList = new List<Entity>();
            for (int i = 0; i < triggerEntities.Length; i++)
            {
                if (triggerEntities[i].GetComponent<EventTrigger>().eventID == id)
                {
                    triggerEntitiesList.Add(triggerEntities[i]);
                }
            }
            return triggerEntitiesList;
        }
    }
}