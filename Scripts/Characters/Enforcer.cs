using Erikduss;
using Godot;
using System;

public partial class Enforcer : BaseCharacter
{
    public override void _Ready()
    {
        //Load Unit Stats

        UnitSettingsConfig loadedUnitSettings;

        //select the correct config file.
        switch (currentAge)
        {
            case Enums.Ages.AGE_01:
                loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age01_EnforcerSettingsConfig;
                break;
            case Enums.Ages.AGE_02:
                loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age02_EnforcerSettingsConfig;
                break;
            default:
                loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age01_EnforcerSettingsConfig;
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
                    currentHealth = UnitsDefaultValues.Age01_Enforcer_UnitHealth;
                    maxHealth = UnitsDefaultValues.Age01_Enforcer_UnitHealth;

                    unitArmor = UnitsDefaultValues.Age01_Enforcer_UnitArmour;
                    unitAttackDamage = UnitsDefaultValues.Age01_Enforcer_UnitAttack;

                    detectionRange = UnitsDefaultValues.Age01_Enforcer_UnitDetectionRange;
                    movementSpeed = UnitsDefaultValues.Age01_Enforcer_UnitMovementSpeed;
                    break;
                case Enums.Ages.AGE_02:
                    currentHealth = UnitsDefaultValues.Age02_Enforcer_UnitHealth;
                    maxHealth = UnitsDefaultValues.Age02_Enforcer_UnitHealth;

                    unitArmor = UnitsDefaultValues.Age02_Enforcer_UnitArmour;
                    unitAttackDamage = UnitsDefaultValues.Age02_Enforcer_UnitAttack;

                    detectionRange = UnitsDefaultValues.Age02_Enforcer_UnitDetectionRange;
                    movementSpeed = UnitsDefaultValues.Age02_Enforcer_UnitMovementSpeed;
                    break;
                default:
                    GD.Print("AGE NOT IMPLEMENTED: Enforcer.cs");

                    currentHealth = UnitsDefaultValues.Age01_Enforcer_UnitHealth;
                    maxHealth = UnitsDefaultValues.Age01_Enforcer_UnitHealth;

                    unitArmor = UnitsDefaultValues.Age01_Enforcer_UnitArmour;
                    unitAttackDamage = UnitsDefaultValues.Age01_Enforcer_UnitAttack;

                    detectionRange = UnitsDefaultValues.Age01_Enforcer_UnitDetectionRange;
                    movementSpeed = UnitsDefaultValues.Age01_Enforcer_UnitMovementSpeed;
                    break;
            }
        }


        unitType = Enums.UnitTypes.Enforcer;

        base._Ready();
    }
}
