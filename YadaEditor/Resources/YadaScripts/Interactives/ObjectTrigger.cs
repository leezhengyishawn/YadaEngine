using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class ObjectTrigger : Component
    {
        private Transform myTransform;
        private Entity directionMarker;
        private Vector3 exitDirection;
        private bool player1Crossed;
        private bool player2Crossed;
        private bool hasActivated;

        void Start()
        {
            myTransform = this.entity.GetComponent<Transform>();
            directionMarker = myTransform.GetChildByIndex(0).entity;
            this.entity.GetComponent<Renderer>().active = false;
            directionMarker.GetComponent<Renderer>().active = false;
            exitDirection = (directionMarker.GetComponent<Transform>().globalPosition - myTransform.globalPosition).normalized;
        }

        void Update()
        {
            if (hasActivated == false && player1Crossed && player2Crossed)
            {
                this.entity.GetComponent<EventTrigger>().SetTrigger(true);
                hasActivated = true;
            }
        }

        void OnTriggerEnter(Entity collider)
        {
            if (hasActivated == false && (collider == SceneController.mainPlayer1 || collider == SceneController.mainPlayer2))
            {
                if (collider == SceneController.mainPlayer1)
                {
                    player1Crossed = false;
                }
                else if (collider = SceneController.mainPlayer2)
                {
                    player2Crossed = false;
                }
            }
        }

        void OnTriggerExit(Entity collider)
        {
            if (hasActivated == false && (collider == SceneController.mainPlayer1 || collider == SceneController.mainPlayer2))
            {
                // Positive dot = facing towards the exit angle
                float dot = Vector3.Dot(exitDirection, (collider.GetComponent<Transform>().globalPosition - myTransform.globalPosition).normalized);
                if (dot > 0.2f)
                {
                    if (collider == SceneController.mainPlayer1)
                    {
                        player1Crossed = true;
                    }
                    else if (collider = SceneController.mainPlayer2)
                    {
                        player2Crossed = true;
                    }
                }
            }
        }
    }
}