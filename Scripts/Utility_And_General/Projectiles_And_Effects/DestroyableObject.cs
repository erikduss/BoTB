using Godot;
using System;

namespace Erikduss
{
	public partial class DestroyableObject : Node2D
	{
		public bool destroyThisAfterTime = true;
		[Export] public float destroyTime = 1f;

		private bool calledDestroy = false;
		protected float destroyTimer = 0f;

		[Export] private Sprite2D usedSprite;

		public bool flipSpite = false;

		public BaseCharacter characterThisEffectIsAttachedTo;

		protected Node2D destroyObjectOverride;

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
				if (characterThisEffectIsAttachedTo.IsDeadOrDestroyed)
				{
					destroyTimer = destroyTime;

					DestroyObject();
                }
			}

            if (!destroyThisAfterTime) return;

			if (destroyTimer > destroyTime)
			{
				if (!calledDestroy)
				{
					calledDestroy = true;
					DestroyObject();
                }
			}
			else destroyTimer += (float)delta;
		}

		protected virtual void DestroyObject()
		{
			if(destroyObjectOverride != null)
			{
				destroyObjectOverride.QueueFree();
			}
			else
			{
				QueueFree();
			}
		}
	}
}
