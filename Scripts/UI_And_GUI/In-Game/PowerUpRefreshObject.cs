using Godot;
using System;

namespace Erikduss
{
    public partial class PowerUpRefreshObject : Control
    {
        [Export] private Label amountOfPowerUpRefreshesAvailable;

        public void SetAmountOfPowerUpRefreshes(int amount)
        {
            amountOfPowerUpRefreshesAvailable.Text = amount.ToString() + "x"; 
        }

        public void RefreshPowerUpButtonClicked()
        {
            //GameManager.Instance.inGameHUDManager
        }
    }
}
