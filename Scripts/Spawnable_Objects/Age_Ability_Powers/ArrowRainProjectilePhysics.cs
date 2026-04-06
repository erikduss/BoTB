using Godot;
using System;

namespace Erikduss
{
	public partial class ArrowRainProjectilePhysics : BaseProjectilePhysics
	{
        public override void _Ready()
        {
            projectileVelocity = 500f;

            float addedXVelocity = attachedProjectileScript.projectileOwner == Enums.TeamOwner.TEAM_01 ? 350f : -350f;

            LinearVelocity = new Vector2(addedXVelocity, projectileVelocity);
        }
    }
}
