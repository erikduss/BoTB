using Godot;
using System;
using System.Collections.Generic;

namespace Erikduss
{
    public partial class MultiplayerManager : Control
    {
        public static MultiplayerManager Instance;

        public bool isCurrentlyConnectedToServices = false;

        public bool isHostOfLobby = false;
        public bool playersAreInGame = false;
        public bool isUsingMultiplayer = false;
        public TitleScreenMultiplayerLobbyManager titlescreenMultiplayerLobby;

        public int currentPlayerID = 1;

        public List<int> playersInLobby = new List<int>();

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

            GDSync.ClientJoined += Client_Joined;
            GDSync.ClientLeft += Client_Left;
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

            GDSync.ClientJoined -= Client_Joined;
            GDSync.ClientLeft -= Client_Left;

            base.Dispose(disposing);
        }

        public void InitializeMultiplayer()
        {
            titlescreenMultiplayerLobby.networkingDebug.Text = titlescreenMultiplayerLobby.networkingDebug.Text + "\n" + "Attemptiong To Connect To Multiplayer Services....";
            GDSync.StartMultiplayer();
        }

        public void CreateNewLobby(string lobbyName = "defaultLobby", string lobbyPassword = "")
        {
            GDSync.CreateLobby(lobbyName, lobbyPassword, true, 2);

            isHostOfLobby = true;

            JoinLobby(lobbyName, lobbyPassword);
        }

        public void JoinLobby(string lobbyName, string lobbyPassword = "")
        {
            GDSync.JoinLobby(lobbyName, lobbyPassword);
        }

        public void Connected()
        {
            GD.Print("Connected");

            titlescreenMultiplayerLobby.networkingDebug.Text = titlescreenMultiplayerLobby.networkingDebug.Text + "\n" + "Connected To Multiplayer Services.";

            isCurrentlyConnectedToServices = true;
        }

        public void Disconnected()
        {
            GD.Print("Disconnected");

            if (!playersAreInGame)
            {
                titlescreenMultiplayerLobby.networkingDebug.Text = titlescreenMultiplayerLobby.networkingDebug.Text + "\n" + "Disconnected from Multiplayer Services.";
            }

            isCurrentlyConnectedToServices = false;
        }

        public void Connection_Failed(int error)
        {

            isHostOfLobby = false;

            switch (error)
            {
                case (int)GDSync.CONNECTION_FAILED.INVALID_PUBLIC_KEY:
                    GD.PrintErr("INVALID PUBLIC KEY ENTERED");
                    titlescreenMultiplayerLobby.networkingDebug.Text = titlescreenMultiplayerLobby.networkingDebug.Text + "\n" + "Invalid Public Key Error";
                    break;
                case (int)GDSync.CONNECTION_FAILED.TIMEOUT:
                    GD.PrintErr("CONNECTION TIMEOUT");
                    titlescreenMultiplayerLobby.networkingDebug.Text = titlescreenMultiplayerLobby.networkingDebug.Text + "\n" + "Connection Timeout Error";
                    break;
                default:
                    titlescreenMultiplayerLobby.networkingDebug.Text = titlescreenMultiplayerLobby.networkingDebug.Text + "\n" + "Other Connection Error";
                    break;
            }
        }

        public void Lobby_Created(string lobbyName)
        {
            GD.Print("Created lobby: " + lobbyName);
            titlescreenMultiplayerLobby.networkingDebug.Text = titlescreenMultiplayerLobby.networkingDebug.Text + "\n" + "Created Lobby";
        }

        public void Lobby_Creation_Failed(string lobbyName, int error)
        {
            GD.Print("Failed to create lobby: " + lobbyName);

            titlescreenMultiplayerLobby.networkingDebug.Text = titlescreenMultiplayerLobby.networkingDebug.Text + "\n" + "Failed to create Lobby";

            if (error == (int)GDSync.LOBBY_CREATION_ERROR.LOBBY_ALREADY_EXISTS)
            {
                JoinLobby(lobbyName);
            }
            else
            {
                titlescreenMultiplayerLobby.titleScreenManager.LeaveLobbyButtonPressed();
            }
        }

        public void Lobby_Joined(string lobbyName)
        {
            GD.Print("Joined lobby: " + lobbyName);

            titlescreenMultiplayerLobby.networkingDebug.Text = titlescreenMultiplayerLobby.networkingDebug.Text + "\n" + "Joined Lobby";

            titlescreenMultiplayerLobby.OpenLobby(isHostOfLobby);
        }

        public void Lobby_Join_Failed(string lobbyName, int error)
        {
            GD.Print("Failed to join lobby: " + lobbyName);

            switch (error)
            {
                case (int)GDSync.LOBBY_JOIN_ERROR.DUPLICATE_USERNAME:
                        titlescreenMultiplayerLobby.networkingDebug.Text = titlescreenMultiplayerLobby.networkingDebug.Text + "\n" + "Failed to join lobby: Duplicate Username";
                    break;
                case (int)GDSync.LOBBY_JOIN_ERROR.INCORRECT_PASSWORD:
                        titlescreenMultiplayerLobby.networkingDebug.Text = titlescreenMultiplayerLobby.networkingDebug.Text + "\n" + "Failed to join lobby: Incorrect Password";
                    break;
                case (int)GDSync.LOBBY_JOIN_ERROR.LOBBY_DOES_NOT_EXIST:
                        titlescreenMultiplayerLobby.networkingDebug.Text = titlescreenMultiplayerLobby.networkingDebug.Text + "\n" + "Failed to join lobby: Lobby does not exist";
                    break;
                case (int)GDSync.LOBBY_JOIN_ERROR.LOBBY_IS_CLOSED:
                        titlescreenMultiplayerLobby.networkingDebug.Text = titlescreenMultiplayerLobby.networkingDebug.Text + "\n" + "Failed to join lobby: Lobby is closed";
                    break;
                case (int)GDSync.LOBBY_JOIN_ERROR.LOBBY_IS_FULL:
                        titlescreenMultiplayerLobby.networkingDebug.Text = titlescreenMultiplayerLobby.networkingDebug.Text + "\n" + "Failed to join lobby: Lobby is full";
                    break;
                default:
                    titlescreenMultiplayerLobby.networkingDebug.Text = titlescreenMultiplayerLobby.networkingDebug.Text + "\n" + "Failed to join lobby: Unknown error";
                    break;
            }

            isHostOfLobby = false;

            //reset ui
            titlescreenMultiplayerLobby.titleScreenManager.LeaveLobbyButtonPressed();
        }

        public void Synced_Event_Triggered(string eventName, Godot.Collections.Array parameters)
        {
            if(eventName == "LoadToGame")
            {
                GD.Print("Loading to Game through Multiplayer");

                isUsingMultiplayer = true; //this only happens when its actually a multiplayer match.

                AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickAudioClip);
                AudioManager.Instance.ClearAudioPlayers();

                GetTree().ChangeSceneToFile("res://Scenes_Prefabs/Scenes/LoadingToGame.tscn");
            }
        }

        public void Client_Joined(int clientID)
        {
            GD.Print("Client Joined: " + clientID);

            titlescreenMultiplayerLobby.networkingDebug.Text = titlescreenMultiplayerLobby.networkingDebug.Text + "\n" + "Player " + clientID + " joined the lobby.";

            titlescreenMultiplayerLobby.ClientJoinedLobby(clientID, currentPlayerID);
            currentPlayerID++;

            playersInLobby.Add(clientID);
        }

        public void Client_Left(int clientID)
        {
            GD.Print("Client Left: " + clientID);

            if (!playersAreInGame)
            {
                titlescreenMultiplayerLobby.networkingDebug.Text = titlescreenMultiplayerLobby.networkingDebug.Text + "\n" + "Player " + clientID + " left the lobby.";

                titlescreenMultiplayerLobby.ClientLeftLobby(clientID);
            }
            
            playersInLobby.Remove(clientID);

            currentPlayerID--;
        }
    }
}
