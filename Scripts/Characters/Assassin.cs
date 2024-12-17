using Erikduss;
using Godot;
using System;

public partial class Assassin : BaseCharacter
{
    public override void _Ready()
    {
        //Load Unit Stats

        UnitSettingsConfig loadedUnitSettings;

        //select the correct config file.
        switch (currentAge)
        {
            case Enums.Ages.AGE_01:
                loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age01_AssassinSettingsConfig;
                break;
            case Enums.Ages.AGE_02:
                loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age02_AssassinSettingsConfig;
                break;
            default:
                loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age01_AssassinSettingsConfig;
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
            switch(currentAge)
            {
                case Enums.Ages.AGE_01:
                    currentHealth = UnitsDefaultValues.Age01_Assassin_UnitHealth;
                    maxHealth = UnitsDefaultValues.Age01_Assassin_UnitHealth;

                    unitArmor = UnitsDefaultValues.Age01_Assassin_UnitArmour;
                    unitAttackDamage = UnitsDefaultValues.Age01_Assassin_UnitAttack;

                    detectionRange = UnitsDefaultValues.Age01_Assassin_UnitDetectionRange;
                    movementSpeed = UnitsDefaultValues.Age01_Assassin_UnitMovementSpeed;
                    break;
                case Enums.Ages.AGE_02:
                    currentHealth = UnitsDefaultValues.Age02_Assassin_UnitHealth;
                    maxHealth = UnitsDefaultValues.Age02_Assassin_UnitHealth;

                    unitArmor = UnitsDefaultValues.Age02_Assassin_UnitArmour;
                    unitAttackDamage = UnitsDefaultValues.Age02_Assassin_UnitAttack;

                    detectionRange = UnitsDefaultValues.Age02_Assassin_UnitDetectionRange;
                    movementSpeed = UnitsDefaultValues.Age02_Assassin_UnitMovementSpeed;
                    break;
                default:
                    GD.Print("AGE NOT IMPLEMENTED: Assassin.cs");

                    currentHealth = UnitsDefaultValues.Age01_Assassin_UnitHealth;
                    maxHealth = UnitsDefaultValues.Age01_Assassin_UnitHealth;

                    unitArmor = UnitsDefaultValues.Age01_Assassin_UnitArmour;
                    unitAttackDamage = UnitsDefaultValues.Age01_Assassin_UnitAttack;

                    detectionRange = UnitsDefaultValues.Age01_Assassin_UnitDetectionRange;
                    movementSpeed = UnitsDefaultValues.Age01_Assassin_UnitMovementSpeed;
                    break;
            }
        }
        

        unitType = Enums.UnitTypes.Asssassin;

        base._Ready();
    }
}
