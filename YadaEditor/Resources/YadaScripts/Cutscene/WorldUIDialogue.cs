using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class WorldUIDialogue : Component
    {
        public Entity worldDialogue;

        void OnTriggerEnter(Entity collider)
        {
            if(collider == SceneController.mainPlayer1 || collider == SceneController.mainPlayer2)
            worldDialogue.GetComponent<PopUp>().SpritePopUp(new Vector3(0f, 2f, 0f), new Vector3(2f, 1.8f, 0.0001f), 5.0f);
        }

    }
}
