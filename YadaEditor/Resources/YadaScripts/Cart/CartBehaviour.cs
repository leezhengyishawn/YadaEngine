using System;
using YadaScriptsLib;

namespace YadaScripts
{
    public class CartBehaviour : Component
    {
        public float moveSpeed = 2000.0f;
        public float rotateSpeed = 1.0f;
        public float waypointRadius = 0.5f;
        public float cartDetectDistance = 3.0f;
        public Entity waypointParent;
        public Entity forwardChecker;

        private Transform transform;
        private RigidBody rigidBody;
        private Collider collider;
        private Vector3[] waypointPositions;
        


        private float waypointRadiusSq;

        private float playerSpeedMultiplier; //Each player makes it move faster

        private Transform cartSFXTransform;
        private AudioSource cartSFX;

        private int currWaypoint = 0;
        public bool isStartPathing = false;
        private bool runOnce = false;

        void Start()
        {
            rigidBody = this.entity.GetComponent<RigidBody>();
            transform = this.entity.GetComponent<Transform>();
            collider = this.entity.GetComponent<Collider>();

            waypointRadiusSq = waypointRadius * waypointRadius;

            // Get the waypoint positions
            Transform parentTransform = waypointParent.GetComponent<Transform>();
            waypointPositions = new Vector3[parentTransform.childCount];
            for (int i = 0; i < waypointPositions.Length; ++i)
                waypointPositions[i] = parentTransform.GetChildByIndex((ulong)i).globalPosition;

            cartSFX = this.entity.GetComponent<AudioSource>();
        }

        void FixedUpdate()
        {
            if (Input.GetKeyPress(KEYCODE.KEY_F))
            {
                isStartPathing = true;
                runOnce = false;
            }


            if (isStartPathing)
            {
                playerSpeedMultiplier = 1.0f;

                Entity[] players = Entity.GetEntitiesWithComponent<PlayerBehaviour>();
                for (int i = 0; i < players.Length; ++i)
                {
                    if ((this.entity.GetComponent<Transform>().globalPosition - players[i].GetComponent<Transform>().globalPosition).magnitudeSq < (cartDetectDistance * cartDetectDistance))
                    {
                        playerSpeedMultiplier += 0.5f;
                    }
                }

                if (forwardChecker.GetComponent<CartForwardCheck>().isColliding)
                    playerSpeedMultiplier = 0;

                Console.WriteLine("current wp length = " + waypointPositions.Length);
                if (currWaypoint < waypointPositions.Length)
                    CartMovement();

            }


        }

        void CartMovement()
        {
            Vector3 targetVec = waypointPositions[currWaypoint] - transform.globalPosition;

            if (targetVec.magnitudeSq <= waypointRadiusSq)
            {
                if (waypointPositions.Length > currWaypoint + 1)
                {
                    // Go to the next waypoint if the cart has reached the current one
                    ++currWaypoint;// = (currWaypoint + 1);// % waypointPositions.Length;
                    targetVec = waypointPositions[currWaypoint] - transform.globalPosition;
                    Audio.PauseSource(cartSFX);
                    playerSpeedMultiplier = 0.0f;
                    if (!runOnce)
                    {
                        isStartPathing = false;
                        runOnce = true;
                    }
                }
                
            }

            targetVec.y = 0.0f;
            targetVec.Normalize();

            //Console.WriteLine("current playerSpeedMultiplier = " + playerSpeedMultiplier);
            rigidBody.linearVelocity = transform.forward * moveSpeed * Time.fixedDeltaTime * playerSpeedMultiplier;

            //Console.WriteLine("current speed = " + rigidBody.linearVelocity.x + rigidBody.linearVelocity.y + rigidBody.linearVelocity.z);
            // Rotate the cart towards the current waypoint
            Quaternion targetRot = Quaternion.LookRotation(targetVec, Vector3.up);
            transform.globalRotation = Quaternion.RotateTowards(transform.globalRotation, targetRot, rotateSpeed * Time.deltaTime * playerSpeedMultiplier);

            //if (!Audio.IsSourcePlaying(cartSFX.channel))
            //    Audio.PlaySource(cartSFX);
        }
    }
}