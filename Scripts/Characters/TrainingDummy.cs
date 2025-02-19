using Erikduss;
using Godot;
using System;

public partial class TrainingDummy : BaseCharacter
{
    [Export] Label currentHealthLabel;

    public override void _Ready()
    {
        //Set the values
        currentHealth = 20;
        maxHealth = 20;

        if(currentHealthLabel != null) currentHealthLabel.Text = currentHealth.ToString();

        unitArmor = 0;
        unitAttackDamage = 0;

        detectionRange = 0;
        movementSpeed = 0;

        characterOwner = Enums.TeamOwner.TEAM_02;
        currentAge = Enums.Ages.AGE_01;

        unitType = Enums.UnitTypes.TrainingDummy;

        base._Ready();

        //uniqueID = GameManager.Instance.unitsSpawner.AddTrainingDummyToAliveDictionary(characterOwner, this);
    }

    public override void TakeDamage(int rawDamage)
    {
        base.TakeDamage(rawDamage);

        currentHealthLabel.Text = currentHealth.ToString();

        EffectsAndProjectilesSpawner.Instance.SpawnDummyTakenDamageNumber(this, rawDamage);
    }

    public override void processDeath()
    {
        base.processDeath();

        currentHealthLabel.Visible = false;
    }
}
