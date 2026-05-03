using Godot;
using System;
using System.Collections.Generic;

namespace Erikduss
{
    public partial class GamepadModeWarning : Control
    {
        [Export] public Label descriptionLabel;
        [Export] public Control returnButton;
        [Export] public Label controllerNamesLabel;

        public override void _Ready()
        {
            List<string> controllerNames = new List<string>();

            for (int i = 0; i < Input.GetConnectedJoypads().Count; i++)
            {
                controllerNames.Add(Input.GetJoyName(i));
            }

            List<string> actualControllerNames = new List<string>();

            //we want to check if this is an actual controller
            foreach (string name in controllerNames)
            {
                string lowerName = name.ToLower();
                if (lowerName.Contains("controller"))
                {
                    actualControllerNames.Add(name);
                }
            }

            if (Input.GetConnectedJoypads().Count > 0 && actualControllerNames.Count != 0)
            {
                descriptionLabel.Text = Tr("GAMEPAD_WARNING_DESCRIPTION_CONNECT");

                for (int i = 0; i < actualControllerNames.Count; i++)
                {
                    if(i != actualControllerNames.Count - 1)
                    {
                        controllerNamesLabel.Text = controllerNamesLabel.Text + actualControllerNames[i] + ", ";
                    }
                    else
                    {
                        //last one
                        controllerNamesLabel.Text = controllerNamesLabel.Text + actualControllerNames[i];
                    }
                } 
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
