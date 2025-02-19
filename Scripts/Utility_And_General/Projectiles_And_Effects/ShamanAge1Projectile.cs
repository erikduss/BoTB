using Godot;
using System;

namespace Erikduss
{
	public partial class ShamanAge1Projectile : BaseProjectile
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

        public override void OnCollisionEnter(Node2D body)
        {
            if (body.Name == projectileOwnerChar.Name) return;

            animatedSprite.Play("TargetHit");

            base.OnCollisionEnter(body);
        }
    }
}
