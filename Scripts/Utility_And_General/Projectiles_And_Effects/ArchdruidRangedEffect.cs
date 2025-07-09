using Godot;
using System;

namespace Erikduss
{
    public partial class ArchdruidRangedEffect : DestroyableObject
    {
        public IDamageable targetThatWeHit;

        [Export] protected Line2D targetingVisualLine;
        [Export] protected AnimatedSprite2D animatedSprite;

        private bool activatedDamageEffect = false;
        private float effectMaxActiveTime = 0.5f;
        private float effectActiveTimer = 0;

        public override void _Ready()
        {
            base._Ready();

            UpdateLineEndPosition();

            if (flipSpite && animatedSprite != null) animatedSprite.FlipH = true;
        }

        public override void _Process(double delta)
        {
            base._Process(delta);

            if (GameManager.Instance.isMultiplayerMatch && !GameManager.Instance.isHostOfMultiplayerMatch) return;

            if(effectActiveTimer < effectMaxActiveTime)
            {
                this.GlobalPosition = characterThisEffectIsAttachedTo.GlobalPosition;

                UpdateLineEndPosition();

                effectActiveTimer++;
            }
            else if(!activatedDamageEffect)
            {
                activatedDamageEffect = true;
                characterThisEffectIsAttachedTo.DealDamage();
                EffectsAndProjectilesSpawner.Instance.SpawnArchdruidRangedAttackImpact(characterThisEffectIsAttachedTo,((Node2D)targetThatWeHit).GlobalPosition);

                targetingVisualLine.Visible = false;
                animatedSprite.Visible = false;
            }
        }

        private void UpdateLineEndPosition()
        {
            if (targetThatWeHit != null)
            {
                Vector2 fixedTargetPosition = new Vector2(((Node2D)targetThatWeHit).GlobalPosition.X, GameManager.unitGroundYPosition);
                Vector2 newValue = fixedTargetPosition - targetingVisualLine.GlobalPosition;

                if (flipSpite)
                {
                    newValue.X -= 16;
                }
                else
                {
                    newValue.X += 16;
                }

                newValue.Y += 8;

                targetingVisualLine.SetPointPosition(1, newValue);
            }
        }
    }
}
