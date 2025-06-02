using Godot;
using System;

public partial class TestLoadSplashScreen : Node
{
    public void LoadLoadingScreen()
    {
        GetTree().ChangeSceneToFile("res://Scenes_Prefabs/Scenes/TitleScreen.tscn");
    }
}
