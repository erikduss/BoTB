using Godot;
using System;
using static Erikduss.Enums;

namespace Erikduss
{
    public partial class AbilityEmpowerPowerUpInfoToggler : BasePowerUpInfoToggler
    {
        public override void ProcessPowerUpEffect()
        {
            base.ProcessPowerUpEffect();

            if (GameManager.Instance.isMultiplayerMatch)
            {
                if (!GameManager.Instance.isHostOfMultiplayerMatch)
                {
                    GDSync.CallFuncOn(GDSync.GetHost(), new Callable(GameManager.Instance, "AwardPlayer2WithPowerupBuff"), [PowerupType.AbilityEmpower.ToString()]);
                    return;
                }
            }

            EffectsAndProjectilesSpawner.Instance.team01AbilityEmpowerAmount += GameSettingsLoader.powerUpAbilityEmpowerAmount;
        }
    }
}
