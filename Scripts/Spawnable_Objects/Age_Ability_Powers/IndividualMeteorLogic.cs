using Godot;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace Erikduss
{
	public partial class IndividualMeteorLogic : RigidBody2D
	{
        public bool isDiagonalMeteor = false;

        public Enums.TeamOwner meteorOwner = Enums.TeamOwner.NONE;

        private List<AnimatedSprite2D> animatedMeteorSprites = new List<AnimatedSprite2D>();

        public float projectileVelocity = 200f;

        private Vector2 savedVelocityBeforePause = Vector2.Zero;
        private bool setVelocity = false;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
            foreach (Node childNode in this.GetChildren())
            {
                if (childNode is AnimatedSprite2D)
                {
                    AnimatedSprite2D spriteComponent = childNode.GetNode<AnimatedSprite2D>(childNode.GetPath());

                    spriteComponent.Visible = false;
                    animatedMeteorSprites.Add(spriteComponent);
                }
            }

            if (isDiagonalMeteor) 
            {
                int amountOfVerticals = (int)Math.Floor((animatedMeteorSprites.Count * 0.5f));

                int spriteID = (int)(GD.Randi() % (amountOfVerticals));
                animatedMeteorSprites[spriteID].Visible = true;

                //for some reason the diagonal is the other way around
                if(projectileVelocity > 0)
                {
                    animatedMeteorSprites[spriteID].FlipH = true;
                    animatedMeteorSprites[spriteID].Position = new Vector2(-animatedMeteorSprites[spriteID].Position.X, animatedMeteorSprites[spriteID].Position.Y);
                }
            }
            else
            {
                int amountOfDiagonals = (int)Math.Floor((animatedMeteorSprites.Count * 0.5f));

                int spriteID = (int)(GD.Randi() % (amountOfDiagonals));

                spriteID += (int)Math.Floor((animatedMeteorSprites.Count * 0.5f));
                animatedMeteorSprites[spriteID].Visible = true;

                if (projectileVelocity < 0)
                {
                    animatedMeteorSprites[spriteID].FlipH = true;
                    animatedMeteorSprites[spriteID].Position = new Vector2(-animatedMeteorSprites[spriteID].Position.X, animatedMeteorSprites[spriteID].Position.Y);
                }
            }

            LinearVelocity = new Vector2(projectileVelocity, 0);
        }

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
            HandlePausingPhysics();
		}

        private void HandlePausingPhysics()
        {
            if (GameManager.Instance.gameIsPaused)
            {
                if (!setVelocity)
                {
                    savedVelocityBeforePause = LinearVelocity;
                    LinearVelocity = Vector2.Zero;
                    GravityScale = 0;
                    setVelocity = true;
                }
            }
            else
            {
                if (setVelocity)
                {
                    LinearVelocity = savedVelocityBeforePause;
                    GravityScale = 0.1f;
                    setVelocity = false;

                    savedVelocityBeforePause = Vector2.Zero;
                }
            }
        }

        public void OnCollisionEnter(Node2D body)
        {
            EffectsAndProjectilesSpawner.Instance.SpawnMeteorImpactAtPosition(this.GlobalPosition, meteorOwner);

            QueueFree();
        }
    }
}
