using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class RollingIndicator : Component
    {
        private bool init = false;
        private Vector3 originalScale;
        private float currRotation;

        private float bounceTimer;
        private float maxBounceTimer;

        private float offsetY;
        
        //player component
        public bool isPlayer1;
        public Entity playerPunchCheck;
        private PunchCheck playerPunch;
        private Entity player;
        private Transform playerTransform;
        private PlayerBehaviour playerBehaviour;

        //other player
        private Entity otherPlayer;
        private Transform otherPlayerTransform;
        private PlayerBehaviour otherPlayerBehaviour;


        //own components
        private Transform transform;
        private Renderer renderer;

        void Start()
        {
            //probably nothing to avoid init order issues
            bounceTimer = 0;
            maxBounceTimer = 1.0f;
            offsetY = 2f;
        }

        void FixedUpdate()
        {
            if (!init)
            {
                if (isPlayer1)
                {
                    player = SceneController.mainPlayer1;
                    otherPlayer = SceneController.mainPlayer2;
                }
                else
                {
                    player = SceneController.mainPlayer2;
                    otherPlayer = SceneController.mainPlayer1;
                }
                
                playerTransform = player.GetComponent<Transform>();
                playerBehaviour = player.GetComponent<PlayerBehaviour>();
                otherPlayerTransform = otherPlayer.GetComponent<Transform>();
                otherPlayerBehaviour = otherPlayer.GetComponent<PlayerBehaviour>();
                playerPunch = playerPunchCheck.GetComponent<PunchCheck>();

                transform = this.entity.GetComponent<Transform>();
                renderer = this.entity.GetComponent<Renderer>();
                originalScale = transform.globalScale;

                init = true;
            }

            //draw rolling indicator - will only show if on puddle and punchable
            if (otherPlayerBehaviour.getIsOnPuddle() && playerPunch.getPunchColliding() && SceneController.hasRollAttack == true)
            {
                renderer.active = true;
                transform.globalPosition = otherPlayerTransform.globalPosition + 
                    2 * (otherPlayerTransform.globalPosition - playerTransform.globalPosition);

                Vector3 directionalVector = new Vector3(otherPlayerTransform.globalPosition - playerTransform.globalPosition);
                float move = PlayerMovement.getXYAngle(directionalVector, new Vector3(1, 0, 0));

                bool playerIsLeft = PlayerMovement.checkIfLeft(new Vector3(1, 0, 0), directionalVector); //check side of camera
                if (playerIsLeft) move = -move;

                transform.globalEulerAngles = new Vector3(0, move, 90);
            }
            else if (playerPunch.getPunchColliding() && playerBehaviour.getPlayerState() == PlayerBehaviour.PlayerState.CHARGING)
            {
                renderer.active = true;
                float newY = 0;
                transform.globalScale = new Vector3(originalScale.x * 0.8f, originalScale.y * playerBehaviour.getChargingPercentage() * 0.8f, originalScale.z * 0.8f);

                if (playerBehaviour.getChargingPercentage() >= 1)
                {
                    if (bounceTimer > maxBounceTimer)
                        bounceTimer = 0;

                    if (bounceTimer < maxBounceTimer * 0.5f)
                        newY = SmoothStep(offsetY, offsetY + 0.5f, bounceTimer / (maxBounceTimer * 0.5f));
                    else if (bounceTimer <= maxBounceTimer)
                        newY = SmoothStep(offsetY + 0.5f, offsetY, (bounceTimer - maxBounceTimer * 0.5f) / (maxBounceTimer - maxBounceTimer * 0.5f));

                    transform.globalPosition = otherPlayerTransform.globalPosition + new Vector3(0, newY, 0);
                    bounceTimer += Time.fixedDeltaTime;
                }
                else
                {
                    transform.globalPosition = otherPlayerTransform.globalPosition + new Vector3(0, offsetY, 0);
                }

                Quaternion quat1 = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), (float)Math.PI);
                Quaternion quat2 = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), currRotation);

                transform.localRotation = quat1 * quat2;

                if (playerBehaviour.getChargingPercentage() >= 1)
                    currRotation += 7 * Time.fixedDeltaTime;
                else
                    currRotation += 5 * Time.fixedDeltaTime;
            }
            else //disable indicator
            {
                transform.globalScale = originalScale;
                renderer.active = false;
                currRotation = 0;
                bounceTimer = 0;
            }
        }

        public static float SmoothStep(float v0, float v1, float time)
        {
            time = (time * time) * (3.0f - (2.0f * time));

            return v0 + time * (v1 - v0);
        }
    }
}
