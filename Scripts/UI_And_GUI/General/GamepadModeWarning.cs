using Godot;
using System;

namespace Erikduss
{
    public partial class GamepadModeWarning : Control
    {
        [Export] public Label descriptionLabel;
        [Export] public Control returnButton;

        public override void _Ready()
        {
            if (Input.GetConnectedJoypads().Count > 0)
            {
                descriptionLabel.Text = Tr("GAMEPAD_WARNING_DESCRIPTION_CONNECT");
            }
            else
            {
                descriptionLabel.Text = Tr("GAMEPAD_WARNING_DESCRIPTION_DISCONNECT");
            }

            returnButton.GrabFocus();
        }

        public void ClosePanel()
        {
            GameSettingsLoader.Instance.previouslySelectedControlBeforeControllerChange.GrabFocus();
            GameSettingsLoader.Instance.hasInstatiatedWarning = false;
            QueueFree();
        }
    }
}
