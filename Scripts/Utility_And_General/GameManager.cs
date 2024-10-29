using Godot;
using System;

namespace Erikduss
{
	public partial class GameManager : Node
	{
		public static GameManager Instance { get; private set; }
		[Export] public In_Game_HUD_Manager inGameHUDManager;
		[Export] public UnitsSpawner unitsSpawner;

        //TODO, Move these settings to a general settings file and script.
		public bool gameIsPaused { get; private set; }

        #region Currency Variables
        public int playerCurrentCurrencyAmount { get; private set; }

		private float currencyGainAmountUpdateTimer = 0;
		private float currencyGainRate = 1f; //every 1 second the player gets currency
		private int currencyGainAmount = 1;
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

			playerCurrentCurrencyAmount = 0;
			inGameHUDManager.UpdatePlayerCurrencyAmountLabel(playerCurrentCurrencyAmount);
        }

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			if (Input.IsActionJustPressed("Pause"))
			{
				gameIsPaused = !gameIsPaused;
				GD.Print("Game is paused: " + gameIsPaused);
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
