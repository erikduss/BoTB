using Godot;
using System;

namespace Erikduss
{
	public partial class In_Game_HUD_Manager : Control
	{
		[Export] Label currencyAmountLabel;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{

		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		public void BuySimpleSoldierButton_Clicked()
		{
			GD.Print("Requesting to buy a Simple Soldier");

			//check for the current age the player is in, this will determine the cost of the soldier and which one it will spawn.

			//check for gold requirement
			if (GameManager.Instance.playerCurrentCurrencyAmount < 50) return; //this needs to be changed to determine the cost based on age and get the cost from a seperate script/file.

			//other requirements?

			//Attempt to spend the currency, if this fails we stop.
			if (!GameManager.Instance.SpendPlayerCurrency(50)) return;

			//add soldier to spawn queue in a (few) second(s).

			//this is always team one due to the player having to click this. If going multiplayer, this needs to be adjusted and processed by the server.
			GameManager.Instance.unitsSpawner.ProcessBuyingSimpleSoldier(Enums.TeamOwner.TEAM_01);
		}

        #region Update the Player Currency Amount Label
		public void UpdatePlayerCurrencyAmountLabel(int newAmount)
		{
			currencyAmountLabel.Text = newAmount.ToString();
		}

        #endregion
    }
}
