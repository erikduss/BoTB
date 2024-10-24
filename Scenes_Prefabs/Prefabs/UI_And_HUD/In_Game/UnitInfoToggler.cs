using Godot;
using System;

namespace Erikduss
{
	public partial class UnitInfoToggler : TextureButton
	{
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

        public void ShowUnitInfoOnHover()
        {
            foreach(var child in this.GetChildren())
            {
                if (child is TextureRect)
                {
                    TextureRect unitInfoPanel = child as TextureRect;

                    if (unitInfoPanel != null)
                    {
                        unitInfoPanel.Visible = true;
                    }

                    continue;
                }
            }
        }

        public void HideUnitInfoOnLoseHover()
        {
            foreach (var child in this.GetChildren())
            {
                if (child is TextureRect)
                {
                    TextureRect unitInfoPanel = child as TextureRect;

                    if (unitInfoPanel != null)
                    {
                        unitInfoPanel.Visible = false;
                    }

                    continue;
                }
            }
        }
    }
}
