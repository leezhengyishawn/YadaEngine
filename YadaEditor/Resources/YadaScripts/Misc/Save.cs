using System;
using YadaScriptsLib;

namespace YadaScripts
{
    class Save : Component
    {
        struct PunchUpgrade
        {
            public int upgrade1;
            public int upgrade2;

            public void initUpgrade(int upgradeVal)
            {
                upgrade1 = upgradeVal / 10;
                upgrade2 = upgrade1 == 0 ? 0 : upgradeVal % 10;
            }

            public int getFinalUpgradeValue()
            {
                if (upgrade1 > 2)
                    return upgrade1;
                return upgrade1 * 10 + upgrade2;
            }
        }

        private static PunchUpgrade player1Upgrade;
        private static PunchUpgrade player2Upgrade;

        private void Start()
        {
            //might to check if first scene

            loadFromFile();

            player1Upgrade = new PunchUpgrade();
            player2Upgrade = new PunchUpgrade();
        }

        //getters and setters
        public static int getUpgradeVal(bool isPlayer1)
        {
            File.ReadJsonFile("tempSave");
            int upgradeVal = isPlayer1 ? File.ReadDataAsInt("Player1Upgrade") : File.ReadDataAsInt("Player2Upgrade");

            if (isPlayer1)
                player1Upgrade.initUpgrade(upgradeVal);
            else
                player2Upgrade.initUpgrade(upgradeVal);

            return upgradeVal;
        }

        public static void setUpgrade(int val, bool isPlayer1)
        {
            if (val > 4)
                return;

            //need to check if has initial upgrade
            if (isPlayer1)
            {
                if (player1Upgrade.upgrade1 == 0) //set to upgrade 2 if upgrade 1 is already set
                    player1Upgrade.upgrade1 = val;
                else if (player1Upgrade.upgrade2 == 0)
                    player1Upgrade.upgrade2 = val;

                File.WriteDataAsInt("Player1Upgrade", player1Upgrade.getFinalUpgradeValue());
            }
            else
            {
                if (player2Upgrade.upgrade1 == 0) //set to upgrade 2 if upgrade 1 is already set
                    player2Upgrade.upgrade1 = val;
                else if (player2Upgrade.upgrade2 == 0)
                    player2Upgrade.upgrade2 = val;

                File.WriteDataAsInt("Player2Upgrade", player2Upgrade.getFinalUpgradeValue());
            }

            File.WriteJsonFile("tempSave");
        }

        private void loadFromFile()
        {
            File.ReadJsonFile("tempSave");
        }

        public static void resetValues()
        {
            File.WriteDataAsInt("Player1Upgrade", 0);
            File.WriteDataAsInt("Player2Upgrade", 0);
            File.WriteJsonFile("tempSave");
        }
    }
}
