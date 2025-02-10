using Godot;
using System;

namespace Erikduss
{
	public static class UnitsDefaultValues
	{
        //IMPORTANT: If adding a unit, modify Save funtion in the UnitsSettingsManager.cs to make sure they get saved correctly.
        //Also add to In_Game_HUD_Manager

        /*
         * HOW TO QUICKLY EDIT SELECTIONS HERE:
         * 1. Control + H to open find/replace.
         * 2. Select the region that you want to edit.
         * 3. Fill in the word to replace + what to replace it with.
         * 4. Press enter until all is replaced.
         */

        #region Age 01

        #region Warrior
        public static int Age01_Warrior_UnitCost = 10;
        public static Enums.Ages Age01_Warrior_UnitAvailableInAge = Enums.Ages.AGE_01;

        public static string Age01_Warrior_UnitName = "Warrior";
        public static string Age01_Warrior_UnitDescription = "Attacks of this unit damages additional units, up to 2 extra units.";

        public static int Age01_Warrior_UnitHealth = 15;
        public static int Age01_Warrior_UnitArmour = 15;
        public static int Age01_Warrior_UnitAttack = 7;

        public static int Age01_Warrior_UnitMovementSpeed = 50; 
        public static int Age01_Warrior_UnitDetectionRange = 30;
        #endregion

        #region Ranger
        public static int Age01_Ranger_UnitCost = 10;
        public static Enums.Ages Age01_Ranger_UnitAvailableInAge = Enums.Ages.AGE_01;

        public static string Age01_Ranger_UnitName = "Ranger";
        public static string Age01_Ranger_UnitDescription = "On kill, double attack speed for a limited short time. The unit cannot move during this period.";

        public static int Age01_Ranger_UnitHealth = 20;
        public static int Age01_Ranger_UnitArmour = 10;
        public static int Age01_Ranger_UnitAttack = 10;

        public static int Age01_Ranger_UnitMovementSpeed = 50;
        public static int Age01_Ranger_UnitDetectionRange = 150;
        #endregion

        #region Assassin
        public static int Age01_Assassin_UnitCost = 5;
        public static Enums.Ages Age01_Assassin_UnitAvailableInAge = Enums.Ages.AGE_01;

        public static string Age01_Assassin_UnitName = "Assassin";
        public static string Age01_Assassin_UnitDescription = "On attack, has a chance to inflict bleeding on the target, armoured targets have a smaller chance to bleed.";

        public static int Age01_Assassin_UnitHealth = 10;
        public static int Age01_Assassin_UnitArmour = 10;
        public static int Age01_Assassin_UnitAttack = 4;

        public static int Age01_Assassin_UnitMovementSpeed = 75;
        public static int Age01_Assassin_UnitDetectionRange = 30;
        #endregion

        #region Enforcer
        public static int Age01_Enforcer_UnitCost = 4;
        public static Enums.Ages Age01_Enforcer_UnitAvailableInAge = Enums.Ages.AGE_01;

        public static string Age01_Enforcer_UnitName = "Enforcer";
        public static string Age01_Enforcer_UnitDescription = "On attack, stun the target for a short amount of time.";

        public static int Age01_Enforcer_UnitHealth = 30;
        public static int Age01_Enforcer_UnitArmour = 20;
        public static int Age01_Enforcer_UnitAttack = 1;

        public static int Age01_Enforcer_UnitMovementSpeed = 60;
        public static int Age01_Enforcer_UnitDetectionRange = 30;
        #endregion

        #region Tank
        public static int Age01_Tank_UnitCost = 10;
        public static Enums.Ages Age01_Tank_UnitAvailableInAge = Enums.Ages.AGE_01;

        public static string Age01_Tank_UnitName = "Tank";
        public static string Age01_Tank_UnitDescription = "On attack, increase the attack speed of 2 units behind this.";

        public static int Age01_Tank_UnitHealth = 30;
        public static int Age01_Tank_UnitArmour = 30;
        public static int Age01_Tank_UnitAttack = 1;

        public static int Age01_Tank_UnitMovementSpeed = 35;
        public static int Age01_Tank_UnitDetectionRange = 30;
        #endregion

        #region MassHealer
        public static int Age01_MassHealer_UnitCost = 25;
        public static Enums.Ages Age01_MassHealer_UnitAvailableInAge = Enums.Ages.AGE_01;

        public static string Age01_MassHealer_UnitName = "MassHealer";
        public static string Age01_MassHealer_UnitDescription = "Can't attack, heals up to 4 allies at once. On spawn, Heal all allied units that are currently on the battlefield for a medium amount.";

        public static int Age01_MassHealer_UnitHealth = 10;
        public static int Age01_MassHealer_UnitArmour = 10;
        public static int Age01_MassHealer_UnitAttack = 0;

        public static int Age01_MassHealer_UnitMovementSpeed = 50;
        public static int Age01_MassHealer_UnitDetectionRange = 30;
        #endregion

        #region Battlemage
        public static int Age01_Battlemage_UnitCost = 15;
        public static Enums.Ages Age01_Battlemage_UnitAvailableInAge = Enums.Ages.AGE_01;

        public static string Age01_Battlemage_UnitName = "Battlemage";
        public static string Age01_Battlemage_UnitDescription = "After 3 regular attacks, unleash an instant powerful magic attack dealing damage to 2 additional units.";

        public static int Age01_Battlemage_UnitHealth = 20;
        public static int Age01_Battlemage_UnitArmour = 10;
        public static int Age01_Battlemage_UnitAttack = 4;

        public static int Age01_Battlemage_UnitMovementSpeed = 50;
        public static int Age01_Battlemage_UnitDetectionRange = 150;
        #endregion

        #region Archdruid
        public static int Age01_Archdruid_UnitCost = 150;
        public static Enums.Ages Age01_Archdruid_UnitAvailableInAge = Enums.Ages.AGE_01;

        public static string Age01_Archdruid_UnitName = "Archdruid";
        public static string Age01_Archdruid_UnitDescription = "Can transform into a very powerful Melee and Ranged unit.";

        public static int Age01_Archdruid_UnitHealth = 50;
        public static int Age01_Archdruid_UnitArmour = 50;
        public static int Age01_Archdruid_UnitAttack = 15;

        public static int Age01_Archdruid_UnitMovementSpeed = 50;
        public static int Age01_Archdruid_UnitDetectionRange = 150;
        #endregion

        #region Shaman
        public static int Age01_Shaman_UnitCost = 8;
        public static Enums.Ages Age01_Shaman_UnitAvailableInAge = Enums.Ages.AGE_01;

        public static string Age01_Shaman_UnitName = "Shaman";
        public static string Age01_Shaman_UnitDescription = "While alive on the battlefield, enhance your special Age power. (Age 1: increase the amount of meteors spawned by 1)";

        public static int Age01_Shaman_UnitHealth = 20;
        public static int Age01_Shaman_UnitArmour = 10;
        public static int Age01_Shaman_UnitAttack = 6;

        public static int Age01_Shaman_UnitMovementSpeed = 50;
        public static int Age01_Shaman_UnitDetectionRange = 30;
        #endregion

        #endregion

        #region Age 02

        #region Warrior
        public static int Age02_Warrior_UnitCost = 10;
        public static Enums.Ages Age02_Warrior_UnitAvailableInAge = Enums.Ages.AGE_02;

        public static string Age02_Warrior_UnitName = "Warrior";
        public static string Age02_Warrior_UnitDescription = "Attacks of this unit damages additional units, up to 2 extra units.";

        public static int Age02_Warrior_UnitHealth = 20;
        public static int Age02_Warrior_UnitArmour = 20;
        public static int Age02_Warrior_UnitAttack = 10;

        public static int Age02_Warrior_UnitMovementSpeed = 50;
        public static int Age02_Warrior_UnitDetectionRange = 30;
        #endregion

        #region Ranger
        public static int Age02_Ranger_UnitCost = 10;
        public static Enums.Ages Age02_Ranger_UnitAvailableInAge = Enums.Ages.AGE_02;

        public static string Age02_Ranger_UnitName = "Ranger";
        public static string Age02_Ranger_UnitDescription = "On kill, double attack speed for a limited short time. The unit cannot move during this period.";

        public static int Age02_Ranger_UnitHealth = 20;
        public static int Age02_Ranger_UnitArmour = 10;
        public static int Age02_Ranger_UnitAttack = 10;

        public static int Age02_Ranger_UnitMovementSpeed = 50;
        public static int Age02_Ranger_UnitDetectionRange = 30;
        #endregion

        #region Assassin
        public static int Age02_Assassin_UnitCost = 5;
        public static Enums.Ages Age02_Assassin_UnitAvailableInAge = Enums.Ages.AGE_02;

        public static string Age02_Assassin_UnitName = "Assassin";
        public static string Age02_Assassin_UnitDescription = "On attack, has a chance to inflict bleeding on the target, armoured targets have a smaller chance to bleed.";

        public static int Age02_Assassin_UnitHealth = 10;
        public static int Age02_Assassin_UnitArmour = 10;
        public static int Age02_Assassin_UnitAttack = 2;

        public static int Age02_Assassin_UnitMovementSpeed = 50;
        public static int Age02_Assassin_UnitDetectionRange = 30;
        #endregion

        #region Enforcer
        public static int Age02_Enforcer_UnitCost = 7;
        public static Enums.Ages Age02_Enforcer_UnitAvailableInAge = Enums.Ages.AGE_02;

        public static string Age02_Enforcer_UnitName = "Enforcer";
        public static string Age02_Enforcer_UnitDescription = "On attack, stun the target for a short amount of time.";

        public static int Age02_Enforcer_UnitHealth = 30;
        public static int Age02_Enforcer_UnitArmour = 30;
        public static int Age02_Enforcer_UnitAttack = 1;

        public static int Age02_Enforcer_UnitMovementSpeed = 50;
        public static int Age02_Enforcer_UnitDetectionRange = 30;
        #endregion

        #region Tank
        public static int Age02_Tank_UnitCost = 10;
        public static Enums.Ages Age02_Tank_UnitAvailableInAge = Enums.Ages.AGE_02;

        public static string Age02_Tank_UnitName = "Tank";
        public static string Age02_Tank_UnitDescription = "On attack, increase the attack speed of 2 units behind this.";

        public static int Age02_Tank_UnitHealth = 30;
        public static int Age02_Tank_UnitArmour = 30;
        public static int Age02_Tank_UnitAttack = 1;

        public static int Age02_Tank_UnitMovementSpeed = 50;
        public static int Age02_Tank_UnitDetectionRange = 30;
        #endregion

        #region MassHealer
        public static int Age02_MassHealer_UnitCost = 25;
        public static Enums.Ages Age02_MassHealer_UnitAvailableInAge = Enums.Ages.AGE_02;

        public static string Age02_MassHealer_UnitName = "MassHealer";
        public static string Age02_MassHealer_UnitDescription = "Can't attack, heals up to 4 allies at once. On spawn, Heal all allied units that are currently on the battlefield for a medium amount.";

        public static int Age02_MassHealer_UnitHealth = 10;
        public static int Age02_MassHealer_UnitArmour = 10;
        public static int Age02_MassHealer_UnitAttack = 0;

        public static int Age02_MassHealer_UnitMovementSpeed = 50;
        public static int Age02_MassHealer_UnitDetectionRange = 30;
        #endregion

        #region Battlemage
        public static int Age02_Battlemage_UnitCost = 15;
        public static Enums.Ages Age02_Battlemage_UnitAvailableInAge = Enums.Ages.AGE_02;

        public static string Age02_Battlemage_UnitName = "Battlemage";
        public static string Age02_Battlemage_UnitDescription = "After 3 melee attacks, unleash an instant powerful magic attack dealing damage to 2 additional units.";

        public static int Age02_Battlemage_UnitHealth = 20;
        public static int Age02_Battlemage_UnitArmour = 10;
        public static int Age02_Battlemage_UnitAttack = 4;

        public static int Age02_Battlemage_UnitMovementSpeed = 50;
        public static int Age02_Battlemage_UnitDetectionRange = 30;
        #endregion

        #region Archdruid
        public static int Age02_Archdruid_UnitCost = 150;
        public static Enums.Ages Age02_Archdruid_UnitAvailableInAge = Enums.Ages.AGE_01;

        public static string Age02_Archdruid_UnitName = "Archdruid";
        public static string Age02_Archdruid_UnitDescription = "Can transform into a very powerful Melee and Ranged unit.";

        public static int Age02_Archdruid_UnitHealth = 50;
        public static int Age02_Archdruid_UnitArmour = 50;
        public static int Age02_Archdruid_UnitAttack = 15;

        public static int Age02_Archdruid_UnitMovementSpeed = 50;
        public static int Age02_Archdruid_UnitDetectionRange = 120;
        #endregion

        #region Shaman
        public static int Age02_Shaman_UnitCost = 8;
        public static Enums.Ages Age02_Shaman_UnitAvailableInAge = Enums.Ages.AGE_01;

        public static string Age02_Shaman_UnitName = "Shaman";
        public static string Age02_Shaman_UnitDescription = "While alive on the battlefield, enhance your special Age power. (Age 1: increase the amount of meteors spawned by 1)";

        public static int Age02_Shaman_UnitHealth = 20;
        public static int Age02_Shaman_UnitArmour = 10;
        public static int Age02_Shaman_UnitAttack = 6;

        public static int Age02_Shaman_UnitMovementSpeed = 50;
        public static int Age02_Shaman_UnitDetectionRange = 30;
        #endregion

        #endregion
    }
}
