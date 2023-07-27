using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class Selector : Component
    {

        public Entity leftPointer;
        public Entity rightPointer;

        public int index;
        int pastIndex;

        public int limit;
        // private Vector3 scale;
        public float yStep;
        public float yValue;

        Vector3 currLeft = new Vector3(0, 0, 0);

        Vector3 leftPos = new Vector3(0, 0, 0);


        float timer = 6.0f;
        float maxTimer = 0.15f;

        float iTimer = 6.0f;
        float iMaxTimer = 0.2f;

        bool idleFlag = true;

        Vector3 idlePos1 = new Vector3(0, 0, 0);
        Vector3 idlePos2 = new Vector3(0, 0, 0);


        void Start()
        {
            index = 0;
            idleFlag = true;
            leftPos = leftPointer.GetComponent<Transform>().localPosition;
            //rightPos = rightPointer.GetComponent<Transform>().localPosition;

            idlePos1 = leftPointer.GetComponent<Transform>().localPosition;
            idlePos2 = idlePos1;
            idlePos2.x += 0.01f;

        }


        void Update()
        {
            //Init();
            idlePos1.y = idlePos2.y = leftPointer.GetComponent<Transform>().localPosition.y;


            if (iTimer < iMaxTimer)
            {
                IdleCursors();
                iTimer += Time.deltaTime;
            }
            else
            {
                iTimer = 0f;
                leftPointer.GetComponent<Transform>().localPosition = idlePos1; // idleFlag ? idlePos1 : idlePos2;
                idleFlag = !idleFlag;
            }



            if (timer < maxTimer)
            {
                LerpCursors();
                timer += Time.deltaTime;
            }



            if (index != pastIndex)
            {
                MoveSelector();
                pastIndex = index;
            }

            pastIndex = index;
        }

        void MoveSelector()
        {
            leftPos.y = yValue - index * yStep;
            //rightPos.y = yValue - index * yStep;
            
            timer = 0.0f;
            currLeft = leftPointer.GetComponent<Transform>().localPosition;
            //currRight = rightPointer.GetComponent<Transform>().localPosition;
        }

        void IdleCursors()
        {
            
            if (!idleFlag)
                leftPointer.GetComponent<Transform>().localPosition = SmoothStep(idlePos1, idlePos2, iTimer / iMaxTimer);
            //else
            //    leftPointer.GetComponent<Transform>().localPosition = SmoothStep(leftPointer.GetComponent<Transform>().localPosition, idlePos1, timer / maxTimer);
        }

        void LerpCursors()
        {

            leftPointer.GetComponent<Transform>().localPosition = SmoothStep(currLeft, leftPos, timer / maxTimer);

        }


        public static Vector3 SmoothStep( Vector3 v0, Vector3 v1, float time)
        {
            time = (time * time) * (3.0f - (2.0f * time));

            return v0 + time * (v1 - v0);
        }
    }
}
