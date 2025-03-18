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

            if (GameSettingsLoader.Instance.gameUserOptionsManager.currentlySavedUserOptions.screenMovement == Enums.ScreenMovementType.Only_Use_Drag_Movement) return;

			Vector2 local_mouse_pos = GetViewport().GetMousePosition();

			//MOVE THE CAMERA TO THE LEFT
			if(local_mouse_pos.X < threshold)
			{
                MoveCamera(true, false, 1);
            }
			//MOVE THE CAMERA TO THE RIGHT
			else if(local_mouse_pos.X > viewport_size.X - threshold)
			{
				MoveCamera(false, false, 1);
            }
        }

		private void MoveCamera(bool moveLeft, bool multiplySpeed, float speedMultiplier)
		{
            float fixedSpeedMultiplier = Mathf.Abs(speedMultiplier);

            //this value seems to be between 1 and 40 or so, this fixes the speed to be between 0.1 and 4x the speed.
            //This creates a way smoother experience with the dragging method.
            if (multiplySpeed) fixedSpeedMultiplier = fixedSpeedMultiplier * 0.1f;

            float fixedStepSensitivity = step;
            //This is a drag movement
            if (multiplySpeed)
            {
                float valueToAddToStep = ((float)GameSettingsLoader.Instance.gameUserOptionsManager.currentlySavedUserOptions.addedDragSensitivity) / 12.5f;
                fixedStepSensitivity += valueToAddToStep;
            }
            else
            {
                float valueToAddToStep = ((float)GameSettingsLoader.Instance.gameUserOptionsManager.currentlySavedUserOptions.addedSidesSensitivity) / 12.5f;
                fixedStepSensitivity += valueToAddToStep;
            }

            //MOVE THE CAMERA TO THE LEFT
            if (moveLeft)
            {
                //make sure we cant go off the map with the camera
                if (mainCamera.Position.X > minimumCameraXValue)
                {
                    mainCamera.Position = new Vector2(mainCamera.Position.X - (fixedStepSensitivity * fixedSpeedMultiplier), mainCamera.Position.Y);
                }
            }
            //MOVE THE CAMERA TO THE RIGHT
            else if (!moveLeft)
            {
                //make sure we cant go off the map with the camera
                if (mainCamera.Position.X < maximumCameraXValue)
                {
                    mainCamera.Position = new Vector2(mainCamera.Position.X + (fixedStepSensitivity * fixedSpeedMultiplier), mainCamera.Position.Y);
                }
            }
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);

            if(GameManager.Instance.gameIsPaused) return;

            if (GameSettingsLoader.Instance.gameUserOptionsManager.currentlySavedUserOptions.screenMovement == Enums.ScreenMovementType.Only_Use_Screen_Sides_Movement) return;

			if(@event is InputEventScreenDrag)
			{
				InputEventScreenDrag dragEvent = @event as InputEventScreenDrag;

				if (dragEvent != null)
				{
                    if (Math.Abs(dragEvent.Relative.X) > 100)
                    {
                        //Seems like drag even also registers on clicks, this is to prevent far away clicks from moving the screen.
                        return;
                    }

                    //only x matters, -x means we are dragging left, so we go right, +x means we drag right so we go left.
                    if(dragEvent.Relative.X < 0)
                    {
                        MoveCamera(false, true, dragEvent.Relative.X);
                    }
                    else if(dragEvent.Relative.X > 0)
                    {
                        MoveCamera(true, true, dragEvent.Relative.X);
                    }
				}
			}

            if (@event is InputEventScreenTouch)
            {
                InputEventScreenTouch touchEvent = @event as InputEventScreenTouch;

                if (touchEvent != null)
                {
                    //GD.Print("Touch Event: " + touchEvent.Position);
                }
            }
        }
    }
}
