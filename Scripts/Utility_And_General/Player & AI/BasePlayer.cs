using Godot;
using System;

namespace Erikduss
{
	public partial class BasePlayer : Node
	{
        public float playerCurrentCurrencyAmount { get; set; }
        public int playerAbilityCurrentCooldown { get; set; }

		public Enums.Ages currentAgeOfPlayer = Enums.Ages.AGE_01;

		public int playerCurrentPowerUpProgressAmount { get; set; }
        public int playerCurrentPowerUpRerollsAmount { get; set; }

        public int playerCurrentAmountOfPowerUpsOwed { get; set; }

		public bool hasUnlockedPowerUpCurrently { get; set; }

        public HomeBaseManager playerBase { get; set; }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}
	}
}
