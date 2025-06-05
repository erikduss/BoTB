using Godot;
using System;

namespace Erikduss
{
    public partial class TitleScreenMultiplayerLobbyManager : Node
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
            MultiplayerManager.Instance.InitializeMultiplayer();
        }

        public async void MultiplayerInitializationButtonPressed()
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
                MultiplayerManager.Instance.CreateNewLobby();
            }
            else
            {
                GD.PrintErr("FAILED TO CONNECT TO SERVICES");
            }
        }

        public async void JoinButtonPressed()
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
                MultiplayerManager.Instance.JoinLobby("defaultLobby");
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
