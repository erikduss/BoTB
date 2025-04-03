using Godot;
using System;

namespace Erikduss
{
    public partial class GameOverInfoScript : Control
    {
        [Export] public Label outcomeLabel;

        [Export] public Label matchDurationLabel;
        [Export] public Label currencyRewardLabel;


        public void ReturnButtonClicked()
        {
            AudioManager.Instance.PlaySFXAudioClip(AudioManager.Instance.buttonClickAudioClip);
            AudioManager.Instance.ClearAudioPlayers();
            //We return back to the main menu.\
            GameManager.Instance.QueueFree();
            GetTree().ChangeSceneToFile("res://Scenes_Prefabs/Scenes/TitleScreen.tscn");
        }
    }
}
