using Godot;
using System;

namespace Erikduss
{
	public partial class BasePowerUpInfoToggler : Control
	{
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

        public virtual void ShowPowerUpInfoOnHover()
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

        public virtual void ShowPowerUpInfoOnFocus()
        {
            if (!GameSettingsLoader.Instance.useHighlightFocusMode) return;

            ShowPowerUpInfoOnHover();
        }

        public virtual void HidePowerUpInfoOnLoseFocus()
        {
            if (!GameSettingsLoader.Instance.useHighlightFocusMode) return;

            HidePowerUpInfoOnLoseHover();
        }

        public virtual void HidePowerUpInfoOnLoseHover()
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

        public virtual void ProcessPowerUpEffect()
        {
            BasePlayer player = GameManager.Instance.clientTeamOwner == Enums.TeamOwner.TEAM_01 ? GameManager.Instance.player01Script : GameManager.Instance.player02Script;

            player.hasUnlockedPowerUpCurrently = false;

            GameManager.Instance.inGameHUDManager.RefreshPowerUp(false);
        }
    }
}
