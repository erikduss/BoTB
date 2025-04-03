using Godot;
using System;

namespace Erikduss
{
	public partial class GameManager : Node
	{
		public static GameManager Instance { get; private set; }
		[Export] public In_Game_HUD_Manager inGameHUDManager;
		[Export] public UnitsSpawner unitsSpawner;

		[Export] public HomeBaseManager team01HomeBase;
		[Export] public HomeBaseManager team02HomeBase;

		public BasePlayer player01Script; //this is always a real player.
		public BasePlayer player02Script; //this can be either a bot or a real player. (in the future)

        [Export] public CameraMovement cameraScript;

        //TODO, Move these settings to a general settings file and script.
		public bool gameIsPaused { get; private set; }
        public bool gameIsFinished { get; private set; }

        public static float unitGroundYPosition = 815f;

        #region Other Unit Settings
        public static float unitStoppingDistance = 42;
		public static int massHealerHealAmount = 7;
        #endregion

        #region Shop Variables

        public static int defaultShopRefreshCost = 5;
        public static int defaultAmountOfUnitsInShop = 3;

        #endregion

        #region Currency Variables

        private static int startingCurrency = 100;
		private float currencyGainAmountUpdateTimer = 0;
		private float currencyGainRate = 1f; //every 1 second the player gets currency
		private int currencyGainAmount = 1;
        #endregion

        #region Ability Variables
        
		public int playerAbilityCooldown = 180; //seconds

        private float playerAbilityUpdateTimer = 0;
        private float playerAbilityCooldownReductionRate = 1f; //every second we reduce it by 1
		private int playerAbilityCooldownReduction = 1; //reduceing it by 1
        #endregion

        public double matchDuration = 0;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                QueueFree();
            }

            gameIsPaused = false;

            player01Script = new BasePlayer();
            player02Script = (BaseAIPlayer)GetParent().GetNode("EnemyAI"); ; //we need to probably instantiate this later with custom ai nodes.

            player01Script.playerBase = team01HomeBase;
            player02Script.playerBase = team02HomeBase;

            player01Script.playerCurrentCurrencyAmount = startingCurrency;
            player01Script.playerAbilityCurrentCooldown = playerAbilityCooldown;

            player02Script.playerCurrentCurrencyAmount = startingCurrency;
            player02Script.playerAbilityCurrentCooldown = playerAbilityCooldown;

            //in this case in singleplayer we are always player 01, in the future in multiplayer this needs to be a network event call.
            inGameHUDManager.UpdatePlayerCurrencyAmountLabel(player01Script.playerCurrentCurrencyAmount);

            AudioManager.Instance.CallDeferred("GenerateAudioStreamPlayers", (Node2D)cameraScript);

            //The ability bar isnt passed yet at this time
            //inGameHUDManager.UpdatePlayerAbilityCooldownBar(playerAbilityCurrentCooldown);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

			//We need to set the instance to null to reset the game variables.
			Instance = null;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
		{
			if (Input.IsActionJustPressed("Pause"))
			{
				ToggleGameIsPaused();
			}

            if (gameIsPaused || gameIsFinished) return;

            matchDuration += delta;

			//Timer for giving the player currency
			if(currencyGainAmountUpdateTimer > currencyGainRate)
			{
				currencyGainAmountUpdateTimer = 0;
                player01Script.playerCurrentCurrencyAmount += currencyGainAmount;
                player02Script.playerCurrentCurrencyAmount += currencyGainAmount;

                //Update HUD
                inGameHUDManager.UpdatePlayerCurrencyAmountLabel(player01Script.playerCurrentCurrencyAmount);
            }
			else
			{
				currencyGainAmountUpdateTimer += (float)delta;
			}

            //Timer for updating ability counter
            if (playerAbilityUpdateTimer > playerAbilityCooldownReductionRate)
            {
                playerAbilityUpdateTimer = 0;
                player01Script.playerAbilityCurrentCooldown -= playerAbilityCooldownReduction;
                player02Script.playerAbilityCurrentCooldown -= playerAbilityCooldownReduction;

                //Update HUD
                inGameHUDManager.UpdatePlayerAbilityCooldownBar(player01Script.playerAbilityCurrentCooldown);
				//update empowred label
				inGameHUDManager.UpdatePlayerAbilityEmpowerAmount(Enums.TeamOwner.TEAM_01);
            }
            else
            {
                playerAbilityUpdateTimer += (float)delta;
            }
        }

		public void ToggleGameIsPaused()
		{
            gameIsPaused = !gameIsPaused;
            GD.Print("Game is paused: " + gameIsPaused);

			if (gameIsPaused)
			{
                //Open pause panel
                inGameHUDManager.ShowPauseMenu();
            }
            else
			{
				//close pause panel
				inGameHUDManager.HidePauseMenu();
			}
        }

		//bool to inducate succes state of removing the currency.
		public bool SpendPlayerCurrency(int amount, Enums.TeamOwner playerTeam)
		{
			BasePlayer playerToSpendCurrencyFor = (playerTeam == Enums.TeamOwner.TEAM_01 ? player01Script : player02Script);

			if (playerToSpendCurrencyFor.playerCurrentCurrencyAmount < amount) return false;

            if (playerTeam == Enums.TeamOwner.TEAM_02) GD.Print("The AI Has: " + playerToSpendCurrencyFor.playerCurrentCurrencyAmount + " After spending: " + amount);

            playerToSpendCurrencyFor.playerCurrentCurrencyAmount -= amount;
			
            //update HUD
            if(playerTeam == Enums.TeamOwner.TEAM_01)
            {
                inGameHUDManager.UpdatePlayerCurrencyAmountLabel(player01Script.playerCurrentCurrencyAmount);
            }

			return true;
        }

		public void ResetPlayerAbilityCooldown(Enums.TeamOwner playerTeam)
		{
            BasePlayer playerToResetCooldownFor = (playerTeam == Enums.TeamOwner.TEAM_01 ? player01Script : player02Script);

            playerToResetCooldownFor.playerAbilityCurrentCooldown = playerAbilityCooldown;

            if (playerTeam == Enums.TeamOwner.TEAM_01)
            {
                inGameHUDManager.UpdatePlayerAbilityCooldownBar(player01Script.playerAbilityCurrentCooldown);
            }
        }

        public void EndCurrentGame()
        {
            if (!gameIsFinished)
            {
                if(team01HomeBase.CurrentHealth <=0 || team02HomeBase.CurrentHealth <= 0)
                {
                    gameIsFinished = true;

                    //We should show "Victory" or "Defeat" depending on the outcome. (Ingame hud manager)
                    string outcomeValue = team01HomeBase.CurrentHealth <= 0 ? "Defeat" : "Victory";

                    inGameHUDManager.GameOverTriggered(outcomeValue);

                    //Afterwards, we show some stats of the game, currency earned and a button prompting to return (to main menu).

                }
                else
                {
                    //Did the player leave?
                }
            }
        }
	}
}
