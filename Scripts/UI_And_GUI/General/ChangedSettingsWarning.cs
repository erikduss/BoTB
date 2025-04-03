using Godot;
using System;

namespace Erikduss
{
    public partial class ChangedSettingsWarning : Control
    {
        [Export] public Label changesDescriptionLabel;

        public OptionsMenu attachedOptionsMenu;

        public void DiscardChanges()
        {
            attachedOptionsMenu.allowSFXFromOptionsMenu = false;

            attachedOptionsMenu.SetLoadedValues();

            DestroyOptionsMenu();
        }

        public void SaveChanges()
        {
            attachedOptionsMenu.allowSFXFromOptionsMenu = false;

            attachedOptionsMenu.SaveButtonPressed();

            DestroyOptionsMenu();
        }

        public void DestroyOptionsMenu()
        {
            AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickAudioClip);

            attachedOptionsMenu.hasChangedSettings = false;
            attachedOptionsMenu.Visible = false;
            attachedOptionsMenu.changesWarningPanel.Visible = false;
        }
    }
}
