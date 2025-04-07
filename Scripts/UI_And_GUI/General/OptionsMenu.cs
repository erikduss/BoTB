using Godot;
using System;
using System.Collections;
using System.Globalization;
using System.Linq;

namespace Erikduss
{
	public partial class OptionsMenu : Control
    {
        //we need to prompt the user if they are sure they want to go back if we have changes.
        public bool hasChangedSettings = false;
        public bool allowSFXFromOptionsMenu = false;

        public ChangedSettingsWarning changesWarningPanel;

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
        [Export] public OptionButton screenMovementTypeOptionButton;

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
			LoadScreenModes();

            SetLoadedValues();

            foreach (Node childNode in this.GetChildren())
            {
                if(childNode is ChangedSettingsWarning)
                {
                    changesWarningPanel = childNode as ChangedSettingsWarning;
                }
            }
        }

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

        public void SetLoadedValues()
        {
            GameUserOptionsConfig currSavedConfig = GameSettingsLoader.Instance.gameUserOptionsManager.currentlySavedUserOptions;

            musicAudioSlider.Value = currSavedConfig.musicVolume;
            otherAudioSlider.Value = currSavedConfig.otherVolume;

            screenMovementTypeOptionButton.Selected = (int)currSavedConfig.screenMovement;
            screenDragSensitivitySlider.Value = currSavedConfig.addedDragSensitivity;
            screenSidesSensitivitySlider.Value = currSavedConfig.addedSidesSensitivity;

            displayModeOptionButton.Selected = (int)currSavedConfig.displayMode;

            if(currSavedConfig.overrideScreenResolution != string.Empty)
            {
                screenResolutionsOptionButton.Selected = screenResolutionsOptionButton.ItemCount - 1;
            }
            else
            {
                screenResolutionsOptionButton.Selected = (int)currSavedConfig.screenResolution;
            }
            limitFPSOptionButton.Selected = currSavedConfig.limitFPS;
            fpsLimitSlider.Value = currSavedConfig.fpsLimit;
        }

		private void SetDefaultValues()
		{
            screenResolutionsOptionButton.Selected = 1;
            displayModeOptionButton.Selected = 2;

			limitFPSOptionButton.Selected = 1;
			screenMovementTypeOptionButton.Selected = 0;
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
        }

		private void LoadScreenModes()
		{
            for (int i = 0; i < Enum.GetNames(typeof(Enums.DisplayMode)).Length; i++)
            {
                string resolutionText = (((Enums.DisplayMode)i).ToString());

                resolutionText = resolutionText.Replace('_', ' ');

                displayModeOptionButton.AddItem(resolutionText);
            }
        }

		public void ReturnButtonPressed()
		{
			GameUserOptionsConfig currSavedConfig = GameSettingsLoader.Instance.gameUserOptionsManager.currentlySavedUserOptions;
			GameUserOptionsConfig overrideConfig = GameSettingsLoader.Instance.gameUserOptionsManager.overriddenUserOptions;

            string changesWarning = "You have unsaved changes, do you wish to exit or save these changes? You have the following section(s) with changes: \n";

			//check if the audio section is the same
			if (currSavedConfig.musicVolume != overrideConfig.musicVolume || currSavedConfig.otherVolume != overrideConfig.otherVolume) 
			{
                changesWarning = changesWarning + " - Audio Section";
                hasChangedSettings = true;
            }

            //check if the gameplay section is the same
            if (currSavedConfig.screenMovement != overrideConfig.screenMovement || currSavedConfig.addedDragSensitivity != overrideConfig.addedDragSensitivity ||
                currSavedConfig.addedSidesSensitivity != overrideConfig.addedSidesSensitivity)
            {
                changesWarning = changesWarning + " - Gameplay Section";
                hasChangedSettings = true;
            }

            //check if the graphics section is the same
            if (currSavedConfig.displayMode != overrideConfig.displayMode || currSavedConfig.screenResolution != overrideConfig.screenResolution || 
				currSavedConfig.overrideScreenResolution != overrideConfig.overrideScreenResolution || currSavedConfig.limitFPS != overrideConfig.limitFPS ||
                currSavedConfig.fpsLimit != overrideConfig.fpsLimit)
			{
                changesWarning = changesWarning + " - Graphics Section";
                hasChangedSettings = true;
            }

            //check if the Accessibility section is the same
            if (currSavedConfig.enableHemophobiaMode != overrideConfig.enableHemophobiaMode)
            {
                changesWarning = changesWarning + " - Accessibility Section";
                hasChangedSettings = true;
            }


			//if any of the sections have changes, we need to do a popup and warn the player about this and not close the options.
			//give the player the option to save or to discard
			//if saving it saves and closes the opions
			//if discarding it discards and closes the options.

            if (!hasChangedSettings)
			{
                AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickAudioClip);
                Visible = false;
            }
            else
            {
                AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickedFailedAudioClip);
                changesWarningPanel.changesDescriptionLabel.Text = changesWarning;
                changesWarningPanel.attachedOptionsMenu = this;
                allowSFXFromOptionsMenu = false;

                changesWarningPanel.Visible = true;
            }
		}

		public void SaveButtonPressed()
		{
            AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickAudioClip);
            GameSettingsLoader.Instance.gameUserOptionsManager.CreateNewSaveFile(true);
        }

		public void MusicAudioSliderOnValueChanged(float value)
		{
            if (allowSFXFromOptionsMenu)
            {
                AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.sliderChangedAudioClip);
            }
            
            musicAudioPercentage.Text = value.ToString() + "%";
			GameSettingsLoader.Instance.gameUserOptionsManager.overriddenUserOptions.musicVolume = (int)value;

        }

        public void OtherAudioSliderOnValueChanged(float value)
        {
            if (allowSFXFromOptionsMenu)
            {
                AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.sliderChangedAudioClip);
            }
            
            otherAudioPercentage.Text = value.ToString() + "%";
            GameSettingsLoader.Instance.gameUserOptionsManager.overriddenUserOptions.otherVolume = (int)value;
        }

        public void ScreenDragSensitivitySliderOnValueChanged(float value)
        {
            if (allowSFXFromOptionsMenu)
            {
                AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.sliderChangedAudioClip);
            }

            string newValueString = value.ToString();


            if (value == 0)
			{
				newValueString = "Default";
			}
			else if(value > 0)
			{
				newValueString = "+" + newValueString;
			}

            screenDragSensitivityValueLabel.Text = newValueString;

            GameSettingsLoader.Instance.gameUserOptionsManager.overriddenUserOptions.addedDragSensitivity = (int)value;
        }

        public void ScreenSidesSensitivitySliderOnValueChanged(float value)
        {
            if (allowSFXFromOptionsMenu)
            {
                AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.sliderChangedAudioClip);
            }

            string newValueString = value.ToString();


            if (value == 0)
            {
                newValueString = "Default";
            }
            else if (value > 0)
            {
                newValueString = "+" + newValueString;
            }

            screenSidesSensitivityValueLabel.Text = newValueString;

            GameSettingsLoader.Instance.gameUserOptionsManager.overriddenUserOptions.addedSidesSensitivity = (int)value;
        }

        public void FPSLimitSliderOnValueChanged(float value)
        {
            if (allowSFXFromOptionsMenu)
            {
                AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.sliderChangedAudioClip);
            }

            fpsLimitValueLabel.Text = value.ToString();

            GameSettingsLoader.Instance.gameUserOptionsManager.overriddenUserOptions.fpsLimit = (int)value;
        }

        public void ScreenMovementTypeSelected(int value)
        {
            if (allowSFXFromOptionsMenu)
            {
                AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.dropdownSelectionAudioClip);
            }

            GameSettingsLoader.Instance.gameUserOptionsManager.overriddenUserOptions.screenMovement = (Enums.ScreenMovementType)value;
        }

        public void DisplayModeSelected(int value)
        {
            if (allowSFXFromOptionsMenu)
            {
                AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.dropdownSelectionAudioClip);
            }

            GameSettingsLoader.Instance.gameUserOptionsManager.overriddenUserOptions.displayMode = (Enums.DisplayMode)value;
        }

        public void ScreenResolutionSelected(int value)
        {
            if (allowSFXFromOptionsMenu)
            {
                AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.dropdownSelectionAudioClip);
            }

            GameSettingsLoader.Instance.gameUserOptionsManager.overriddenUserOptions.screenResolution = (Enums.ScreenResolution)value;
        }

        public void LimitFpsOptionSelected(int value)
        {
            if (allowSFXFromOptionsMenu)
            {
                AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.dropdownSelectionAudioClip);
            }

            GameSettingsLoader.Instance.gameUserOptionsManager.overriddenUserOptions.limitFPS = value;
        }

        public void PlayGenericButtonHoverSound()
        {
            AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonHoverAudioClip);
        }
    }
}
