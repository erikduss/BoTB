using Godot;
using System;

namespace Erikduss
{
	public partial class AgeAbilityInfoToggler : Node
	{
        [Export] TextureProgressBar abilityProgressbar;
        Texture2D defaultProgressBarTexture;
        [Export] Texture2D hoverProgressBarTexture;

        [Export] public Label abilityCooldownLabel;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
            GameManager.Instance.inGameHUDManager.abilityCooldownBar = abilityProgressbar;

            defaultProgressBarTexture = abilityProgressbar.TextureProgress;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

        public void ShowAgeAbilityInfoOnHover()
        {
            abilityProgressbar.TextureProgress = hoverProgressBarTexture;

            foreach (var child in this.GetChildren())
            {
                if (child is TextureRect)
                {
                    TextureRect ageAbilityInfoPanel = child as TextureRect;

                    if (ageAbilityInfoPanel != null)
                    {
                        ageAbilityInfoPanel.Visible = true;
                    }

                    continue;
                }
            }
        }

        public void HideAgeAbilityInfoOnLoseHover()
        {
            abilityProgressbar.TextureProgress = defaultProgressBarTexture;

            foreach (var child in this.GetChildren())
            {
                if (child is TextureRect)
                {
                    TextureRect ageAbilityInfoPanel = child as TextureRect;

                    if (ageAbilityInfoPanel != null)
                    {
                        ageAbilityInfoPanel.Visible = false;
                    }

                    continue;
                }
            }
        }
    }
}