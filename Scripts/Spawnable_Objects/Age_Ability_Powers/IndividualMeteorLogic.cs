using Godot;
using System;

namespace Erikduss
{
	public partial class IndividualMeteorLogic : RigidBody2D
	{
        private float projectileVelocity = 200f;
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
            float xVelocity = (float)(GD.Randi() % (400f));
            xVelocity -= 200f;
            projectileVelocity = xVelocity;

            LinearVelocity = new Vector2(projectileVelocity, 0);
        }

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

        public void OnCollisionEnter(Node2D body)
        {
            QueueFree();
        }
    }
}
