using Godot;
using System;

namespace Erikduss
{
	public partial class FloatingDamageNumber : Control
	{
		[Export] private Label healthAmountReducedLabel;
        [Export] private TextureRect textureRect;

        public BaseCharacter characterThisEffectIsAttachedTo;

        public bool isHealingDamageInstead = false;
        private Color overrideColor = new Color(0, 1, 0.5f);

        public bool isHomeBaseDamage = false;
        private Color homeBaseOverrideColor = new Color(0.5f, 0.5f, 0.5f);

        public bool destroyThisAfterTime = true;
        [Export] public float destroyTime = 1f;

        private bool calledDestroy = false;
        protected float destroyTimer = 0f;

        private float movementSpeed = 1.25f;

        public override void _Ready()
        {
            base._Ready();

            if (isHealingDamageInstead)
            {
                textureRect.SelfModulate = overrideColor;
            }

            if (isHomeBaseDamage)
            {
                textureRect.SelfModulate = homeBaseOverrideColor;
            }
        }

        public override void _Process(double delta)
        {
            //doesnt need to be checked for multiplayer stuff, we just delete.
            base._Process(delta);

            if (GameManager.Instance.gameIsPaused) return;

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

        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);

            if (GameManager.Instance.gameIsPaused) return;

            SetGlobalPosition(new Vector2(GlobalPosition.X, (GlobalPosition.Y - movementSpeed)));
        }

        public void SetHealthLabelValue(int value)
		{
            if (isHealingDamageInstead)
            {
                textureRect.SelfModulate = overrideColor;
                healthAmountReducedLabel.Text = "+" + value;
            }
            else
            {
                if (isHomeBaseDamage)
                {
                    textureRect.SelfModulate = homeBaseOverrideColor;
                }
                healthAmountReducedLabel.Text = "-" + value;
            }
		}
    }
}
