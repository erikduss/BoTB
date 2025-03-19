using Godot;
using System;

namespace Erikduss
{
    public partial class GameUserOptionsConfig : Node
    {
        #region Audio Settings

        public int musicVolume = 25;
        public int otherVolume = 25;

        #endregion

        #region Gameplay Settings

        public Enums.ScreenMovementType screenMovement = Enums.ScreenMovementType.Use_Both;
        public int addedDragSensitivity = 0;
        public int addedSidesSensitivity = 0;

        #endregion

        #region Graphics Settings

        public Enums.DisplayMode displayMode = Enums.DisplayMode.Fullscreen;
        public Enums.ScreenResolution screenResolution = Enums.ScreenResolution.RES_1920x1080;
        public string overrideScreenResolution = "";
        public int limitFPS = 1; //default limit fps to 144.
        public int fpsLimit = 144;

        #endregion

        #region Accessibility Settings

        public bool enableHemophobiaMode = false; //alternate blood color

        #endregion
    }
}
