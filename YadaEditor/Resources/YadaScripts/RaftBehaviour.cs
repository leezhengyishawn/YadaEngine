using System;
using YadaScriptsLib;


namespace YadaScripts
{
    class RaftBehaviour: Component
    {
        public Entity Piece1;
        public Entity Piece2;
        public Entity Piece3;
        public Entity Piece4;

        private float maxTimer;
        private float freezeTimer;
        private bool freeze;
        private float currTimer;
        private Vector3 originalPlankPos;
        private Vector3 originalPlankScale;
        private Quaternion originalPlankRot;

        private Entity currPiece;
        private Transform currPieceTransform;
        private Entity currPlank;
        private Transform currPlankTransform;
        private bool receivingPlank;

        public Entity pathBlock;

        public int numberOfActiveRaft;
        public int totalNumberOfRafts;
        private int numberOfFixedRaft = 0;
        public Entity cart;
        // Start is called before the first frame update
        void Start()
        {
            freezeTimer = 0.1f;
            maxTimer = 1.5f;
            currTimer = 0;
            freeze = true;
            receivingPlank = false;

            if (numberOfActiveRaft == 4)
            {
                Piece1.active = true;
                Piece2.active = true;
                Piece3.active = true;
                Piece4.active = true;
            }
            else if (numberOfActiveRaft == 3)
            {
                Piece1.active = true;
                Piece2.active = true;
                Piece3.active = true;
                Piece4.active = false;
            }
            else if (numberOfActiveRaft == 2)
            {
                Piece1.active = true;
                Piece2.active = true;
                Piece3.active = false;
                Piece4.active = false;
            }
            else if (numberOfActiveRaft == 1)
            {
                Piece1.active = true;
                Piece2.active = false;
                Piece3.active = false;
                Piece4.active = false;
            }
            else
            {
                Piece1.active = false;
                Piece2.active = false;
                Piece3.active = false;
                Piece4.active = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (numberOfFixedRaft >= totalNumberOfRafts)
            {
                if (pathBlock != null)
                    pathBlock.GetComponent<Transform>().globalPosition = new Vector3(0, 100, 0);
                this.entity.GetComponent<Transform>().globalPosition = new Vector3(0, 150, 0);


            }

            if (receivingPlank)
            {
                if (freeze)
                {
                    if (currTimer < freezeTimer)
                    {
                        currPlank.GetComponent<RigidBody>().linearVelocity = Vector3.zero;
                        currTimer += Time.deltaTime;
                    }
                    else
                    {
                        currTimer = 0;
                        freeze = false;
                    }
                }
                else if (currTimer < maxTimer)
                {
                    //currPlank.GetComponent<RigidBody>().linearVelocity = Vector3.zero;
                    currPlankTransform.globalPosition = Selector.SmoothStep(originalPlankPos, currPieceTransform.globalPosition, currTimer / maxTimer);
                    currPlankTransform.globalScale = Selector.SmoothStep(originalPlankScale, currPieceTransform.globalScale, currTimer / maxTimer);
                    currPlankTransform.globalRotation = SmoothStep(originalPlankRot, currPieceTransform.globalRotation, currTimer / maxTimer);

                    currTimer += Time.deltaTime;
                }
                else
                {
                    currTimer = 0;
                    receivingPlank = false;
                    freeze = true;

                    //end of lerp
                    currPiece.active = true;
                    currPlank.active = false;
                    numberOfFixedRaft++;
                    
                    //Raft is fixed
                    if (numberOfActiveRaft + numberOfFixedRaft >= totalNumberOfRafts)
                    {
                        if (this.entity.GetComponent<EventTrigger>() != null)
                        {
                            //Console.WriteLine("Bridge fixed");
                            this.entity.GetComponent<EventTrigger>().SetTrigger(true);
                        }

                        //if (this.entity.GetComponent<EventResponse>() != null)
                        //{
                        //    //Console.WriteLine("Bridge fixed");
                        //    this.entity.GetComponent<EventResponse>().fired = true;
                        //}
                    }
                }
            }
        }


        private void OnTriggerEnter(Entity collision)
        {
            if (collision.GetComponent<PlankBehaviour>() != null)
            {
                if (!Piece1.active)
                {
                    currPiece = Piece1;
                }
                else if (!Piece2.active)
                {
                    currPiece = Piece2;
                }
                else if (!Piece3.active)
                {
                    currPiece = Piece3;
                }
                else if (!Piece4.active)
                {
                    currPiece = Piece4;
                }

                currPieceTransform = currPiece.GetComponent<Transform>();
                currPlank = collision;
                collision.GetComponent<RigidBody>().useGravity = false;
                collision.GetComponent<Collider>().active = false;
                currPlankTransform = collision.GetComponent<Transform>();

                originalPlankPos = currPlankTransform.globalPosition;
                originalPlankScale = currPlankTransform.globalScale;
                originalPlankRot = currPlankTransform.globalRotation;
                originalPlankRot.y %= (float)Math.PI;

                receivingPlank = true;
            }
        }

        public Quaternion SmoothStep(Quaternion q1, Quaternion q2, float time)
        {
            time = (time * time) * (3.0f - (2.0f * time));

            Quaternion newQ = new Quaternion(new Vector4(q2.x - q1.x, q2.y - q1.y, q2.z - q1.z, q2.w - q1.w));
            newQ *= time;
            newQ = new Quaternion(new Vector4(q1.x + newQ.x, q1.y + newQ.y, q1.z + newQ.z, q1.w + newQ.w));

            return newQ;
        }
    }
}