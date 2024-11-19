using Godot;
using Godot.Collections;
using System;

namespace Erikduss
{
	public partial class SimpleSoldier : BaseCharacter
	{
        //This unit is called "Warrior"

        public override void _Ready()
        {
            //Load Unit Stats

            UnitSettingsConfig loadedUnitSettings;

            //select the correct config file.
            switch (currentAge)
            {
                case Enums.Ages.AGE_01:
                    loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age01_WarriorSettingsConfig;
                    break;
                case Enums.Ages.AGE_02:
                    loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age02_WarriorSettingsConfig;
                    break;
                default:
                    loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age01_WarriorSettingsConfig;
                    break;
            }

            //Set the values
            currentHealth = loadedUnitSettings.unitHealth;
            maxHealth = loadedUnitSettings.unitHealth;

            unitArmor = loadedUnitSettings.unitArmour;
            unitAttackDamage = loadedUnitSettings.unitAttack;

            detectionRange = loadedUnitSettings.unitDetectionRange;
            movementSpeed = loadedUnitSettings.unitMovementSpeed;

            unitType = Enums.UnitTypes.Warrior;

            base._Ready();
        }

        public override void DealDamage()
        {
            //base.DealDamage();

            if (currentTarget == null) return;

            if (isDead && !canStillDamage) return; //we cant deal damage if we are dead.

            //add any multipliers here
            int damage = unitAttackDamage;

            currentTarget.TakeDamage(damage);

            System.Collections.Generic.Dictionary<string, BaseCharacter> dictionaryToSearch;

            if(characterOwner == Enums.TeamOwner.TEAM_01)
            {
                dictionaryToSearch = GameManager.Instance.unitsSpawner.team02AliveUnitDictionary;
            }
            else
            {
                dictionaryToSearch = GameManager.Instance.unitsSpawner.team01AliveUnitDictionary;
            }

            foreach (BaseCharacter oppositeTeamUnit in dictionaryToSearch.Values)
            {
                float distance = oppositeTeamUnit.GlobalPosition.X - GlobalPosition.X;

                if (distance < 0) distance = -distance;

                if (distance > 144) continue; //chose 144 due to characters being 64x64, plus keeping some margin of error.

                if (characterOwner == Enums.TeamOwner.TEAM_01)
                {
                    //This should not be possible, but we check if the unit is behind the unit or not.
                    if (oppositeTeamUnit.GlobalPosition.X < GlobalPosition.X) continue;
                }
                else
                {
                    if (oppositeTeamUnit.GlobalPosition.X > GlobalPosition.X) continue;
                }

                //Make sure this is not the same target as we just attacked.
                if (currentTarget == oppositeTeamUnit) continue;
                if (currentTarget.uniqueID == oppositeTeamUnit.uniqueID) continue;

                oppositeTeamUnit.TakeDamage(damage);
            }
        }
    }
}
