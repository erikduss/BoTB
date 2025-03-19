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
            attachedOptionsMenu.SetLoadedValues();

            DestroyOptionsMenu();
        }

        public void SaveChanges()
        {
            attachedOptionsMenu.SaveButtonPressed();

            DestroyOptionsMenu();
        }

        public void DestroyOptionsMenu()
        {
            attachedOptionsMenu.hasChangedSettings = false;
            attachedOptionsMenu.Visible = false;
            attachedOptionsMenu.changesWarningPanel.Visible = false;
        }
    }
}
