using Godot;
using System;
using System.Linq;
using static Erikduss.Enums;

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

        public PackedScene aiPlayerNode = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Spawnable_Objects/enemy_ai.tscn");
        public PackedScene playerSceneNodePrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Spawnable_Objects/Player_Scene.tscn");

        [Export] public CameraMovement cameraScript;
        public Enums.TeamOwner clientTeamOwner = Enums.TeamOwner.NONE; //this is set to eithe team 1 or team 2, depending on multiplayer host priority.

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
		private float currencyGainAmount = 1f;

        public int currencyGainPercentagePlayer01 = 0;
        public int currencyGainPercentagePlayer02 = 0;
        #endregion

        #region Ability Variables

        public int playerAbilityCooldown = 180; //seconds 180

        private float playerAbilityUpdateTimer = 0;
        private float playerAbilityCooldownReductionRate = 1f; //every second we reduce it by 1
		private int playerAbilityCooldownReduction = 1; //reduceing it by 1
        #endregion

        public double matchDuration = 0;

        public bool isMultiplayerMatch = false;

        public bool isHostOfMultiplayerMatch = false;

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

            if (MultiplayerManager.Instance != null)
            {
                if (MultiplayerManager.Instance.isUsingMultiplayer)
                {
                    isMultiplayerMatch = true;
                }
            }

            gameIsPaused = false;

            if (isMultiplayerMatch)
            {
                //add multiplayer event connections
                GDSync.SyncedEventTriggered += Synced_Event_Triggered;

                GDSync.ExposeFunction(new Callable(this, "SpendNonHostClientCurrency"));
                GDSync.ExposeFunction(new Callable(this, "ProcessSpawnRequestPlayer2")); 
                GDSync.ExposeFunction(new Callable(this, "ProcessExecuteAbilityRequestPlayer2"));
                GDSync.ExposeFunction(new Callable(inGameHUDManager, "RefreshPowerUp"));
                GDSync.ExposeFunction(new Callable(this, "AwardPlayer2WithPowerupBuff"));
                GDSync.ExposeFunction(new Callable(this, "SpendPlayer2PowerupRefresh"));
                GDSync.ExposeFunction(new Callable(this, "Player2LockedPowerupChanged"));
                GDSync.ExposeFunction(new Callable(this, "Player2UnlockedNewPowerup"));


                if (GDSync.IsHost() && MultiplayerManager.Instance.isHostOfLobby)
                {
                    clientTeamOwner = Enums.TeamOwner.TEAM_01;

                    isHostOfMultiplayerMatch = true;

                    int clientID = GDSync.GetClientId();

                    GD.Print("I am the host!" + clientID);

                    Node instantiatedPlayer = GDSync.MultiplayerInstantiate(playerSceneNodePrefab, this);
                    //AddChild(instantiatedPlayer);
                    instantiatedPlayer.Name = clientID.ToString();

                    player01Script = (BasePlayer)instantiatedPlayer;
                    //GDSync.SetGDSyncOwner(instantiatedPlayer, clientID);

                    //spawn other player
                    int otherClientID = MultiplayerManager.Instance.playersInLobby.Where(a => a != GDSync.GetClientId()).First();
                    Node otherInstantiatedPlayer = GDSync.MultiplayerInstantiate(playerSceneNodePrefab, this);
                    //AddChild(otherInstantiatedPlayer);
                    otherInstantiatedPlayer.Name = otherClientID.ToString();

                    player02Script = (BasePlayer)otherInstantiatedPlayer;
                    //GDSync.SetGDSyncOwner(otherInstantiatedPlayer, otherClientID);
                }
                else
                {
                    clientTeamOwner = Enums.TeamOwner.TEAM_02;

                    //we dont need to do anything else here. Spawning is done through the host.
                }
            }
            else
            {
                //singleplayer
                clientTeamOwner = Enums.TeamOwner.TEAM_01;

                player01Script = new BasePlayer();

                Node instantiatedAI = aiPlayerNode.Instantiate();

                AddChild(instantiatedAI);

                player02Script = (BaseAIPlayer)instantiatedAI; //we need to probably instantiate this later with custom ai nodes.
            }

            if(!isMultiplayerMatch || isHostOfMultiplayerMatch)
            {
                player01Script.playerBase = team01HomeBase;
                player02Script.playerBase = team02HomeBase;

                player01Script.playerCurrentCurrencyAmount = startingCurrency;
                player01Script.playerAbilityCurrentCooldown = playerAbilityCooldown;
                player01Script.playerCurrentPowerUpProgressAmount = 0;
                player01Script.playerCurrentPowerUpRerollsAmount = 0;
                player01Script.playerCurrentAmountOfPowerUpsOwed = 0;
                player01Script.hasUnlockedPowerUpCurrently = false;
                player01Script.playerTeam = TeamOwner.TEAM_01;

                player02Script.playerCurrentCurrencyAmount = startingCurrency;
                player02Script.playerAbilityCurrentCooldown = playerAbilityCooldown;
                player02Script.playerCurrentPowerUpProgressAmount = 0;
                player02Script.playerCurrentPowerUpRerollsAmount = 0;
                player02Script.playerCurrentAmountOfPowerUpsOwed = 0;
                player02Script.hasUnlockedPowerUpCurrently = false;
                player02Script.playerTeam = TeamOwner.TEAM_02;

                GD.Print("P1 has: " + player01Script.playerAbilityCurrentCooldown);
                GD.Print("P2 has: " + player02Script.playerAbilityCurrentCooldown);

                if (isMultiplayerMatch)
                {
                    player01Script.SyncCurrency();
                    player02Script.SyncCurrency();
                }
            }

            if (isMultiplayerMatch)
            {
                if (isHostOfMultiplayerMatch)
                {
                    //this will call it for all clients
                    GDSync.SyncedEventCreate("SyncUpdatePlayerHud");
                }
            }
            else
            {
                //in singleplayer we are always the first player
                inGameHUDManager.UpdatePlayerCurrencyAmountLabel(player01Script.playerCurrentCurrencyAmount);
                inGameHUDManager.UpdatePlayerPowerUPRerollAmount(player01Script);
            }

            AudioManager.Instance.CallDeferred("GenerateAudioStreamPlayers", (Node2D)cameraScript);

            EffectsAndProjectilesSpawner.Instance.ExposeMultiplayerFunctions();

            //The ability bar isnt passed yet at this time
            //inGameHUDManager.UpdatePlayerAbilityCooldownBar(playerAbilityCurrentCooldown);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if(isMultiplayerMatch)
            {
                GDSync.SyncedEventTriggered -= Synced_Event_Triggered;
            }

			//We need to set the instance to null to reset the game variables.
			Instance = null;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
		{
			if (Input.IsActionJustPressed("Pause"))
			{
                if (isMultiplayerMatch)
                {
                    //dont think we need to be the host to do this.
                    GDSync.SyncedEventCreate("PauseGameToggle");
                }
                else
                {
                    ToggleGameIsPaused();
                }
            }

            if (gameIsPaused || gameIsFinished) return;

            matchDuration += delta;

            //if this is a multiplayer game and we are not the host, we dont execute any code below this.
            if (isMultiplayerMatch && !isHostOfMultiplayerMatch)
            {
                //we have access to our updated values on the non host client, so we can get our own powerup as non host but we still need a process function for this.

                bool giveNewPowerUp = false;

                //not set yet
                if (GetLocalClientPlayerScript() == null) return;

                //this is synced through the host so we always have an updated value.
                if(GetLocalClientPlayerScript().playerCurrentPowerUpProgressAmount >= GameSettingsLoader.progressNeededToUnlockPower)
                {
                    giveNewPowerUp  = true;
                }
                else if (!GetLocalClientPlayerScript().hasUnlockedPowerUpCurrently && GetLocalClientPlayerScript().playerCurrentAmountOfPowerUpsOwed > 0)
                {
                    giveNewPowerUp  = true;
                }

                if (giveNewPowerUp)
                {
                    inGameHUDManager.RefreshPowerUp(false);
                }

                return;
            }

            bool updatePlayerHudMultiplayer = false;

			//Timer for giving the player currency
			if(currencyGainAmountUpdateTimer > currencyGainRate)
			{
				currencyGainAmountUpdateTimer = 0;

                player01Script.playerCurrentCurrencyAmount += (currencyGainAmount * (currencyGainPercentagePlayer01 > 0 ? 1f + (currencyGainPercentagePlayer01 / 100f) : 1f ));
                player02Script.playerCurrentCurrencyAmount += (currencyGainAmount * (currencyGainPercentagePlayer02 > 0 ? 1f + (currencyGainPercentagePlayer02 / 100f) : 1f ));

                //Update HUD
                if (isMultiplayerMatch)
                {
                    player01Script.SyncCurrency();
                    player02Script.SyncCurrency();

                    //We do the call at the end of the update loop to make sure it's not called multiple times to optimize internet data usage.
                    updatePlayerHudMultiplayer = true; 
                }
                else
                {
                    inGameHUDManager.UpdatePlayerCurrencyAmountLabel(player01Script.playerCurrentCurrencyAmount);
                }

                UpdatePlayerPowerUpProgress(Enums.TeamOwner.TEAM_01, GameSettingsLoader.powerUpProgressAmountIdle);
                UpdatePlayerPowerUpProgress(Enums.TeamOwner.TEAM_02, GameSettingsLoader.powerUpProgressAmountIdle);
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

                if (isMultiplayerMatch)
                {
                    //We do the call at the end of the update loop to make sure it's not called multiple times to optimize internet data usage.
                    updatePlayerHudMultiplayer = true;
                }
                else
                {
                    //Update HUD
                    inGameHUDManager.UpdatePlayerAbilityCooldownBar(player01Script.playerAbilityCurrentCooldown);
                    //update empowred label
                    inGameHUDManager.UpdatePlayerAbilityEmpowerAmount(Enums.TeamOwner.TEAM_01);
                }
            }
            else
            {
                playerAbilityUpdateTimer += (float)delta;
            }

            if(isMultiplayerMatch && updatePlayerHudMultiplayer)
            {
                //this will call it for all clients
                GDSync.SyncedEventCreate("SyncUpdatePlayerHud");
            }
        }

		public void ToggleGameIsPaused(bool forceSetPaused = false)
		{
            if (!forceSetPaused)
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
            else if(!gameIsPaused)
            {
                if (isMultiplayerMatch)
                {
                    //dont think we need to be the host to do this.
                    GDSync.SyncedEventCreate("PauseGameToggle");
                }
                else
                {
                    gameIsPaused = true;
                    inGameHUDManager.ShowPauseMenu();
                }
            }
        }

		//bool to inducate succes state of removing the currency.
		public bool SpendPlayerCurrency(int amount, Enums.TeamOwner playerTeam)
		{
			BasePlayer playerToSpendCurrencyFor = (playerTeam == Enums.TeamOwner.TEAM_01 ? player01Script : player02Script);

			if (playerToSpendCurrencyFor.playerCurrentCurrencyAmount < amount) return false;

            if (playerTeam == Enums.TeamOwner.TEAM_02) GD.Print("Player 2 Has: " + playerToSpendCurrencyFor.playerCurrentCurrencyAmount + " After spending: " + amount);

            playerToSpendCurrencyFor.playerCurrentCurrencyAmount -= amount;

            //update HUD
            if (isMultiplayerMatch)
            {
                if (!isHostOfMultiplayerMatch)
                {
                    //call the function on the host to spend our currency.
                    GDSync.CallFuncOn(GDSync.GetHost(), new Callable(this, "SpendNonHostClientCurrency"), [amount]);
                    inGameHUDManager.UpdatePlayerCurrencyAmountLabel(player02Script.playerCurrentCurrencyAmount);
                }
                else
                {
                    inGameHUDManager.UpdatePlayerCurrencyAmountLabel(player01Script.playerCurrentCurrencyAmount);
                }
            }
            else
            {
                if (playerTeam == Enums.TeamOwner.TEAM_01)
                {
                    inGameHUDManager.UpdatePlayerCurrencyAmountLabel(player01Script.playerCurrentCurrencyAmount);
                }
            }

			return true;
        }

        public void SpendNonHostClientCurrency(int amount)
        {
            player02Script.playerCurrentCurrencyAmount -= amount;

            //this will call it for all clients
            GDSync.SyncedEventCreate("SyncUpdatePlayerHud");
        }

        public void ProcessSpawnRequestPlayer2(int unitType)
        {
            GD.Print("received buy request");
            unitsSpawner.ProcessBuyingUnit(TeamOwner.TEAM_02, (Enums.UnitTypes)unitType);
        }

		public void ResetPlayerAbilityCooldown(Enums.TeamOwner playerTeam)
		{
            BasePlayer playerToResetCooldownFor = (playerTeam == Enums.TeamOwner.TEAM_01 ? player01Script : player02Script);

            playerToResetCooldownFor.playerAbilityCurrentCooldown = playerAbilityCooldown;

            if (isMultiplayerMatch)
            {
                //this will call it for all clients
                GDSync.SyncedEventCreate("SyncUpdatePlayerHud");
            }
            else if (playerTeam == Enums.TeamOwner.TEAM_01)
            {
                inGameHUDManager.UpdatePlayerAbilityCooldownBar(player01Script.playerAbilityCurrentCooldown);
            }
        }

        public void ProcessExecuteAbilityRequestPlayer2(bool randomBool)
        {
            GD.Print("received ability execution request");

            EffectsAndProjectilesSpawner.Instance.SpawnMeteorsAgeAbilityProjectiles(TeamOwner.TEAM_02);

            ResetPlayerAbilityCooldown(TeamOwner.TEAM_02);
        }

        public void EndCurrentGame(bool forceEndGame = false)
        {
            if (!gameIsFinished)
            {
                if(team01HomeBase.CurrentHealth <=0 || team02HomeBase.CurrentHealth <= 0)
                {
                    gameIsFinished = true;

                    HomeBaseManager playerHomeBase = clientTeamOwner == Enums.TeamOwner.TEAM_01 ? team01HomeBase : team02HomeBase;

                    //We should show "Victory" or "Defeat" depending on the outcome. (Ingame hud manager)
                    string outcomeValue = playerHomeBase.CurrentHealth <= 0 ? Tr("DEFEAT") : Tr("VICTORY");

                    inGameHUDManager.GameOverTriggered(outcomeValue);

                    //Afterwards, we show some stats of the game, currency earned and a button prompting to return (to main menu).

                }
                else
                {
                    //Did the player leave?
                    if (forceEndGame)
                    {
                        //We should show "Victory" or "Defeat" depending on the outcome. (Ingame hud manager)
                        string outcomeValue = Tr("DISCONNECT");

                        inGameHUDManager.GameOverTriggered(outcomeValue);
                    }
                }
            }
        }

        public void UpdatePlayerPowerUpProgress(Enums.TeamOwner playerTeam, int addedPowerUpProgress)
        {
            BasePlayer playerToChangePowerUpProgressFor = (playerTeam == Enums.TeamOwner.TEAM_01 ? player01Script : player02Script);

            int newPowerUpProgress = playerToChangePowerUpProgressFor.playerCurrentPowerUpProgressAmount + addedPowerUpProgress;

            //check if we go over the max amount.
            //If we do, reduce it by max amount and leftover powerup progress is saved.
            if(newPowerUpProgress > GameSettingsLoader.progressNeededToUnlockPower)
            {
                newPowerUpProgress -= GameSettingsLoader.progressNeededToUnlockPower;
                playerToChangePowerUpProgressFor.playerCurrentAmountOfPowerUpsOwed++;
                playerToChangePowerUpProgressFor.playerCurrentPowerUpRerollsAmount++;

                if (isMultiplayerMatch)
                {
                    //always the host, and we are the host when we are here.
                    if(playerTeam == TeamOwner.TEAM_01)
                    {
                        inGameHUDManager.RefreshPowerUp(false);
                    }
                    //else
                    //{
                    //    playerToChangePowerUpProgressFor.hasUnlockedPowerUpCurrently = true;
                    //    int otherClient = MultiplayerManager.Instance.playersInLobby.Where(a => a != GDSync.GetClientId()).First();
                    //    GDSync.CallFuncOn(otherClient, new Callable(GameManager.Instance.inGameHUDManager, "RefreshPowerUp"), [false]);
                    //}
                }
                else if (playerTeam == Enums.TeamOwner.TEAM_01) //singleplayer
                {
                    inGameHUDManager.RefreshPowerUp(false);
                    inGameHUDManager.UpdatePlayerPowerUPRerollAmount(player01Script);
                }
            }

            playerToChangePowerUpProgressFor.playerCurrentPowerUpProgressAmount = newPowerUpProgress;

            if (isMultiplayerMatch && isHostOfMultiplayerMatch)
            {
                //we want to prevent double updates.
                if(addedPowerUpProgress != GameSettingsLoader.powerUpProgressAmountIdle)
                {
                    //this also refreshes the power up shop if needed. (locked powerup completed)
                    GDSync.SyncedEventCreate("SyncUpdatePlayerHud");
                }
            }
            else if (playerTeam == Enums.TeamOwner.TEAM_01)
            {
                inGameHUDManager.UpdateCurrentLockedPowerUpProgress();
            }
        }

        public void SpendPlayer2PowerupRefresh()
        {
            GD.Print("Reduce player 2 powerup owed count by 1");
            player02Script.playerCurrentPowerUpRerollsAmount -= 1;
            GDSync.SyncedEventCreate("SyncUpdatePlayerHud");
        }

        public void Player2LockedPowerupChanged(bool playerHasPowerupUnlocked)
        {
            player02Script.hasUnlockedPowerUpCurrently = playerHasPowerupUnlocked;
        }

        public void Player2UnlockedNewPowerup(bool spendReroll)
        {
            //we only remove an owed powerup if its refreshed through any other means than refreshing.
            if (!spendReroll)
            {
                GD.Print("Reduce player 2 powerup owed count by 1");
                player02Script.playerCurrentAmountOfPowerUpsOwed -= 1;
                GDSync.SyncedEventCreate("SyncUpdatePlayerHud");
            }
        }

        public void AwardPlayer2WithPowerupBuff(PowerupType powerupType)
        {
            if(player02Script.playerCurrentAmountOfPowerUpsOwed < 0 || !player02Script.hasUnlockedPowerUpCurrently)
            {
                //player spammed button, prevent purchase and reset to 0.
                player02Script.playerCurrentAmountOfPowerUpsOwed = 0;
                player02Script.hasUnlockedPowerUpCurrently = false;

                return;
            }

            switch (powerupType)
            {
                case PowerupType.GoldGain:
                        currencyGainPercentagePlayer02 += GameSettingsLoader.powerUpGoldGainExtraAmount;
                    break;
                case PowerupType.HealBase:
                        team02HomeBase.HealDamage(GameSettingsLoader.powerUpBaseHealAmount);
                    break;
                case PowerupType.AbilityEmpower:
                        EffectsAndProjectilesSpawner.Instance.team02AbilityEmpowerAmount += GameSettingsLoader.powerUpAbilityEmpowerAmount;
                    break;
                default:
                    break;
            }

            ////this needs to get updated by the host.
            //GD.Print("Reduce player 2 powerup owed count by 1");
            //player02Script.playerCurrentAmountOfPowerUpsOwed -= 1;

            GDSync.SyncedEventCreate("SyncUpdatePlayerHud");

            if (player02Script.playerCurrentAmountOfPowerUpsOwed <= 0)
            {
                GD.Print("Only 1 powerup owed which was this one that we bought.");
                
                player02Script.hasUnlockedPowerUpCurrently = false;
            }
        }

        #region Multiplayer events

        public void Synced_Event_Triggered(string eventName, Godot.Collections.Array parameters)
        {
            switch (eventName)
            {
                case "SyncUpdatePlayerHud":
                    //Update Currency

                    inGameHUDManager.UpdatePlayerCurrencyAmountLabel(GetLocalClientPlayerScript().playerCurrentCurrencyAmount);
                    inGameHUDManager.UpdatePlayerPowerUPRerollAmount(GetLocalClientPlayerScript());

                    //Update Ability Info
                    inGameHUDManager.UpdatePlayerAbilityCooldownBar(GetLocalClientPlayerScript().playerAbilityCurrentCooldown);
                    //update empowred label
                    inGameHUDManager.UpdatePlayerAbilityEmpowerAmount(clientTeamOwner);

                    //Update powerups
                    inGameHUDManager.UpdatePlayerPowerUPRerollAmount(GetLocalClientPlayerScript());
                    inGameHUDManager.UpdateCurrentLockedPowerUpProgress();

                    break;
                case "PauseGameToggle":
                    GD.Print("Networking: Pausing Game");

                    ToggleGameIsPaused();
                    break;
                case "UpdateAbilityInfo":

                    
                    break;
                case "UpdatePowerupInfo":
                    

                    break;
            }
        }

        #endregion

        public BasePlayer GetLocalClientPlayerScript()
        {
            BasePlayer player = GameManager.Instance.clientTeamOwner == Enums.TeamOwner.TEAM_01 ? GameManager.Instance.player01Script : GameManager.Instance.player02Script;

            return player;
        }
    }
}
