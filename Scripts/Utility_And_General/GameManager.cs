using Godot;
using System;

namespace Erikduss
{
	public partial class GameManager : Node
	{
		public static GameManager Instance { get; private set; }
		public In_Game_HUD_Manager inGameHUDManager;

		//TODO, Move these settings to a general settings file and script.
		public int playerCurrentCurrencyAmount { get; private set; }

		private float currencyGainAmountUpdateTimer = 0;
		private float currencyGainRate = 1f; //every 1 second the player gets currency
		private int currencyGainAmount = 5;

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
        }

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
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

		public void TestFunction()
		{
			GD.Print("Game Manager is being reached.");
		}
	}
}
