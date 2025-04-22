using Godot;
using System;

namespace Erikduss
{
    public partial class LockedPowerUpInfoToggler : BasePowerUpInfoToggler
    {
        [Export] private Label currentPowerUpProgressLabel;

        public void UpdatePowerUpProgressLabel(int currentPowerUpProgress)
        {
            currentPowerUpProgressLabel.Text = currentPowerUpProgress.ToString() + "/" + GameSettingsLoader.progressNeededToUnlockPower;
        }
    }
}
