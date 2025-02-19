using Erikduss;
using Godot;
using System;

namespace Erikduss
{
	public partial class Shaman : BaseCharacter
	{
        //This unit is called "Shaman"

        public override void _Ready()
        {
            //Load Unit Stats

            UnitSettingsConfig loadedUnitSettings;

            //select the correct config file.
            switch (currentAge)
            {
                case Enums.Ages.AGE_01:
                    loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age01_ShamanSettingsConfig;
                    break;
                case Enums.Ages.AGE_02:
                    loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age02_ShamanSettingsConfig;
                    break;
                default:
                    loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age01_ShamanSettingsConfig;
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
                        currentHealth = UnitsDefaultValues.Age01_Shaman_UnitHealth;
                        maxHealth = UnitsDefaultValues.Age01_Shaman_UnitHealth;

                        unitArmor = UnitsDefaultValues.Age01_Shaman_UnitArmour;
                        unitAttackDamage = UnitsDefaultValues.Age01_Shaman_UnitAttack;

                        detectionRange = UnitsDefaultValues.Age01_Shaman_UnitDetectionRange;
                        movementSpeed = UnitsDefaultValues.Age01_Shaman_UnitMovementSpeed;
                        break;
                    case Enums.Ages.AGE_02:
                        currentHealth = UnitsDefaultValues.Age02_Shaman_UnitHealth;
                        maxHealth = UnitsDefaultValues.Age02_Shaman_UnitHealth;

                        unitArmor = UnitsDefaultValues.Age02_Shaman_UnitArmour;
                        unitAttackDamage = UnitsDefaultValues.Age02_Shaman_UnitAttack;

                        detectionRange = UnitsDefaultValues.Age02_Shaman_UnitDetectionRange;
                        movementSpeed = UnitsDefaultValues.Age02_Shaman_UnitMovementSpeed;
                        break;
                    default:
                        GD.Print("AGE NOT IMPLEMENTED: Shaman.cs");

                        currentHealth = UnitsDefaultValues.Age01_Shaman_UnitHealth;
                        maxHealth = UnitsDefaultValues.Age01_Shaman_UnitHealth;

                        unitArmor = UnitsDefaultValues.Age01_Shaman_UnitArmour;
                        unitAttackDamage = UnitsDefaultValues.Age01_Shaman_UnitAttack;

                        detectionRange = UnitsDefaultValues.Age01_Shaman_UnitDetectionRange;
                        movementSpeed = UnitsDefaultValues.Age01_Shaman_UnitMovementSpeed;
                        break;
                }
            }

            unitType = Enums.UnitTypes.Shaman;

            isRangedCharacter = true;

            if (characterOwner == Enums.TeamOwner.TEAM_01)
            {
                EffectsAndProjectilesSpawner.Instance.team01AbilityEmpowerAmount++;
            }
            else
            {
                EffectsAndProjectilesSpawner.Instance.team02AbilityEmpowerAmount++;
            }

            base._Ready();
        }

        public override void processDeath()
        {
            base.processDeath();

            if(characterOwner == Enums.TeamOwner.TEAM_01)
            {
                EffectsAndProjectilesSpawner.Instance.team01AbilityEmpowerAmount--;
            }
            else
            {
                EffectsAndProjectilesSpawner.Instance.team02AbilityEmpowerAmount--;
            }

            GameManager.Instance.inGameHUDManager.UpdatePlayerAbilityEmpowerAmount(characterOwner);
        }
    }
}
