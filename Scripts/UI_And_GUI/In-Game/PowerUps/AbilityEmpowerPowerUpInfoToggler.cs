using Godot;
using System;

namespace Erikduss
{
    public partial class AbilityEmpowerPowerUpInfoToggler : BasePowerUpInfoToggler
    {
        public override void ProcessPowerUpEffect()
        {
            EffectsAndProjectilesSpawner.Instance.team01AbilityEmpowerAmount += 1;

            base.ProcessPowerUpEffect();
        }
    }
}
