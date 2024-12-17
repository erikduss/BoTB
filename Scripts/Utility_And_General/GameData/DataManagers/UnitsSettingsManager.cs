using Godot;
using System;

namespace Erikduss
{
	public partial class UnitsSettingsManager : BaseSettingsManager
	{
        //This settings manager uses multiple files, so for every unit the file name changes to the variable name.

        private UnitSettingsConfig currentWorkingConfig;
        private Enums.UnitTypes currentWorkingUnitType;
        private Enums.Ages currentWorkingUnitAge;

        #region UnitConfigs

        //Age 01
        public UnitSettingsConfig Age01_WarriorSettingsConfig = new UnitSettingsConfig();
        public UnitSettingsConfig Age01_RangerSettingsConfig = new UnitSettingsConfig();
        public UnitSettingsConfig Age01_AssassinSettingsConfig = new UnitSettingsConfig();
        public UnitSettingsConfig Age01_TankSettingsConfig = new UnitSettingsConfig();
        public UnitSettingsConfig Age01_BattlemageSettingsConfig = new UnitSettingsConfig();
        public UnitSettingsConfig Age01_MassHealerSettingsConfig = new UnitSettingsConfig();
        public UnitSettingsConfig Age01_EnforcerSettingsConfig = new UnitSettingsConfig();
        public UnitSettingsConfig Age01_ArchdruidSettingsConfig = new UnitSettingsConfig();

        //Age 02
        public UnitSettingsConfig Age02_WarriorSettingsConfig = new UnitSettingsConfig();
        public UnitSettingsConfig Age02_RangerSettingsConfig = new UnitSettingsConfig();
        public UnitSettingsConfig Age02_AssassinSettingsConfig = new UnitSettingsConfig();
        public UnitSettingsConfig Age02_TankSettingsConfig = new UnitSettingsConfig();
        public UnitSettingsConfig Age02_BattlemageSettingsConfig = new UnitSettingsConfig();
        public UnitSettingsConfig Age02_MassHealerSettingsConfig = new UnitSettingsConfig();
        public UnitSettingsConfig Age02_EnforcerSettingsConfig = new UnitSettingsConfig();
        public UnitSettingsConfig Age02_ArchdruidSettingsConfig = new UnitSettingsConfig();

        #endregion

        private void SetFileInfo(Enums.UnitTypes unitType, Enums.Ages unitAge)
        {
            directoryLocation = "user://Settings//Units//" + unitAge.ToString() + "//";
            fileName = unitType.ToString() +".cfg";
            fullFilePath = directoryLocation + fileName;
        }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        }

        public virtual void LoadUnitSettings(Enums.UnitTypes unitType, Enums.Ages unitAge)
        {
            //We need to ensure the correct file paths are set before we continue.
            SetFileInfo(unitType, unitAge);

            var config = new ConfigFile();

            // Load data from a file.
            var err = config.Load(fullFilePath);

            SetCurrentWorkingValues(unitType, unitAge);

            // If the file didn't load, ignore it.
            if (err != Error.Ok)
                CreateNewSaveFile(true);

            // Iterate over all sections.
            foreach (String section in config.GetSections())
            {
                if (section == Enums.UnitSettingsConfigHeader.CONFIG_SETTINGS.ToString())
                {
                    currentWorkingConfig.useCustomVariables = (bool)config.GetValue(section, "useCustomVariables");
                }
                else if (section == Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString())
                {
                    currentWorkingConfig.unitCost = (int)config.GetValue(section, "unitCost");
                    currentWorkingConfig.unitAvailableInAge = (Enums.Ages)(int)config.GetValue(section, "unitAvailableInAge");

                    currentWorkingConfig.unitName = (string)config.GetValue(section, "unitName");
                    currentWorkingConfig.unitDescription = (string)config.GetValue(section, "unitDescription");
                }
                else if (section == Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString())
                {
                    currentWorkingConfig.unitHealth = (int)config.GetValue(section, "unitHealth");
                    currentWorkingConfig.unitArmour = (int)config.GetValue(section, "unitArmour");
                    currentWorkingConfig.unitAttack = (int)config.GetValue(section, "unitAttack");
                }
                else if (section == Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString())
                {
                    currentWorkingConfig.unitMovementSpeed = (int)config.GetValue(section, "unitMovementSpeed");
                    currentWorkingConfig.unitDetectionRange = (int)config.GetValue(section, "unitDetectionRange");
                }
                else
                {
                    GD.Print("SECTION NOT FOUND: " + section);
                }
            }

            //WorldSettingsManager.Instance.SetValuesOfPlayerGlobalSettings(settingsConfig);
        }

        private void SetCurrentWorkingValues(Enums.UnitTypes unitType, Enums.Ages unitAge)
        {
            currentWorkingUnitAge = unitAge;
            currentWorkingUnitType = unitType;

            switch (unitAge)
            {
                case Enums.Ages.AGE_01:
                    switch (unitType)
                    {
                        case Enums.UnitTypes.Warrior:
                            currentWorkingConfig = Age01_WarriorSettingsConfig;
                            break;
                        case Enums.UnitTypes.Asssassin:
                            currentWorkingConfig = Age01_AssassinSettingsConfig;
                            break;
                        case Enums.UnitTypes.Enforcer:
                            currentWorkingConfig = Age01_EnforcerSettingsConfig;
                            break;
                        case Enums.UnitTypes.Tank:
                            currentWorkingConfig = Age01_TankSettingsConfig;
                            break;
                        case Enums.UnitTypes.Battlemage:
                            currentWorkingConfig = Age01_BattlemageSettingsConfig;
                            break;
                        case Enums.UnitTypes.Mass_Healer:
                            currentWorkingConfig = Age01_MassHealerSettingsConfig;
                            break;
                        case Enums.UnitTypes.Ranger:
                            currentWorkingConfig = Age01_RangerSettingsConfig;
                            break;
                        case Enums.UnitTypes.Archdruid:
                            currentWorkingConfig = Age01_ArchdruidSettingsConfig;
                            break;
                        default:
                            GD.PrintErr("Unit type not implemented: UnitSettingsManager.cs (SetCurrentWorkingValues)");
                            currentWorkingConfig = Age01_WarriorSettingsConfig;
                            break;
                    }
                    break;
                case Enums.Ages.AGE_02:
                    switch (unitType)
                    {
                        case Enums.UnitTypes.Warrior:
                            currentWorkingConfig = Age02_WarriorSettingsConfig;
                            break;
                        case Enums.UnitTypes.Asssassin:
                            currentWorkingConfig = Age02_AssassinSettingsConfig;
                            break;
                        case Enums.UnitTypes.Enforcer:
                            currentWorkingConfig = Age02_EnforcerSettingsConfig;
                            break;
                        case Enums.UnitTypes.Tank:
                            currentWorkingConfig = Age02_TankSettingsConfig;
                            break;
                        case Enums.UnitTypes.Battlemage:
                            currentWorkingConfig = Age02_BattlemageSettingsConfig;
                            break;
                        case Enums.UnitTypes.Mass_Healer:
                            currentWorkingConfig = Age02_MassHealerSettingsConfig;
                            break;
                        case Enums.UnitTypes.Ranger:
                            currentWorkingConfig = Age02_RangerSettingsConfig;
                            break;
                        case Enums.UnitTypes.Archdruid:
                            currentWorkingConfig = Age02_ArchdruidSettingsConfig;
                            break;
                        default:
                            GD.PrintErr("Unit type not implemented: UnitSettingsManager.cs (SetCurrentWorkingValues Age2)");
                            currentWorkingConfig = Age02_WarriorSettingsConfig;
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        public override void CreateNewSaveFile(bool overrideSaveFile)
        {
            base.CreateNewSaveFile(overrideSaveFile);

            // Store some values.

            //if adding more units, add them here to save default values.
            /*
             * HOW TO QUICKLY EDIT SELECTIONS HERE:
             * 1. Control + H to open find/replace.
             * 2. Select the region that you want to edit.
             * 3. Fill in the word to replace + what to replace it with.
             * 4. Press enter until all is replaced.
             */

            switch (currentWorkingUnitAge)
            {
                case Enums.Ages.AGE_01:
                    switch (currentWorkingUnitType)
                    {
                        case Enums.UnitTypes.Warrior:

                            config.SetValue(Enums.UnitSettingsConfigHeader.CONFIG_SETTINGS.ToString(), "useCustomVariables", false);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitCost", UnitsDefaultValues.Age01_Warrior_UnitCost);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitAvailableInAge", (int)UnitsDefaultValues.Age01_Warrior_UnitAvailableInAge);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitName", UnitsDefaultValues.Age01_Warrior_UnitName);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitDescription", UnitsDefaultValues.Age01_Warrior_UnitDescription);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitHealth", UnitsDefaultValues.Age01_Warrior_UnitHealth);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitArmour", UnitsDefaultValues.Age01_Warrior_UnitArmour);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitAttack", UnitsDefaultValues.Age01_Warrior_UnitAttack);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitMovementSpeed", UnitsDefaultValues.Age01_Warrior_UnitMovementSpeed);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitDetectionRange", UnitsDefaultValues.Age01_Warrior_UnitDetectionRange);

                            break;
                        case Enums.UnitTypes.Asssassin:

                            config.SetValue(Enums.UnitSettingsConfigHeader.CONFIG_SETTINGS.ToString(), "useCustomVariables", false);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitCost", UnitsDefaultValues.Age01_Assassin_UnitCost);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitAvailableInAge", (int)UnitsDefaultValues.Age01_Assassin_UnitAvailableInAge);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitName", UnitsDefaultValues.Age01_Assassin_UnitName);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitDescription", UnitsDefaultValues.Age01_Assassin_UnitDescription);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitHealth", UnitsDefaultValues.Age01_Assassin_UnitHealth);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitArmour", UnitsDefaultValues.Age01_Assassin_UnitArmour);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitAttack", UnitsDefaultValues.Age01_Assassin_UnitAttack);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitMovementSpeed", UnitsDefaultValues.Age01_Assassin_UnitMovementSpeed);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitDetectionRange", UnitsDefaultValues.Age01_Assassin_UnitDetectionRange);

                            break;
                        case Enums.UnitTypes.Enforcer:

                            config.SetValue(Enums.UnitSettingsConfigHeader.CONFIG_SETTINGS.ToString(), "useCustomVariables", false);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitCost", UnitsDefaultValues.Age01_Enforcer_UnitCost);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitAvailableInAge", (int)UnitsDefaultValues.Age01_Enforcer_UnitAvailableInAge);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitName", UnitsDefaultValues.Age01_Enforcer_UnitName);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitDescription", UnitsDefaultValues.Age01_Enforcer_UnitDescription);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitHealth", UnitsDefaultValues.Age01_Enforcer_UnitHealth);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitArmour", UnitsDefaultValues.Age01_Enforcer_UnitArmour);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitAttack", UnitsDefaultValues.Age01_Enforcer_UnitAttack);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitMovementSpeed", UnitsDefaultValues.Age01_Enforcer_UnitMovementSpeed);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitDetectionRange", UnitsDefaultValues.Age01_Enforcer_UnitDetectionRange);

                            break;
                        case Enums.UnitTypes.Ranger:

                            config.SetValue(Enums.UnitSettingsConfigHeader.CONFIG_SETTINGS.ToString(), "useCustomVariables", false);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitCost", UnitsDefaultValues.Age01_Ranger_UnitCost);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitAvailableInAge", (int)UnitsDefaultValues.Age01_Ranger_UnitAvailableInAge);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitName", UnitsDefaultValues.Age01_Ranger_UnitName);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitDescription", UnitsDefaultValues.Age01_Ranger_UnitDescription);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitHealth", UnitsDefaultValues.Age01_Ranger_UnitHealth);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitArmour", UnitsDefaultValues.Age01_Ranger_UnitArmour);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitAttack", UnitsDefaultValues.Age01_Ranger_UnitAttack);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitMovementSpeed", UnitsDefaultValues.Age01_Ranger_UnitMovementSpeed);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitDetectionRange", UnitsDefaultValues.Age01_Ranger_UnitDetectionRange);

                            break;
                        case Enums.UnitTypes.Tank:

                            config.SetValue(Enums.UnitSettingsConfigHeader.CONFIG_SETTINGS.ToString(), "useCustomVariables", false);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitCost", UnitsDefaultValues.Age01_Tank_UnitCost);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitAvailableInAge", (int)UnitsDefaultValues.Age01_Tank_UnitAvailableInAge);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitName", UnitsDefaultValues.Age01_Tank_UnitName);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitDescription", UnitsDefaultValues.Age01_Tank_UnitDescription);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitHealth", UnitsDefaultValues.Age01_Tank_UnitHealth);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitArmour", UnitsDefaultValues.Age01_Tank_UnitArmour);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitAttack", UnitsDefaultValues.Age01_Tank_UnitAttack);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitMovementSpeed", UnitsDefaultValues.Age01_Tank_UnitMovementSpeed);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitDetectionRange", UnitsDefaultValues.Age01_Tank_UnitDetectionRange);

                            break;
                        case Enums.UnitTypes.Battlemage:

                            config.SetValue(Enums.UnitSettingsConfigHeader.CONFIG_SETTINGS.ToString(), "useCustomVariables", false);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitCost", UnitsDefaultValues.Age01_Battlemage_UnitCost);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitAvailableInAge", (int)UnitsDefaultValues.Age01_Battlemage_UnitAvailableInAge);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitName", UnitsDefaultValues.Age01_Battlemage_UnitName);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitDescription", UnitsDefaultValues.Age01_Battlemage_UnitDescription);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitHealth", UnitsDefaultValues.Age01_Battlemage_UnitHealth);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitArmour", UnitsDefaultValues.Age01_Battlemage_UnitArmour);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitAttack", UnitsDefaultValues.Age01_Battlemage_UnitAttack);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitMovementSpeed", UnitsDefaultValues.Age01_Battlemage_UnitMovementSpeed);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitDetectionRange", UnitsDefaultValues.Age01_Battlemage_UnitDetectionRange);

                            break;
                        case Enums.UnitTypes.Mass_Healer:

                            config.SetValue(Enums.UnitSettingsConfigHeader.CONFIG_SETTINGS.ToString(), "useCustomVariables", false);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitCost", UnitsDefaultValues.Age01_MassHealer_UnitCost);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitAvailableInAge", (int)UnitsDefaultValues.Age01_MassHealer_UnitAvailableInAge);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitName", UnitsDefaultValues.Age01_MassHealer_UnitName);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitDescription", UnitsDefaultValues.Age01_MassHealer_UnitDescription);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitHealth", UnitsDefaultValues.Age01_MassHealer_UnitHealth);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitArmour", UnitsDefaultValues.Age01_MassHealer_UnitArmour);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitAttack", UnitsDefaultValues.Age01_MassHealer_UnitAttack);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitMovementSpeed", UnitsDefaultValues.Age01_MassHealer_UnitMovementSpeed);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitDetectionRange", UnitsDefaultValues.Age01_MassHealer_UnitDetectionRange);

                            break;
                        case Enums.UnitTypes.Archdruid:

                            config.SetValue(Enums.UnitSettingsConfigHeader.CONFIG_SETTINGS.ToString(), "useCustomVariables", false);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitCost", UnitsDefaultValues.Age01_Archdruid_UnitCost);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitAvailableInAge", (int)UnitsDefaultValues.Age01_Archdruid_UnitAvailableInAge);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitName", UnitsDefaultValues.Age01_Archdruid_UnitName);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitDescription", UnitsDefaultValues.Age01_Archdruid_UnitDescription);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitHealth", UnitsDefaultValues.Age01_Archdruid_UnitHealth);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitArmour", UnitsDefaultValues.Age01_Archdruid_UnitArmour);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitAttack", UnitsDefaultValues.Age01_Archdruid_UnitAttack);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitMovementSpeed", UnitsDefaultValues.Age01_Archdruid_UnitMovementSpeed);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitDetectionRange", UnitsDefaultValues.Age01_Archdruid_UnitDetectionRange);

                            break;
                        default:
                            GD.PrintErr("UNIT NOT IMPLEMENTED, UnitSettingsManager");

                            config.SetValue(Enums.UnitSettingsConfigHeader.CONFIG_SETTINGS.ToString(), "useCustomVariables", false);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitCost", UnitsDefaultValues.Age01_Warrior_UnitCost);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitAvailableInAge", (int)UnitsDefaultValues.Age01_Warrior_UnitAvailableInAge);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitName", UnitsDefaultValues.Age01_Warrior_UnitName);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitDescription", UnitsDefaultValues.Age01_Warrior_UnitDescription);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitHealth", UnitsDefaultValues.Age01_Warrior_UnitHealth);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitArmour", UnitsDefaultValues.Age01_Warrior_UnitArmour);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitAttack", UnitsDefaultValues.Age01_Warrior_UnitAttack);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitMovementSpeed", UnitsDefaultValues.Age01_Warrior_UnitMovementSpeed);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitDetectionRange", UnitsDefaultValues.Age01_Warrior_UnitDetectionRange);

                            break;
                    }
                    break;
                case Enums.Ages.AGE_02:
                    switch (currentWorkingUnitType)
                    {
                        case Enums.UnitTypes.Warrior:

                            config.SetValue(Enums.UnitSettingsConfigHeader.CONFIG_SETTINGS.ToString(), "useCustomVariables", false);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitCost", UnitsDefaultValues.Age02_Warrior_UnitCost);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitAvailableInAge", (int)UnitsDefaultValues.Age02_Warrior_UnitAvailableInAge);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitName", UnitsDefaultValues.Age02_Warrior_UnitName);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitDescription", UnitsDefaultValues.Age02_Warrior_UnitDescription);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitHealth", UnitsDefaultValues.Age02_Warrior_UnitHealth);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitArmour", UnitsDefaultValues.Age02_Warrior_UnitArmour);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitAttack", UnitsDefaultValues.Age02_Warrior_UnitAttack);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitMovementSpeed", UnitsDefaultValues.Age02_Warrior_UnitMovementSpeed);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitDetectionRange", UnitsDefaultValues.Age02_Warrior_UnitDetectionRange);

                            break;
                        case Enums.UnitTypes.Asssassin:

                            config.SetValue(Enums.UnitSettingsConfigHeader.CONFIG_SETTINGS.ToString(), "useCustomVariables", false);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitCost", UnitsDefaultValues.Age02_Assassin_UnitCost);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitAvailableInAge", (int)UnitsDefaultValues.Age02_Assassin_UnitAvailableInAge);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitName", UnitsDefaultValues.Age02_Assassin_UnitName);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitDescription", UnitsDefaultValues.Age02_Assassin_UnitDescription);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitHealth", UnitsDefaultValues.Age02_Assassin_UnitHealth);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitArmour", UnitsDefaultValues.Age02_Assassin_UnitArmour);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitAttack", UnitsDefaultValues.Age02_Assassin_UnitAttack);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitMovementSpeed", UnitsDefaultValues.Age02_Assassin_UnitMovementSpeed);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitDetectionRange", UnitsDefaultValues.Age02_Assassin_UnitDetectionRange);

                            break;
                        case Enums.UnitTypes.Enforcer:

                            config.SetValue(Enums.UnitSettingsConfigHeader.CONFIG_SETTINGS.ToString(), "useCustomVariables", false);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitCost", UnitsDefaultValues.Age02_Enforcer_UnitCost);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitAvailableInAge", (int)UnitsDefaultValues.Age02_Enforcer_UnitAvailableInAge);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitName", UnitsDefaultValues.Age02_Enforcer_UnitName);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitDescription", UnitsDefaultValues.Age02_Enforcer_UnitDescription);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitHealth", UnitsDefaultValues.Age02_Enforcer_UnitHealth);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitArmour", UnitsDefaultValues.Age02_Enforcer_UnitArmour);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitAttack", UnitsDefaultValues.Age02_Enforcer_UnitAttack);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitMovementSpeed", UnitsDefaultValues.Age02_Enforcer_UnitMovementSpeed);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitDetectionRange", UnitsDefaultValues.Age02_Enforcer_UnitDetectionRange);

                            break;
                        case Enums.UnitTypes.Ranger:

                            config.SetValue(Enums.UnitSettingsConfigHeader.CONFIG_SETTINGS.ToString(), "useCustomVariables", false);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitCost", UnitsDefaultValues.Age02_Ranger_UnitCost);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitAvailableInAge", (int)UnitsDefaultValues.Age02_Ranger_UnitAvailableInAge);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitName", UnitsDefaultValues.Age02_Ranger_UnitName);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitDescription", UnitsDefaultValues.Age02_Ranger_UnitDescription);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitHealth", UnitsDefaultValues.Age02_Ranger_UnitHealth);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitArmour", UnitsDefaultValues.Age02_Ranger_UnitArmour);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitAttack", UnitsDefaultValues.Age02_Ranger_UnitAttack);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitMovementSpeed", UnitsDefaultValues.Age02_Ranger_UnitMovementSpeed);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitDetectionRange", UnitsDefaultValues.Age02_Ranger_UnitDetectionRange);

                            break;
                        case Enums.UnitTypes.Tank:

                            config.SetValue(Enums.UnitSettingsConfigHeader.CONFIG_SETTINGS.ToString(), "useCustomVariables", false);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitCost", UnitsDefaultValues.Age02_Tank_UnitCost);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitAvailableInAge", (int)UnitsDefaultValues.Age02_Tank_UnitAvailableInAge);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitName", UnitsDefaultValues.Age02_Tank_UnitName);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitDescription", UnitsDefaultValues.Age02_Tank_UnitDescription);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitHealth", UnitsDefaultValues.Age02_Tank_UnitHealth);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitArmour", UnitsDefaultValues.Age02_Tank_UnitArmour);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitAttack", UnitsDefaultValues.Age02_Tank_UnitAttack);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitMovementSpeed", UnitsDefaultValues.Age02_Tank_UnitMovementSpeed);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitDetectionRange", UnitsDefaultValues.Age02_Tank_UnitDetectionRange);

                            break;
                        case Enums.UnitTypes.Battlemage:

                            config.SetValue(Enums.UnitSettingsConfigHeader.CONFIG_SETTINGS.ToString(), "useCustomVariables", false);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitCost", UnitsDefaultValues.Age02_Battlemage_UnitCost);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitAvailableInAge", (int)UnitsDefaultValues.Age02_Battlemage_UnitAvailableInAge);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitName", UnitsDefaultValues.Age02_Battlemage_UnitName);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitDescription", UnitsDefaultValues.Age02_Battlemage_UnitDescription);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitHealth", UnitsDefaultValues.Age02_Battlemage_UnitHealth);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitArmour", UnitsDefaultValues.Age02_Battlemage_UnitArmour);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitAttack", UnitsDefaultValues.Age02_Battlemage_UnitAttack);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitMovementSpeed", UnitsDefaultValues.Age02_Battlemage_UnitMovementSpeed);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitDetectionRange", UnitsDefaultValues.Age02_Battlemage_UnitDetectionRange);

                            break;
                        case Enums.UnitTypes.Mass_Healer:

                            config.SetValue(Enums.UnitSettingsConfigHeader.CONFIG_SETTINGS.ToString(), "useCustomVariables", false);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitCost", UnitsDefaultValues.Age02_MassHealer_UnitCost);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitAvailableInAge", (int)UnitsDefaultValues.Age02_MassHealer_UnitAvailableInAge);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitName", UnitsDefaultValues.Age02_MassHealer_UnitName);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitDescription", UnitsDefaultValues.Age02_MassHealer_UnitDescription);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitHealth", UnitsDefaultValues.Age02_MassHealer_UnitHealth);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitArmour", UnitsDefaultValues.Age02_MassHealer_UnitArmour);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitAttack", UnitsDefaultValues.Age02_MassHealer_UnitAttack);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitMovementSpeed", UnitsDefaultValues.Age02_MassHealer_UnitMovementSpeed);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitDetectionRange", UnitsDefaultValues.Age02_MassHealer_UnitDetectionRange);

                            break;
                        case Enums.UnitTypes.Archdruid:

                            config.SetValue(Enums.UnitSettingsConfigHeader.CONFIG_SETTINGS.ToString(), "useCustomVariables", false);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitCost", UnitsDefaultValues.Age02_Archdruid_UnitCost);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitAvailableInAge", (int)UnitsDefaultValues.Age02_Archdruid_UnitAvailableInAge);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitName", UnitsDefaultValues.Age02_Archdruid_UnitName);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitDescription", UnitsDefaultValues.Age02_Archdruid_UnitDescription);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitHealth", UnitsDefaultValues.Age02_Archdruid_UnitHealth);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitArmour", UnitsDefaultValues.Age02_Archdruid_UnitArmour);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitAttack", UnitsDefaultValues.Age02_Archdruid_UnitAttack);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitMovementSpeed", UnitsDefaultValues.Age02_Archdruid_UnitMovementSpeed);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitDetectionRange", UnitsDefaultValues.Age02_Archdruid_UnitDetectionRange);

                            break;
                        default:
                            GD.PrintErr("UNIT NOT IMPLEMENTED, UnitSettingsManager Age2");

                            config.SetValue(Enums.UnitSettingsConfigHeader.CONFIG_SETTINGS.ToString(), "useCustomVariables", false);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitCost", UnitsDefaultValues.Age02_Warrior_UnitCost);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitAvailableInAge", (int)UnitsDefaultValues.Age02_Warrior_UnitAvailableInAge);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitName", UnitsDefaultValues.Age02_Warrior_UnitName);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitDescription", UnitsDefaultValues.Age02_Warrior_UnitDescription);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitHealth", UnitsDefaultValues.Age02_Warrior_UnitHealth);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitArmour", UnitsDefaultValues.Age02_Warrior_UnitArmour);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitAttack", UnitsDefaultValues.Age02_Warrior_UnitAttack);

                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitMovementSpeed", UnitsDefaultValues.Age02_Warrior_UnitMovementSpeed);
                            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitDetectionRange", UnitsDefaultValues.Age02_Warrior_UnitDetectionRange);

                            break;
                    }
                    break;
                default:
                    GD.PrintErr("CONFIG INFO NOT FOUND " + currentWorkingUnitAge + " _ " + currentWorkingUnitType);
                    break;
            }

            // Save it to a file.
            config.Save(fullFilePath);

            //Reload all unit settings
            GameSettingsLoader.Instance.InitializeAllUnits();
        }
    }
}
