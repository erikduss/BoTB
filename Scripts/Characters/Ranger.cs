using Erikduss;
using Godot;
using System;

namespace Erikduss
{
	public partial class Ranger : BaseCharacter
	{
        //This unit is called "Ranger"

        public override void _Ready()
        {
            //Load Unit Stats

            UnitSettingsConfig loadedUnitSettings;

            //select the correct config file.
            switch (currentAge)
            {
                case Enums.Ages.AGE_01:
                    loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age01_RangerSettingsConfig;
                    break;
                case Enums.Ages.AGE_02:
                    loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age02_RangerSettingsConfig;
                    break;
                default:
                    loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age01_RangerSettingsConfig;
                    break;
            }

            //Set the values
            currentHealth = loadedUnitSettings.unitHealth;
            maxHealth = loadedUnitSettings.unitHealth;

            unitArmor = loadedUnitSettings.unitArmour;
            unitAttackDamage = loadedUnitSettings.unitAttack;

            detectionRange = loadedUnitSettings.unitDetectionRange;
            movementSpeed = loadedUnitSettings.unitMovementSpeed;

            unitType = Enums.UnitTypes.Ranger;

            isRangedCharacter = true;

            base._Ready();
        }
    }
}
