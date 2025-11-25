using Erikduss;
using Godot;
using System;

public partial class GDScriptConnector : Node
{
    public string playerSteamUserName;
    public int playerSteamUserID;

    public void SetSteamInfo(string steamUserName, int steamUserID)
    {
        playerSteamUserName = steamUserName;
        playerSteamUserID = steamUserID;
    }

    public void SteamOverlayToggled(bool active)
    {
        if (active)
        {
            if (GameManager.Instance != null)
            {
                if (!GameManager.Instance.gameIsPaused)
                {
                    GameManager.Instance.ToggleGameIsPaused(true);
                    GD.Print("Steam toggled from C#!");
                }
            }
        }
    }
}
