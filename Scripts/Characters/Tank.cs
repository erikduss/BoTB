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
            loadDefaultValues = false;

            //Set the values
            currentHealth = loadedUnitSettings.unitHealth;
            maxHealth = loadedUnitSettings.unitHealth;

            unitArmor = loadedUnitSettings.unitArmour;
            unitAttackDamage = loadedUnitSettings.unitAttack;

            detectionRange = loadedUnitSettings.unitDetectionRange;
            movementSpeed = loadedUnitSettings.unitMovementSpeed;
        }

        unitType = Enums.UnitTypes.Tank;

        base._Ready();
    }
}
