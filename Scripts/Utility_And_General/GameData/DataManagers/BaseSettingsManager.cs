using Godot;
using System;

namespace Erikduss
{
	public partial class BaseSettingsManager : Node
	{
        protected String directoryLocation = "user://Settings//Player//";
        protected String fileName = "PlayerGlobalSettings.cfg";
        protected String fullFilePath;

        protected DirAccess directoryAccessor;
        protected ConfigFile config;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            fullFilePath = directoryLocation + fileName;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        }

        public virtual void CreateNewSaveFile(bool overrideSaveFile)
        {
            config = new ConfigFile();

            // Load data from a file.
            var err = config.Load(fullFilePath);

            // If the file loaded, we dont want to override it.
            if (err == Error.Ok && !overrideSaveFile)
                return;

            //Check if the directory is defined.
            if (string.IsNullOrEmpty(directoryLocation))
                directoryLocation = "user://Settings//Player//";

            //Check if the directory exists, otherwise create the directory.
            var dir = DirAccess.Open(directoryLocation);

            if (dir != null)
                GD.Print("Directory exists!");
            else
            {
                var error_code = DirAccess.MakeDirRecursiveAbsolute(directoryLocation);
                GD.Print("Directory Doesnt exists!");
            }
        }
    }
}
