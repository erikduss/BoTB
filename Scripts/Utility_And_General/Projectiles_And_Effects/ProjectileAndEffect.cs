using Godot;
using System;

namespace Erikduss
{
	public partial class ProjectileAndEffect : Node2D
	{
		public bool destroyThisAfterTime = true;
		[Export] public float destroyTime = 1f;

		private bool calledDestroy = false;
		private float destroyTimer = 0f;

		[Export] private Sprite2D usedSprite;

		public bool flipSpite = false;

		public BaseCharacter characterThisEffectIsAttachedTo;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			if (flipSpite && usedSprite != null) usedSprite.FlipH = true;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
            if (GameManager.Instance.gameIsPaused) return;

			if(characterThisEffectIsAttachedTo != null)
			{
				if (characterThisEffectIsAttachedTo.isDead)
				{
					destroyTimer = destroyTime;

                    QueueFree();
                }
			}

            if (!destroyThisAfterTime) return;

			if (destroyTimer > destroyTime)
			{
				if (!calledDestroy)
				{
					calledDestroy = true;
					QueueFree();
				}
			}
			else destroyTimer += (float)delta;
		}
	}
}
