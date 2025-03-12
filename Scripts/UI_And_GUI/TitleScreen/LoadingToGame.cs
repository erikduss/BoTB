using Godot;
using System;

namespace Erikduss
{
	public partial class LoadingToGame : Node
	{
        [Export] public string gameSceneName = "TestScene";
        public bool startedLoading = false;

        //currently loading it in like this to prevent infinite loading
        public PackedScene backupGameScene = GD.Load<PackedScene>("res://Scenes_Prefabs/Scenes/GameScene_Singleplayer.tscn");

        private int currentLoadAttempts = 0;
        private int maxLoadAttempts = 200;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
            //GetTree().ChangeSceneToFile("res://Scenes_Prefabs/Scenes/" + gameSceneName + ".tscn");
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
            if (!startedLoading)
            {
                startedLoading = true;

                LoadGameScene();
            }
            else
            {
                Godot.Collections.Array progress = new Godot.Collections.Array();
                ResourceLoader.LoadThreadedGetStatus("res://Scenes_Prefabs/Scenes/" + gameSceneName + ".tscn", progress);
                GD.Print(progress[0]);

                currentLoadAttempts++;

                if ((int)progress[0] == 1)
                {
                    PackedScene packed_Scene = (PackedScene)ResourceLoader.LoadThreadedGet("res://Scenes_Prefabs/Scenes/" + gameSceneName + ".tscn");
                    GetTree().ChangeSceneToPacked(packed_Scene);
                }
                else if(currentLoadAttempts >= maxLoadAttempts)
                {
                    GD.Print("Force loading into game.");
                    GetTree().ChangeSceneToPacked(backupGameScene);
                    //GetTree().ChangeSceneToFile("res://Scenes_Prefabs/Scenes/" + gameSceneName + ".tscn");
                }
            }
        }

        public void LoadGameScene()
        {
            ResourceLoader.LoadThreadedRequest("res://Scenes_Prefabs/Scenes/" + gameSceneName + ".tscn" ,useSubThreads: true);
        }
    }
}
