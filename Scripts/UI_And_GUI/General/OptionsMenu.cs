using Godot;
using System;

namespace Erikduss
{
	public partial class OptionsMenu : Control
	{
		//we need to prompt the user if they are sure they want to go back if we have changes.
		private bool hasChangedSettings = false;


		//What do we need to do?
		/*
		 * We need to load the settings from a save file.
		 * We need to save these values in a temporary file here so we can compare the current working file to the saved one.
		 * We need to be able to save the current working file to the save file.
		 * 
		 * Probably will need to change the music volume live or play a small jingle
		 * ^ Same for other audio volumes
		 * 
		 * Resolution and full screen stuff only needs to be available on pc.
		 * 
		 * Do we need anything else?
		 */


		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		public void ReturnButtonPressed()
		{
			if (!hasChangedSettings)
			{
				QueueFree();
			}
		}

		public void SaveButtonPressed()
		{

		}
	}
}
