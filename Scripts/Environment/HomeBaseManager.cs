using Godot;
using System;
using System.Collections.Generic;

namespace Erikduss
{
	public partial class HomeBaseManager : Node2D
	{
		[Export] public bool requiresToBeFlipped = false;

        public Sprite2D homeBaseSprite;
        public CollisionShape2D colliderShape;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
            foreach (Node childComponent in this.GetChildren())
            {
                if (childComponent is Sprite2D)
                {
                    Sprite2D spriteComponent = childComponent.GetNode<Sprite2D>(childComponent.GetPath());

                    //spriteComponent.Visible = false;
                    homeBaseSprite = spriteComponent;
                }
                else if (childComponent is StaticBody2D)
                {
                    colliderShape = childComponent.GetNode<CollisionShape2D>("CollisionShape2D");
                    //colliderShape = childComponent.GetNode<CollisionShape2D>(childComponent.GetPath());
                }
            }

            if(colliderShape == null)
            {
                colliderShape = GetNode<CollisionShape2D>("CollisionShape2D");
            }

            if(requiresToBeFlipped)
            {
                homeBaseSprite.FlipH = true;
                colliderShape.Position = new Vector2(64, colliderShape.Position.Y);
            }
        }

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}
	}
}
