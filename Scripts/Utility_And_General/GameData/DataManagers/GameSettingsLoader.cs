using Godot;
using System;

namespace Erikduss
{
	public partial class GameSettingsLoader : Node
	{
        public static GameSettingsLoader Instance { get; private set; }

        public static bool buildIsADemo = true;
        public Color focussedControlColor = new Color(0.6f, 0.6f, 0.5f);
        public bool useHighlightFocusMode = false;

        public bool userLanguageSupported = false;
        public bool userHasControllerConnected = false;

        public UnitsSettingsManager unitSettingsManager = new UnitsSettingsManager();
        public GameUserOptionsManager gameUserOptionsManager = new GameUserOptionsManager();

        //Change these to loading in through a file later.
        public bool useAlternativeBloodColor = false;
        public int assassinBleedApplyChance = 35; //this is a percentage (0-100)
        public int enforcerStunApplyChance = 70; //this is a percentage (0-100)
        public int tankBuffApplyChance = 70; //this is a percentage (0-100)

        //Power ups
        public static int progressNeededToUnlockPower = 1000;
        public static int powerUpProgressAmountIdle = 1;
        public static int powerUpProgressAmountAbilityUsed = 200;
        public static int powerUpProgressMultiplierOwnBaseDamage = 5;
        public static int powerUpProgressMultiplierOtherBaseDamage = 3;
        public static int powerUpProgressMultiplierOwnUnitDamage = 1;
        public static float powerUpProgressMultiplierOtherUnitDamage = 0.5f;
        public static float powerUpProgressMultiplierOwnUnitHealing = 0.5f;

        public PackedScene gamepadWarningPanel = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/UI_And_HUD/General/GamePadModeWarning.tscn");
        private Control currentlyInstantiatedWarning;
        public bool hasInstatiatedWarning = false;
        public Control previouslySelectedControlBeforeControllerChange;
        private bool initializedDevices = false;
        private int amountOfDevicesConnectedPreviously = -1;

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

            Input.JoyConnectionChanged += ControllerConnectionChanged;

            InitializeWorld();
        }

        public override void _Notification(int what)
        {
            base._Notification(what);

            //we need to release the mouse if we are tabbing out, otherwise its very annoying for the user.
            if (Input.GetConnectedJoypads().Count == 0)
            {
                return;
            }

            if (what == NotificationApplicationFocusOut)
            {
                Input.MouseMode = Input.MouseModeEnum.Visible;
            }
            else if (what == NotificationApplicationFocusIn)
            {
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }
        }

        async void InitializeWorld()
        {
            if (GameUserOptionsManager.availableLanguageTranslations.Contains(OS.GetLocaleLanguage()))
            {
                GD.Print("User custom language supported");
                userLanguageSupported = true;
            }

            gameUserOptionsManager.SetAndLoad();

            InitializeAllUnits();

            await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
            //playerGlobalSettingsManager.LoadGlobalPlayerSettings();
        }

        public void ControllerConnectionChanged(long device, bool isConnected)
        {
            //gets called when a device is already connected on game launch.

            if(Input.GetConnectedJoypads().Count == amountOfDevicesConnectedPreviously)
            {
                return;
            }

            if (Input.GetConnectedJoypads().Count > 0)
            {
                userHasControllerConnected = true;
                useHighlightFocusMode = true;
                Input.MouseMode = Input.MouseModeEnum.Captured;

                //we dont want to show this during initialize
                if (initializedDevices)
                {
                    if (hasInstatiatedWarning)
                    {
                        previouslySelectedControlBeforeControllerChange.GrabFocus();
                        currentlyInstantiatedWarning.QueueFree();

                        hasInstatiatedWarning = false;
                    }
                    else
                    {
                        if (GetViewport().GuiGetFocusOwner() != null)
                        {
                            previouslySelectedControlBeforeControllerChange = GetViewport().GuiGetFocusOwner();
                        }

                        currentlyInstantiatedWarning = (Control)gamepadWarningPanel.Instantiate();
                        GetViewport().AddChild(currentlyInstantiatedWarning);

                        hasInstatiatedWarning = true;

                        //only pause when the game is in progress.
                        if (GameManager.Instance != null)
                        {
                            if (GameManager.Instance.matchDuration > 0)
                            {
                                GameManager.Instance.ToggleGameIsPaused(true);
                            }
                        }
                    }
                }
            }
            else
            {
                userHasControllerConnected = false;

                //should prob not happen, or at least be prompted to.
                useHighlightFocusMode = false;
                Input.MouseMode = Input.MouseModeEnum.Visible;

                if (initializedDevices)
                {
                    if (hasInstatiatedWarning)
                    {
                        previouslySelectedControlBeforeControllerChange.GrabFocus();
                        currentlyInstantiatedWarning.QueueFree();

                        hasInstatiatedWarning = false;
                    }
                    else
                    {
                        if (GetViewport().GuiGetFocusOwner() != null)
                        {
                            previouslySelectedControlBeforeControllerChange = GetViewport().GuiGetFocusOwner();
                        }

                        currentlyInstantiatedWarning = (Control)gamepadWarningPanel.Instantiate();
                        GetViewport().AddChild(currentlyInstantiatedWarning);

                        hasInstatiatedWarning = true;
                    }
                }
            }

            amountOfDevicesConnectedPreviously = Input.GetConnectedJoypads().Count;
            initializedDevices = true;

            GD.Print("User has " + Input.GetConnectedJoypads().Count + "devices connected");
        }

        public async void InitializeAllUnits()
        {
            #region Age 01
            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
            unitSettingsManager.LoadUnitSettings(Enums.UnitTypes.Warrior, Enums.Ages.AGE_01);

            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
            unitSettingsManager.LoadUnitSettings(Enums.UnitTypes.Assassin, Enums.Ages.AGE_01);

            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
            unitSettingsManager.LoadUnitSettings(Enums.UnitTypes.Ranger, Enums.Ages.AGE_01);

            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
            unitSettingsManager.LoadUnitSettings(Enums.UnitTypes.Battlemage, Enums.Ages.AGE_01);

            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
            unitSettingsManager.LoadUnitSettings(Enums.UnitTypes.Tank, Enums.Ages.AGE_01);

            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
            unitSettingsManager.LoadUnitSettings(Enums.UnitTypes.Mass_Healer, Enums.Ages.AGE_01);

            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
            unitSettingsManager.LoadUnitSettings(Enums.UnitTypes.Enforcer, Enums.Ages.AGE_01);

            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
            unitSettingsManager.LoadUnitSettings(Enums.UnitTypes.Archdruid, Enums.Ages.AGE_01);
            #endregion

            #region Age 02
            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
            unitSettingsManager.LoadUnitSettings(Enums.UnitTypes.Warrior, Enums.Ages.AGE_02);

            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
            unitSettingsManager.LoadUnitSettings(Enums.UnitTypes.Assassin, Enums.Ages.AGE_02);

            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
            unitSettingsManager.LoadUnitSettings(Enums.UnitTypes.Ranger, Enums.Ages.AGE_02);

            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
            unitSettingsManager.LoadUnitSettings(Enums.UnitTypes.Battlemage, Enums.Ages.AGE_02);

            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
            unitSettingsManager.LoadUnitSettings(Enums.UnitTypes.Tank, Enums.Ages.AGE_02);

            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
            unitSettingsManager.LoadUnitSettings(Enums.UnitTypes.Mass_Healer, Enums.Ages.AGE_02);

            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
            unitSettingsManager.LoadUnitSettings(Enums.UnitTypes.Enforcer, Enums.Ages.AGE_02);

            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
            unitSettingsManager.LoadUnitSettings(Enums.UnitTypes.Archdruid, Enums.Ages.AGE_02);
            #endregion
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
            if (Input.IsActionJustPressed("ToggleScreenMode"))
            {
                gameUserOptionsManager.ToggleScreenMode();
            }
        }

        /*public void SetValuesOfPlayerGlobalSettings(PlayerGlobalSettingsConfig config)
        {
            PlayerManager.Instance.playerLocomotion.movementSpeed = config.movementSpeed;
            PlayerManager.Instance.playerLocomotion.jumpVelocity = -config.jumpVelocity;

            PlayerManager.Instance.isLoaded = true;
        }

        public void SetValuesOfGrassWorldGeneratorAsync(GrassWorldGeneratorSettingsConfig config)
        {
            if (config == null) GD.Print("Config is null");
            if (GrassWorldGenerator.Instance == null) GD.Print("Grass World Generator is null");

            GrassWorldGenerator.Instance.grassGroundTilePrefabPath = config.grassGroundTilePrefabPath;
            GrassWorldGenerator.Instance.grassGroundLeftCornerTilePrefabPath = config.grassGroundLeftCornerTilePrefabPath;
            GrassWorldGenerator.Instance.grassGroundRightCornerTilePrefabPath = config.grassGroundRightCornerTilePrefabPath;
            GrassWorldGenerator.Instance.grassGroundDirtTilePrefabPath = config.grassGroundDirtTilePrefabPath;

            GrassWorldGenerator.Instance.groundLeftLineOverridePrefabPath = config.groundLeftLineOverridePrefabPath;
            GrassWorldGenerator.Instance.groundRightLineOverridePrefabPath = config.groundRightLineOverridePrefabPath; ;

            GrassWorldGenerator.Instance.tileSize = config.tileSize;
            GrassWorldGenerator.Instance.tileStartingPosition = config.tileStartingPosition;

            GrassWorldGenerator.Instance.minAmountOfTilesGoingLeft = config.minAmountOfTilesGoingLeft;
            GrassWorldGenerator.Instance.maxAmountOfTimesGoingLeft = config.maxAmountOfTimesGoingLeft;

            GrassWorldGenerator.Instance.minChanceToChangeHeightPerTile = config.minChanceToChangeHeightPerTile;
            GrassWorldGenerator.Instance.maxChanceToChangeHeightPerTile = config.maxChanceToChangeHeightPerTile;

            GrassWorldGenerator.Instance.minChanceToGoDownInsteadOfUp = config.minChanceToGoDownInsteadOfUp;
            GrassWorldGenerator.Instance.maxChanceToGoDownInsteadOfUp = config.maxChanceToGoDownInsteadOfUp;

            GrassWorldGenerator.Instance.maxAmoutOfTilesUp = config.maxAmoutOfTilesUp;
            GrassWorldGenerator.Instance.maxAmountOfTilesDown = config.maxAmountOfTilesDown;

            GrassWorldGenerator.Instance.maxAmountOfWorldTileLength = config.maxAmountOfWorldTileLength;
            GrassWorldGenerator.Instance.minAmountOfWorldTileLength = config.minAmountOfWorldTileLength;

            GrassWorldGenerator.Instance.minAmountOfDirtTilesPerTile = config.minAmountOfDirtTilesPerTile;
            GrassWorldGenerator.Instance.maxAmountOfDirtTilesPerTile = config.maxAmountOfDirtTilesPerTile;


            GrassWorldGenerator.Instance.InitializeAndGenerateWorld();
        }*/
    }
}
