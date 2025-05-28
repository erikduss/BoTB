using Godot;
using System;

namespace Erikduss
{
    public partial class HealBasePowerUpInfoToggler : BasePowerUpInfoToggler
    {
        public override void ProcessPowerUpEffect()
        {
            GameManager.Instance.team01HomeBase.HealDamage(100);

            base.ProcessPowerUpEffect();
        }
    }
}
