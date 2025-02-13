using Erikduss;
using Godot;
using System;

namespace Erikduss
{
	public partial class Mass_Healer : BaseCharacter
	{
        //This unit is called "Mass_Healer"

        public override void _Ready()
        {
            //Load Unit Stats

            UnitSettingsConfig loadedUnitSettings;

            //select the correct config file.
            switch (currentAge)
            {
                case Enums.Ages.AGE_01:
                    loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age01_MassHealerSettingsConfig;
                    break;
                case Enums.Ages.AGE_02:
                    loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age02_MassHealerSettingsConfig;
                    break;
                default:
                    loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age01_MassHealerSettingsConfig;
                    break;
            }

            if (loadedUnitSettings.useCustomVariables)
            {
                //Set the values
                currentHealth = loadedUnitSettings.unitHealth;
                maxHealth = loadedUnitSettings.unitHealth;

                unitArmor = loadedUnitSettings.unitArmour;
                unitAttackDamage = loadedUnitSettings.unitAttack;

                detectionRange = loadedUnitSettings.unitDetectionRange;
                movementSpeed = loadedUnitSettings.unitMovementSpeed;
            }
            else
            {
                //Set the default values
                switch (currentAge)
                {
                    case Enums.Ages.AGE_01:
                        currentHealth = UnitsDefaultValues.Age01_MassHealer_UnitHealth;
                        maxHealth = UnitsDefaultValues.Age01_MassHealer_UnitHealth;

                        unitArmor = UnitsDefaultValues.Age01_MassHealer_UnitArmour;
                        unitAttackDamage = UnitsDefaultValues.Age01_MassHealer_UnitAttack;

                        detectionRange = UnitsDefaultValues.Age01_MassHealer_UnitDetectionRange;
                        movementSpeed = UnitsDefaultValues.Age01_MassHealer_UnitMovementSpeed;
                        break;
                    case Enums.Ages.AGE_02:
                        currentHealth = UnitsDefaultValues.Age02_MassHealer_UnitHealth;
                        maxHealth = UnitsDefaultValues.Age02_MassHealer_UnitHealth;

                        unitArmor = UnitsDefaultValues.Age02_MassHealer_UnitArmour;
                        unitAttackDamage = UnitsDefaultValues.Age02_MassHealer_UnitAttack;

                        detectionRange = UnitsDefaultValues.Age02_MassHealer_UnitDetectionRange;
                        movementSpeed = UnitsDefaultValues.Age02_MassHealer_UnitMovementSpeed;
                        break;
                    default:
                        GD.Print("AGE NOT IMPLEMENTED: MassHealer.cs");

                        currentHealth = UnitsDefaultValues.Age01_MassHealer_UnitHealth;
                        maxHealth = UnitsDefaultValues.Age01_MassHealer_UnitHealth;

                        unitArmor = UnitsDefaultValues.Age01_MassHealer_UnitArmour;
                        unitAttackDamage = UnitsDefaultValues.Age01_MassHealer_UnitAttack;

                        detectionRange = UnitsDefaultValues.Age01_MassHealer_UnitDetectionRange;
                        movementSpeed = UnitsDefaultValues.Age01_MassHealer_UnitMovementSpeed;
                        break;
                }
            }

            unitType = Enums.UnitTypes.Mass_Healer;

            isRangedCharacter = true;
            checkForAlliesRaycastInstead = true;

            base._Ready();

            GD.Print("READY");
        }

        public override void SignalUnitHasTakenDamage(BaseCharacter unitThatTookDamage)
        {
            base.SignalUnitHasTakenDamage(unitThatTookDamage);


        }
    }
}
