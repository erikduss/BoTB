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
            if(GameManager.Instance.player01Script.playerCurrentPowerUpRerollsAmount <= 0)
            {
                AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickedFailedAudioClip);
                return;
            }

            if (!GameManager.Instance.inGameHUDManager.DoesThePlayerHaveAPowerUpUnlocked())
            {
                AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickedFailedAudioClip);
                return;
            }

            AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickAudioClip);
            GameManager.Instance.inGameHUDManager.RefreshPowerUp();
        }
    }
}
