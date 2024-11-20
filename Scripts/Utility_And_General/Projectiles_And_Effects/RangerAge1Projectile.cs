using Godot;
using System;

namespace Erikduss
{
	public partial class RangerAge1Projectile : ProjectileAndEffect
	{
		[Export] public RangerAge1ProjectilePhysics rigidBody;

		public Enums.TeamOwner projectileOwner = Enums.TeamOwner.TEAM_01;

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
            rigidBody.addVelocity = false;
            //rigidBody.SetAxisVelocity(new Vector2(0, rigidBody.LinearVelocity.Y));
            GD.Print("Hit a character");
		}

		public void OnCollisionExit(Node2D body)
		{
            GD.Print("leaving a character");
        }
    }
}
