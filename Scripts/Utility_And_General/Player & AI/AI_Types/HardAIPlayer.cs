using Godot;
using System;
using System.Linq;

namespace Erikduss
{
	public partial class HardAIPlayer : BaseAIPlayer
	{
        /* The normal AI (we just want to press buttons, nothing else)
		 * 
		 * We buy units at random, meaning that we will try to buy a new unit every few seconds, which is a different one every time.
		 * We randomize the buy cooldown timer so we dont buy a unit at exact intervals everytime.
		 * Every unit buy attempt has a chance to instead be a refresh button press. (If we have 2 or more druids in our shop, we refresh guarenteed)
		 * 
		 */

        public override void _Ready()
        {
            abilityActivationChance = 50; //we have a 50% chance when there's a certain amount of enemies.
            ageUpgradeChance = 75;
            redeemPowerUpAttemptCooldown = 5;
            activationAttemptCooldown = 10;
            unitBuyAttemptCooldown = 5;
            comboBuyChance = 20;
            refreshChance = 15;

            minimumAmountOfEnemiesRequiredToUseAbility = 3;
            minimumAmountOfUnitDifferenctRequiredToSaveCurrency = 2;

            base._Ready();
        }

        protected override void BuyUnitAttemptSuccess()
        {
            //we want to specifically think about which unit we want to buy from out shop.

            //we want to first try to buy a tank type unit, enforcer, tank or druid.
            //behind it we want 2 ranged units, battlemage, ranger or druid.
            //as extra we might want a healer.

            //we only want to buy the whole combo if the enemy is far enough away.

            bool weBoughtUnitSmart = false;

            float closestEnemyDistanceFromHomebase = Mathf.Inf;
            Enums.UnitTypes closestUnitType = Enums.UnitTypes.TrainingDummy;

            foreach (BaseCharacter unit in GameManager.Instance.unitsSpawner.team01AliveUnitDictionary.Values)
            {
                float distance = -(unit.GlobalPosition.X - GameManager.Instance.team02HomeBase.GlobalPosition.X);

                if (distance < closestEnemyDistanceFromHomebase)
                {
                    closestEnemyDistanceFromHomebase = distance;
                    closestUnitType = unit.unitType;
                }
            }

            if (closestUnitType == Enums.UnitTypes.Enforcer || closestUnitType == Enums.UnitTypes.Tank)
            {
                //we want a warrior to hit through
                if (currentUnitsInShop.Contains(Enums.UnitTypes.Warrior))
                {
                    if (GameManager.Instance.unitsSpawner.team02AliveUnitDictionary.Count == 0 || GameManager.Instance.unitsSpawner.team02AliveUnitDictionary.Values.Where(x => x.unitType == Enums.UnitTypes.Warrior).First() == null)
                    {
                        //we dont have a warrior yet.
                        int indexOfWarrior = currentUnitsInShop.IndexOf(Enums.UnitTypes.Warrior);
                        BuyUnitFromShop(indexOfWarrior);
                        weBoughtUnitSmart = true;
                    }
                    //otherwise we try to create a strong combo of units after.
                }
            }

            if (currentUnitsInShop.Contains(Enums.UnitTypes.Enforcer) || currentUnitsInShop.Contains(Enums.UnitTypes.Tank))
            {
                //we have a defensive option.
                if (closestEnemyDistanceFromHomebase < 250f)
                {
                    if (GameManager.Instance.unitsSpawner.team02AliveUnitDictionary.Count == 0 || GameManager.Instance.unitsSpawner.team02AliveUnitDictionary.Values.Where
                        (x => (x.unitType == Enums.UnitTypes.Tank) || (x.unitType == Enums.UnitTypes.Enforcer)).First() == null)
                    {
                        //we have to spend money cus an enemy is close.
                        BuyTankFromCurrentShop();
                        weBoughtUnitSmart = true;
                    }
                }
                else if (playerCurrentCurrencyAmount > 50) //we only want to spend this amount of money if we have a good combo.
                {
                    //we already know we have a tank, now we should check if we have a ranged unit.

                    if (currentUnitsInShop.Contains(Enums.UnitTypes.Shaman) || currentUnitsInShop.Contains(Enums.UnitTypes.Battlemage) || 
                        currentUnitsInShop.Contains(Enums.UnitTypes.Ranger))
                    {
                        BuyTankFromCurrentShop();
                        BuyRangedUnitFromCurrentShop();
                        weBoughtUnitSmart = true;
                    }
                }
            }

            //we buy a random one if we cant buy smart.
            if (!weBoughtUnitSmart)
            {
                base.BuyUnitAttemptSuccess();
            }
        }

        private void BuyTankFromCurrentShop()
        {
            //we have to spend money cus an enemy is close.
            bool hasTank = currentUnitsInShop.Contains(Enums.UnitTypes.Tank);
            bool hasEnforcer = currentUnitsInShop.Contains(Enums.UnitTypes.Enforcer);

            if (hasTank)
            {
                int indexOfTank = currentUnitsInShop.IndexOf(Enums.UnitTypes.Tank);
                BuyUnitFromShop(indexOfTank);
            }
            else if (hasEnforcer)
            {
                int indexOfEnforcer = currentUnitsInShop.IndexOf(Enums.UnitTypes.Enforcer);
                BuyUnitFromShop(indexOfEnforcer);
            }
        }

        private void BuyRangedUnitFromCurrentShop()
        {
            //we have to spend money cus an enemy is close.
            bool hasRanger = currentUnitsInShop.Contains(Enums.UnitTypes.Ranger);
            bool hasBattlemage = currentUnitsInShop.Contains(Enums.UnitTypes.Battlemage);
            bool hasShaman = currentUnitsInShop.Contains(Enums.UnitTypes.Shaman);

            if((hasRanger && hasBattlemage) || (hasRanger && hasShaman) || (hasShaman && hasBattlemage))
            {
                //we can actually buy 2
                if (hasRanger && hasBattlemage)
                {
                    int indexOfUnit = currentUnitsInShop.IndexOf(Enums.UnitTypes.Battlemage);
                    BuyUnitFromShop(indexOfUnit);

                    int indexOfUnit1 = currentUnitsInShop.IndexOf(Enums.UnitTypes.Ranger);
                    BuyUnitFromShop(indexOfUnit1);
                }
                else if (hasRanger && hasShaman)
                {
                    int indexOfUnit = currentUnitsInShop.IndexOf(Enums.UnitTypes.Ranger);
                    BuyUnitFromShop(indexOfUnit);

                    int indexOfUnit1 = currentUnitsInShop.IndexOf(Enums.UnitTypes.Shaman);
                    BuyUnitFromShop(indexOfUnit1);
                }
                else if (hasShaman && hasBattlemage)
                {
                    int indexOfUnit = currentUnitsInShop.IndexOf(Enums.UnitTypes.Battlemage);
                    BuyUnitFromShop(indexOfUnit);

                    int indexOfUnit1 = currentUnitsInShop.IndexOf(Enums.UnitTypes.Shaman);
                    BuyUnitFromShop(indexOfUnit1);
                }
            }
            else
            {
                if (hasBattlemage)
                {
                    int indexOfUnit = currentUnitsInShop.IndexOf(Enums.UnitTypes.Battlemage);
                    BuyUnitFromShop(indexOfUnit);
                }
                else if (hasRanger)
                {
                    int indexOfUnit = currentUnitsInShop.IndexOf(Enums.UnitTypes.Ranger);
                    BuyUnitFromShop(indexOfUnit);
                }
                else if (hasShaman)
                {
                    int indexOfUnit = currentUnitsInShop.IndexOf(Enums.UnitTypes.Shaman);
                    BuyUnitFromShop(indexOfUnit);
                }
            }
        }
    }
}
