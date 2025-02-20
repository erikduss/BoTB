using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Erikduss
{
	public partial class BaseAIPlayer : BasePlayer
	{

        //We will need:

        /*
		 * A current shop, with (3) units we can buy. (link the value to the same one the player has)
		 * A way to refresh the shop
		 * A way to buy a unit
		 * A way to activate our age ability
		*/

        #region AI Unit Shop

        protected List<Enums.UnitTypes> currentUnitsInShop = new List<Enums.UnitTypes>();

        protected Dictionary<string, int> unitsDefaultValueCosts = new Dictionary<string, int>();
        //if using custom values we would need a dictionary here that loads them in.

        #endregion


        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
            GD.Print(UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == (Enums.Ages.AGE_01.ToString() + "_" + Enums.UnitTypes.Warrior.ToString())).FirstOrDefault().Value.unitDescription);

            //for (int i = 0; i < Enum.GetNames(typeof(Enums.UnitTypes)).Length; i++)
            //{
            //    for (int k = 0; k < Enum.GetNames(typeof(Enums.Ages)).Length; k++)
            //    {
            //        string dictionaryID = (Enums.UnitTypes)i + "_" + ((Enums.Ages)k);

            //        string unitCost = UnitsDefaultValues

            //        GD.Print(unitCost);
            //    }
            //}
        }

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

        public void BuyUnitFromShop(Enums.UnitTypes unitType, int unitCost)
        {

            //check for the current age the player is in, this will determine the cost of the soldier and which one it will spawn.

            //check for gold requirement
            if (playerCurrentCurrencyAmount < unitCost) return; //this needs to be changed to determine the cost based on age and get the cost from a seperate script/file.

            //other requirements?

            //Attempt to spend the currency, if this fails we stop.
            if (!GameManager.Instance.SpendPlayerCurrency(unitCost, Enums.TeamOwner.TEAM_02)) return;

            //add soldier to spawn queue in a (few) second(s).

            //this is always team one due to the player having to click this. If going multiplayer, this needs to be adjusted and processed by the server.

            GameManager.Instance.unitsSpawner.ProcessBuyingUnit(Enums.TeamOwner.TEAM_02, unitType);

            //Refresh bought unit slot.
            //RefreshUnitShopSpecificButton();
        }

        public void PlayerAbilityButtonPressed()
        {
            //we need to check if the cooldown is over.

            if (GameManager.Instance.player01Script.playerAbilityCurrentCooldown > 0) return;

            GameManager.Instance.ResetPlayerAbilityCooldown(Enums.TeamOwner.TEAM_01);

            //is always going to be team 1 for now, due to this being the player.
            EffectsAndProjectilesSpawner.Instance.SpawnMeteorsAgeAbilityProjectiles(Enums.TeamOwner.TEAM_01);
        }

        //public void RefreshUnitShop(bool spendPlayerGold = true)
        //{
        //    if (GameManager.Instance.gameIsPaused) return;

        //    if (spendPlayerGold)
        //    {
        //        if (GameManager.Instance.player01Script.playerCurrentCurrencyAmount < GameManager.Instance.shopRefreshCost) return;

        //        //Attempt to spend the currency, if this fails we stop.
        //        if (!GameManager.Instance.SpendPlayerCurrency(GameManager.Instance.shopRefreshCost, Enums.TeamOwner.TEAM_01)) return;
        //    }

        //    for (int i = unitShopParentNode.GetChildren().Count - 1; i >= 0; i--)
        //    {
        //        unitShopParentNode.GetChild(i).QueueFree();
        //    }

        //    for (int i = 0; i < GameManager.Instance.amountOfUnitsInShop; i++)
        //    {
        //        Control instantiatedBuyButton = (Control)availableUnitsBuyButtons[UnitTheShopRolledFor()].Instantiate();

        //        unitShopParentNode.AddChild(instantiatedBuyButton);
        //    }
        //}

        //private int UnitTheShopRolledFor()
        //{
        //    int rand = (int)(GD.Randi() % (availableUnitsBuyButtons.Count));

        //    if (rand == 7)
        //    {
        //        int randDruid = (int)(GD.Randi() % 100);

        //        //random chance to reroll to a different unit.
        //        if (randDruid < 69)
        //        {
        //            rand = (int)(GD.Randi() % (availableUnitsBuyButtons.Count));
        //        }
        //    }

        //    return rand;
        //}

        public void RefreshUnitShopSpecificButton(ulong id)
        {
            //for (int i = currentUnitsInShop.Count - 1; i >= 0; i--)
            //{
            //    if (currentUnitsInShop[i] == id)
            //    {
            //        currentUnitsInShop.GetChild(i).QueueFree();

            //        Control instantiatedBuyButton = (Control)availableUnitsBuyButtons[UnitTheShopRolledFor()].Instantiate();

            //        unitShopParentNode.AddChild(instantiatedBuyButton);
            //        unitShopParentNode.MoveChild(instantiatedBuyButton, i);
            //    }
            //}
        }
    }
}
