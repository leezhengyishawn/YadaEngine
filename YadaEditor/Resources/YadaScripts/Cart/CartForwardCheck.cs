using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class CartForwardCheck : Component
    {
        public bool isColliding;
        public bool prevColliding;
        public Collider collider;
        public Transform transform;
        public Entity cart;

        void Start()
        {
            collider = this.entity.GetComponent<Collider>();
            transform = this.entity.GetComponent<Transform>();
            collider.active = true; //should change?
            isColliding = false;
            prevColliding = false;
        }

        void FixedUpdate()
        {
            //update position to cart
            Vector3 push_up = new Vector3( 0F, 0.3F, 0F ); //push the box up to not touch the ground
            transform.globalPosition = cart.GetComponent<Transform>().globalPosition + cart.GetComponent<Transform>().forward + push_up; //it's actually to the front
            transform.globalRotation = cart.GetComponent<Transform>().globalRotation;

            prevColliding = isColliding;
            isColliding = false;
        }

        void OnTriggerStay(Entity collider)
        {
            if (collider != null)
            {
                if (collider.GetComponent<PlayerBehaviour>() == null && collider.GetComponent<MeleeEnemyBehaviour>() == null &&
                    collider.GetComponent<CartBehaviour>() == null
                    && collider.GetComponent<MerchantBehaviour>() == null && collider.GetComponent<MerchantItem>() == null
                    && collider.GetComponent<EnemyProjectileBehaviour>() == null && collider.GetComponent<CameraTrigger>() == null)
                {
                    isColliding = true;
                }
            }
        }

        void OnTriggerEnter(Entity collider)
        {
            if (collider != null)
            {
                if (collider.GetComponent<PlayerBehaviour>() == null && collider.GetComponent<MeleeEnemyBehaviour>() == null &&
                    collider.GetComponent<CartBehaviour>() == null
                    && collider.GetComponent<MerchantBehaviour>() == null && collider.GetComponent<MerchantItem>() == null
                    && collider.GetComponent<EnemyProjectileBehaviour>() == null && collider.GetComponent<CameraTrigger>() == null)
                {
                    isColliding = true;
                }
            }
        }
    }
}
