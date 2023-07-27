using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class CameraTriggerBoss : Component
    {
        private Transform myTransform;
        private Vector3 stationPosition;
        private CameraBehaviour mainCamera;
        private bool hasInit;

        void Start()
        {
            if (this.entity.GetComponent<Renderer>() != null)
            {
                this.entity.GetComponent<Renderer>().active = false;
            }
            myTransform = this.entity.GetComponent<Transform>();
            myTransform.GetChildByIndex(0).entity.GetComponent<Renderer>().active = false;
            stationPosition = myTransform.GetChildByIndex(0).entity.GetComponent<Transform>().localPosition;
        }

        void Update()
        {
            if (hasInit == false)
            {
                mainCamera = SceneController.mainCamera.GetComponent<CameraBehaviour>();
                hasInit = true;
            }
        }

        void OnTriggerEnter(Entity collider)
        {
            if (collider.GetComponent<CameraPoint>() != null)
            {
                mainCamera.SetCustomLookAndFollow(true, myTransform.globalPosition, stationPosition);
            }
        }
    }
}