using Godot;
using System;

namespace Erikduss
{
	public partial class MeteorImpactLogic : DestroyableObject
	{
        public Enums.TeamOwner meteorImpactOwner = Enums.TeamOwner.NONE;

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
            if (enemyChar.characterOwner == meteorImpactOwner) return;

            if (enemyChar != null)
            {
                enemyChar.TakeDamage(30);
            }
        }
    }
}
