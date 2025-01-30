using Godot;
using System;

namespace Erikduss
{
	public partial class GameManager : Node
	{
		public static GameManager Instance { get; private set; }
		[Export] public In_Game_HUD_Manager inGameHUDManager;
		[Export] public UnitsSpawner unitsSpawner;

		[Export] public HomeBaseManager team01HomeBase;
		[Export] public HomeBaseManager team02HomeBase;

        //TODO, Move these settings to a general settings file and script.
		public bool gameIsPaused { get; private set; }

        #region Currency Variables
        public int playerCurrentCurrencyAmount { get; private set; }

		private float currencyGainAmountUpdateTimer = 0;
		private float currencyGainRate = 1f; //every 1 second the player gets currency
		private int currencyGainAmount = 1;
        #endregion

        #region Ability Variables
        public int playerAbilityCurrentCooldown { get; private set; }

		public int playerAbilityCooldown = 180; //seconds

        private float playerAbilityUpdateTimer = 0;
        private float playerAbilityCooldownReductionRate = 1f; //every second we reduce it by 1
		private int playerAbilityCooldownReduction = 1; //reduceing it by 1
        #endregion

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                QueueFree();
            }

			gameIsPaused = false;

			playerCurrentCurrencyAmount = 100;
			playerAbilityCurrentCooldown = playerAbilityCooldown;

            inGameHUDManager.UpdatePlayerCurrencyAmountLabel(playerCurrentCurrencyAmount);

			//The ability bar isnt passed yet at this time
			//inGameHUDManager.UpdatePlayerAbilityCooldownBar(playerAbilityCurrentCooldown);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

			//We need to set the instance to null to reset the game variables.
			Instance = null;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
		{
			if (Input.IsActionJustPressed("Pause"))
			{
				ToggleGameIsPaused();
			}

			if (gameIsPaused) return;

			//Timer for giving the player currency
			if(currencyGainAmountUpdateTimer > currencyGainRate)
			{
				currencyGainAmountUpdateTimer = 0;
				playerCurrentCurrencyAmount += currencyGainAmount;

				//Update HUD
				inGameHUDManager.UpdatePlayerCurrencyAmountLabel(playerCurrentCurrencyAmount);
            }
			else
			{
				currencyGainAmountUpdateTimer += (float)delta;
			}

            //Timer for giving the player currency
            if (playerAbilityUpdateTimer > playerAbilityCooldownReductionRate)
            {
                playerAbilityUpdateTimer = 0;
                playerAbilityCurrentCooldown -= playerAbilityCooldownReduction;

                //Update HUD
				inGameHUDManager.UpdatePlayerAbilityCooldownBar(playerAbilityCurrentCooldown);
            }
            else
            {
                playerAbilityUpdateTimer += (float)delta;
            }
        }

		public void ToggleGameIsPaused()
		{
            gameIsPaused = !gameIsPaused;
            GD.Print("Game is paused: " + gameIsPaused);

			if (gameIsPaused)
			{
                //Open pause panel
                inGameHUDManager.ShowPauseMenu();
            }
            else
			{
				//close pause panel
				inGameHUDManager.HidePauseMenu();
			}
        }

		//bool to inducate succes state of removing the currency.
		public bool SpendPlayerCurrency(int amount)
		{
			if (playerCurrentCurrencyAmount < amount) return false;

			playerCurrentCurrencyAmount -= amount;
			//update HUD
            inGameHUDManager.UpdatePlayerCurrencyAmountLabel(playerCurrentCurrencyAmount);

			return true;
        }
	}
}
