using Godot;
using System;

namespace Erikduss
{
	public partial class LoadingToGame : Node
	{
        [Export] public string gameSceneName = "TestScene";
        public bool startedLoading = false;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
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

                if ((int)progress[0] == 1)
                {
                    PackedScene packed_Scene = (PackedScene)ResourceLoader.LoadThreadedGet("res://Scenes_Prefabs/Scenes/" + gameSceneName + ".tscn");
                    GetTree().ChangeSceneToPacked(packed_Scene);
                }
            }
        }

        public void LoadGameScene()
        {
            ResourceLoader.LoadThreadedRequest("res://Scenes_Prefabs/Scenes/" + gameSceneName + ".tscn");
        }
    }
}
