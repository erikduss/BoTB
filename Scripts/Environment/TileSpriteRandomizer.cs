using Godot;
using System;
using System.Collections.Generic;

namespace Erikduss
{
	public partial class TileSpriteRandomizer : Node2D
	{
        public List<Sprite2D> availableTileTextures = new List<Sprite2D>();
        public int chosenTexture;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            //We need to add all available texture to the list and set them to not visible
            foreach (Node sprite in this.GetChildren())
            {
                if (sprite is Sprite2D)
                {
                    Sprite2D spriteComponent = sprite.GetNode<Sprite2D>(sprite.GetPath());

                    spriteComponent.Visible = false;
                    availableTileTextures.Add(spriteComponent);
                }
            }

            //Now we choose a random texture and turn it visible.
            chosenTexture = (int)(GD.Randi() % (availableTileTextures.Count - 1));

            availableTileTextures[chosenTexture].Visible = true;
        }
    }
}
