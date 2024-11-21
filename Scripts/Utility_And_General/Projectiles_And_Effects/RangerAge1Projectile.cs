using Godot;
using System;

namespace Erikduss
{
	public partial class RangerAge1Projectile : ProjectileAndEffect
	{
		[Export] public RangerAge1ProjectilePhysics rigidBody;

        public BaseCharacter projectileOwnerChar;
		public Enums.TeamOwner projectileOwner = Enums.TeamOwner.TEAM_01;

        bool dealtDamage = false;

        public override void _Ready()
        {
            base._Ready();

            SetNewOwner(projectileOwner);
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
        }

        public override void _PhysicsProcess(double delta)
        {
            if (GameManager.Instance.gameIsPaused) return;

            base._PhysicsProcess(delta);

           //rigidBody.MoveAndCollide(new Vector2(1000, 0));
        }

        public void SetNewOwner(Enums.TeamOwner owner)
		{
			projectileOwner = owner;

            if (owner == Enums.TeamOwner.TEAM_01)
            {
                rigidBody.CollisionMask = 0b100;
            }
            else
            {
                rigidBody.CollisionMask = 0b10;
            }
        }

        public void OnCollisionEnter(Node2D body)
		{
            rigidBody.StopForces();

            if (dealtDamage) return;

            if (body.GetInstanceId() == projectileOwnerChar.currentTarget.GetInstanceId())
            {
                dealtDamage = true;
                projectileOwnerChar.DealDamage();
            }
            else 
            {
                BaseCharacter enemyChar = body.GetNode<BaseCharacter>(body.GetPath());
                if (enemyChar.characterOwner == projectileOwnerChar.characterOwner) return;

                dealtDamage = true;
                projectileOwnerChar.currentTarget = enemyChar;
                projectileOwnerChar.DealDamage();
            }
		}

		public void OnCollisionExit(Node2D body)
		{

        }
    }
}
