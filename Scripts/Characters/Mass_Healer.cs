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
                loadDefaultValues = false;

                //Set the values
                currentHealth = loadedUnitSettings.unitHealth;
                maxHealth = loadedUnitSettings.unitHealth;

                unitArmor = loadedUnitSettings.unitArmour;
                unitAttackDamage = loadedUnitSettings.unitAttack;

                detectionRange = loadedUnitSettings.unitDetectionRange;
                movementSpeed = loadedUnitSettings.unitMovementSpeed;
            }

            unitType = Enums.UnitTypes.Mass_Healer;

            isRangedCharacter = true;
            checkForAlliesRaycastInstead = true;

            base._Ready();

            EffectsAndProjectilesSpawner.Instance.SpawnMass_Healer_HealingEffect(this, false);
        }

        //public override void SignalUnitHasTakenDamage(BaseCharacter unitThatTookDamage)
        //{
        //    base.SignalUnitHasTakenDamage(unitThatTookDamage);


        //}
    }
}
