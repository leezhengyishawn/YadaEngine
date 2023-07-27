using System;
using System.Threading;
using YadaScriptsLib;

namespace YadaScripts
{
    class DurianBehaviour : Component
    {
        public Entity durianMesh;

        Animator durianAnim;
        Dialogue dialogue;


        bool dialogueStarted = false;

        void Start()
        {
            durianAnim = durianMesh.GetComponent<Animator>();
            dialogue = this.entity.GetComponent<Dialogue>();
        }

        void FixedUpdate()
        {
            //Part 1: Alert from noticing player
            if (dialogue.dialogueIsShowing && dialogueStarted == false)
            {
                durianAnim.animationIndex = 2;
                durianAnim.animateCount = 1;
                dialogueStarted = true;
            }

            //Part 2: Talk anim
            if (dialogueStarted && durianAnim.animateCount == 0)
            {
                durianAnim.animationIndex = 21;
                durianAnim.animateCount = -1;
            }

            //Part 3: After UI disappears, set to idle look
            if (durianAnim.animationIndex == 21 && dialogueStarted && dialogue.dialogueIsShowing == false)
            {
                durianAnim.animationIndex = 7;
                durianAnim.animateCount = -1;
            }
        }
    }
}
