using Godot;
using System;

namespace Erikduss
{
	public partial class RangerAge1ProjectilePhysics : RigidBody2D
	{
        [Export] public RangerAge1Projectile attachedProjectileScript;

        public bool addVelocity = true;
        private float projectileVelocity = 500f;

        public override void _Ready()
        {
            base._Ready();

            if (attachedProjectileScript.projectileOwner == Enums.TeamOwner.TEAM_02) projectileVelocity = -projectileVelocity;
            LinearVelocity = new Vector2(projectileVelocity, 0);
        }

        //public override void _IntegrateForces(PhysicsDirectBodyState2D state)
        //{
        //    GD.Print(LinearVelocity);

        //    if (GameManager.Instance.gameIsPaused || !addVelocity) return;

        //    base._IntegrateForces(state);

        //    MoveAndCollide(new Vector2(25, 0));

        //    //AddConstantForce(new Vector2(1000, 0));
        //}

        public void StopForces()
        {
            addVelocity = false;

            ConstantForce = Vector2.Zero;
            LinearVelocity = Vector2.Zero;
            SetAxisVelocity(Vector2.Zero);
        }
    }
}
