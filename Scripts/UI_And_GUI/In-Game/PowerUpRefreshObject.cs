using Godot;
using System;

namespace Erikduss
{
    public partial class PowerUpRefreshObject : Control
    {
        [Export] private Label amountOfPowerUpRefreshesAvailable;

        public void SetAmountOfPowerUpRefreshes(int amount)
        {
            amountOfPowerUpRefreshesAvailable.Text = amount.ToString() + "x"; 
        }

        public void RefreshPowerUpButtonClicked()
        {
            BasePlayer player = GameManager.Instance.clientTeamOwner == Enums.TeamOwner.TEAM_01 ? GameManager.Instance.player01Script : GameManager.Instance.player02Script;

            if (player.playerCurrentPowerUpRerollsAmount <= 0)
            {
                GD.Print("The player does not have any rerolls");
                AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickedFailedAudioClip);
                return;
            }

            if (!GameManager.Instance.inGameHUDManager.DoesThePlayerHaveAPowerUpUnlocked(player))
            {
                GD.Print("The player does not have a powerup unlocked");
                AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickedFailedAudioClip);
                return;
            }

            AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickAudioClip);
            GameManager.Instance.inGameHUDManager.RefreshPowerUp();
        }
    }
}
