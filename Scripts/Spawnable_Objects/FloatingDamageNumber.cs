using Godot;
using System;

namespace Erikduss
{
	public partial class FloatingDamageNumber : Control
	{
		[Export] private Label healthAmountReducedLabel;

        public bool destroyThisAfterTime = true;
        [Export] public float destroyTime = 1f;

        private bool calledDestroy = false;
        protected float destroyTimer = 0f;

        public BaseCharacter characterThisEffectIsAttachedTo;

        public override void _Process(double delta)
        {
            base._Process(delta);

            if (GameManager.Instance.gameIsPaused) return;

            SetGlobalPosition(new Vector2(GlobalPosition.X, (GlobalPosition.Y - 0.75f)));

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

        public void SetHealthLabelValue(int value)
		{
			healthAmountReducedLabel.Text = "-" + value;
		}
    }
}
