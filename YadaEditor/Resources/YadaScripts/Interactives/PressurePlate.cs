using System;
using YadaScriptsLib;

namespace YadaScripts
{
    public class PressurePlate : Component
    {
        private Transform myTransform;
        private Vector3 startingPos;
        private float steppedDownAmount;
        private bool stepped = false;

        void Start()
        {
            myTransform = this.entity.GetComponent<Transform>();
            startingPos = myTransform.localPosition;
            steppedDownAmount = -0.2f;
        }

        void Update()
        {
            if (stepped == true)
            {
                myTransform.localPosition = startingPos + (Vector3.up * steppedDownAmount);
            }
            else
            {
                myTransform.localPosition = startingPos;
            }
        }

        void OnTriggerStay(Entity collider)
        {
            GroundCheck touchedGroundCheck = collider.GetComponent<GroundCheck>();
            if (touchedGroundCheck != null && !stepped)
            {
                AudioController.PlaySFX("SFX Plate Trigger", AudioController.AudioVolume.VOLUME_50);
                this.entity.GetComponent<EventTrigger>().SetTrigger(true);
                stepped = true;
            }
        }

        void OnTriggerExit(Entity collider)
        {
            GroundCheck touchedGroundCheck = collider.GetComponent<GroundCheck>();
            if (touchedGroundCheck != null && stepped)
            {
                //AudioController.PlaySFX("SFX Plate Trigger");
                this.entity.GetComponent<EventTrigger>().SetTrigger(false);
                stepped = false;
            }
        }
    }
}