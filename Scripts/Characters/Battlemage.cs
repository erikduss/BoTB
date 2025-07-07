using Erikduss;
using Godot;
using System;

namespace Erikduss
{
	public partial class Battlemage : BaseCharacter
	{
        //This unit is called "Battlemage"

        public int amountOfBasicAttacksPerformed = 0;

        public override void _Ready()
        {
            if (GameManager.Instance.isMultiplayerMatch && !GameManager.Instance.isHostOfMultiplayerMatch)
            {
                base._Ready();
                return;
            }

            //Load Unit Stats

            UnitSettingsConfig loadedUnitSettings;

            //select the correct config file.
            switch (currentAge)
            {
                case Enums.Ages.AGE_01:
                    loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age01_BattlemageSettingsConfig;
                    break;
                case Enums.Ages.AGE_02:
                    loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age02_BattlemageSettingsConfig;
                    break;
                default:
                    loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age01_BattlemageSettingsConfig;
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

            unitType = Enums.UnitTypes.Battlemage;

            isRangedCharacter = true;

            base._Ready();
        }
    }
}
