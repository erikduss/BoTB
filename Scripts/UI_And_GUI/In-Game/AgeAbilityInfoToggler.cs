using Godot;
using System;

namespace Erikduss
{
	public partial class AgeAbilityInfoToggler : Node
	{
        [Export] ProgressBar abilityProgressbar;

        [Export] public Label abilityCooldownLabel;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
            GameManager.Instance.inGameHUDManager.abilityCooldownBar = abilityProgressbar;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

        public void ShowAgeAbilityInfoOnHover()
        {
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
