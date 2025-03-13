using Godot;
using System;
using System.Linq;

namespace Erikduss
{
    public partial class GameUserOptionsManager : BaseSettingsManager
    {
        public GameUserOptionsConfig currentlySavedUserOptions = new GameUserOptionsConfig();
        public GameUserOptionsConfig overriddenUserOptions = new GameUserOptionsConfig();

        public override void _Ready()
        {
            base._Ready();
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
        }

        public void ToggleScreenMode()
        {
            if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Windowed)
            {
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
            }
            else if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.ExclusiveFullscreen)
            {
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
                DisplayServer.WindowSetSize(new Vector2I(1080, 720));
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.ResizeDisabled, false);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
                DisplayServer.WindowSetPosition(new Vector2I(100,100));
            }
        }

        public void SetAndLoad()
        {
            SetFileInfo();

            LoadUserOptions();

            SetScreenMode();
            SetScreenResolution();
        }

        public void SetScreenMode()
        {
            DisplayServer.WindowMode loadedWindowMode;

            if (currentlySavedUserOptions.displayMode == Enums.DisplayMode.Windowed) loadedWindowMode = DisplayServer.WindowMode.Windowed;
            else if (currentlySavedUserOptions.displayMode == Enums.DisplayMode.Fullscreen) loadedWindowMode = DisplayServer.WindowMode.ExclusiveFullscreen;
            else loadedWindowMode = DisplayServer.WindowMode.ExclusiveFullscreen; //we will also always need a backup, thats why this is else instead of else if.

            if(DisplayServer.WindowGetMode() != loadedWindowMode)
            {
                DisplayServer.WindowSetMode(loadedWindowMode);
            }

            if(loadedWindowMode == DisplayServer.WindowMode.Windowed)
            {
                DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.ResizeDisabled, false);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
                DisplayServer.WindowSetPosition(new Vector2I(100, 100));
            }
        }

        public void SetScreenResolution()
        {
            Vector2I newResolution = new Vector2I(1920, 1080); //default value
            bool passedOverrideResolutionCheck = false;

            if(currentlySavedUserOptions.screenResolution == Enums.ScreenResolution.OVERRIDE_CUSTOM)
            {
                if (currentlySavedUserOptions.overrideScreenResolution != string.Empty)
                {
                    try
                    {
                        string[] splitString = currentlySavedUserOptions.overrideScreenResolution.Split('x');
                        int xValueResolution = int.Parse(splitString[0].ToLower().Replace("res_", "")); //we remove the res in case the user inserted this in the string.
                        int yValueResolution = int.Parse(splitString[1]);

                        newResolution = new Vector2I(xValueResolution, yValueResolution);
                        passedOverrideResolutionCheck = true;
                    }
                    catch
                    {
                        GD.Print("Failed to split the string and set the override value");
                    }
                }
            }
            
            if (!passedOverrideResolutionCheck)
            {
                if(currentlySavedUserOptions.screenResolution == Enums.ScreenResolution.OVERRIDE_CUSTOM)
                {
                    newResolution = new Vector2I(1920,1080);
                }
                else
                {
                    string[] splitString = currentlySavedUserOptions.screenResolution.ToString().Split('x');
                    int xValueResolution = int.Parse(splitString[0].ToLower().Replace("res_", "")); 
                    int yValueResolution = int.Parse(splitString[1]);

                    newResolution = new Vector2I(xValueResolution, yValueResolution);
                }
            }

            //we only change the resolution if its not the same
            if (DisplayServer.WindowGetSize() != newResolution)
            {
                DisplayServer.WindowSetSize(newResolution);
            }
        }

        private void SetFileInfo()
        {
            directoryLocation = "user://Settings//";
            fileName = "GameUserOptions" + ".cfg";
            fullFilePath = directoryLocation + fileName;
        }

        public virtual void LoadUserOptions()
        {
            var config = new ConfigFile();

            // Load data from a file.
            var err = config.Load(fullFilePath);

            // If the file didn't load, ignore it.
            if (err != Error.Ok)
                CreateNewSaveFile(true);

            // Iterate over all sections.
            foreach (String section in config.GetSections())
            {
                if (section == Enums.UserOptionsConfigHeader.AUDIO_SETTINGS.ToString())
                {
                    currentlySavedUserOptions.musicVolume = (int)config.GetValue(section, "MusicVolume");
                    currentlySavedUserOptions.otherVolume = (int)config.GetValue(section, "OtherVolume");
                }
                else if (section == Enums.UserOptionsConfigHeader.GAMEPLAY_SETTINGS.ToString())
                {
                    if (!Enum.TryParse(config.GetValue(section, "ScreenMovement").ToString(), out currentlySavedUserOptions.screenMovement))
                    {
                        currentlySavedUserOptions.screenMovement = Enums.ScreenMovementType.Use_Both;//we failed so we set it to default
                    }

                    currentlySavedUserOptions.addedDragSensitivity = (int)config.GetValue(section, "AddedDragSensitivity");
                    currentlySavedUserOptions.addedSidesSensitivity = (int)config.GetValue(section, "AddedSidesSensitivity");
                }
                else if (section == Enums.UserOptionsConfigHeader.GRAPHICS_SETTINGS.ToString())
                {
                    if(!Enum.TryParse(config.GetValue(section, "DisplayMode").ToString(), out currentlySavedUserOptions.displayMode))
                    {
                        currentlySavedUserOptions.displayMode = Enums.DisplayMode.Fullscreen;//we failed so we set it to default
                    }

                    if (!Enum.TryParse(config.GetValue(section, "ScreenResolution").ToString(), out currentlySavedUserOptions.screenResolution))
                    {
                        currentlySavedUserOptions.screenResolution = Enums.ScreenResolution.RES_1920x1080;//we failed so we set it to default
                    }

                    currentlySavedUserOptions.overrideScreenResolution = config.GetValue(section, "OverrideScreenResolution").ToString();
                    currentlySavedUserOptions.limitFPS = (int)config.GetValue(section, "LimitFPS");
                    currentlySavedUserOptions.fpsLimit = (int)config.GetValue(section, "FpsLimit");
                }
                else if (section == Enums.UserOptionsConfigHeader.ACCESSIBILITY_SETTINGS.ToString())
                {
                    currentlySavedUserOptions.enableHemophobiaMode = (bool)config.GetValue(section, "EnableHemophobiaMode");
                }
                else
                {
                    GD.Print("SECTION NOT FOUND: " + section);
                }
            }

            //set the values to the ones that were loaded
            overriddenUserOptions = (GameUserOptionsConfig)currentlySavedUserOptions.Clone();
        }

        public override void CreateNewSaveFile(bool overrideSaveFile)
        {
            base.CreateNewSaveFile(overrideSaveFile);

            GameUserOptionsConfig userOptionsToSave = overrideSaveFile ? overriddenUserOptions : currentlySavedUserOptions;

            config.SetValue(Enums.UserOptionsConfigHeader.AUDIO_SETTINGS.ToString(), "MusicVolume", userOptionsToSave.musicVolume);
            config.SetValue(Enums.UserOptionsConfigHeader.AUDIO_SETTINGS.ToString(), "OtherVolume", userOptionsToSave.otherVolume);

            config.SetValue(Enums.UserOptionsConfigHeader.GAMEPLAY_SETTINGS.ToString(), "ScreenMovement", userOptionsToSave.screenMovement.ToString());
            config.SetValue(Enums.UserOptionsConfigHeader.GAMEPLAY_SETTINGS.ToString(), "AddedDragSensitivity", userOptionsToSave.addedDragSensitivity);
            config.SetValue(Enums.UserOptionsConfigHeader.GAMEPLAY_SETTINGS.ToString(), "AddedSidesSensitivity", userOptionsToSave.addedSidesSensitivity);

            config.SetValue(Enums.UserOptionsConfigHeader.GRAPHICS_SETTINGS.ToString(), "DisplayMode", userOptionsToSave.displayMode.ToString());
            config.SetValue(Enums.UserOptionsConfigHeader.GRAPHICS_SETTINGS.ToString(), "ScreenResolution", userOptionsToSave.screenResolution.ToString());
            config.SetValue(Enums.UserOptionsConfigHeader.GRAPHICS_SETTINGS.ToString(), "OverrideScreenResolution", userOptionsToSave.overrideScreenResolution);
            config.SetValue(Enums.UserOptionsConfigHeader.GRAPHICS_SETTINGS.ToString(), "LimitFPS", userOptionsToSave.limitFPS);
            config.SetValue(Enums.UserOptionsConfigHeader.GRAPHICS_SETTINGS.ToString(), "FpsLimit", userOptionsToSave.fpsLimit);

            config.SetValue(Enums.UserOptionsConfigHeader.ACCESSIBILITY_SETTINGS.ToString(), "EnableHemophobiaMode", userOptionsToSave.enableHemophobiaMode);

            // Save it to a file.
            config.Save(fullFilePath);

            //we need to make sure we set the values
            if (overrideSaveFile)
            {
                currentlySavedUserOptions = (GameUserOptionsConfig)userOptionsToSave.Clone();
                overriddenUserOptions = (GameUserOptionsConfig)userOptionsToSave.Clone();
            }
            else
            {
                overriddenUserOptions = (GameUserOptionsConfig)userOptionsToSave.Clone();
            }
        }
    }
}
