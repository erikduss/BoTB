using Godot;
using System;
using System.Linq;

namespace Erikduss
{
	public partial class OptionsMenu : Control
	{
		//we need to prompt the user if they are sure they want to go back if we have changes.
		private bool hasChangedSettings = false;


		//What do we need to do?
		/*
		 * We need to load the settings from a save file.
		 * We need to save these values in a temporary file here so we can compare the current working file to the saved one.
		 * We need to be able to save the current working file to the save file.
		 * 
		 * Probably will need to change the music volume live or play a small jingle
		 * ^ Same for other audio volumes
		 * 
		 * Resolution and full screen stuff only needs to be available on pc.
		 * 
		 * Do we need anything else?
		 * 
		 * 
		 * Drop down for screen move setting:
		 * Both - Drag only - Screen side hover
		 * 
		 * Sensitivity for drag impulse
		 * Sensitivity for side of screen hover
		 * 
		 * 
		 * Max FPS setting
		 */

		//Volumes Settings
		[Export] public Label musicAudioPercentage;
        [Export] public Slider musicAudioSlider;

        [Export] public Label otherAudioPercentage;
        [Export] public Slider otherAudioSlider;

		//Gameplay Settings
		public Enums.ScreenMovementType screenMovementTypeToUse;

        [Export] public Label screenDragSensitivityValueLabel;
        [Export] public Slider screenDragSensitivitySlider;

        [Export] public Label screenSidesSensitivityValueLabel;
        [Export] public Slider screenSidesSensitivitySlider;

        //Graphics Settings
        [Export] public OptionButton displayModeOptionButton;
        [Export] public OptionButton screenResolutionsOptionButton;
		[Export] public OptionButton limitFPSOptionButton;
		[Export] public Slider fpsLimitSlider;
		[Export] public Label fpsLimitValueLabel;


        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
			LoadScreenResolutions();

        }

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		private void LoadScreenResolutions()
		{
            for (int i = 0; i < Enum.GetNames(typeof(Enums.ScreenResolution)).Length; i++)
            {
                string resolutionText = (((Enums.ScreenResolution)i).ToString());

				resolutionText = resolutionText.Split('_')[1];
				//will give for example: 1920x1080

				screenResolutionsOptionButton.AddItem(resolutionText);
            }

			screenResolutionsOptionButton.Selected = 1;
        }

		public void ReturnButtonPressed()
		{
			if (!hasChangedSettings)
			{
				QueueFree();
			}
		}

		public void SaveButtonPressed()
		{

		}
	}
}
