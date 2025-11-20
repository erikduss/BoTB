using Godot;
using System;
using System.Collections.Generic;
using static Erikduss.Enums;

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

        [Export] public Control pauseMenuReturnControl;

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

            if(!GameManager.Instance.isMultiplayerMatch || GameManager.Instance.isHostOfMultiplayerMatch) 
            {
                RefreshUnitShop(false);
                RefreshPowerUp(false);
            }
            else if (GameManager.Instance.isMultiplayerMatch)
            {
                RefreshUnitShop(false);
                RefreshPowerUp(false);
            }
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
            UpdatePlayerAbilityEmpowerAmount(GameManager.Instance.clientTeamOwner);
        }

        public void SubscribeToEvents()
        {
            GetViewport().GuiFocusChanged += OnControlElementFocusChanged;
            optionsPanel.VisibilityChanged += OptionsPanelClosed;
            GameSettingsLoader.Instance.gameUserOptionsManager.LanguageUpdated += UpdateLanguage;

            Input.JoyConnectionChanged += ReselectControlElementFocusOnControllerChange;
        }

        public void UnsubscribeFromEvents()
        {
            GetViewport().GuiFocusChanged -= OnControlElementFocusChanged;
            optionsPanel.VisibilityChanged -= OptionsPanelClosed;
            GameSettingsLoader.Instance.gameUserOptionsManager.LanguageUpdated -= UpdateLanguage;

            Input.JoyConnectionChanged -= ReselectControlElementFocusOnControllerChange;
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
            BasePlayer playerToCheck = GameManager.Instance.GetLocalClientPlayerScript();

            if (playerToCheck.playerCurrentCurrencyAmount < unitCost) return false; //this needs to be changed to determine the cost based on age and get the cost from a seperate script/file.

			//other requirements?

			//Attempt to spend the currency, if this fails we stop.
			if (!GameManager.Instance.SpendPlayerCurrency(unitCost, GameManager.Instance.clientTeamOwner)) return false;

            //add soldier to spawn queue in a (few) second(s).

            //this is always team one due to the player having to click this. If going multiplayer, this needs to be adjusted and processed by the server.

            if(GameManager.Instance.isMultiplayerMatch && !GameManager.Instance.isHostOfMultiplayerMatch)
            {
                //call the function on the host to spend our currency.
                GDSync.CallFuncOn(GDSync.GetHost(), new Callable(GameManager.Instance, "ProcessSpawnRequestPlayer2"), [(int)unitType]);
            }
            else
            {
                GameManager.Instance.unitsSpawner.ProcessBuyingUnit(GameManager.Instance.clientTeamOwner, unitType);
            }

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
            BasePlayer playerToCheck = GameManager.Instance.clientTeamOwner == Enums.TeamOwner.TEAM_01 ? GameManager.Instance.player01Script : GameManager.Instance.player02Script;

            if (playerToCheck.playerAbilityCurrentCooldown > 0)
            {
                AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickedFailedAudioClip);
                return;
            }

            AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickAudioClip);

            if (GameManager.Instance.isMultiplayerMatch)
            {
                if (GameManager.Instance.isHostOfMultiplayerMatch)
                {
                    GameManager.Instance.ResetPlayerAbilityCooldown(GameManager.Instance.clientTeamOwner);

                    EffectsAndProjectilesSpawner.Instance.SpawnMeteorsAgeAbilityProjectiles(GameManager.Instance.clientTeamOwner);
                }
                else
                {
                    GDSync.CallFuncOn(GDSync.GetHost(), new Callable(GameManager.Instance, "ProcessExecuteAbilityRequestPlayer2"), [true]);
                }
            }
            else
            {
                GameManager.Instance.ResetPlayerAbilityCooldown(GameManager.Instance.clientTeamOwner);

                EffectsAndProjectilesSpawner.Instance.SpawnMeteorsAgeAbilityProjectiles(GameManager.Instance.clientTeamOwner);
            }
                
        }

        public void RefreshPowerUp(bool spendRerollToken = true)
        {
            if (GameManager.Instance.gameIsPaused || GameManager.Instance.gameIsFinished) return;

            BasePlayer player = GameManager.Instance.GetLocalClientPlayerScript();

            if(player != null)
            {
                GD.Print("Refreshing power up!");
                GD.Print(player.playerTeam);
                GD.Print(player.playerCurrentPowerUpProgressAmount);
                GD.Print(player.playerCurrentPowerUpRerollsAmount);
                GD.Print(player.playerCurrentAmountOfPowerUpsOwed);
                GD.Print(player.hasUnlockedPowerUpCurrently);
            }

            if (spendRerollToken)
            {
                if (player.playerCurrentPowerUpRerollsAmount < 1)
                {
                    AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickedFailedAudioClip);
                    return;
                }

                AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickAudioClip);

                player.playerCurrentPowerUpRerollsAmount -= 1;
                UpdatePlayerPowerUPRerollAmount(player);
            }

            //due to this being called on ready, player will be null for the client that is not the host due to delayed assignment
            if(player != null)
            {
                if (player.hasUnlockedPowerUpCurrently && !spendRerollToken)
                {
                    //We dont need to refesh anything if we already have a powerup active.
                    GD.Print("Prevent refresh");
                    return;
                }
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

            int powerupsOwed = 0;

            if (player != null) powerupsOwed = player.playerCurrentAmountOfPowerUpsOwed;

            //if we unlock a power up from the locked state, or we refresh the shop, we give the player a random powerup option.
            if (powerupsOwed > 0 || spendRerollToken)
            {
                int randPowerupID = (int)(GD.Randi() % (availablePowerUpButtons.Count));

                Control instantiatedPowerUpButton = (Control)availablePowerUpButtons[randPowerupID].Instantiate();

                powerUpsParentNode.AddChild(instantiatedPowerUpButton);
                currentShownPowerUp = instantiatedPowerUpButton;

                //make sure we only reduce the amount owed if we buy process another powerup
                if (!spendRerollToken)
                {
                    player.playerCurrentAmountOfPowerUpsOwed -= 1;
                }

                player.hasUnlockedPowerUpCurrently = true;
            }
            else
            {
                Control instantiatedPowerUpButton = (Control)lockedPowerUpButtonPrefab.Instantiate();
                powerUpsParentNode.AddChild(instantiatedPowerUpButton);
                currentShownPowerUp = instantiatedPowerUpButton;

                currentLockedPowerUpInfo = (LockedPowerUpInfoToggler)currentShownPowerUp;

                if(player != null)
                {
                    player.hasUnlockedPowerUpCurrently = false;
                }
            }

            if (GameManager.Instance.isMultiplayerMatch)
            {
                if(GameManager.Instance.GetLocalClientPlayerScript().GetInstanceId() != player.GetInstanceId())
                {
                    return;
                }
            }

            GD.Print("we are the local player and need to refresh connections.");

            RefreshFocusConnections();
            if (!spendRerollToken) SelectFirstControlInShop();
        }

        public bool DoesThePlayerHaveAPowerUpUnlocked(BasePlayer player)
        {
            return player.hasUnlockedPowerUpCurrently;
        }

        public void UpdateCurrentLockedPowerUpProgress()
        {
            if (currentLockedPowerUpInfo != null)
            {
                currentLockedPowerUpInfo.UpdatePowerUpProgressLabel(GameManager.Instance.GetLocalClientPlayerScript().playerCurrentPowerUpProgressAmount);
            }
        }

        public void UpdatePlayerPowerUPRerollAmount(BasePlayer player)
        {
            powerUpRefreshButton.SetAmountOfPowerUpRefreshes(player.playerCurrentPowerUpRerollsAmount);
        }

        public void RefreshUnitShop(bool spendPlayerGold = true)
		{
            //this is controlled by the client, currency check is done through both the client and the host.

            if (GameManager.Instance.gameIsPaused || GameManager.Instance.gameIsFinished) return;

            BasePlayer player = GameManager.Instance.GetLocalClientPlayerScript();

            if (spendPlayerGold)
			{
                if (player.playerCurrentCurrencyAmount < GameManager.defaultShopRefreshCost)
                {
                    AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickedFailedAudioClip);
                    return;
                }

                //Attempt to spend the currency, if this fails we stop.
                if (!GameManager.Instance.SpendPlayerCurrency(GameManager.defaultShopRefreshCost, player.playerTeam))
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

            //sometimes gets called before power up is generated.
            if (currentShownPowerUp != null)
            {
                currentShownPowerUp.FocusNeighborLeft = refreshButtonControl.GetPath();
                currentShownPowerUp.FocusNeighborRight = powerUpRefreshButton.GetPath();
                currentShownPowerUp.FocusNeighborTop = ageAbilityControl.GetPath();
                currentShownPowerUp.FocusNeighborBottom = ageUpControl.GetPath();

                refreshButtonControl.FocusNeighborRight = currentShownPowerUp.GetPath();
                powerUpRefreshButton.FocusNeighborLeft = currentShownPowerUp.GetPath();
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

            if (GameManager.Instance.isMultiplayerMatch)
            {
                GDSync.SyncedEventCreate("PauseGameToggle");
            }
            else
            {
                GameManager.Instance.ToggleGameIsPaused();
            }
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

        private void ReselectControlElementFocusOnControllerChange(long device, bool isConnected)
        {
            bool hasControllerConnected = false;

            if (Input.GetConnectedJoypads().Count > 0)
            {
                hasControllerConnected = true;
            }

            if (hasControllerConnected || GameSettingsLoader.Instance.useHighlightFocusMode)
            {
                //change color back
                if (currentlySelectedControl != null)
                {
                    currentlySelectedControl.SelfModulate = new Color(1, 1, 1);
                    AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonHoverAudioClip);

                    currentlySelectedControl.SelfModulate = GameSettingsLoader.Instance.focussedControlColor;
                }
            }
        }
    }
}
