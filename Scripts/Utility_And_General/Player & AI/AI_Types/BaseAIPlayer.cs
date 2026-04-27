using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static Erikduss.Enums;

namespace Erikduss
{
	public partial class BaseAIPlayer : BasePlayer
	{
        #region AI Unit Shop

        protected List<Enums.UnitTypes> currentUnitsInShop = new List<Enums.UnitTypes>();
        protected List<Enums.UnitTypes> availableUnitsThatCanBeBought = new List<Enums.UnitTypes>();

        protected List<Enums.PowerupType> possiblePowerUps = new List<Enums.PowerupType>();

        protected Dictionary<string, int> unitsDefaultValueCosts = new Dictionary<string, int>();
        //if using custom values we would need a dictionary here that loads them in.

        #endregion

        #region Player Ability Activation Attempt Timer
        protected float abiltyActivationAttemptTimer = 0;
        protected int activationAttemptCooldown = 7; //this will be + or - based on the randomizer.
        protected int activationAttemptCooldownMaximum = 4;
        protected int activationAttemptCooldownDeductor = 2;
        protected int abilityActivationChance = 25; //in percentage
        #endregion

        #region Unit Buy Attempt Timer
        protected float unitBuyAttemptTimer = 0;
        protected int unitBuyAttemptCooldown = 7; //this will be + or - based on the randomizer.
        protected int unitBuyAttemptCooldownMaximum = 8;
        protected int unitBuyAttemptCooldownDeductor = 4;
        protected int refreshChance = 15; //in percentage
        protected int comboBuyChance = 15;
        #endregion

        #region Age Upgrade Attempt Timer
        protected float ageUpgradeAttemptTimer = 0;
        protected int ageUpgradeAttemptCooldown = 7; //this will be + or - based on the randomizer.
        protected int ageUpgradeAttemptCooldownMaximum = 6;
        protected int ageUpgradeAttemptCooldownDeductor = 3;
        protected int ageUpgradeChance = 50; //in percentage
        #endregion

        #region Redeem Powerup Attempt Timer
        protected float redeemPowerUpAttemptTimer = 0;
        protected int redeemPowerUpAttemptCooldown = 7; //this will be + or - based on the randomizer.
        protected int redeemPowerUpAttemptCooldownMaximum = 6;
        protected int redeemPowerUpAttemptCooldownDeductor = 3;
        #endregion

        protected int minimumAmountOfUnitDifferenctRequiredToSaveCurrency = 3;
        protected int minimumAmountOfEnemiesRequiredToUseAbility = 3;


        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
            //We load in all the cost values of the units
            for (int i = 0; i < Enum.GetNames(typeof(Enums.UnitTypes)).Length; i++)
            {
                if((Enums.UnitTypes)i != Enums.UnitTypes.TrainingDummy)
                {
                    //add the units that we can buy to the buy list
                    availableUnitsThatCanBeBought.Add((Enums.UnitTypes)i);

                    for (int k = 0; k < Enum.GetNames(typeof(Enums.Ages)).Length; k++)
                    {
                        string dictionaryID = (((Enums.Ages)k).ToString() + "_" + ((Enums.UnitTypes)i).ToString());

                        int unitCost = UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == dictionaryID).FirstOrDefault().Value.unitCost;

                        //For example the key is: AGE_01_Warrior
                        unitsDefaultValueCosts.Add(dictionaryID, unitCost);
                    }
                }
            }

            for (int i = 0; i < Enum.GetNames(typeof(Enums.PowerupType)).Length; i++)
            {
                possiblePowerUps.Add((Enums.PowerupType)i);
            }

            //make sure to refresh the shop at the start of the game for free.
            RefreshUnitShop(false);

            SetTimerStartingValues();
        }

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
            if (GameManager.Instance.gameIsPaused || GameManager.Instance.gameIsFinished || GameManager.Instance.isMultiplayerMatch) return;

            #region Age Upgrading
            if (ageUpgradeAttemptTimer <= 0)
            {
                AttemptToAgeUp();
            }
            else
            {
                ageUpgradeAttemptTimer -= (float)delta;
            }
            #endregion

            #region Redeem Powerup
            if (redeemPowerUpAttemptTimer <= 0)
            {
                AttemptRedeemPowerUp();
            }
            else
            {
                redeemPowerUpAttemptTimer -= (float)delta;
            }
            #endregion

            #region Ability Activation
            if (playerAbilityCurrentCooldown <= 0)
            {
                if (abiltyActivationAttemptTimer <= 0)
                {
                    AttemptToActivateOurAbility();
                }
                else
                {
                    abiltyActivationAttemptTimer -= (float)delta;
                }
            }
            #endregion


            #region Unit Buying
            if (unitBuyAttemptTimer <= 0)
            {
                AttemptToBuyUnit();
            }
            else
            {
                unitBuyAttemptTimer -= (float)delta;
            }
            #endregion
        }

        protected virtual void BuyUnitFromShop(int shopID)
        {
            //We need to figure out the cost of this unit first, cus we need to verify that we have the currency to buy this.
            string dictionaryID = (currentAgeOfPlayer.ToString() + "_" + currentUnitsInShop[shopID].ToString());

            int thisUnitsCost = UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == dictionaryID).FirstOrDefault().Value.unitCost;

            //check for gold requirement
            if (playerCurrentCurrencyAmount < thisUnitsCost) return; 

            //other requirements?

            //Attempt to spend the currency, if this fails we stop.
            if (!GameManager.Instance.SpendPlayerCurrency(thisUnitsCost, Enums.TeamOwner.TEAM_02)) return;

            GameManager.Instance.unitsSpawner.ProcessBuyingUnit(Enums.TeamOwner.TEAM_02, currentUnitsInShop[shopID]);

            //Refresh bought unit slot.
            RefreshUnitShopSpecificID(shopID);
        }

        protected virtual void ActivatePlayerAbility()
        {
            //we need to check if the cooldown is over.
            if (playerAbilityCurrentCooldown > 0) return;

            GameManager.Instance.ResetPlayerAbilityCooldown(Enums.TeamOwner.TEAM_02);

            if(currentAgeOfPlayer == Enums.Ages.AGE_01)
            {
                EffectsAndProjectilesSpawner.Instance.SpawnMeteorsAgeAbilityProjectiles(Enums.TeamOwner.TEAM_02);
            }
            else
            {
                EffectsAndProjectilesSpawner.Instance.SpawnArrowRainAgeAbilityProjectiles(Enums.TeamOwner.TEAM_02);
            }
        }

        protected virtual void AgeUpToNewAge()
        {
            GD.Print("We aged up");

            if (!GameManager.Instance.SpendPlayerCurrency(GameSettingsLoader.age1UpgradeToAge2Cost, Enums.TeamOwner.TEAM_02)) return;

            currentAgeOfPlayer = Ages.AGE_02;

            RefreshUnitShop(false);
        }

        protected virtual void RedeemNewPowerUp()
        {
            if (playerCurrentAmountOfPowerUpsOwed > 0 || hasUnlockedPowerUpCurrently)
            {
                int rand = (int)(GD.Randi() % (possiblePowerUps.Count));

                GameManager.Instance.AwardPlayer2WithPowerupBuff(possiblePowerUps[rand]);
            }
        }

        protected virtual void RefreshUnitShop(bool spendPlayerGold = true)
        {
            if (spendPlayerGold)
            {
                if (playerCurrentCurrencyAmount < GameManager.defaultShopRefreshCost) return;

                //Attempt to spend the currency, if this fails we stop.
                if (!GameManager.Instance.SpendPlayerCurrency(GameManager.defaultShopRefreshCost, Enums.TeamOwner.TEAM_02)) return;
            }

            //we need to clear the shop to make room for the new units.
            currentUnitsInShop.Clear();

            for (int i = 0; i < GameManager.defaultAmountOfUnitsInShop; i++)
            {
                Enums.UnitTypes chosenUnitToAddToShop = availableUnitsThatCanBeBought[UnitTheShopRolledFor()];

                currentUnitsInShop.Add(chosenUnitToAddToShop);
            }
        }

        protected virtual int UnitTheShopRolledFor()
        {
            int rand = (int)(GD.Randi() % (availableUnitsThatCanBeBought.Count));

            if (rand == 7)
            {
                int randDruid = (int)(GD.Randi() % 100);

                //random chance to reroll to a different unit.
                if (randDruid < 69)
                {
                    rand = (int)(GD.Randi() % (availableUnitsThatCanBeBought.Count));
                }
            }

            return rand;
        }

        protected virtual void RefreshUnitShopSpecificID(int id)
        {
            //we want to reset either 0, 1 or 2 and replace it with a new one.
            currentUnitsInShop.RemoveAt(id);

            Enums.UnitTypes chosenUnitToAddToShop = availableUnitsThatCanBeBought[UnitTheShopRolledFor()];

            //we dont need to set the index position of the newly added unit, cus the AI doesnt care. (I think)
            currentUnitsInShop.Add(chosenUnitToAddToShop);
        }

        protected virtual void SetTimerStartingValues()
        {
            SetNewAbilityActivationTimer();
            SetNewUnitBuyTimer();
        }

        protected virtual void AttemptToBuyUnit()
        {
            int randRefreshRoll = (int)(GD.Randi() % 100);

            SetNewUnitBuyTimer();

            //we need to semi smartly check if we need to actually spend gold or if we want to save it.

            int amountOfAliveUnitsTeam1 = GameManager.Instance.unitsSpawner.team01AliveUnitDictionary.Count;
            int amountOfAliveUnitsTeam2 = GameManager.Instance.unitsSpawner.team02AliveUnitDictionary.Count;

            int differenceInUnits = amountOfAliveUnitsTeam1 - amountOfAliveUnitsTeam2;

            if (differenceInUnits > -minimumAmountOfUnitDifferenctRequiredToSaveCurrency || currentAgeOfPlayer == Enums.Ages.AGE_02) //we have 2 more units than the player. We can save up some money.
            {
                //we hit the roll
                if (randRefreshRoll <= refreshChance)
                {
                    RefreshUnitShop(); //We have a chance to fail this if we dont have enough gold.
                }
                else
                {
                    BuyUnitAttemptSuccess();
                }
            }
        }

        protected virtual void BuyUnitAttemptSuccess()
        {
            //base function of attempting to buy units is the same, more difficult units will be smarter with buying specific kinds of units.
            int randUnitFromShophRoll = (int)(GD.Randi() % 3); //number is not inclusive.

            BuyUnitFromShop(randUnitFromShophRoll);
        }

        protected virtual void AttemptToAgeUp()
        {
            SetNewAgeUpgradeTimer();

            if (playerCurrentCurrencyAmount > GameSettingsLoader.age1UpgradeToAge2Cost)
            {
                if (currentAgeOfPlayer == Enums.Ages.AGE_01)
                {
                    int randUpgradeRoll = (int)(GD.Randi() % 100);

                    //we hit the roll
                    if (randUpgradeRoll <= ageUpgradeChance)
                    {
                        AgeUpToNewAge();
                    }
                }
            }
        }

        protected virtual void AttemptRedeemPowerUp()
        {
            SetNewRedeemPowerupTimer();

            if (playerCurrentAmountOfPowerUpsOwed > 0)
            {
                RedeemNewPowerUp();
            }
        }

        protected virtual void AttemptToActivateOurAbility()
        {
            if (playerAbilityCurrentCooldown <= 0)
            {
                SetNewAbilityActivationTimer();

                int randActivateRoll = (int)(GD.Randi() % 100);

                //we hit the roll
                if (randActivateRoll <= abilityActivationChance)
                {
                    //we at least want SOME value.
                    if (GameManager.Instance.unitsSpawner.team01AliveUnitDictionary.Count >= minimumAmountOfEnemiesRequiredToUseAbility)
                    {
                        ActivatePlayerAbility();
                    }
                }
            }
        }

        protected virtual void SetNewAbilityActivationTimer()
        {
            int randTimerRoll = (int)(GD.Randi() % activationAttemptCooldownMaximum);
            randTimerRoll -= activationAttemptCooldownDeductor;

            int newCooldownTime = activationAttemptCooldown - randTimerRoll;

            if (newCooldownTime <= 1) newCooldownTime = 1;

            abiltyActivationAttemptTimer = newCooldownTime;
        }

        protected virtual void SetNewAgeUpgradeTimer()
        {
            int randTimerRoll = (int)(GD.Randi() % ageUpgradeAttemptCooldownMaximum);
            randTimerRoll -= ageUpgradeAttemptCooldownDeductor;

            int newCooldownTime = ageUpgradeAttemptCooldown - randTimerRoll;

            if (newCooldownTime <= 1) newCooldownTime = 1;

            ageUpgradeAttemptTimer = newCooldownTime;
        }

        protected virtual void SetNewRedeemPowerupTimer()
        {
            int randTimerRoll = (int)(GD.Randi() % redeemPowerUpAttemptCooldownMaximum);
            randTimerRoll -= redeemPowerUpAttemptCooldownDeductor;

            int newCooldownTime = redeemPowerUpAttemptCooldown - randTimerRoll;

            if (newCooldownTime <= 1) newCooldownTime = 1;

            redeemPowerUpAttemptTimer = newCooldownTime;
        }

        protected virtual void SetNewUnitBuyTimer()
        {
            int randTimerRoll = (int)(GD.Randi() % unitBuyAttemptCooldownMaximum);
            randTimerRoll -= unitBuyAttemptCooldownDeductor;

            int newCooldownTime = unitBuyAttemptCooldown - randTimerRoll;

            int randComboBuyRoll = (int)(GD.Randi() % 100);

            if (randComboBuyRoll <= comboBuyChance)
            {
                newCooldownTime = 1;
            }

            if (newCooldownTime <= 1) newCooldownTime = 1;

            unitBuyAttemptTimer = newCooldownTime;
        }
    }
}
