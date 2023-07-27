using System;
using YadaScriptsLib;


namespace YadaScripts
{
    public class voonTest : Component
    {
        public Entity playerSave;
        private Save playerSaveScript;

        private int player1Val;
        private int player2Val;
        
        void Start()
        {
            playerSaveScript = playerSave.GetComponent<Save>();
        }

        void Update()
        {
            if(Input.GetKey(KEYCODE.KEY_F1))
            {
                Save.setUpgrade(1, true);
                Save.setUpgrade(2, false);
                Console.WriteLine("Creating save file with values (1, 2)");
            }
            if(Input.GetKey(KEYCODE.KEY_F2))
            {
                player1Val = Save.getUpgradeVal(true);
                player2Val = Save.getUpgradeVal(false);
                Console.WriteLine("Loaded - Player 1: " + player1Val + " Player 2: " + player2Val);
            }
            if(Input.GetKey(KEYCODE.KEY_F3))
            {
                Scene.ChangeScene("Level 1 voon2");
                Console.WriteLine("Scene Changed!");
            }
            if(Input.GetKey(KEYCODE.KEY_F4))
            {
                Save.resetValues();
                Console.WriteLine("Save File Reset");
            }
        }
    }
}
