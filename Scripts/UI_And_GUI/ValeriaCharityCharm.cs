using Godot;
using System;

namespace Erikduss
{
    public partial class ValeriaCharityCharm : Control
    {
        [Export] private Sprite2D charmSprite;

        public override void _Ready()
        {
            GameSettingsLoader.Instance.currentCharmsLoaded.Add(this);
            SetCharmVisibility();
        }

        public void SetCharmVisibility()
        {
            charmSprite.Visible = GameSettingsLoader.Instance.showCharms;
        }

        protected override void Dispose(bool disposing)
        {
            GameSettingsLoader.Instance.currentCharmsLoaded.Remove(this);
            base.Dispose(disposing);
        }
    }
}

