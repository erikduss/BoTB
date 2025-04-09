using Godot;
using System;

namespace Erikduss
{
	public partial class AgeAbilityInfoToggler : Control
	{
        [Export] TextureProgressBar abilityProgressbar;
        Texture2D defaultProgressBarTexture;
        [Export] Texture2D hoverProgressBarTexture;

        [Export] public Label abilityCooldownLabel;
        [Export] public Label abilityEmpowerLabel;

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
            //we dont want to collapse this if we still have focus.
            if (GameSettingsLoader.Instance.useHighlightFocusMode)
            {
                if (!HasFocus()) return;
            }

            AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonHoverAudioClip);

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
