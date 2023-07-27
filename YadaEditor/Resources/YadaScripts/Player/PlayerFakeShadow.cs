﻿using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class PlayerFakeShadow : Component
    {
        //player reference components
        private bool init = false;
        public bool isPlayer1;
        private Entity player;
        private Transform playerTransform;
        private Collider playerCollider;
        private PlayerBehaviour behaviourScript;

        //own components
        private Transform transform;

        private float gravity = 9.8f;
        private float time = 0;
        private Vector3 maxScale;
        public float maxDist; //approx max jump height

        public Vector3 PlotTrajectoryAtTime(Vector3 start, Vector3 startVelocity, float time)
        {
            return start + startVelocity * time + new Vector3(0, -gravity, 0) * time * time * 0.5f;
        }

        //probably need a calculate height

        //copy transform values for start of jump
        public void copyTransform()
        {
            transform.globalPosition = playerTransform.globalPosition;
        }

        //check if start works, might need a separate init on update
        private void Start()
        {
            //init player references
            //moved to the fixedUpdate due to init order issues
        }

        private void FixedUpdate()
        {
            if (!init)
            {
                if (isPlayer1)
                    player = SceneController.mainPlayer1;
                else
                    player = SceneController.mainPlayer2;
                behaviourScript = player.GetComponent<PlayerBehaviour>();
                playerCollider = player.GetComponent<Collider>();
                playerTransform = player.GetComponent<Transform>();

                //get own references
                transform = this.entity.GetComponent<Transform>();
                maxScale = transform.globalScale;

                //this.entity.GetComponent<Renderer>().active = false;
                init = true;
            }

            //should be activated once player is off the ground
            if (behaviourScript.getIsJumping())
            {
                ////////////////////////////////////////////////////
                ///RAYCAST METHOD
                ///////////////////////////////////////////////////
                Vector3 newVelocity = new Vector3(0 , -1, 0);

                Vector3 position = new Vector3(playerTransform.globalPosition.x, playerTransform.globalPosition.y - playerCollider.halfExtents.y,
                    playerTransform.globalPosition.z);
                //Vector3 position = new Vector3(playerTransform.globalPosition);

                Vector3 newPos = position;

                playerCollider.active = false;

                ColliderRaycastInfoMulti ray;
                ray.m_isHit = false;

                int countLoop = 2000;
                while (!ray.m_isHit)
                {
                    time += Time.deltaTime;
                    newPos = new Vector3(PlotTrajectoryAtTime(position, newVelocity, time));
                    ray = playerCollider.RaycastColliderMulti(position, newPos);

                    --countLoop;
                    if (countLoop == 0) //temporary failsafe
                    {
                        Console.WriteLine("Cast Failed!");
                        break;
                    }
                }

                time = 0;

                transform.globalPosition = new Vector3(newPos.x, newPos.y + 0.1f, newPos.z);

                playerCollider.active = true;

                //scaling visual
                float temp = playerTransform.globalPosition.y - playerCollider.halfExtents.y - transform.globalPosition.y;
                float fraction = temp / maxDist;
                if (fraction > 1.0f)
                    fraction = 1.0f;
                else if (fraction < 0.2f)
                    fraction = 0.2f;

                fraction = 1.0f - fraction;

                transform.globalScale = new Vector3(fraction * maxScale.x, fraction * maxScale.y, fraction * maxScale.z);
            }
            else
            {
                transform.globalPosition = new Vector3(playerTransform.globalPosition.x, playerTransform.globalPosition.y - playerCollider.halfExtents.y * 0.65f,
                    playerTransform.globalPosition.z);
                transform.globalScale = maxScale;
                time = 0;
            }

        }
    }
}
