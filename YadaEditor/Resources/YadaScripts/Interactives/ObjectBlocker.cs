using System;
using System.Collections;
using System.Collections.Generic;
using YadaScriptsLib;

namespace YadaScripts
{
    class ObjectBlocker : Component
    {
        private Entity objectBlockerParent;
        private EventResponse eventRes;
        private Transform myTransform;
        private Transform childTransform;
        private Vector3 startingPos;
        private Vector3 skyPos;
        private float speedBuildUp;
        private bool isFalling;
        private bool hasPlayedOnce;
        private bool hasShake;

        void Start()
        {
            eventRes = this.entity.GetComponent<EventResponse>();
            myTransform = this.entity.GetComponent<Transform>();
            objectBlockerParent = myTransform.GetChildByIndex(0).entity;
            childTransform = objectBlockerParent.GetComponent<Transform>();
            startingPos = childTransform.localPosition;
            skyPos = startingPos + (Vector3.up * 10.0f);
            childTransform.localPosition = skyPos;
            //objectBlockerParent.active = false;

            for (ulong i = 0; i < childTransform.childCount; i++)
            {
                childTransform.GetChildByIndex(i).entity.GetComponent<Renderer>().active = false;
            }
        }

        void Update()
        {
            CheckFire();
            CheckMovement();
        }

        private void CheckFire()
        {
            if (eventRes.fired == true && hasPlayedOnce == false)
            {
                //objectBlockerParent.active = true;
                for (ulong i = 0; i < childTransform.childCount; i++)
                {
                    childTransform.GetChildByIndex(i).entity.GetComponent<Renderer>().active = true;
                }
                isFalling = true;
                hasPlayedOnce = true;
            }
        }

        private void CheckMovement()
        {
            if (isFalling == true)
            {
                if (childTransform.localPosition != startingPos)
                {
                    speedBuildUp += 0.1f;
                    childTransform.localPosition = Vector3.MoveTowards(childTransform.localPosition, startingPos, 10.0f * speedBuildUp * Time.deltaTime);
                }
                else if (hasShake == false)
                {
                    SceneController.mainCamera.GetComponent<CameraBehaviour>().ShakeCamera(true);
                    hasShake = true;
                }
            }
        }
    }
}