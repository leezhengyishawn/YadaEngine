using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class PunchingPlate : Component
    {
        private Transform myTransform;
        private Vector3 startingPos;
        private float punchedDownAmount;
        private bool punched = false;

        void Start()
        {
            myTransform = this.entity.GetComponent<Transform>();
            startingPos = myTransform.localPosition;
            punchedDownAmount = -0.2f;
        }

        void Update()
        {
            if (punched == true)
            {
                myTransform.localPosition = startingPos + (-Vector3.forward * punchedDownAmount);
            }
        }

        void OnTriggerStay(Entity collider)
        {
            PunchCheck punchCheckCollider = collider.GetComponent<PunchCheck>();
            if (punchCheckCollider != null && punchCheckCollider.GetOwner().playerState == PlayerBehaviour.PlayerState.PUNCHING && punched == false)
            {
                this.entity.GetComponent<EventTrigger>().SetTrigger(true);
                punched = true;
            }
        }
    }
}