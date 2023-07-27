using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class RainParticleMovement : Component
    {
        public Vector3 offsetAmount;
        private Transform myTransform;
        private Transform cameraTransform;

        void Start()
        {
            myTransform = this.entity.GetComponent<Transform>();
        }

        void Update()
        {
            CheckCamera();
            CheckMovement();
        }

        private void CheckCamera()
        {
            if (cameraTransform == null)
            {
                cameraTransform = SceneController.mainCamera.GetComponent<CameraBehaviour>().mainCamera.GetComponent<Transform>();
            }
        }

        private void CheckMovement()
        {
            myTransform.globalPosition = cameraTransform.globalPosition + offsetAmount;
        }
    }
}