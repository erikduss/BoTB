using Godot;
using System;

namespace Erikduss
{
	public partial class TitleScreenManager : Control
	{
        [Export] private Control optionsPanel;
        [Export] private Label currentVersionLabel;

        public PackedScene mobileAdsPrefab;

        [Export] public string gameLoadingSceneName = "LoadingToGame";

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
            AudioManager.Instance.CallDeferred("GenerateAudioStreamPlayers");

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

            string versionLabelText = string.Empty;

            if (GameSettingsLoader.buildIsADemo) versionLabelText = "This is a Demo build, some features may be limited. ";

            versionLabelText = versionLabelText + "Version: " +ProjectSettings.GetSetting("application/config/version").ToString();

            currentVersionLabel.Text = versionLabelText;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

        public void StartGame()
        {
            AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickAudioClip);
            GetTree().ChangeSceneToFile("res://Scenes_Prefabs/Scenes/" + gameLoadingSceneName + ".tscn");
        }

        public void OpenOptions()
        {
            AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickAudioClip);
            optionsPanel.Visible = true;
        }

        public void CloseGame()
        {
            AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickAudioClip);
            GetTree().Quit();
        }

        public void OpenAdsTestingScene()
        {
            GetTree().ChangeSceneToFile("res://addons/admob/sample/Main.tscn");
        }
    }
}
