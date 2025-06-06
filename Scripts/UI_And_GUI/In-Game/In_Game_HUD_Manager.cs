using Godot;
using System;
using System.Collections.Generic;

namespace Erikduss
{
	public partial class In_Game_HUD_Manager : Control
	{
		[Export] Label currencyAmountLabel;

		[Export] Control unitShopParentNode;
        [Export] private Control pauseMenuNode;
        [Export] private Control gameOverNode;

        public TextureProgressBar abilityCooldownBar;
		private AgeAbilityInfoToggler abilityCooldownBarScript;

		private List<PackedScene> availableUnitsBuyButtons = new List<PackedScene>();

        [Export] private Control optionsPanel;

        public List<Control> currentUnitsInShop = new List<Control>();

        //needed for focus selection
        [Export] private Control ageUpControl;
        [Export] private Control ageAbilityControl;
        [Export] private Control refreshButtonControl;
        [Export] private Control pauseButtonControl;

        [Export] private Control pauseMenuReturnControl;

        private Control currentlySelectedControl = null;

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

        //PowerUp Hud
        [Export] private Control powerUpsParentNode;
        private LockedPowerUpInfoToggler currentLockedPowerUpInfo;

        [Export] private PowerUpRefreshObject powerUpRefreshButton;

        public Control currentShownPowerUp;

        private List<PackedScene> availablePowerUpButtons = new List<PackedScene>();
        public PackedScene lockedPowerUpButtonPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/UI_And_HUD/In_Game/PowerUpButtons/locked_powerup_button.tscn");
        
        public PackedScene goldGainPowerUpButtonPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/UI_And_HUD/In_Game/PowerUpButtons/GoldGain_powerup_button.tscn");
        public PackedScene abilityEmpowerPowerUpButtonPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/UI_And_HUD/In_Game/PowerUpButtons/AbilityEmpower_powerup_button.tscn");
        public PackedScene healBasePowerUpButtonPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/UI_And_HUD/In_Game/PowerUpButtons/HealBase_powerup_button.tscn");

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

            availablePowerUpButtons.Add(goldGainPowerUpButtonPrefab);
            availablePowerUpButtons.Add(abilityEmpowerPowerUpButtonPrefab);
            availablePowerUpButtons.Add(healBasePowerUpButtonPrefab);

            HidePauseMenu();
            gameOverNode.Visible = false;

            SubscribeToEvents();

            RefreshUnitShop(false);
            RefreshPowerUp(false);
        }

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

        protected override void Dispose(bool disposing)
        {
            UnsubscribeFromEvents();
            base.Dispose(disposing);
        }

        public void UpdateLanguage(object o, EventArgs e)
        {
            //update string that is set through code.
            UpdatePlayerAbilityEmpowerAmount(Enums.TeamOwner.TEAM_01);
        }

        public void SubscribeToEvents()
        {
            GetViewport().GuiFocusChanged += OnControlElementFocusChanged;
            optionsPanel.VisibilityChanged += OptionsPanelClosed;
            GameSettingsLoader.Instance.gameUserOptionsManager.LanguageUpdated += UpdateLanguage;
        }

        public void UnsubscribeFromEvents()
        {
            GetViewport().GuiFocusChanged -= OnControlElementFocusChanged;
            optionsPanel.VisibilityChanged -= OptionsPanelClosed;
            GameSettingsLoader.Instance.gameUserOptionsManager.LanguageUpdated -= UpdateLanguage;
        }

        public void SelectFirstControlInShop()
        {
            if(currentUnitsInShop.Count > 0)
            {
                currentUnitsInShop[0].GrabFocus();
            }
            else
            {
                refreshButtonControl.GrabFocus();
            }
        }

		public bool BuyUnitButtonClicked(Enums.UnitTypes unitType, int unitCost)
		{

			//check for the current age the player is in, this will determine the cost of the soldier and which one it will spawn.

			//check for gold requirement
			if (GameManager.Instance.player01Script.playerCurrentCurrencyAmount < unitCost) return false; //this needs to be changed to determine the cost based on age and get the cost from a seperate script/file.

			//other requirements?

			//Attempt to spend the currency, if this fails we stop.
			if (!GameManager.Instance.SpendPlayerCurrency(unitCost, Enums.TeamOwner.TEAM_01)) return false;

            //add soldier to spawn queue in a (few) second(s).

            //this is always team one due to the player having to click this. If going multiplayer, this needs to be adjusted and processed by the server.

            GameManager.Instance.unitsSpawner.ProcessBuyingUnit(Enums.TeamOwner.TEAM_01, unitType);

			return true;
		}

        #region Update the Player Currency Amount Label
		public void UpdatePlayerCurrencyAmountLabel(float newAmount)
		{
            int newAmountInt = (int)MathF.Floor(newAmount);
			currencyAmountLabel.Text = newAmountInt.ToString();
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

            if(fixedCooldownPercentage <= 0) abilityCooldownBarScript.abilityCooldownLabel.Text = Tr("COOLDOWN") + ": " + Tr("READY");
            else abilityCooldownBarScript.abilityCooldownLabel.Text = Tr("COOLDOWN") + ": " + secondsLeftOnCooldown.ToString();
        }

        public void UpdatePlayerAbilityEmpowerAmount(Enums.TeamOwner team)
        {
            int empowerAmount = team == Enums.TeamOwner.TEAM_01 ? EffectsAndProjectilesSpawner.Instance.team01AbilityEmpowerAmount : EffectsAndProjectilesSpawner.Instance.team02AbilityEmpowerAmount;
            
            abilityCooldownBarScript.abilityEmpowerLabel.Text = Tr("EMPOWERED") + ": " + empowerAmount;
        }

        public void PlayerAbilityButtonPressed()
        {
            //we need to check if the cooldown is over.

            if (GameManager.Instance.player01Script.playerAbilityCurrentCooldown > 0)
            {
                AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickedFailedAudioClip);
                return;
            }

            AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickAudioClip);

            GameManager.Instance.ResetPlayerAbilityCooldown(Enums.TeamOwner.TEAM_01);

            //is always going to be team 1 for now, due to this being the player.
            EffectsAndProjectilesSpawner.Instance.SpawnMeteorsAgeAbilityProjectiles(Enums.TeamOwner.TEAM_01);
        }

        public void RefreshPowerUp(bool spendRerollToken = true)
        {
            if (GameManager.Instance.gameIsPaused || GameManager.Instance.gameIsFinished) return;

            if (spendRerollToken)
            {
                if (GameManager.Instance.player01Script.playerCurrentPowerUpRerollsAmount < 1)
                {
                    AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickedFailedAudioClip);
                    return;
                }

                AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickAudioClip);

                GameManager.Instance.player01Script.playerCurrentPowerUpRerollsAmount -= 1;
                UpdatePlayerPowerUPRerollAmount();
            }

            if(GameManager.Instance.player01Script.hasUnlockedPowerUpCurrently && !spendRerollToken)
            {
                //We dont need to refesh anything if we already have a powerup active.
                GD.Print("Prevent refresh");
                return;
            }

            if(powerUpsParentNode.GetChildren().Count > 0)
            {
                for (int i = powerUpsParentNode.GetChildren().Count - 1; i >= 0; i--)
                {
                    powerUpsParentNode.GetChild(i).QueueFree();
                }
            }

            currentShownPowerUp = null;
            currentLockedPowerUpInfo = null;

            //if we unlock a power up from the locked state, or we refresh the shop, we give the player a random powerup option.
            if(GameManager.Instance.player01Script.playerCurrentAmountOfPowerUpsOwed > 0 || spendRerollToken)
            {
                int randPowerupID = (int)(GD.Randi() % (availablePowerUpButtons.Count));

                Control instantiatedPowerUpButton = (Control)availablePowerUpButtons[randPowerupID].Instantiate();

                powerUpsParentNode.AddChild(instantiatedPowerUpButton);
                currentShownPowerUp = instantiatedPowerUpButton;

                //make sure we only reduce the amount owed if we buy process another powerup
                if (!spendRerollToken)
                {
                    GameManager.Instance.player01Script.playerCurrentAmountOfPowerUpsOwed -= 1;
                }

                GameManager.Instance.player01Script.hasUnlockedPowerUpCurrently = true;
            }
            else
            {
                Control instantiatedPowerUpButton = (Control)lockedPowerUpButtonPrefab.Instantiate();
                powerUpsParentNode.AddChild(instantiatedPowerUpButton);
                currentShownPowerUp = instantiatedPowerUpButton;

                currentLockedPowerUpInfo = (LockedPowerUpInfoToggler)currentShownPowerUp;

                GameManager.Instance.player01Script.hasUnlockedPowerUpCurrently = false;
            }

            //RefreshFocusConnections();

            //if (!spendRerollToken) SelectFirstControlInShop();

        }

        public bool DoesThePlayerHaveAPowerUpUnlocked()
        {
            return GameManager.Instance.player01Script.hasUnlockedPowerUpCurrently;
        }

        public void UpdateCurrentLockedPowerUpProgress()
        {
            if (currentLockedPowerUpInfo != null)
            {
                currentLockedPowerUpInfo.UpdatePowerUpProgressLabel(GameManager.Instance.player01Script.playerCurrentPowerUpProgressAmount);
            }
        }

        public void UpdatePlayerPowerUPRerollAmount()
        {
            powerUpRefreshButton.SetAmountOfPowerUpRefreshes(GameManager.Instance.player01Script.playerCurrentPowerUpRerollsAmount);
        }

        public void RefreshUnitShop(bool spendPlayerGold = true)
		{
            if (GameManager.Instance.gameIsPaused || GameManager.Instance.gameIsFinished) return;

            if (spendPlayerGold)
			{
                if (GameManager.Instance.player01Script.playerCurrentCurrencyAmount < GameManager.defaultShopRefreshCost)
                {
                    AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickedFailedAudioClip);
                    return;
                }

                //Attempt to spend the currency, if this fails we stop.
                if (!GameManager.Instance.SpendPlayerCurrency(GameManager.defaultShopRefreshCost, Enums.TeamOwner.TEAM_01))
                {
                    AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickedFailedAudioClip);
                    return;
                }

                AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickAudioClip);
            }

			for (int i = unitShopParentNode.GetChildren().Count-1; i >= 0; i--)
			{
				unitShopParentNode.GetChild(i).QueueFree();
            }

            currentUnitsInShop.Clear();

			for(int i = 0; i < GameManager.defaultAmountOfUnitsInShop; i++)
			{
                Control instantiatedBuyButton = (Control)availableUnitsBuyButtons[UnitTheShopRolledFor()].Instantiate();

                unitShopParentNode.AddChild(instantiatedBuyButton);
                currentUnitsInShop.Add(instantiatedBuyButton);
            }

            RefreshFocusConnections();

            if (!spendPlayerGold) SelectFirstControlInShop();

        }

        private void RefreshFocusConnections()
        {
            int controlIndex = 0;

            foreach (Node child in currentUnitsInShop)
            {
                Control childControl = (Control)child;

                //this is always the same.
                childControl.FocusNeighborBottom = ageUpControl.GetPath();
                childControl.FocusNeighborTop = ageAbilityControl.GetPath();

                int leftNeightborIndex = controlIndex - 1;

                if (leftNeightborIndex < 0)
                {
                    //we need to set the focus to the pause button instead.
                    childControl.FocusNeighborLeft = pauseButtonControl.GetPath();
                }
                else
                {
                    childControl.FocusNeighborLeft = currentUnitsInShop[leftNeightborIndex].GetPath();
                }

                int rightNeighborIndex = controlIndex + 1;

                if (rightNeighborIndex > currentUnitsInShop.Count - 1)
                {
                    //we need to set the focus to the refresh button instead.
                    childControl.FocusNeighborRight = refreshButtonControl.GetPath();
                }
                else
                {
                    childControl.FocusNeighborRight = currentUnitsInShop[rightNeighborIndex].GetPath();
                }

                controlIndex++;
            }

            ageUpControl.FocusNeighborTop = currentUnitsInShop[0].GetPath();
            ageAbilityControl.FocusNeighborBottom = currentUnitsInShop[0].GetPath();

            pauseButtonControl.FocusNeighborRight = currentUnitsInShop[0].GetPath();
        }

        private int UnitTheShopRolledFor()
        {
            int rand = (int)(GD.Randi() % (availableUnitsBuyButtons.Count));

            if (rand == 7)
            {
                int randDruid = (int)(GD.Randi() % 100);

                //random chance to reroll to a different unit.
                if (randDruid < 69)
                {
                    rand = (int)(GD.Randi() % (availableUnitsBuyButtons.Count));
                }
            }

            return rand;
        }

		public void RefreshUnitShopSpecificButton(ulong id)
		{
            for (int i = unitShopParentNode.GetChildren().Count - 1; i >= 0; i--)
            {
				if(unitShopParentNode.GetChild(i).GetInstanceId() == id)
				{
                    Control controlToRemove = (Control)unitShopParentNode.GetChild(i);

                    int index = currentUnitsInShop.IndexOf(controlToRemove);

                    currentUnitsInShop.Remove(controlToRemove);

                    unitShopParentNode.GetChild(i).QueueFree();

                    Control instantiatedBuyButton = (Control)availableUnitsBuyButtons[UnitTheShopRolledFor()].Instantiate();

                    unitShopParentNode.AddChild(instantiatedBuyButton);
					unitShopParentNode.MoveChild(instantiatedBuyButton, i);

                    currentUnitsInShop.Insert(index, instantiatedBuyButton);

                    ((Control)unitShopParentNode.GetChild(i)).GrabFocus();
                }
            }

            RefreshFocusConnections();
        }

        #region Handle Game Pausing menu

        public void PauseGameButtonClicked()
        {
            AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickAudioClip);
            GameManager.Instance.ToggleGameIsPaused();
        }

        public void ShowPauseMenu()
        {
            pauseMenuNode.Visible = true;
            pauseMenuReturnControl.GrabFocus();
        }

        public void HidePauseMenu()
        {
            pauseMenuNode.Visible = false;
            pauseButtonControl.GrabFocus();
        }

        #endregion

        public void InGameOptionsButtonClicked()
        {
            AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickAudioClip);
            //We open the in game options menu.
            optionsPanel.Visible = true;
            ((OptionsMenu)optionsPanel).allowSFXFromOptionsMenu = true;
            ((OptionsMenu)optionsPanel).SelectDefaultControl();
        }

        public void OptionsPanelClosed()
        {
            if (!optionsPanel.Visible)
            {
                pauseMenuReturnControl.GrabFocus();
            }
        }

        public void InGameExitButtonClicked()
        {
            AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickAudioClip);
            AudioManager.Instance.ClearAudioPlayers();
            //We return back to the main menu.\
            GameManager.Instance.QueueFree();
            GetTree().ChangeSceneToFile("res://Scenes_Prefabs/Scenes/TitleScreen.tscn");
        }

        public void GenericButtonHover()
        {
            AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonHoverAudioClip);
        }

        public void GameOverTriggered(string outcomeValue)
        {
            GameOverInfoScript gameOverInfoScript = gameOverNode.GetNode<GameOverInfoScript>(gameOverNode.GetPath());

            gameOverInfoScript.outcomeLabel.Text = outcomeValue;

            int fixedMatchDuration = (int)GameManager.Instance.matchDuration;

            int minutes = fixedMatchDuration / 60;
            int seconds = fixedMatchDuration - (minutes * 60);

            gameOverInfoScript.matchDurationLabel.Text = minutes + " " + Tr("MINUTES") + " " + seconds + " " + Tr("SECONDS");

            gameOverNode.Visible = true;
        }

        private void OnControlElementFocusChanged(Control control)
        {
            if (GameSettingsLoader.Instance.useHighlightFocusMode)
            {
                if (control != currentlySelectedControl)
                {
                    //change color back
                    if (currentlySelectedControl != null)
                    {
                        currentlySelectedControl.SelfModulate = new Color(1, 1, 1);
                        AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonHoverAudioClip);
                    }
                }

                control.SelfModulate = GameSettingsLoader.Instance.focussedControlColor;
            }

            currentlySelectedControl = control;
        }
    }
}
