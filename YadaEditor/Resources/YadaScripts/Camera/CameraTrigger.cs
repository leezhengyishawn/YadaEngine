using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class CameraTrigger : Component
    {
        public Vector3 cameraLocalPos;
        public Vector3 cameraLookPos;
        public bool isCamTrigger;

        private CameraBehaviour mainCamera;
        private bool hasInit;

        void Start()
        {
            if (this.entity.GetComponent<Renderer>() != null)
            {
                this.entity.GetComponent<Renderer>().active = false;
            }
        }

        void Update()
        {
            if (hasInit == false)
            {
                mainCamera = SceneController.mainCamera.GetComponent<CameraBehaviour>();
                hasInit = true;
            }
        }

        void OnTriggerStay(Entity collider)
        {
            if (collider.GetComponent<CameraPoint>() != null)
            {
                mainCamera.SetLocalPosition(cameraLocalPos, cameraLookPos);
                isCamTrigger = true;
            }
        }

        void OnTriggerExit(Entity collider)
        {
            if (collider.GetComponent<CameraPoint>() != null)
            {
                //mainCamera.ResetLocalPosition();
                isCamTrigger = false;
            }
        }
    }
}