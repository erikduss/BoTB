using Godot;
using System;

namespace Erikduss
{
	public partial class BattlemageFireballLogic : ProjectileAndEffect
	{
        public Enums.TeamOwner fireballOwner = Enums.TeamOwner.NONE;

        public override void _Ready()
        {
            base._Ready();
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
        }

        public void OnCollisionEnter(Node2D body)
        {
            BaseCharacter enemyChar = body.GetNode<BaseCharacter>(body.GetPath());

            //We should not deal damage to our own units.
            if (enemyChar.characterOwner == fireballOwner) return;

            if (enemyChar != null)
            {
                enemyChar.TakeDamage(characterThisEffectIsAttachedTo.unitAttackDamage * 5);
            }
        }
    }
}
