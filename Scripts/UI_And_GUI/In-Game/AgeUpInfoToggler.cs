using Godot;
using System;

namespace Erikduss
{
	public partial class AgeUpInfoToggler : Node
	{
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

        public void ShowAgeUpInfoOnHover()
        {
            AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonHoverAudioClip);

            foreach (var child in this.GetChildren())
            {
                if (child is TextureRect)
                {
                    TextureRect ageUpInfoPanel = child as TextureRect;

                    if (ageUpInfoPanel != null)
                    {
                        ageUpInfoPanel.Visible = true;
                    }

                    continue;
                }
            }
        }

        public void HideAgeUpInfoOnLoseHover()
        {
            foreach (var child in this.GetChildren())
            {
                if (child is TextureRect)
                {
                    TextureRect ageUpInfoPanel = child as TextureRect;

                    if (ageUpInfoPanel != null)
                    {
                        ageUpInfoPanel.Visible = false;
                    }

                    continue;
                }
            }
        }
    }
}
