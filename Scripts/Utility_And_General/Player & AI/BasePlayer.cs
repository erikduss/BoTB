using Godot;
using System;
using System.Linq;

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

        public Enums.TeamOwner playerTeam = Enums.TeamOwner.NONE;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
			if (GameManager.Instance.isMultiplayerMatch)
			{
                GDSync.ExposeNode(this);

				if (!GameManager.Instance.isHostOfMultiplayerMatch)
				{
                    GD.Print("Syncing up: " + this.Name);

                    //link up to non host client

                    if (GameManager.Instance.player01Script == null)
                    {
                        GameManager.Instance.player01Script = this;
                        playerTeam = Enums.TeamOwner.TEAM_01;
                        Name = (MultiplayerManager.Instance.playersInLobby.Where(a => a != GDSync.GetClientID()).First()).ToString();
                    }
                    else
                    {
                        GameManager.Instance.player02Script = this;
                        playerTeam = Enums.TeamOwner.TEAM_02;
                        Name = GDSync.GetClientID().ToString();
                    }

                    //if (this.Name == GDSync.GetClientID().ToString())
                    //{
                    //	GameManager.Instance.player02Script = this;
                    //}
                    //else
                    //{
                    //	GameManager.Instance.player01Script = this;
                    //}
                }
            }
		}

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
		{
		}

        public void SyncCurrency()
        {
            GDSync.SyncVar(this, "playerCurrentCurrencyAmount");

            GDSync.SyncVar(this, "playerAbilityCurrentCooldown");

            GDSync.SyncVar(this, "playerCurrentPowerUpProgressAmount");

            GDSync.SyncVar(this, "playerCurrentPowerUpRerollsAmount");

            GDSync.SyncVar(this, "playerCurrentAmountOfPowerUpsOwed");

            GDSync.SyncVar(this, "hasUnlockedPowerUpCurrently");
        }
	}
}
