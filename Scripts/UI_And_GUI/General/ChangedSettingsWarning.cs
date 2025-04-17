using Godot;
using System;

namespace Erikduss
{
    public partial class ChangedSettingsWarning : Control
    {
        [Export] public Label changesDescriptionLabel;

        public OptionsMenu attachedOptionsMenu;

        private Control currentlySelectedControl = null;
        [Export] private Control defaultSelectedControl;

        public override void _Ready()
        {
            base._Ready();

            GetViewport().GuiFocusChanged += OnControlElementFocusChanged;
        }

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

        private void OnControlElementFocusChanged(Control control)
        {
            if (GameSettingsLoader.Instance.useHighlightFocusMode)
            {
                if (control != currentlySelectedControl)
                {
                    //change color back
                    if (currentlySelectedControl != null)
                    {
                        currentlySelectedControl.SelfModulate = new Color(1, 1, 1);
                        AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonHoverAudioClip);
                    }
                }

                control.SelfModulate = GameSettingsLoader.Instance.focussedControlColor;
            }

            currentlySelectedControl = control;
        }

        public void SetDefaultSelectedControl()
        {
            defaultSelectedControl.GrabFocus();
        }
    }
}
