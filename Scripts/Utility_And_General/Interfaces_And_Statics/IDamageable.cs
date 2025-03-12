using Godot;
using System;

namespace Erikduss
{
    public interface IDamageable
    {
        //We set the protected variables that this gets in the specific scripts.
        int CurrentHealth { get;}

        int MaxHealth { get;}

        bool IsDeadOrDestroyed { get;}

        public void TakeDamage(int rawDamage);

        public void HealDamage(int healAmount);
    }
}
