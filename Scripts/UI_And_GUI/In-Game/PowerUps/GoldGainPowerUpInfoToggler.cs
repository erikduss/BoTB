using Godot;
using System;
using static Erikduss.Enums;

namespace Erikduss
{
    public partial class GoldGainPowerUpInfoToggler : BasePowerUpInfoToggler
    {
        public override void ProcessPowerUpEffect()
        {
            base.ProcessPowerUpEffect();

            GD.Print("Award gold gain buff");

            if (GameManager.Instance.isMultiplayerMatch)
            {
                if (!GameManager.Instance.isHostOfMultiplayerMatch)
                {
                    GDSync.CallFuncOn(GDSync.GetHost(), new Callable(GameManager.Instance, "AwardPlayer2WithPowerupBuff"), [PowerupType.GoldGain.ToString()]);
                    return;
                }
            }

            GameManager.Instance.currencyGainPercentagePlayer01 += GameSettingsLoader.powerUpGoldGainExtraAmount;
        }
    }
}
