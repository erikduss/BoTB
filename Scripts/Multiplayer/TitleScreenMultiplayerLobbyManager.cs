using Godot;
using System;

namespace Erikduss
{
    public partial class TitleScreenMultiplayerLobbyManager : Control
    {
        [Export] public Label networkingDebug;

        public int maxConnectionAttemptTime = 10;

        public override void _Ready()
        {
            base._Ready();

            MultiplayerManager.Instance.networkingDebug = networkingDebug;
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
    }
}
