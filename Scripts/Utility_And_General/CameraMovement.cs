using Godot;
using System;

namespace Erikduss
{
	public partial class CameraMovement : Camera2D
	{
		
		[Export] public Camera2D mainCamera;

        private float threshold = 100;
        private float step = 10;
		private Vector2 viewport_size;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			viewport_size = GetViewport().GetVisibleRect().Size;

			if(mainCamera == null) mainCamera = GetNode("Camera2D") as Camera2D;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			Vector2 local_mouse_pos = GetViewport().GetMousePosition();

			if(local_mouse_pos.X < threshold)
			{
				mainCamera.Position = new Vector2(mainCamera.Position.X - step, mainCamera.Position.Y);
			}
			else if(local_mouse_pos.X > viewport_size.X - threshold)
			{
                mainCamera.Position = new Vector2(mainCamera.Position.X + step, mainCamera.Position.Y);
            }
        }
	}
}
