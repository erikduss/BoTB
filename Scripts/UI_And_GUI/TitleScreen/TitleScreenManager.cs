using Godot;
using System;

namespace Erikduss
{
	public partial class TitleScreenManager : Control
	{
        public PackedScene optionsPanelPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/UI_And_HUD/General/OptionsMenu.tscn");

        public PackedScene mobileAdsPrefab;

        [Export] public string gameLoadingSceneName = "LoadingToGame";

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
            //we need to load our ads
            if (OS.GetName() == "Android" || OS.GetName() == "iOS")
            {
                mobileAdsPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/UI_And_HUD/Titlescreen/MobileAdsPrefab.tscn");

                Control instantiatedAdsComponent = (Control)mobileAdsPrefab.Instantiate();

                AddChild(instantiatedAdsComponent);
            }
            else
            {
                GD.Print("we did not load the ads.");
            }
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

        public void StartGame()
        {
            GD.Print("Buttin clicked");
            GetTree().ChangeSceneToFile("res://Scenes_Prefabs/Scenes/" + gameLoadingSceneName + ".tscn");
        }

        public void OpenOptions()
        {
            GD.Print("Options clicked");
            Control instantiatedOptionsPanel = (Control)optionsPanelPrefab.Instantiate();

            AddChild(instantiatedOptionsPanel);
        }

        public void CloseGame()
        {
            GetTree().Quit();
        }

        public void OpenAdsTestingScene()
        {
            GetTree().ChangeSceneToFile("res://addons/admob/sample/Main.tscn");
        }
    }
}
