using Godot;
using System;

namespace Erikduss
{
	public static class Enums
	{
		public enum TeamOwner
		{
			TEAM_01,
			TEAM_02,
			NONE
		}

		public enum Ages
		{
			AGE_01,
			AGE_02
		}

		public enum UnitTypes
		{
			Warrior,
			Ranger,
			Assassin,
			Enforcer,
			Tank,
			Mass_Healer,
			Battlemage,
			Archdruid,
			Shaman,
			TrainingDummy
		}

		public enum UnitSettingsConfigHeader
		{
			CONFIG_SETTINGS,
			UNIT_INFO,
			UNIT_STATS,
			UNIT_GAMEPLAY_VARIABLES
		}

        public enum UserOptionsConfigHeader
        {
            AUDIO_SETTINGS,
            GAMEPLAY_SETTINGS,
            GRAPHICS_SETTINGS,
            ACCESSIBILITY_SETTINGS
        }

        public enum ScreenMovementType
		{
			Use_Both,
			Only_Use_Drag_Movement,
			Only_Use_Screen_Sides_Movement
		}

		public enum DisplayMode
		{
			Windowed,
			Fullscreen
		}

		public enum ScreenResolution
        {
			RES_1280x720, 
			RES_1920x1080, 
			RES_2560x1440, 
			RES_3840x2160,
			OVERRIDE_CUSTOM
		}

		public enum AvailableLanguages
		{
            čeština_Czech,
            Deutsch_German,
            English,
            Española_Spanish,
            suomi_Finnish,
            Français_French,
            Italiana_Italian,
            日本語_Japanese,
            한국인_Korean,
            Lietuvių_Lithuanian,
            norsk_Norwegian,
            Nederlands_Dutch,
            Polski_Polish,
            Português_Portuguese,
            Русский_Russian,
            Svenska_Swedish,
            Türkçe_Turkish,
            简体中文_Chinese
        }
	}
}
