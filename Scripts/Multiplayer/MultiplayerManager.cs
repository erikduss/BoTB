using Godot;
using System;

namespace Erikduss
{
    public partial class MultiplayerManager : Control
    {
        public static MultiplayerManager Instance;

        public bool isCurrentlyConnectedToServices = false;

        public Label networkingDebug;

        public override void _Ready()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                QueueFree();
            }

            base._Ready();

            GDSync.Connected += Connected;
            GDSync.ConnectionFailed += Connection_Failed;

            GDSync.LobbyCreated += Lobby_Created;
            GDSync.LobbyCreationFailed += Lobby_Creation_Failed;

            GDSync.LobbyJoined += Lobby_Joined;
            GDSync.LobbyJoinFailed += Lobby_Join_Failed;

            GDSync.Disconnected += Disconnected;

            GDSync.SyncedEventTriggered += Synced_Event_Triggered;
        }

        protected override void Dispose(bool disposing)
        {
            GDSync.Connected -= Connected;
            GDSync.ConnectionFailed -= Connection_Failed;

            GDSync.LobbyCreated -= Lobby_Created;
            GDSync.LobbyCreationFailed -= Lobby_Creation_Failed;

            GDSync.LobbyJoined -= Lobby_Joined;
            GDSync.LobbyJoinFailed -= Lobby_Join_Failed;

            GDSync.Disconnected -= Disconnected;

            GDSync.SyncedEventTriggered -= Synced_Event_Triggered;

            base.Dispose(disposing);
        }

        public void InitializeMultiplayer()
        {
            GDSync.StartMultiplayer();
        }

        public void JoinButtonPressed()
        {
            JoinLobby("defaultLobby");
        }

        public void CreateNewLobby(string lobbyName = "defaultLobby")
        {
            GDSync.CreateLobby(lobbyName, "", true, 2);

            JoinLobby(lobbyName);
        }

        public void JoinLobby(string lobbyName)
        {
            GDSync.JoinLobby(lobbyName);
        }

        public void Connected()
        {
            GD.Print("Connected");

            networkingDebug.Text = networkingDebug.Text + "\n" + "Connected To Multiplayer Services.";

            isCurrentlyConnectedToServices = true;
        }

        public void Disconnected()
        {
            GD.Print("Disconnected");

            networkingDebug.Text = networkingDebug.Text + "\n" + "Disconnected from Multiplayer Services.";

            isCurrentlyConnectedToServices = false;
        }

        public void Connection_Failed(int error)
        {

            switch (error)
            {
                case (int)GDSync.CONNECTION_FAILED.INVALID_PUBLIC_KEY:
                    GD.PrintErr("INVALID PUBLIC KEY ENTERED");
                    networkingDebug.Text = networkingDebug.Text + "\n" + "Invalid Public Key Error";
                    break;
                case (int)GDSync.CONNECTION_FAILED.TIMEOUT:
                    GD.PrintErr("CONNECTION TIMEOUT");
                    networkingDebug.Text = networkingDebug.Text + "\n" + "Connection Timeout Error";
                    break;
                default:
                    networkingDebug.Text = networkingDebug.Text + "\n" + "Other Connection Error";
                    break;
            }
        }

        public void Lobby_Created(string lobbyName)
        {
            GD.Print("Created lobby: " + lobbyName);
            networkingDebug.Text = networkingDebug.Text + "\n" + "Created Lobby";
        }

        public void Lobby_Creation_Failed(string lobbyName, int error)
        {
            GD.Print("Failed to create lobby: " + lobbyName);

            networkingDebug.Text = networkingDebug.Text + "\n" + "Failed to create Lobby";

            if (error == (int)GDSync.LOBBY_CREATION_ERROR.LOBBY_ALREADY_EXISTS)
            {
                JoinLobby(lobbyName);
            }
        }

        public void Lobby_Joined(string lobbyName)
        {
            GD.Print("Joined lobby: " + lobbyName);

            networkingDebug.Text = networkingDebug.Text + "\n" + "Joined Lobby";
        }

        public void Lobby_Join_Failed(string lobbyName, int error)
        {
            GD.Print("Failed to join lobby: " + lobbyName);

            networkingDebug.Text = networkingDebug.Text + "\n" + "Failed to join lobby";
        }

        public void Synced_Event_Triggered(string eventName, Godot.Collections.Array parameters)
        {
            if(eventName == "LoadToGame")
            {
                GD.Print("Loading to Game through Multiplayer");

                GetTree().ChangeSceneToFile("res://Scenes_Prefabs/Scenes/LoadingToGame.tscn");
            }
        }
    }
}
