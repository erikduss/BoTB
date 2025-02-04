using Erikduss;
using Godot;
using System;

namespace Erikduss
{
	public partial class Ranger : BaseCharacter
	{
        //This unit is called "Ranger"

        public bool rangerBuffActive = false;
        public float buffAttackSpeed = 0.5f;

        private float buffDuration = 2f;
        private float buffTimer = 0f;

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
                        currentHealth = UnitsDefaultValues.Age01_Ranger_UnitHealth;
                        maxHealth = UnitsDefaultValues.Age01_Ranger_UnitHealth;

                        unitArmor = UnitsDefaultValues.Age01_Ranger_UnitArmour;
                        unitAttackDamage = UnitsDefaultValues.Age01_Ranger_UnitAttack;

                        detectionRange = UnitsDefaultValues.Age01_Ranger_UnitDetectionRange;
                        movementSpeed = UnitsDefaultValues.Age01_Ranger_UnitMovementSpeed;
                        break;
                    case Enums.Ages.AGE_02:
                        currentHealth = UnitsDefaultValues.Age02_Ranger_UnitHealth;
                        maxHealth = UnitsDefaultValues.Age02_Ranger_UnitHealth;

                        unitArmor = UnitsDefaultValues.Age02_Ranger_UnitArmour;
                        unitAttackDamage = UnitsDefaultValues.Age02_Ranger_UnitAttack;

                        detectionRange = UnitsDefaultValues.Age02_Ranger_UnitDetectionRange;
                        movementSpeed = UnitsDefaultValues.Age02_Ranger_UnitMovementSpeed;
                        break;
                    default:
                        GD.Print("AGE NOT IMPLEMENTED: Ranger.cs");

                        currentHealth = UnitsDefaultValues.Age01_Ranger_UnitHealth;
                        maxHealth = UnitsDefaultValues.Age01_Ranger_UnitHealth;

                        unitArmor = UnitsDefaultValues.Age01_Ranger_UnitArmour;
                        unitAttackDamage = UnitsDefaultValues.Age01_Ranger_UnitAttack;

                        detectionRange = UnitsDefaultValues.Age01_Ranger_UnitDetectionRange;
                        movementSpeed = UnitsDefaultValues.Age01_Ranger_UnitMovementSpeed;
                        break;
                }
            }

            unitType = Enums.UnitTypes.Ranger;

            isRangedCharacter = true;

            base._Ready();
        }

        public override void _Process(double delta)
        {
            base._Process(delta);

            if (rangerBuffActive)
            {
                if(buffTimer > buffDuration)
                {
                    rangerBuffActive = false;

                    if (hasActiveTankBuff)
                    {
                        SetTankBuffedAttackSpeedValue(true);
                    }
                    currentAttackCooldownDuration = -1f;
                }
                else
                {
                    buffTimer += (float)delta;
                }
            }
        }

        public override void DealDamage()
        {
            if(currentTarget != null)
            {
                //we need to be signaled by this unit since we damaged it!
                currentTarget.unitsThatNeedToBeSignaledOnDeath.Add(this);
            }

            base.DealDamage();
        }

        public override void UnitSignaledForDeathEvent()
        {
            base.UnitSignaledForDeathEvent();

            rangerBuffActive = true;

            //reset buff timer to reset the buff
            buffTimer = 0f;

            if (hasActiveTankBuff)
            {
                currentAttackCooldownDuration = buffAttackSpeed;
                SetTankBuffedAttackSpeedValue();
            }
            else currentAttackCooldownDuration = buffAttackSpeed;
        }
    }
}
