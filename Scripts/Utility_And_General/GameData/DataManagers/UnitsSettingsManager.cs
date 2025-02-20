using Godot;
using System;
using System.Linq;

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
        public UnitSettingsConfig Age01_ShamanSettingsConfig = new UnitSettingsConfig();

        //Age 02
        public UnitSettingsConfig Age02_WarriorSettingsConfig = new UnitSettingsConfig();
        public UnitSettingsConfig Age02_RangerSettingsConfig = new UnitSettingsConfig();
        public UnitSettingsConfig Age02_AssassinSettingsConfig = new UnitSettingsConfig();
        public UnitSettingsConfig Age02_TankSettingsConfig = new UnitSettingsConfig();
        public UnitSettingsConfig Age02_BattlemageSettingsConfig = new UnitSettingsConfig();
        public UnitSettingsConfig Age02_MassHealerSettingsConfig = new UnitSettingsConfig();
        public UnitSettingsConfig Age02_EnforcerSettingsConfig = new UnitSettingsConfig();
        public UnitSettingsConfig Age02_ArchdruidSettingsConfig = new UnitSettingsConfig();
        public UnitSettingsConfig Age02_ShamanSettingsConfig = new UnitSettingsConfig();

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
                        case Enums.UnitTypes.Assassin:
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
                        case Enums.UnitTypes.Shaman:
                            currentWorkingConfig = Age01_ShamanSettingsConfig;
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
                        case Enums.UnitTypes.Assassin:
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
                        case Enums.UnitTypes.Shaman:
                            currentWorkingConfig = Age02_ShamanSettingsConfig;
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

            if (currentWorkingUnitType == Enums.UnitTypes.TrainingDummy) return;

            GD.Print(currentWorkingUnitAge.ToString() + "_" + currentWorkingUnitType.ToString());

            UnitSettingsConfig unitDefaultSettings = UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == (currentWorkingUnitAge.ToString() + "_" + currentWorkingUnitType.ToString())).FirstOrDefault().Value;

            config.SetValue(Enums.UnitSettingsConfigHeader.CONFIG_SETTINGS.ToString(), "useCustomVariables", false);

            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitCost", unitDefaultSettings.unitCost);
            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitAvailableInAge", (int)unitDefaultSettings.unitAvailableInAge);
            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitName", unitDefaultSettings.unitName);
            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_INFO.ToString(), "unitDescription", unitDefaultSettings.unitDescription);

            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitHealth", unitDefaultSettings.unitHealth);
            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitArmour", unitDefaultSettings.unitArmour);
            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_STATS.ToString(), "unitAttack", unitDefaultSettings.unitAttack);

            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitMovementSpeed", unitDefaultSettings.unitMovementSpeed);
            config.SetValue(Enums.UnitSettingsConfigHeader.UNIT_GAMEPLAY_VARIABLES.ToString(), "unitDetectionRange", unitDefaultSettings.unitDetectionRange);

            // Save it to a file.
            config.Save(fullFilePath);

            //Reload all unit settings
            GameSettingsLoader.Instance.InitializeAllUnits();
        }
    }
}
