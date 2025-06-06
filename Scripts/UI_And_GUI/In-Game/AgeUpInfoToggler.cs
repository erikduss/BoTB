using Godot;
using System;

namespace Erikduss
{
	public partial class AgeUpInfoToggler : Control
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
            //we dont want to collapse this if we still have focus.
            if (GameSettingsLoader.Instance.useHighlightFocusMode)
            {
                if (!HasFocus()) return;
            }

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

        public void ShowAgeUpInfoOnFocus()
        {
            if (!GameSettingsLoader.Instance.useHighlightFocusMode) return;

            ShowAgeUpInfoOnHover();
        }

        public void HideAgeUpInfoOnLoseFocus()
        {
            if (!GameSettingsLoader.Instance.useHighlightFocusMode) return;

            HideAgeUpInfoOnLoseHover();
        }

        public void HideAgeUpInfoOnLoseHover()
        {
            //we dont want to collapse this if we still have focus.
            if (GameSettingsLoader.Instance.useHighlightFocusMode)
            {
                if (HasFocus()) return;
            }

            //we dont want to collapse this if we still have focus.
            if (GameSettingsLoader.Instance.useHighlightFocusMode)
            {
                if (HasFocus()) return;
            }

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
