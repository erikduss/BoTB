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
	}
}
