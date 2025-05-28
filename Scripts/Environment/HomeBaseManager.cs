using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Erikduss
{
	public partial class HomeBaseManager : Node2D, IDamageable
	{
        public Enums.TeamOwner homeBaseOwner = Enums.TeamOwner.NONE;

		[Export] public bool requiresToBeFlipped = false;
        public StaticBody2D StaticBody;

        public List<Sprite2D> homeBaseSprites = new List<Sprite2D>();
        public CollisionShape2D colliderShape;

        [Export] public ColorRect healthBarFiller;
        [Export] public Label healthBarValueLabel;

        private int currentBaseSpriteIndex = 0;

        #region Interface Implementation
        public bool IsDeadOrDestroyed
        {
            get { return isDeadOrDestroyed; }
        }
        protected bool isDeadOrDestroyed = false;

        public int CurrentHealth
        {
            get { return currentHealth; }
        }
        protected int currentHealth = 1000; //should be set from a general options static value

        public int MaxHealth
        {
            get { return maxHealth; }
        }
        protected int maxHealth = 1000; //should be set from a general options static value
        #endregion

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
                    StaticBody = childComponent.GetNode<StaticBody2D>(childComponent.GetPath());
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

                //StaticBody.Position = new Vector2(105f, StaticBody.Position.Y);
                //colliderShape.GlobalPosition = new Vector2(52.5f, colliderShape.Position.Y);
                colliderShape.Position = new Vector2(52.5f, colliderShape.Position.Y);

                StaticBody.ForceUpdateTransform();

                homeBaseOwner = Enums.TeamOwner.TEAM_02;
            }
            else
            {
                StaticBody.CollisionLayer = 0b100000; 
                StaticBody.CollisionMask = 0b100;

                homeBaseOwner = Enums.TeamOwner.TEAM_01;
            }

            healthBarValueLabel.Text = CurrentHealth.ToString();
        }

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

        public void TakeDamage(int rawDamage)
        {
            currentHealth -= rawDamage;

            //award teams with power up progress.
            #region PowerUp progress reward
            int powerUpProgressAmountRewardedSelf = rawDamage * GameSettingsLoader.powerUpProgressMultiplierOwnBaseDamage;
            int powerUpProgressAmountRewardedEnemy = (int)MathF.Floor((float)rawDamage * GameSettingsLoader.powerUpProgressMultiplierOtherBaseDamage);

            GameManager.Instance.UpdatePlayerPowerUpProgress(homeBaseOwner, powerUpProgressAmountRewardedSelf);
            Enums.TeamOwner enemyTeamOwner = homeBaseOwner == Enums.TeamOwner.TEAM_01 ? Enums.TeamOwner.TEAM_02 : Enums.TeamOwner.TEAM_01;

            if (powerUpProgressAmountRewardedEnemy < 0) powerUpProgressAmountRewardedEnemy = 1;

            GameManager.Instance.UpdatePlayerPowerUpProgress(enemyTeamOwner, powerUpProgressAmountRewardedEnemy);
            #endregion

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                isDeadOrDestroyed = true;
                GameManager.Instance.EndCurrentGame();
            }

            healthBarValueLabel.Text = CurrentHealth.ToString();

            EffectsAndProjectilesSpawner.Instance.SpawnHomeBaseFloatingDamageNumber(this, rawDamage);

            UpdateBaseHealthbarAndSprite();
        }

        public void HealDamage(int healAmount)
        {
            //we dont have a way to heal the homebase yet

            if (IsDeadOrDestroyed) return;

            currentHealth += healAmount;

            if (currentHealth > maxHealth) currentHealth = maxHealth;

            UpdateBaseHealthbarAndSprite();
        }

        private void UpdateBaseHealthbarAndSprite()
        {
            //The health bar filler is 55px wide, which should be equal to 100% of the health.

            float percentageMultiplier = (float)MaxHealth / 100f; //should equal 10
            float amountOfHealthPerPercentage = 55f / 100f; //should be 0.55

            float healthPercentage = (float)CurrentHealth / percentageMultiplier;

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
            else if(healthPercentage <= 0)
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
                GD.Print("OTHER VALUE: " + healthPercentage);
            }

            float newFillerWith = healthPercentage * amountOfHealthPerPercentage;

            healthBarFiller.Size = new Vector2(newFillerWith, healthBarFiller.Size.Y);
        }
	}
}
