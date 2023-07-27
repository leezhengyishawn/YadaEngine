using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class TransitionTraverse : Component
    {
        private Transform myTransform;
        private Entity directionMarker;
        private Vector3 exitDirection;
        private Vector3 defaultColour;
        private Vector3 mangroveColour;
        private float transitionSpeed;
        private bool isEnteringMangrove;

        void Start()
        {
            myTransform = this.entity.GetComponent<Transform>();
            directionMarker = myTransform.GetChildByIndex(0).entity;
            this.entity.GetComponent<Renderer>().active = false;
            directionMarker.GetComponent<Renderer>().active = false;
            exitDirection = (directionMarker.GetComponent<Transform>().globalPosition - myTransform.globalPosition).normalized;
            defaultColour = SceneController.mainLightComponent.color;
            mangroveColour = new Vector3(6.0f, 9.0f, 100.0f)/255.0f;
            transitionSpeed = 1.0f;
        }

        void Update()
        {
            if (isEnteringMangrove == true)
            {
                //SceneController.mainLightComponent.color = Vector3.Lerp(SceneController.mainLightComponent.color, SceneController.defaultLightColour, transitionSpeed * Time.deltaTime);
                AudioController.isPlayingTransitionBGM = true;
            }
            else
            {
                //SceneController.mainLightComponent.color = Vector3.Lerp(SceneController.mainLightComponent.color, SceneController.defaultLightColour, transitionSpeed * Time.deltaTime);
                AudioController.isPlayingTransitionBGM = false;
            }
        }

        void OnTriggerExit(Entity collider)
        {
            if (collider.GetComponent<CameraPoint>() != null)
            {
                // Positive dot = facing towards the exit angle
                float dot = Vector3.Dot(exitDirection, (collider.GetComponent<Transform>().globalPosition - myTransform.globalPosition).normalized);
                if (dot > 0.0f)
                {
                    SceneController.defaultLightColour = mangroveColour;
                    isEnteringMangrove = true;
                }
                else
                {
                    SceneController.defaultLightColour = defaultColour;
                    isEnteringMangrove = false;
                }
            }
        }
    }
}