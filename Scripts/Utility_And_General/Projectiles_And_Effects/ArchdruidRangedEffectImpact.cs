using Godot;
using System;

namespace Erikduss
{
	public partial class ArchdruidRangedEffectImpact : DestroyableObject
	{
        [Export] protected AnimatedSprite2D animatedSprite2D;

        public override void _Ready()
        {
            base._Ready();

            if (flipSpite && animatedSprite2D != null) animatedSprite2D.FlipH = true;
        }
    }
}
