using Godot;
using System;
using System.Collections.Generic;

namespace Erikduss
{
    public partial class AudioManager : Node
    {
        public static AudioManager Instance { get; private set; }

        public AudioStream gameMusicAudioClip = GD.Load<AudioStream>("res://Assets/Audio/Music/BotBGameMusicNew.wav");

        public AudioStream buttonClickAudioClip = GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/Button_Click_Amped-Raw.wav");
        public AudioStream buttonClickedFailedAudioClip = GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/Button_Click_Fail_Amped-Raw.wav");
        public AudioStream buttonHoverAudioClip = GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/Button_Hover-Raw.wav");
        public AudioStream dropdownOpenAudioClip = GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/Dropdown_Open-Raw.wav");
        public AudioStream dropdownSelectionAudioClip = GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/Dropdown_Selection_Clicked-Raw.wav");
        public AudioStream sliderChangedAudioClip = GD.Load<AudioStream>("res://Assets/Audio/SFX/UI/Slider_Changed-Raw.wav");

        private AudioStreamPlayer2D musicAudioPlayer;
        private AudioStreamPlayer2D sfxAudioPlayer;
        private AudioStreamPolyphonic sfxPolyStream;

        private int amountOfDefaultMaxSFXClipsAtOnce = 100;

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
        }

        public void GenerateAudioStreamPlayers(Node2D attachToNode)
        {
            if(musicAudioPlayer == null)
            {
                musicAudioPlayer = new AudioStreamPlayer2D();
                musicAudioPlayer.Bus = "Music";
                musicAudioPlayer.Autoplay = true;
                musicAudioPlayer.Stream = gameMusicAudioClip;
                musicAudioPlayer.MaxDistance = 2000;
                musicAudioPlayer.PanningStrength = 0;

                if (attachToNode != null)
                {
                    attachToNode.AddChild(musicAudioPlayer);
                }
                else
                {
                    GetTree().CurrentScene.AddChild(musicAudioPlayer);
                }
            }

            if(sfxAudioPlayer == null)
            {
                sfxAudioPlayer = new AudioStreamPlayer2D();
                sfxAudioPlayer.Bus = "Other";
                sfxAudioPlayer.Autoplay = false;
                sfxAudioPlayer.MaxDistance = 2000;
                sfxAudioPlayer.MaxPolyphony = amountOfDefaultMaxSFXClipsAtOnce;
                sfxAudioPlayer.PanningStrength = 0;
                sfxAudioPlayer.Stream = new AudioStreamPolyphonic();

                if (attachToNode != null)
                {
                    attachToNode.AddChild(sfxAudioPlayer);
                }
                else
                {
                    GetTree().CurrentScene.AddChild(sfxAudioPlayer);
                }

                //needs to kickstart the polyphonic stream.
                sfxAudioPlayer.Play();
            }
        }

        public void ClearAudioPlayers()
        {
            musicAudioPlayer = null;
            sfxAudioPlayer = null;
        }

        public void PlayMusicAudioClip(AudioStream audioClip)
        {

        }

        public void PlaySFXAudioClip(AudioStream audioClip)
        {
            if (sfxAudioPlayer == null) return;
            if (!sfxAudioPlayer.HasStreamPlayback()) return;

            AudioStreamPlaybackPolyphonic playback = (AudioStreamPlaybackPolyphonic)sfxAudioPlayer.GetStreamPlayback();
            playback.PlayStream(audioClip, bus: sfxAudioPlayer.Bus);
        }
    }
}
