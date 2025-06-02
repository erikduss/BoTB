using Godot;
using System;
using System.Collections.Generic;

namespace Erikduss
{
	public partial class TitleScreenManager : Control
	{
        [Export] private Control optionsPanel;
        [Export] private Label currentVersionLabel;

        public PackedScene mobileAdsPrefab;

        [Export] public string gameLoadingSceneName = "LoadingToGame";

        public bool handleInput = true;

        [Export] public Control defaultControlSelected;
        private Control currentlySelectedControl = null;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
            AudioManager.Instance.CallDeferred("GenerateAudioStreamPlayers", GetTree().CurrentScene);

            
            //we need to load our ads
            if (OS.GetName() == "Android" || OS.GetName() == "iOS")
            {
                handleInput = false; //we dont need to handle the input on mobile.

                mobileAdsPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/UI_And_HUD/Titlescreen/MobileAdsPrefab.tscn");

                Control instantiatedAdsComponent = (Control)mobileAdsPrefab.Instantiate();

                AddChild(instantiatedAdsComponent);
            }
            else
            {
                GD.Print("we did not load the ads.");
            }

            string versionLabelText = string.Empty;

            //if (GameSettingsLoader.buildIsADemo) versionLabelText = Tr("DEMO_TEXT");

            // "This is a Demo build, some features may be limited. Version: "

            versionLabelText = ProjectSettings.GetSetting("application/config/version").ToString();

            currentVersionLabel.Text = GameSettingsLoader.buildIsADemo ? Tr("DEMO_TEXT") + versionLabelText : versionLabelText;

            SubscribeToEvents();

            defaultControlSelected.GrabFocus();
		}

        public void UpdateLanguage(object o, EventArgs e)
        {
            string versionLabelText = string.Empty;

            versionLabelText = ProjectSettings.GetSetting("application/config/version").ToString();

            currentVersionLabel.Text = GameSettingsLoader.buildIsADemo ? Tr("DEMO_TEXT") + versionLabelText : versionLabelText;
        }

        public void SubscribeToEvents()
        {
            GetViewport().GuiFocusChanged += OnControlElementFocusChanged;
            optionsPanel.VisibilityChanged += OptionsPanelClosed;
            GameSettingsLoader.Instance.gameUserOptionsManager.LanguageUpdated += UpdateLanguage;
        }

        public void UnsubscribeFromEvents()
        {
            GetViewport().GuiFocusChanged -= OnControlElementFocusChanged;
            optionsPanel.VisibilityChanged -= OptionsPanelClosed;
            GameSettingsLoader.Instance.gameUserOptionsManager.LanguageUpdated -= UpdateLanguage;
        }

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
            base._Process(delta);
        }

        public void StartGame()
        {
            AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickAudioClip);
            AudioManager.Instance.ClearAudioPlayers();

            UnsubscribeFromEvents();

            GetTree().ChangeSceneToFile("res://Scenes_Prefabs/Scenes/" + gameLoadingSceneName + ".tscn");
        }

        public void OpenOptions()
        {
            AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickAudioClip);
            optionsPanel.Visible = true;
            ((OptionsMenu)optionsPanel).allowSFXFromOptionsMenu = true;
            ((OptionsMenu)optionsPanel).SelectDefaultControl();
        }

        public void OptionsPanelClosed()
        {
            if (!optionsPanel.Visible)
            {
                defaultControlSelected.GrabFocus();
            }
        }

        public void CloseGame()
        {
            UnsubscribeFromEvents();

            AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickAudioClip);
            GetTree().Quit();
        }

        public void PlayGenericButtonHoverSound()
        {
            AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonHoverAudioClip);
        }

        public void OpenAdsTestingScene()
        {
            GetTree().ChangeSceneToFile("res://addons/admob/sample/Main.tscn");
        }

        private void OnControlElementFocusChanged(Control control)
        {
            if (GameSettingsLoader.Instance.useHighlightFocusMode)
            {
                if (control != currentlySelectedControl)
                {
                    //change color back
                    if (currentlySelectedControl != null)
                    {
                        currentlySelectedControl.SelfModulate = new Color(1, 1, 1);
                        AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonHoverAudioClip);
                    }
                }

                control.SelfModulate = GameSettingsLoader.Instance.focussedControlColor;
            }

            currentlySelectedControl = control;
        }
    }
}
