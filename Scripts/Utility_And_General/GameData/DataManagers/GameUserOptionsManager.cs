﻿using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Erikduss
{
    public partial class GameUserOptionsManager : BaseSettingsManager
    {
        public GameUserOptionsConfig currentlySavedUserOptions = new GameUserOptionsConfig();
        public GameUserOptionsConfig overriddenUserOptions = new GameUserOptionsConfig();

        public static List<string> availableLanguageTranslations = new List<string>() { 
            "en",
            "nl",
            "ru",
            "de",
            "pt",
            "cs",
            "es",
            "fi",
            "fr",
            "ja",
            "ko",
            "lt",
            "pl",
            "tr",
            "zh",
            "it",
            "sv",
            "no"
        };

        public bool muteMusicAudio = false;
        public bool muteOtherAudio = false;

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
            SetAudioBusVolume("Music");
            SetAudioBusVolume("Other");
            SetFPSLimitingMode();
            SetHemophobiaMode();
            SetControllerHighlightMode();
            SetControllerFocusColor();
            SetLanguage();
        }

        public event EventHandler LanguageUpdated;

        public void SetLanguage()
        {
            string localeString = "en";

            switch (currentlySavedUserOptions.language)
            {
                case Enums.AvailableLanguages.English:
                    localeString = "en";
                    break;
                case Enums.AvailableLanguages.Nederlands_Dutch:
                    localeString = "nl";
                    break;
                case Enums.AvailableLanguages.Русский_Russian:
                    localeString = "ru";
                    break;
                case Enums.AvailableLanguages.Deutsch_German:
                    localeString = "de";
                    break;
                case Enums.AvailableLanguages.Português_Portuguese:
                    localeString = "pt";
                    break;
                case Enums.AvailableLanguages.čeština_Czech:
                    localeString = "cs";
                    break;
                case Enums.AvailableLanguages.Española_Spanish:
                    localeString = "es";
                    break;
                case Enums.AvailableLanguages.suomi_Finnish:
                    localeString = "fi";
                    break;
                case Enums.AvailableLanguages.Français_French:
                    localeString = "fr";
                    break;
                case Enums.AvailableLanguages.日本語_Japanese:
                    localeString = "ja";
                    break;
                case Enums.AvailableLanguages.한국인_Korean:
                    localeString = "ko";
                    break;
                case Enums.AvailableLanguages.Lietuvių_Lithuanian:
                    localeString = "lt";
                    break;
                case Enums.AvailableLanguages.Polski_Polish:
                    localeString = "pl";
                    break;
                case Enums.AvailableLanguages.Türkçe_Turkish:
                    localeString = "tr";
                    break;
                case Enums.AvailableLanguages.简体中文_Chinese:
                    localeString = "zh";
                    break;
                case Enums.AvailableLanguages.Italiana_Italian:
                    localeString = "it";
                    break;
                case Enums.AvailableLanguages.Svenska_Swedish:
                    localeString = "sv";
                    break;
                case Enums.AvailableLanguages.norsk_Norwegian:
                    localeString = "no";
                    break;
                default:
                    //if adding a language, make sure to also add the language string to the static list above.
                    GD.PrintErr("Language not implemented in SetLanguage() in GameUserOptionsManager.cs! " + currentlySavedUserOptions.language);
                    break;
            }

            TranslationServer.SetLocale(localeString);

            LanguageUpdated?.Invoke(this, new EventArgs());
        }

        public void SetHemophobiaMode()
        {
            GameSettingsLoader.Instance.useAlternativeBloodColor = currentlySavedUserOptions.enableHemophobiaMode;
        }

        public void SetControllerFocusColor()
        {
            GameSettingsLoader.Instance.focussedControlColor = currentlySavedUserOptions.focussedControlColor;

            if (GetViewport() != null)
            {
                if (GetViewport().GuiGetFocusOwner() != null)
                {
                    GetViewport().GuiGetFocusOwner().SelfModulate = GameSettingsLoader.Instance.focussedControlColor;
                }
            }
        }

        public void SetControllerHighlightMode()
        {
            GameSettingsLoader.Instance.useHighlightFocusMode = currentlySavedUserOptions.useHighlightFocusMode;
            if(GetViewport() != null)
            {
                if(GetViewport().GuiGetFocusOwner() != null)
                {
                    GetViewport().GuiGetFocusOwner().SelfModulate = new Color(1, 1, 1);
                }
            }
        }

        public void SetFPSLimitingMode()
        {
            switch (currentlySavedUserOptions.limitFPS)
            {
                //No limit
                case 0:
                    DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
                    Engine.MaxFps = 0; //0 means max fps
                    break;
                //Limit to number
                case 1:
                    DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
                    Engine.MaxFps = currentlySavedUserOptions.fpsLimit; 
                    break;
                //Limit with Vsync
                case 2:
                    DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
                    Engine.MaxFps = 0; //0 means max fps
                    break;
                //Vsync Adaptive
                case 3:
                    DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Adaptive);
                    Engine.MaxFps = 0; //0 means max fps
                    break;
                //Vsync Mailbox
                case 4:
                    DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Mailbox);
                    Engine.MaxFps = 0; //0 means max fps
                    break;
                //Default is a wrong number is entered in the file.
                default:
                    DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
                    Engine.MaxFps = 0; //0 means max fps
                    break;
            }
        }

        public void SetNewFPSLimit()
        {
            if(currentlySavedUserOptions.limitFPS == 1)
            {
                Engine.MaxFps = currentlySavedUserOptions.fpsLimit;
            }
            else
            {
                Engine.MaxFps = 0;
            }
        }

        public void SetAudioBusVolume(string audioBusName)
        {
            //DB goes from -80 to +24
            //every 6 db difference = audio volume is halved/doubled
            /*
             * -24 -> 0% Volume
             * -12 25%
             * 0 -> 50% Volume
             * 12 -> 75% 
             * 24 -> 100% Volume
             */
            //Every % is equal to 0,48 DB
            //For example: 69% = 9,12 DB ((69 - 50) * 0,48)

            float audioVolumePercentage = audioBusName == "Music" ? currentlySavedUserOptions.musicVolume : currentlySavedUserOptions.otherVolume;

            float percentageConvertedToDB = (audioVolumePercentage - 50f) * 0.48f;

            AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex(audioBusName), percentageConvertedToDB);

            if(audioVolumePercentage <= 0)
            {
                AudioServer.SetBusMute(AudioServer.GetBusIndex(audioBusName), true);

                if (audioBusName == "Music")
                {
                    muteMusicAudio = true;
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.SetMuteMusicAudio(true);
                    }
                }
                else
                {
                    muteOtherAudio = true;
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.SetMuteOtherAudio(true);
                    }
                }
            }
            else if (AudioServer.IsBusMute(AudioServer.GetBusIndex(audioBusName)))
            {
                AudioServer.SetBusMute(AudioServer.GetBusIndex(audioBusName), false);

                if (audioBusName == "Music")
                {
                    muteMusicAudio = false;
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.SetMuteMusicAudio(false);
                    }
                }
                else
                {
                    muteOtherAudio = false;
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.SetMuteOtherAudio(false);
                    }
                }
            }
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

            bool requiresToBeSaved = false;

            // Iterate over all sections.
            foreach (String section in config.GetSections())
            {
                if (section == Enums.UserOptionsConfigHeader.AUDIO_SETTINGS.ToString())
                {
                    if (config.HasSectionKey(section, "MusicVolume"))
                    {
                        currentlySavedUserOptions.musicVolume = (int)config.GetValue(section, "MusicVolume");
                    }
                    else
                    {
                        requiresToBeSaved = true;
                    }
                    
                    if (config.HasSectionKey(section, "OtherVolume"))
                    {
                        currentlySavedUserOptions.otherVolume = (int)config.GetValue(section, "OtherVolume");
                    }
                    else
                    {
                        requiresToBeSaved = true;
                    }
                }
                else if (section == Enums.UserOptionsConfigHeader.GAMEPLAY_SETTINGS.ToString())
                {
                    if (config.HasSectionKey(section, "ScreenMovement"))
                    {
                        if (!Enum.TryParse(config.GetValue(section, "ScreenMovement").ToString(), out currentlySavedUserOptions.screenMovement))
                        {
                            currentlySavedUserOptions.screenMovement = Enums.ScreenMovementType.Use_Both;//we failed so we set it to default
                        }
                    }
                    else
                    {
                        requiresToBeSaved = true;
                    }

                    if (config.HasSectionKey(section, "AddedDragSensitivity"))
                    {
                        currentlySavedUserOptions.addedDragSensitivity = (int)config.GetValue(section, "AddedDragSensitivity");
                    }
                    else
                    {
                        requiresToBeSaved = true;
                    }

                    if (config.HasSectionKey(section, "AddedSidesSensitivity"))
                    {
                        currentlySavedUserOptions.addedSidesSensitivity = (int)config.GetValue(section, "AddedSidesSensitivity");
                    }
                    else
                    {
                        requiresToBeSaved = true;
                    }
                }
                else if (section == Enums.UserOptionsConfigHeader.GRAPHICS_SETTINGS.ToString())
                {
                    if (config.HasSectionKey(section, "DisplayMode"))
                    {
                        if (!Enum.TryParse(config.GetValue(section, "DisplayMode").ToString(), out currentlySavedUserOptions.displayMode))
                        {
                            currentlySavedUserOptions.displayMode = Enums.DisplayMode.Fullscreen;//we failed so we set it to default
                        }
                    }
                    else
                    {
                        requiresToBeSaved = true;
                    }

                    if (config.HasSectionKey(section, "ScreenResolution"))
                    {
                        if (!Enum.TryParse(config.GetValue(section, "ScreenResolution").ToString(), out currentlySavedUserOptions.screenResolution))
                        {
                            currentlySavedUserOptions.screenResolution = Enums.ScreenResolution.RES_1920x1080;//we failed so we set it to default
                        }
                    }
                    else
                    {
                        requiresToBeSaved = true;
                    }
                    

                    if (config.HasSectionKey(section, "OverrideScreenResolution"))
                    {
                        currentlySavedUserOptions.overrideScreenResolution = config.GetValue(section, "OverrideScreenResolution").ToString();
                    }
                    else
                    {
                        requiresToBeSaved = true;
                    }
                    
                    if (config.HasSectionKey(section, "LimitFPS"))
                    {
                        currentlySavedUserOptions.limitFPS = (int)config.GetValue(section, "LimitFPS");
                    }
                    else
                    {
                        requiresToBeSaved = true;
                    }

                    if (config.HasSectionKey(section, "FpsLimit"))
                    {
                        currentlySavedUserOptions.fpsLimit = (int)config.GetValue(section, "FpsLimit");
                    }
                    else
                    {
                        requiresToBeSaved = true;
                    }
                }
                else if (section == Enums.UserOptionsConfigHeader.ACCESSIBILITY_SETTINGS.ToString())
                {
                    if(config.HasSectionKey(section, "UseHighlightFocusMode"))
                    {
                        currentlySavedUserOptions.useHighlightFocusMode = (bool)config.GetValue(section, "UseHighlightFocusMode");
                    }
                    else
                    {
                        requiresToBeSaved = true;
                    }

                    if (config.HasSectionKey(section, "FocussedControlColor"))
                    {
                        currentlySavedUserOptions.focussedControlColor = (Color)config.GetValue(section, "FocussedControlColor");
                    }
                    else
                    {
                        requiresToBeSaved = true;
                    }

                    if (config.HasSectionKey(section, "EnableHemophobiaMode"))
                    {
                        currentlySavedUserOptions.enableHemophobiaMode = (bool)config.GetValue(section, "EnableHemophobiaMode");
                    }
                    else
                    {
                        requiresToBeSaved = true;
                    }

                    if (config.HasSectionKey(section, "Language"))
                    {
                        if (!Enum.TryParse(config.GetValue(section, "Language").ToString(), out currentlySavedUserOptions.language))
                        {
                            currentlySavedUserOptions.language = Enums.AvailableLanguages.English;//we failed so we set it to default
                        } 
                    }
                    else
                    {
                        requiresToBeSaved = true;
                    }
                }
                else
                {
                    GD.Print("SECTION NOT FOUND: " + section);
                }
            }

            if (!requiresToBeSaved)
            {
                //set the values to the ones that were loaded
                OverrideSaveOptions(currentlySavedUserOptions, false);
            }
            else
            {
                CreateNewSaveFile(false);
            }
            //overriddenUserOptions = (GameUserOptionsConfig)currentlySavedUserOptions.Clone();
        }

        public override void CreateNewSaveFile(bool overrideSaveFile)
        {
            base.CreateNewSaveFile(overrideSaveFile);

            GD.Print("We are creating a new options file override:" + overriddenUserOptions);

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

            config.SetValue(Enums.UserOptionsConfigHeader.ACCESSIBILITY_SETTINGS.ToString(), "UseHighlightFocusMode", userOptionsToSave.useHighlightFocusMode);
            config.SetValue(Enums.UserOptionsConfigHeader.ACCESSIBILITY_SETTINGS.ToString(), "FocussedControlColor", userOptionsToSave.focussedControlColor);
            config.SetValue(Enums.UserOptionsConfigHeader.ACCESSIBILITY_SETTINGS.ToString(), "EnableHemophobiaMode", userOptionsToSave.enableHemophobiaMode);
            config.SetValue(Enums.UserOptionsConfigHeader.ACCESSIBILITY_SETTINGS.ToString(), "Language", userOptionsToSave.language.ToString());

            // Save it to a file.
            config.Save(fullFilePath);

            //we need to make sure we set the values
            if (overrideSaveFile)
            {
                //GameUserOptionsConfig overrideWithThis = (GameUserOptionsConfig)userOptionsToSave.Clone();

                //currentlySavedUserOptions = (GameUserOptionsConfig)overrideWithThis.Clone();
                //overriddenUserOptions = (GameUserOptionsConfig)overrideWithThis.Clone();

                //Using the clone interface causes the game to crash if changed during the game. To prevent crashing, we use the ugly method.
                OverrideSaveOptions(userOptionsToSave);
                OverrideSaveOptions(userOptionsToSave, false);


                SetAndLoad();
            }
            else
            {
                OverrideSaveOptions(userOptionsToSave, false);
                //overriddenUserOptions = (GameUserOptionsConfig)userOptionsToSave.Clone();
            }
        }

        private void OverrideSaveOptions(GameUserOptionsConfig newOptions, bool overrideCurrentSaved = true)
        {
            if (overrideCurrentSaved)
            {
                currentlySavedUserOptions.musicVolume = newOptions.musicVolume;
                currentlySavedUserOptions.otherVolume = newOptions.otherVolume;

                currentlySavedUserOptions.screenMovement = newOptions.screenMovement;
                currentlySavedUserOptions.addedDragSensitivity = newOptions.addedDragSensitivity;
                currentlySavedUserOptions.addedSidesSensitivity = newOptions.addedSidesSensitivity;

                currentlySavedUserOptions.displayMode = newOptions.displayMode;
                currentlySavedUserOptions.screenResolution = newOptions.screenResolution;
                currentlySavedUserOptions.overrideScreenResolution = newOptions.overrideScreenResolution;
                currentlySavedUserOptions.limitFPS = newOptions.limitFPS;
                currentlySavedUserOptions.fpsLimit = newOptions.fpsLimit;

                currentlySavedUserOptions.useHighlightFocusMode = newOptions.useHighlightFocusMode;
                currentlySavedUserOptions.focussedControlColor = newOptions.focussedControlColor;
                currentlySavedUserOptions.enableHemophobiaMode = newOptions.enableHemophobiaMode;
                currentlySavedUserOptions.language = newOptions.language;
            }
            else
            {
                overriddenUserOptions.musicVolume = newOptions.musicVolume;
                overriddenUserOptions.otherVolume = newOptions.otherVolume;

                overriddenUserOptions.screenMovement = newOptions.screenMovement;
                overriddenUserOptions.addedDragSensitivity = newOptions.addedDragSensitivity;
                overriddenUserOptions.addedSidesSensitivity = newOptions.addedSidesSensitivity;

                overriddenUserOptions.displayMode = newOptions.displayMode;
                overriddenUserOptions.screenResolution = newOptions.screenResolution;
                overriddenUserOptions.overrideScreenResolution = newOptions.overrideScreenResolution;
                overriddenUserOptions.limitFPS = newOptions.limitFPS;
                overriddenUserOptions.fpsLimit = newOptions.fpsLimit;

                overriddenUserOptions.useHighlightFocusMode = newOptions.useHighlightFocusMode;
                overriddenUserOptions.focussedControlColor = newOptions.focussedControlColor;
                overriddenUserOptions.enableHemophobiaMode = newOptions.enableHemophobiaMode;
                overriddenUserOptions.language = newOptions.language;
            }
        }

        public void ResetOverrideOptionsConfig()
        {
            overriddenUserOptions.musicVolume = currentlySavedUserOptions.musicVolume;
            overriddenUserOptions.otherVolume = currentlySavedUserOptions.otherVolume;

            overriddenUserOptions.screenMovement = currentlySavedUserOptions.screenMovement;
            overriddenUserOptions.addedDragSensitivity = currentlySavedUserOptions.addedDragSensitivity;
            overriddenUserOptions.addedSidesSensitivity = currentlySavedUserOptions.addedSidesSensitivity;

            overriddenUserOptions.displayMode = currentlySavedUserOptions.displayMode;
            overriddenUserOptions.screenResolution = currentlySavedUserOptions.screenResolution;
            overriddenUserOptions.overrideScreenResolution = currentlySavedUserOptions.overrideScreenResolution;
            overriddenUserOptions.limitFPS = currentlySavedUserOptions.limitFPS;
            overriddenUserOptions.fpsLimit = currentlySavedUserOptions.fpsLimit;

            overriddenUserOptions.useHighlightFocusMode = currentlySavedUserOptions.useHighlightFocusMode;
            overriddenUserOptions.focussedControlColor = currentlySavedUserOptions.focussedControlColor;
            overriddenUserOptions.enableHemophobiaMode = currentlySavedUserOptions.enableHemophobiaMode;
            overriddenUserOptions.language = currentlySavedUserOptions.language;
        }
    }
}
