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

		public enum ScreenMovementType
		{
			Use_Both,
			Only_Use_Drag_Movement,
			Only_Use_Screen_Sides_Movement
		}

		public enum DisplayMode
		{
			Windowed,
			Fullscreen,
			Fullscreen_Windowed
		}

		public enum ScreenResolution
        {
			RES_1280x720, 
			RES_1920x1080, 
			RES_2560x1440, 
			RES_3840x2160 
		}
	}
}
