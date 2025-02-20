using Godot;
using Godot.Collections;
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

        //DICTIONARY IS AT THE BOTTOM DUE TO LOADING/CREATING PRIORITY.

        #region Age 01

        #region Warrior
        public static UnitSettingsConfig Age01_Warrior = new UnitSettingsConfig()
        {
            unitCost = 10,
            unitAvailableInAge = Enums.Ages.AGE_01,
            unitName = "Warrior",
            unitDescription = "Attacks of this unit damages additional units, up to 2 extra units.",
            unitHealth = 15,
            unitArmour = 15,
            unitAttack = 7,
            unitMovementSpeed = 50,
            unitDetectionRange = 30
        };
        #endregion

        #region Ranger
        public static UnitSettingsConfig Age01_Ranger = new UnitSettingsConfig()
        {
            unitCost = 10,
            unitAvailableInAge = Enums.Ages.AGE_01,
            unitName = "Ranger",
            unitDescription = "On kill, double the attack speed of this unit for a short time. This unit cannot move during this period.",
            unitHealth = 20,
            unitArmour = 10,
            unitAttack = 9,
            unitMovementSpeed = 50,
            unitDetectionRange = 130
        };
        #endregion

        #region Assassin
        public static UnitSettingsConfig Age01_Assassin = new UnitSettingsConfig()
        {
            unitCost = 5,
            unitAvailableInAge = Enums.Ages.AGE_01,
            unitName = "Assassin",
            unitDescription = "On attack, has a chance to inflict bleeding on the target, armoured targets have a smaller chance to bleed.",
            unitHealth = 10,
            unitArmour = 10,
            unitAttack = 4,
            unitMovementSpeed = 75,
            unitDetectionRange = 30
        };
        #endregion

        #region Enforcer
        public static UnitSettingsConfig Age01_Enforcer = new UnitSettingsConfig()
        {
            unitCost = 4,
            unitAvailableInAge = Enums.Ages.AGE_01,
            unitName = "Enforcer",
            unitDescription = "On attack, has a chance to stun the target for a short amount of time.",
            unitHealth = 30,
            unitArmour = 20,
            unitAttack = 1,
            unitMovementSpeed = 60,
            unitDetectionRange = 30
        };
        #endregion

        #region Tank
        public static UnitSettingsConfig Age01_Tank = new UnitSettingsConfig()
        {
            unitCost = 10,
            unitAvailableInAge = Enums.Ages.AGE_01,
            unitName = "Tank",
            unitDescription = "On attack, has a chance to increase the attack speed of up to 2 units behind this.",
            unitHealth = 30,
            unitArmour = 30,
            unitAttack = 1,
            unitMovementSpeed = 35,
            unitDetectionRange = 30
        };
        #endregion

        #region MassHealer
        public static UnitSettingsConfig Age01_Mass_Healer = new UnitSettingsConfig()
        {
            unitCost = 25,
            unitAvailableInAge = Enums.Ages.AGE_01,
            unitName = "MassHealer",
            unitDescription = "Can't attack, heals up to 6 allies (plus self) around this unit at once. On spawn, Heal all allied units that are currently on the battlefield.",
            unitHealth = 10,
            unitArmour = 10,
            unitAttack = 0,
            unitMovementSpeed = 50,
            unitDetectionRange = 130
        };
        #endregion

        #region Battlemage
        public static UnitSettingsConfig Age01_Battlemage = new UnitSettingsConfig()
        {
            unitCost = 15,
            unitAvailableInAge = Enums.Ages.AGE_01,
            unitName = "Battlemage",
            unitDescription = "After 3 regular attacks, unleash a very powerful magic attack that deals damage to up to 3 units.",
            unitHealth = 20,
            unitArmour = 10,
            unitAttack = 4,
            unitMovementSpeed = 50,
            unitDetectionRange = 130
        };
        #endregion

        #region Archdruid
        public static UnitSettingsConfig Age01_Archdruid = new UnitSettingsConfig()
        {
            unitCost = 150,
            unitAvailableInAge = Enums.Ages.AGE_01,
            unitName = "Archdruid",
            unitDescription = "Can transform into a very powerful Melee and Ranged unit.",
            unitHealth = 50,
            unitArmour = 50,
            unitAttack = 15,
            unitMovementSpeed = 50,
            unitDetectionRange = 150
        };
        #endregion

        #region Shaman
        public static UnitSettingsConfig Age01_Shaman = new UnitSettingsConfig()
        {
            unitCost = 8,
            unitAvailableInAge = Enums.Ages.AGE_01,
            unitName = "Shaman",
            unitDescription = "While alive on the battlefield, enhance your special Age power. (Age 1: increase the amount of meteors spawned by 1)",
            unitHealth = 20,
            unitArmour = 10,
            unitAttack = 6,
            unitMovementSpeed = 50,
            unitDetectionRange = 130
        };
        #endregion

        #endregion

        #region Age 02

        #region Warrior
        public static UnitSettingsConfig Age02_Warrior = new UnitSettingsConfig()
        {
            unitCost = 10,
            unitAvailableInAge = Enums.Ages.AGE_02,
            unitName = "Warrior",
            unitDescription = "Attacks of this unit damages additional units, up to 2 extra units.",
            unitHealth = 15,
            unitArmour = 15,
            unitAttack = 7,
            unitMovementSpeed = 50,
            unitDetectionRange = 30
        };
        #endregion

        #region Ranger
        public static UnitSettingsConfig Age02_Ranger = new UnitSettingsConfig()
        {
            unitCost = 10,
            unitAvailableInAge = Enums.Ages.AGE_02,
            unitName = "Ranger",
            unitDescription = "On kill, double the attack speed of this unit for a short time. This unit cannot move during this period.",
            unitHealth = 20,
            unitArmour = 10,
            unitAttack = 9,
            unitMovementSpeed = 50,
            unitDetectionRange = 130
        };
        #endregion

        #region Assassin
        public static UnitSettingsConfig Age02_Assassin = new UnitSettingsConfig()
        {
            unitCost = 5,
            unitAvailableInAge = Enums.Ages.AGE_02,
            unitName = "Assassin",
            unitDescription = "On attack, has a chance to inflict bleeding on the target, armoured targets have a smaller chance to bleed.",
            unitHealth = 10,
            unitArmour = 10,
            unitAttack = 4,
            unitMovementSpeed = 75,
            unitDetectionRange = 30
        };
        #endregion

        #region Enforcer
        public static UnitSettingsConfig Age02_Enforcer = new UnitSettingsConfig()
        {
            unitCost = 4,
            unitAvailableInAge = Enums.Ages.AGE_02,
            unitName = "Enforcer",
            unitDescription = "On attack, has a chance to stun the target for a short amount of time.",
            unitHealth = 30,
            unitArmour = 20,
            unitAttack = 1,
            unitMovementSpeed = 60,
            unitDetectionRange = 30
        };
        #endregion

        #region Tank
        public static UnitSettingsConfig Age02_Tank = new UnitSettingsConfig()
        {
            unitCost = 10,
            unitAvailableInAge = Enums.Ages.AGE_02,
            unitName = "Tank",
            unitDescription = "On attack, has a chance to increase the attack speed of up to 2 units behind this.",
            unitHealth = 30,
            unitArmour = 30,
            unitAttack = 1,
            unitMovementSpeed = 35,
            unitDetectionRange = 30
        };
        #endregion

        #region MassHealer
        public static UnitSettingsConfig Age02_Mass_Healer = new UnitSettingsConfig()
        {
            unitCost = 25,
            unitAvailableInAge = Enums.Ages.AGE_02,
            unitName = "MassHealer",
            unitDescription = "Can't attack, heals up to 6 allies (plus self) around this unit at once. On spawn, Heal all allied units that are currently on the battlefield.",
            unitHealth = 10,
            unitArmour = 10,
            unitAttack = 0,
            unitMovementSpeed = 50,
            unitDetectionRange = 130
        };
        #endregion

        #region Battlemage
        public static UnitSettingsConfig Age02_Battlemage = new UnitSettingsConfig()
        {
            unitCost = 15,
            unitAvailableInAge = Enums.Ages.AGE_02,
            unitName = "Battlemage",
            unitDescription = "After 3 regular attacks, unleash a very powerful magic attack that deals damage to up to 3 units.",
            unitHealth = 20,
            unitArmour = 10,
            unitAttack = 4,
            unitMovementSpeed = 50,
            unitDetectionRange = 130
        };
        #endregion

        #region Archdruid
        public static UnitSettingsConfig Age02_Archdruid = new UnitSettingsConfig()
        {
            unitCost = 150,
            unitAvailableInAge = Enums.Ages.AGE_02,
            unitName = "Archdruid",
            unitDescription = "Can transform into a very powerful Melee and Ranged unit.",
            unitHealth = 50,
            unitArmour = 50,
            unitAttack = 15,
            unitMovementSpeed = 50,
            unitDetectionRange = 150
        };
        #endregion

        #region Shaman
        public static UnitSettingsConfig Age02_Shaman = new UnitSettingsConfig()
        {
            unitCost = 8,
            unitAvailableInAge = Enums.Ages.AGE_02,
            unitName = "Shaman",
            unitDescription = "While alive on the battlefield, enhance your special Age power. (Age 1: increase the amount of meteors spawned by 1)",
            unitHealth = 20,
            unitArmour = 10,
            unitAttack = 6,
            unitMovementSpeed = 50,
            unitDetectionRange = 130
        };
        #endregion

        #endregion

        public static Dictionary<string, UnitSettingsConfig> defaultUnitValuesDictionary = new Dictionary<string, UnitSettingsConfig>()
        {
            { (Enums.Ages.AGE_01.ToString() + "_" + Enums.UnitTypes.Warrior.ToString()), Age01_Warrior},
            { (Enums.Ages.AGE_01.ToString() + "_" + Enums.UnitTypes.Ranger.ToString()), Age01_Ranger},
            { (Enums.Ages.AGE_01.ToString() + "_" + Enums.UnitTypes.Assassin.ToString()), Age01_Assassin},
            { (Enums.Ages.AGE_01.ToString() + "_" + Enums.UnitTypes.Enforcer.ToString()), Age01_Enforcer},
            { (Enums.Ages.AGE_01.ToString() + "_" + Enums.UnitTypes.Tank.ToString()), Age01_Tank},
            { (Enums.Ages.AGE_01.ToString() + "_" + Enums.UnitTypes.Mass_Healer.ToString()), Age01_Mass_Healer},
            { (Enums.Ages.AGE_01.ToString() + "_" + Enums.UnitTypes.Battlemage.ToString()), Age01_Battlemage},
            { (Enums.Ages.AGE_01.ToString() + "_" + Enums.UnitTypes.Archdruid.ToString()), Age01_Archdruid},
            { (Enums.Ages.AGE_01.ToString() + "_" + Enums.UnitTypes.Shaman.ToString()), Age01_Shaman},
            { (Enums.Ages.AGE_02.ToString() + "_" + Enums.UnitTypes.Warrior.ToString()), Age02_Warrior},
            { (Enums.Ages.AGE_02.ToString() + "_" + Enums.UnitTypes.Ranger.ToString()), Age02_Ranger},
            { (Enums.Ages.AGE_02.ToString() + "_" + Enums.UnitTypes.Assassin.ToString()), Age02_Assassin},
            { (Enums.Ages.AGE_02.ToString() + "_" + Enums.UnitTypes.Enforcer.ToString()), Age02_Enforcer},
            { (Enums.Ages.AGE_02.ToString() + "_" + Enums.UnitTypes.Tank.ToString()), Age02_Tank},
            { (Enums.Ages.AGE_02.ToString() + "_" + Enums.UnitTypes.Mass_Healer.ToString()), Age02_Mass_Healer},
            { (Enums.Ages.AGE_02.ToString() + "_" + Enums.UnitTypes.Battlemage.ToString()), Age02_Battlemage},
            { (Enums.Ages.AGE_02.ToString() + "_" + Enums.UnitTypes.Archdruid.ToString()), Age02_Archdruid},
            { (Enums.Ages.AGE_02.ToString() + "_" + Enums.UnitTypes.Shaman.ToString()), Age02_Shaman},
        };
    }
}
