using Godot;
using System;

namespace Erikduss
{
	public partial class ArrowRainProjectile : BaseProjectile
	{
        public override void _Ready()
        {
            destroyTime = 5f;

            base._Ready();
        }

        public override void SetNewOwner(Enums.TeamOwner owner)
        {
            projectileOwner = owner;

            if (owner == Enums.TeamOwner.TEAM_01)
            {
                rigidBody.CollisionMask = 0b000101;
            }
            else
            {
                rigidBody.CollisionMask = 0b000011;
            }
        }

        public override void OnCollisionEnter(Node2D body)
        {
            if (body == null) return;
            if (!body.IsInsideTree()) return;

            if (dealtDamage) return;

            try
            {
                BaseCharacter enemyChar = body.GetNode<BaseCharacter>(body.GetPath()); //will fail try if its not a character
                
                if (enemyChar.characterOwner == projectileOwner) return;

                rigidBody.StopForces();
                destroyTimer = destroyTime - 1f; 
                rigidBody.CollisionMask = 0b1;

                dealtDamage = true;

                if (GameManager.Instance.isHostOfMultiplayerMatch || !GameManager.Instance.isMultiplayerMatch)
                {
                    enemyChar.TakeDamage(15);
                }
            }
            catch
            {
                //we hit a ground tile.

                StaticBody2D colBody = body.GetNode<StaticBody2D>(body.GetPath());
                if (colBody != null)
                {
                    rigidBody.StopForces();
                    destroyTimer = destroyTime - 1f;
                    rigidBody.CollisionMask = 0b1;
                    dealtDamage = true;
                }
            }
        }
    }
}
