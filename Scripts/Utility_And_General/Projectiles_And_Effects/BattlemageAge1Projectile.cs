using Godot;
using System;

namespace Erikduss
{
	public partial class BattlemageAge1Projectile : BaseProjectile
	{
        [Export] public AnimatedSprite2D animatedSprite;

        public override void _Ready()
        {
            base._Ready();

            if (animatedSprite != null)
            {
                if (flipSpite) animatedSprite.FlipH = true;
            }
        }
    }
}
