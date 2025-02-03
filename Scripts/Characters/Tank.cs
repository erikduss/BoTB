using Erikduss;
using Godot;
using System;

public partial class Tank : BaseCharacter
{
    public override void _Ready()
    {
        //Load Unit Stats

        UnitSettingsConfig loadedUnitSettings;

        //select the correct config file.
        switch (currentAge)
        {
            case Enums.Ages.AGE_01:
                loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age01_TankSettingsConfig;
                break;
            case Enums.Ages.AGE_02:
                loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age02_TankSettingsConfig;
                break;
            default:
                loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age01_TankSettingsConfig;
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
                    currentHealth = UnitsDefaultValues.Age01_Tank_UnitHealth;
                    maxHealth = UnitsDefaultValues.Age01_Tank_UnitHealth;

                    unitArmor = UnitsDefaultValues.Age01_Tank_UnitArmour;
                    unitAttackDamage = UnitsDefaultValues.Age01_Tank_UnitAttack;

                    detectionRange = UnitsDefaultValues.Age01_Tank_UnitDetectionRange;
                    movementSpeed = UnitsDefaultValues.Age01_Tank_UnitMovementSpeed;
                    break;
                case Enums.Ages.AGE_02:
                    currentHealth = UnitsDefaultValues.Age02_Tank_UnitHealth;
                    maxHealth = UnitsDefaultValues.Age02_Tank_UnitHealth;

                    unitArmor = UnitsDefaultValues.Age02_Tank_UnitArmour;
                    unitAttackDamage = UnitsDefaultValues.Age02_Tank_UnitAttack;

                    detectionRange = UnitsDefaultValues.Age02_Tank_UnitDetectionRange;
                    movementSpeed = UnitsDefaultValues.Age02_Tank_UnitMovementSpeed;
                    break;
                default:
                    GD.Print("AGE NOT IMPLEMENTED: Tank.cs");

                    currentHealth = UnitsDefaultValues.Age01_Tank_UnitHealth;
                    maxHealth = UnitsDefaultValues.Age01_Tank_UnitHealth;

                    unitArmor = UnitsDefaultValues.Age01_Tank_UnitArmour;
                    unitAttackDamage = UnitsDefaultValues.Age01_Tank_UnitAttack;

                    detectionRange = UnitsDefaultValues.Age01_Tank_UnitDetectionRange;
                    movementSpeed = UnitsDefaultValues.Age01_Tank_UnitMovementSpeed;
                    break;
            }
        }


        unitType = Enums.UnitTypes.Tank;

        base._Ready();
    }
}
