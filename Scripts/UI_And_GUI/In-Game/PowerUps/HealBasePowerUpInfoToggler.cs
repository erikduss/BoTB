using Godot;
using System;
using static Erikduss.Enums;

namespace Erikduss
{
    public partial class HealBasePowerUpInfoToggler : BasePowerUpInfoToggler
    {
        public override void ProcessPowerUpEffect()
        {
            base.ProcessPowerUpEffect();

            if (GameManager.Instance.isMultiplayerMatch)
            {
                if (!GameManager.Instance.isHostOfMultiplayerMatch)
                {
                    GDSync.CallFuncOn(GDSync.GetHost(), new Callable(GameManager.Instance, "AwardPlayer2WithPowerupBuff"), [PowerupType.HealBase.ToString()]);
                    return;
                }
            }

            GameManager.Instance.team01HomeBase.HealDamage(GameSettingsLoader.powerUpBaseHealAmount);
        }
    }
}
