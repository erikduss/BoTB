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

        [Export] private Control mainTitleScreenUI;
        [Export] private Control matchPrepScreenUI;

        [Export] private Control createLobbyPanel;
        [Export] private Control joinLobbyPanel;

        //Joining And creating lobby variables
        [Export] private LineEdit lobbyCreateNameLineEdit;
        [Export] private LineEdit lobbyCreatePasswordLineEdit;

        [Export] private LineEdit lobbyJoinNameLineEdit;
        [Export] private LineEdit lobbyJoinPasswordLineEdit;

        //multiplayer
        [Export] public TitleScreenMultiplayerLobbyManager lobbyMultiplayerManager;

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
            if (GetViewport() != null)
            {
                GetViewport().GuiFocusChanged -= OnControlElementFocusChanged;
            }

            if( optionsPanel != null)
            {
                optionsPanel.VisibilityChanged -= OptionsPanelClosed;
            }
            
            if(GameSettingsLoader.Instance != null)
            {
                GameSettingsLoader.Instance.gameUserOptionsManager.LanguageUpdated -= UpdateLanguage;
            }
        }

        protected override void Dispose(bool disposing)
        {
            //UnsubscribeFromEvents();

            base.Dispose(disposing);
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

        public void ShowMatchPrepUI()
        {
            mainTitleScreenUI.Visible = false;
            matchPrepScreenUI.Visible = true;
        }

        public void HideMatchPrepUI()
        {
            mainTitleScreenUI.Visible = true;
            matchPrepScreenUI.Visible = false;
        }

        public void ShowLobbyCreateUI()
        {
            mainTitleScreenUI.Visible = false;
            matchPrepScreenUI.Visible = false;

            createLobbyPanel.Visible = true;
        }

        public void HideLobbyCreateUI()
        {
            mainTitleScreenUI.Visible = false;
            matchPrepScreenUI.Visible = true;

            createLobbyPanel.Visible = false;

            lobbyCreateNameLineEdit.Text = string.Empty;
            lobbyCreatePasswordLineEdit.Text = string.Empty;
        }

        public void ShowLobbyJoinUI()
        {
            mainTitleScreenUI.Visible = false;
            matchPrepScreenUI.Visible = false;

            joinLobbyPanel.Visible = true;
        }

        public void HideLobbyJoinUI()
        {
            mainTitleScreenUI.Visible = false;
            matchPrepScreenUI.Visible = true;

            joinLobbyPanel.Visible = false;

            lobbyJoinNameLineEdit.Text = string.Empty;
            lobbyJoinPasswordLineEdit.Text = string.Empty;
        }

        public void AttemptToCreateLobbyButtonPressed()
        {
            bool success = DidWeSucceedCreatingALobby();

            string lobbyName = lobbyCreateNameLineEdit.Text;
            string lobbyPassword = lobbyCreatePasswordLineEdit.Text;

            createLobbyPanel.Visible = false;

            lobbyMultiplayerManager.CreateNewLobby(lobbyName, lobbyPassword);

            //we join with a lobby joined event from the server.
        }

        private bool DidWeSucceedCreatingALobby()
        {
            if (lobbyCreateNameLineEdit.Text == string.Empty) return false;

            return true;
        }

        public void AttemptToJoinLobbyButtonPressed()
        {
            bool success = DidWeSucceedJoiningALobby();

            string lobbyName = lobbyJoinNameLineEdit.Text;
            string lobbyPassword = lobbyJoinPasswordLineEdit.Text;

            joinLobbyPanel.Visible = false;

            lobbyMultiplayerManager.JoinLobby(lobbyName, lobbyPassword);

            //we join with a lobby joined event from the server.
        }

        public void LeaveLobbyButtonPressed()
        {
            lobbyMultiplayerManager.LeaveCurrentLobby();

            ShowMatchPrepUI();
        }

        private bool DidWeSucceedJoiningALobby()
        {
            if (lobbyJoinNameLineEdit.Text == string.Empty) return false;

            return true;
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
