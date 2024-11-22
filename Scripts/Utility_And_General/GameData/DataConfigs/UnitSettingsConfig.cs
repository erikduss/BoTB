using Godot;
using System;

namespace Erikduss
{
	public partial class UnitSettingsConfig : Node
	{
        public bool useCustomVariables = false;

        //Unit Info
        public int unitCost = 10;
        public Enums.Ages unitAvailableInAge = Enums.Ages.AGE_01; //make this enum?

        public string unitName = "Warrior";
        public string unitDescription = "This is where the unit's description is being loaded from and saved to.";

        //Unit Stats
        public int unitHealth = 20;
        public int unitArmour = 20;
        public int unitAttack = 10;

        //Unit gameplay variables
        public int unitMovementSpeed = 5; //might needs to be a float
        public int unitDetectionRange = 5; //might needs to be a float
    }
}
