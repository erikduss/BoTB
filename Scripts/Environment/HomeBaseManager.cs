using Godot;
using System;
using System.Collections.Generic;

namespace Erikduss
{
	public partial class HomeBaseManager : Node2D
	{
		[Export] public bool requiresToBeFlipped = false;
        [Export] public StaticBody2D StaticBody;

        public List<Sprite2D> homeBaseSprites = new List<Sprite2D>();
        public CollisionShape2D colliderShape;

        [Export] public ColorRect healthBarFiller;

        private int currentBaseSpriteIndex = 0;

        public int currentBaseHealth = 1000;
        public int maxBaseHealth = 1000;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
            int spriteID = 0;

            foreach (Node childComponent in this.GetChildren())
            {
                if (childComponent is Sprite2D)
                {
                    Sprite2D spriteComponent = childComponent.GetNode<Sprite2D>(childComponent.GetPath());

                    //spriteComponent.Visible = false;
                    homeBaseSprites.Add(spriteComponent);
                    
                    if (spriteID > 0) spriteComponent.Visible = false;
                    
                    spriteID++;
                }
                else if (childComponent is StaticBody2D)
                {
                    colliderShape = childComponent.GetNode<CollisionShape2D>("CollisionShape2D");
                    //colliderShape = childComponent.GetNode<CollisionShape2D>(childComponent.GetPath());
                }
                else if(requiresToBeFlipped && childComponent is Control)
                {
                    foreach (Node uiChild in childComponent.GetChildren())
                    {
                        if (uiChild is ColorRect)
                        {
                            ColorRect healthBarFiller = uiChild.GetNode<ColorRect>(uiChild.GetPath());
                            healthBarFiller.Scale = new Vector2(-1, -1);
                            healthBarFiller.Position = new Vector2(59, 40);
                        }
                        else if (uiChild is TextureRect)
                        {
                            TextureRect healthBarBorder = uiChild.GetNode<TextureRect>(uiChild.GetPath());
                            healthBarBorder.FlipH = true;
                        }
                    }
                }
            }

            if(colliderShape == null)
            {
                colliderShape = GetNode<CollisionShape2D>("CollisionShape2D");
            }

            if(requiresToBeFlipped)
            {
                StaticBody.CollisionLayer = 0b1000000; //this is our layer, in this case team 2
                StaticBody.CollisionMask = 0b10; //what we check for

                foreach(Sprite2D sprite in homeBaseSprites)
                {
                    sprite.FlipH = true;
                }

                colliderShape.Position = new Vector2(52.5f, colliderShape.Position.Y);
                GD.Print(colliderShape.Position);
            }
            else
            {
                StaticBody.CollisionLayer = 0b100000; 
                StaticBody.CollisionMask = 0b100; 
            }
        }

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

        public void TakeDamage(int rawDamage)
        {
            GD.Print("Home base took " + rawDamage + " Damage");
            currentBaseHealth -= rawDamage;
            UpdateBaseHealthbarAndSprite();
        }

        private void UpdateBaseHealthbarAndSprite()
        {
            //The health bar filler is 55px wide, which should be equal to 100% of the health.

            float percentageMultiplier = (float)maxBaseHealth / 100f; //should equal 10
            float amountOfHealthPerPercentage = 55f / 100f; //should be 0.55

            float healthPercentage = (float)currentBaseHealth / 10f;

            if(healthPercentage > 50 && healthPercentage <= 75)
            {
                if(currentBaseSpriteIndex != 1)
                {
                    homeBaseSprites[1].Visible = true;

                    homeBaseSprites[0].Visible = false;
                    homeBaseSprites[2].Visible = false;
                    homeBaseSprites[3].Visible = false;
                    homeBaseSprites[4].Visible = false;

                    currentBaseSpriteIndex = 1;
                }
            }
            else if (healthPercentage > 25 && healthPercentage <=50)
            {
                if (currentBaseSpriteIndex != 2)
                {
                    homeBaseSprites[2].Visible = true;

                    homeBaseSprites[0].Visible = false;
                    homeBaseSprites[1].Visible = false;
                    homeBaseSprites[3].Visible = false;
                    homeBaseSprites[4].Visible = false;

                    currentBaseSpriteIndex = 2;
                }
            }
            else if(healthPercentage > 0 && healthPercentage <= 25)
            {
                if (currentBaseSpriteIndex != 3)
                {
                    homeBaseSprites[3].Visible = true;

                    homeBaseSprites[0].Visible = false;
                    homeBaseSprites[1].Visible = false;
                    homeBaseSprites[2].Visible = false;
                    homeBaseSprites[4].Visible = false;

                    currentBaseSpriteIndex = 3;
                }
            }
            else if(healthPercentage == 0)
            {
                if (currentBaseSpriteIndex != 4)
                {
                    homeBaseSprites[4].Visible = true;

                    homeBaseSprites[0].Visible = false;
                    homeBaseSprites[1].Visible = false;
                    homeBaseSprites[2].Visible = false;
                    homeBaseSprites[3].Visible = false;

                    currentBaseSpriteIndex = 4;
                }
            }
            else
            {

            }

            float newFillerWith = healthPercentage * amountOfHealthPerPercentage;

            healthBarFiller.Size = new Vector2(newFillerWith, healthBarFiller.Size.Y);
        }
	}
}
