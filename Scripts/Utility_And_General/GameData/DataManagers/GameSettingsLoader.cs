using Godot;
using System;

namespace Erikduss
{
	public partial class GameSettingsLoader : Node
	{
        public static GameSettingsLoader Instance { get; private set; }

        public UnitsSettingsManager unitSettingsManager = new UnitsSettingsManager();
        //public GrassWorldGeneratorSettingsManager grassWorldGeneratorSettingsManager = new GrassWorldGeneratorSettingsManager();

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

            //grassWorldGeneratorSettingsManager._Ready();

            InitializeWorld();
        }

        async void InitializeWorld()
        {
            InitializeAllUnits();

            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
            //playerGlobalSettingsManager.LoadGlobalPlayerSettings();
        }

        private async void InitializeAllUnits()
        {
            #region Age 01
            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
            unitSettingsManager.LoadUnitSettings(Enums.UnitTypes.Warrior, Enums.Ages.AGE_01);

            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
            unitSettingsManager.LoadUnitSettings(Enums.UnitTypes.Asssassin, Enums.Ages.AGE_01);

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
            #endregion

            #region Age 02
            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
            unitSettingsManager.LoadUnitSettings(Enums.UnitTypes.Warrior, Enums.Ages.AGE_02);

            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
            unitSettingsManager.LoadUnitSettings(Enums.UnitTypes.Asssassin, Enums.Ages.AGE_02);

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
            #endregion
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
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
