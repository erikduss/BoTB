using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Erikduss
{
    public partial class TitleScreenMultiplayerLobbyManager : Control
    {
        [Export] public TitleScreenManager titleScreenManager;

        [Export] public Label networkingDebug;

        public int maxConnectionAttemptTime = 10;

        [Export] public Control playerPanelParent;
        public PackedScene playerPanelPrefab;

        [Export] public Control lobbyPanel;
        [Export] public Label lobbyNameLabel;
        [Export] public TextureButton startGameButton;

        List<Control> currentPlayers = new List<Control>();

        public override void _Ready()
        {
            base._Ready();

            playerPanelPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Multiplayer/Lobby/player_lobby_panel.tscn");

            MultiplayerManager.Instance.titlescreenMultiplayerLobby = this;
        }

        public void InitializeMultiplayerServices()
        {
            networkingDebug.Visible = true;
            MultiplayerManager.Instance.InitializeMultiplayer();
        }

        public async void CreateNewLobby(string lobbyName, string lobbyPassword)
        {
            InitializeMultiplayerServices();

            //max 10 seconds timeout
            for (int i = 0; i < maxConnectionAttemptTime; i++)
            {
                await ToSignal(GetTree().CreateTimer(1f), "timeout");

                if (MultiplayerManager.Instance.isCurrentlyConnectedToServices) break;
            }

            if (MultiplayerManager.Instance.isCurrentlyConnectedToServices)
            {
                MultiplayerManager.Instance.CreateNewLobby(lobbyName, lobbyPassword);
            }
            else
            {
                GD.PrintErr("FAILED TO CONNECT TO SERVICES");
            }
        }

        public async void JoinLobby(string lobbyName, string lobbyPassword)
        {
            InitializeMultiplayerServices();

            //max 10 seconds timeout
            for (int i = 0; i < maxConnectionAttemptTime; i++)
            {
                await ToSignal(GetTree().CreateTimer(1f), "timeout");

                if (MultiplayerManager.Instance.isCurrentlyConnectedToServices) break;
            }

            if (MultiplayerManager.Instance.isCurrentlyConnectedToServices)
            {
                MultiplayerManager.Instance.JoinLobby(lobbyName, lobbyPassword);
            }
            else
            {
                GD.PrintErr("FAILED TO CONNECT TO SERVICES");
            }
        }

        public void StartGameButtonPressed()
        {
            GD.Print("There are currently: " + GDSync.GetLobbyPlayerCount() + " players in the lobby");

            GDSync.CreateSyncedEvent("LoadToGame");
        }

        public async void LeaveCurrentLobby()
        {
            GDSync.LeaveLobby();

            lobbyPanel.Visible = false;

            networkingDebug.Text = "Networking Status: ";
            networkingDebug.Visible = false;

            await ToSignal(GetTree().CreateTimer(2.5f), "timeout");

            //reset everything
            MultiplayerManager.Instance.currentPlayerID = 1;

            for (int i = 0; i < currentPlayers.Count; i++)
            {
                if (currentPlayers[i] != null && !currentPlayers[i].IsQueuedForDeletion())
                {
                    currentPlayers[i].CallDeferred("DeleteThisLobbyEntry");
                }
            }

            currentPlayers.Clear();
        }

        private void ClearLastLobby()
        {

        }

        public void OpenLobby(bool isTheHost)
        {
            lobbyNameLabel.Text = GDSync.GetLobbyName();
            lobbyPanel.Visible = true;

            if (!isTheHost) startGameButton.Disabled = true;
        }

        public void ClientJoinedLobby(int clientID, int playerID)
        {
            Control playerPanel = (Control)playerPanelPrefab.Instantiate();

            playerPanelParent.AddChild(playerPanel);
            playerPanel.Name = clientID.ToString();
            GDSync.SetGDSyncOwner(playerPanel, clientID);

            GD.Print("There are: " + GDSync.GetLobbyPlayerCount() + " players in the lobby." + clientID);

            playerPanel.Position = new Vector2(0, ((playerID * 100) - 50 ));

            currentPlayers.Add(playerPanel);
        }

        public void ClientLeftLobby(int clientID)
        {
            //this means the host left, so we abandon the lobby.
            if (!MultiplayerManager.Instance.isHostOfLobby)
            {
                titleScreenManager.LeaveLobbyButtonPressed();
            }
            else
            {
                Control playerPanel = (Control)GetNodeOrNull(clientID.ToString());

                GD.Print("We should remove player: " + clientID);

                if (playerPanel != null)
                {
                    GD.Print("Removed player");
                    playerPanel.QueueFree();
                }
                else
                {
                    Control playerToRemove = currentPlayers.First(a => a.Name == clientID.ToString());

                    if (playerToRemove != null)
                    {
                        playerToRemove.QueueFree();
                    }
                }
            }
        }
    }
}
