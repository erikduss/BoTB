using Erikduss;
using Godot;
using System;

namespace Erikduss
{
	public partial class AssassinBleedingEffect : DestroyableObject
	{
		[Export] AnimatedSprite2D regularBloodColorSprites;
		[Export] AnimatedSprite2D alternativeGloodColorSprites;

		public BaseCharacter unitThisIsDamaging;

        public int bleedTimes = 3;
		private int currentBleedTimes = 0;
		public float bleedDamageInterval = 1f;
		public int bleedDamage = 4;

		private float bleedingTimer = 0f;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			base._Ready();

			if(flipSpite)
			{
                regularBloodColorSprites.FlipH = true;
                alternativeGloodColorSprites.FlipH = true;
            }

			if (GameSettingsLoader.Instance.useAlternativeBloodColor)
			{
				regularBloodColorSprites.Visible = false;
				alternativeGloodColorSprites.Visible = true;
			}
			else
			{
				regularBloodColorSprites.Visible = true;
				alternativeGloodColorSprites.Visible = false;
			}
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			base._Process(delta);

			if (currentBleedTimes >= bleedTimes) return;
			if (unitThisIsDamaging.isDead) return;

			if (bleedingTimer > bleedDamageInterval)
			{
				if (GameSettingsLoader.Instance.useAlternativeBloodColor) alternativeGloodColorSprites.Play();
				else regularBloodColorSprites.Play();

				unitThisIsDamaging.TakeDamage(bleedDamage);
				bleedingTimer = 0;
				currentBleedTimes++;
			}
			else
			{
				bleedingTimer += (float)delta;
			}
		}
	}
}
