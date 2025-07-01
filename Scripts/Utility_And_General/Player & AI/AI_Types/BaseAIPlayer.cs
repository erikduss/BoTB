using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Erikduss
{
	public partial class BaseAIPlayer : BasePlayer
	{
        #region AI Unit Shop

        protected List<Enums.UnitTypes> currentUnitsInShop = new List<Enums.UnitTypes>();
        protected List<Enums.UnitTypes> availableUnitsThatCanBeBought = new List<Enums.UnitTypes>();

        protected Dictionary<string, int> unitsDefaultValueCosts = new Dictionary<string, int>();
        //if using custom values we would need a dictionary here that loads them in.

        #endregion


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

            //make sure to refresh the shop at the start of the game for free.
            RefreshUnitShop(false);
        }

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
            if (GameManager.Instance.gameIsPaused || GameManager.Instance.gameIsFinished || GameManager.Instance.isMultiplayerMatch) return;
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

            EffectsAndProjectilesSpawner.Instance.SpawnMeteorsAgeAbilityProjectiles(Enums.TeamOwner.TEAM_02);
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
    }
}
