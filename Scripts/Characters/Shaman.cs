using Erikduss;
using Godot;
using System;

namespace Erikduss
{
	public partial class Shaman : BaseCharacter
	{
        //This unit is called "Shaman"
        bool processedDeathEvent = false;
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
                loadDefaultValues = false;

                //Set the values
                currentHealth = loadedUnitSettings.unitHealth;
                maxHealth = loadedUnitSettings.unitHealth;

                unitArmor = loadedUnitSettings.unitArmour;
                unitAttackDamage = loadedUnitSettings.unitAttack;

                detectionRange = loadedUnitSettings.unitDetectionRange;
                movementSpeed = loadedUnitSettings.unitMovementSpeed;
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

            if (processedDeathEvent) return;

            processedDeathEvent = true;

            if (characterOwner == Enums.TeamOwner.TEAM_01)
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
