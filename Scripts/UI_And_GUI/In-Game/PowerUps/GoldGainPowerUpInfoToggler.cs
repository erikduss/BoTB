using Godot;
using System;

namespace Erikduss
{
    public partial class GoldGainPowerUpInfoToggler : BasePowerUpInfoToggler
    {
        public override void ProcessPowerUpEffect()
        {
            GD.Print("Award gold gain buff");

            GameManager.Instance.currencyGainPercentagePlayer01 += 20;

            base.ProcessPowerUpEffect();
        }
    }
}
