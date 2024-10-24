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

		public float minimumCameraXValue = -1150f;
		public float maximumCameraXValue = 2070f;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			viewport_size = GetViewport().GetVisibleRect().Size;

			if(mainCamera == null) mainCamera = GetNode("Camera2D") as Camera2D;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			if (GameManager.Instance.gameIsPaused) return;

			Vector2 local_mouse_pos = GetViewport().GetMousePosition();

			//MOVE THE CAMERA TO THE LEFT
			if(local_mouse_pos.X < threshold)
			{
				//make sure we cant go off the map with the camera
				if (mainCamera.Position.X > minimumCameraXValue)
				{
					mainCamera.Position = new Vector2(mainCamera.Position.X - step, mainCamera.Position.Y);
				}
			}
			//MOVE THE CAMERA TO THE RIGHT
			else if(local_mouse_pos.X > viewport_size.X - threshold)
			{
                //make sure we cant go off the map with the camera
                if (mainCamera.Position.X < maximumCameraXValue)
				{
                    mainCamera.Position = new Vector2(mainCamera.Position.X + step, mainCamera.Position.Y);
                }
            }
        }
	}
}
