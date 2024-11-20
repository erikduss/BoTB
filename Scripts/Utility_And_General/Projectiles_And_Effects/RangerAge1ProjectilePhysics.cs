using Godot;
using System;

namespace Erikduss
{
	public partial class RangerAge1ProjectilePhysics : RigidBody2D
	{
        [Export] public RangerAge1Projectile attachedProjectileScript;

        public bool addVelocity = true;

        public override void _IntegrateForces(PhysicsDirectBodyState2D state)
        {
            if (GameManager.Instance.gameIsPaused || !addVelocity) return;

            base._IntegrateForces(state);

            AddConstantForce(new Vector2(10, 0));
        }
    }
}
