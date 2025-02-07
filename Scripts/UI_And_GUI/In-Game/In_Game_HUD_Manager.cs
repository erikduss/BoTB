using Godot;
using System;
using System.Collections.Generic;
using static Godot.OpenXRInterface;

namespace Erikduss
{
	public partial class In_Game_HUD_Manager : Control
	{
		[Export] Label currencyAmountLabel;

		[Export] Control unitShopParentNode;
        [Export] private Control pauseMenuNode;

		public TextureProgressBar abilityCooldownBar;
		private AgeAbilityInfoToggler abilityCooldownBarScript;

		//Load this from data file later
		private int shopRefreshCost = 5;
		private int amountOfUnitsInShop = 3;

		private List<PackedScene> availableUnitsBuyButtons = new List<PackedScene>();

        #region Buy Buttons

        public PackedScene warriorBuyButtonPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/UI_And_HUD/In_Game/UnitBuyButtons/In-Use/simple_soldier_buy_button.tscn");
        public PackedScene assassinBuyButtonPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/UI_And_HUD/In_Game/UnitBuyButtons/In-Use/Assassin_buy_button.tscn");
        public PackedScene battlemageBuyButtonPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/UI_And_HUD/In_Game/UnitBuyButtons/In-Use/Battlemage_buy_button.tscn");
        public PackedScene enforcerBuyButtonPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/UI_And_HUD/In_Game/UnitBuyButtons/In-Use/Enforcer_buy_button.tscn");
        public PackedScene masshealerBuyButtonPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/UI_And_HUD/In_Game/UnitBuyButtons/In-Use/MassHealer_buy_button.tscn");
        public PackedScene rangerBuyButtonPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/UI_And_HUD/In_Game/UnitBuyButtons/In-Use/Ranger_buy_button.tscn");
        public PackedScene tankBuyButtonPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/UI_And_HUD/In_Game/UnitBuyButtons/In-Use/Tank_buy_button.tscn");
        public PackedScene archdruidBuyButtonPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/UI_And_HUD/In_Game/UnitBuyButtons/In-Use/Archdruid_buy_button.tscn");
        public PackedScene shamanBuyButtonPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/UI_And_HUD/In_Game/UnitBuyButtons/In-Use/Shaman_buy_button.tscn");

        #endregion

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
			availableUnitsBuyButtons.Add(warriorBuyButtonPrefab);
            availableUnitsBuyButtons.Add(assassinBuyButtonPrefab);
            availableUnitsBuyButtons.Add(battlemageBuyButtonPrefab);
            availableUnitsBuyButtons.Add(enforcerBuyButtonPrefab);
            availableUnitsBuyButtons.Add(masshealerBuyButtonPrefab);
            availableUnitsBuyButtons.Add(rangerBuyButtonPrefab);
            availableUnitsBuyButtons.Add(tankBuyButtonPrefab);
            availableUnitsBuyButtons.Add(archdruidBuyButtonPrefab);
            availableUnitsBuyButtons.Add(shamanBuyButtonPrefab);

            HidePauseMenu();

            RefreshUnitShop(false);
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		public bool BuyUnitButtonClicked(Enums.UnitTypes unitType, int unitCost)
		{

			//check for the current age the player is in, this will determine the cost of the soldier and which one it will spawn.

			//check for gold requirement
			if (GameManager.Instance.playerCurrentCurrencyAmount < unitCost) return false; //this needs to be changed to determine the cost based on age and get the cost from a seperate script/file.

			//other requirements?

			//Attempt to spend the currency, if this fails we stop.
			if (!GameManager.Instance.SpendPlayerCurrency(unitCost)) return false;

			//add soldier to spawn queue in a (few) second(s).

			//this is always team one due to the player having to click this. If going multiplayer, this needs to be adjusted and processed by the server.

			switch (unitType)
			{
				case Enums.UnitTypes.Warrior:
                    GameManager.Instance.unitsSpawner.ProcessBuyingSimpleSoldier(Enums.TeamOwner.TEAM_01);
                    break;
                case Enums.UnitTypes.Asssassin:
                    GameManager.Instance.unitsSpawner.ProcessBuyingAssassin(Enums.TeamOwner.TEAM_01);
                    break;
                case Enums.UnitTypes.Enforcer:
                    GameManager.Instance.unitsSpawner.ProcessBuyingEnforcer(Enums.TeamOwner.TEAM_01);
                    break;
                case Enums.UnitTypes.Tank:
                    GameManager.Instance.unitsSpawner.ProcessBuyingTank(Enums.TeamOwner.TEAM_01);
                    break;
                case Enums.UnitTypes.Ranger:
                    GameManager.Instance.unitsSpawner.ProcessBuyingRanger(Enums.TeamOwner.TEAM_01);
                    break;
                case Enums.UnitTypes.Mass_Healer:
                    break;
				default:
					GD.PrintErr("UNIT TYPE NOT IMPLEMENTED: INGAME HUD MANAGER, BUY BUTTON CLICKED");
                    GameManager.Instance.unitsSpawner.ProcessBuyingSimpleSoldier(Enums.TeamOwner.TEAM_01);
                    break;
            }

			

			return true;
		}

        #region Update the Player Currency Amount Label
		public void UpdatePlayerCurrencyAmountLabel(int newAmount)
		{
			currencyAmountLabel.Text = newAmount.ToString();
		}

        #endregion

        public void UpdatePlayerAbilityCooldownBar(int secondsLeftOnCooldown)
        {
			double fixedCooldownPercentage = (double)((double)secondsLeftOnCooldown / (double)GameManager.Instance.playerAbilityCooldown) * 100;

            if (fixedCooldownPercentage < 0) fixedCooldownPercentage = 0;

			abilityCooldownBar.Value = fixedCooldownPercentage;

			if (abilityCooldownBarScript == null)
			{
                abilityCooldownBarScript = abilityCooldownBar.GetNode<AgeAbilityInfoToggler>(abilityCooldownBar.GetParent().GetPath());
			}

            if(fixedCooldownPercentage <= 0) abilityCooldownBarScript.abilityCooldownLabel.Text = "Cooldown: READY";
            else abilityCooldownBarScript.abilityCooldownLabel.Text = "Cooldown: " + secondsLeftOnCooldown.ToString();
        }

        public void PlayerAbilityButtonPressed()
        {
            //we need to check if the cooldown is over.

            if (GameManager.Instance.playerAbilityCurrentCooldown > 0) return;

            GameManager.Instance.ResetPlayerAbilityCooldown();

            //is always going to be team 1 for now, due to this being the player.
            EffectsAndProjectilesSpawner.Instance.SpawnMeteorsAgeAbilityProjectiles(Enums.TeamOwner.TEAM_01);
        }

        public void RefreshUnitShop(bool spendPlayerGold = true)
		{
            if (GameManager.Instance.gameIsPaused) return;

            if (spendPlayerGold)
			{
                if (GameManager.Instance.playerCurrentCurrencyAmount < shopRefreshCost) return;

                //Attempt to spend the currency, if this fails we stop.
                if (!GameManager.Instance.SpendPlayerCurrency(shopRefreshCost)) return;
            }

			for (int i = unitShopParentNode.GetChildren().Count-1; i >= 0; i--)
			{
				unitShopParentNode.GetChild(i).QueueFree();
            }

			for(int i = 0; i < amountOfUnitsInShop; i++)
			{
				int rand = (int)(GD.Randi() % (availableUnitsBuyButtons.Count));

                Control instantiatedBuyButton = (Control)availableUnitsBuyButtons[rand].Instantiate();

                unitShopParentNode.AddChild(instantiatedBuyButton);
            }
		}

		public void RefreshUnitShopSpecificButton(ulong id)
		{
            for (int i = unitShopParentNode.GetChildren().Count - 1; i >= 0; i--)
            {
				if(unitShopParentNode.GetChild(i).GetInstanceId() == id)
				{
                    unitShopParentNode.GetChild(i).QueueFree();

                    int rand = (int)(GD.Randi() % (availableUnitsBuyButtons.Count));

                    Control instantiatedBuyButton = (Control)availableUnitsBuyButtons[rand].Instantiate();

                    unitShopParentNode.AddChild(instantiatedBuyButton);
					unitShopParentNode.MoveChild(instantiatedBuyButton, i);
                }
            }
        }

        #region Handle Game Pausing menu

        public void PauseGameButtonClicked()
        {
            GameManager.Instance.ToggleGameIsPaused();
        }

        public void ShowPauseMenu()
        {
            pauseMenuNode.Visible = true;
        }

        public void HidePauseMenu()
        {
            pauseMenuNode.Visible = false;
        }

        #endregion

        public void InGameOptionsButtonClicked()
        {
            //We open the in game options menu.
        }

        public void InGameExitButtonClicked()
        {
            //We return back to the main menu.\
            GameManager.Instance.QueueFree();
            GetTree().ChangeSceneToFile("res://Scenes_Prefabs/Scenes/TitleScreen.tscn");
        }
    }
}
