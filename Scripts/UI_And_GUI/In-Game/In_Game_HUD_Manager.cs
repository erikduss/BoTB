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
			GameManager.Instance.inGameHUDManager = this;
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

			//other requirements?

			//add soldier to spawn queue in a (few) second(s).

			GameManager.Instance.TestFunction();
		}

        #region Update the Player Currency Amount Label
		public void UpdatePlayerCurrencyAmountLabel(int newAmount)
		{
			currencyAmountLabel.Text = newAmount.ToString();
		}

        #endregion
    }
}
