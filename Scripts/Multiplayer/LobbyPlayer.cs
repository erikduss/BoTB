using Godot;
using System;

namespace Erikduss
{
    public partial class LobbyPlayer : Control
    {
        private bool isOwner;

        [Export] private CheckBox readyCheckbox;

        public override void _Ready()
        {
            base._Ready();

            GDSync.ConnectGDSyncOwnerChanged(this, new Callable(this, "OwnerChanged"));
        }

        public void OwnerChanged(int ownerID)
        {
            isOwner = GDSync.IsGDSyncOwner(this);

            if(!isOwner) readyCheckbox.MouseFilter = MouseFilterEnum.Ignore; //disble the button without disabling it. Probably doesnt work for controller
            //for controller later to fix being able to press the other ready button. While linking the focus elements, skip the one that is not yours.
        }

        public void readyValueChanged(bool toggledOn)
        {
            readyCheckbox.Text = toggledOn ? "Ready" : "Not Ready";
        }
    }
}
