using Godot;
using System;

namespace Erikduss
{
	public partial class UnitsSpawner : Node
	{
		[Export] public Node2D team01UnitsSpawnerLocation;
        [Export] public Node2D team02UnitsSpawnerLocation;

		public PackedScene simpleSoldierPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/SimpleSoldier.tscn");

		private int lastUsedUnitID = 0;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
			//GameManager.Instance.unitsSpawner = this;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		public void ProcessBuyingSimpleSoldier(Enums.TeamOwner team)
		{
			//queue?

			GD.Print("We should spawn now");

			//spawn prefab
			Enums.Ages currentAge = Enums.Ages.AGE_01; //This should first check the age of the specific team

            //NOTE: IF CAST TO NOTE2D DOESNT WORK, DOUBLE CHECK SCRIPTS ATTACHED TO PREFAB, MAKE SURE THEY INHERIT NOTE2D NOT NODE.
            SimpleSoldier instantiatedSimpleSoldier = (SimpleSoldier)simpleSoldierPrefab.Instantiate();

			//determine the position based on the team
            instantiatedSimpleSoldier.GlobalPosition = team == Enums.TeamOwner.TEAM_01 ? team01UnitsSpawnerLocation.Position : team02UnitsSpawnerLocation.Position;
            instantiatedSimpleSoldier.characterOwner = team;
            instantiatedSimpleSoldier.currentAge = currentAge;

            instantiatedSimpleSoldier.Name = "InstantiatedSimpleSoldier_" + lastUsedUnitID;
            lastUsedUnitID++;

            AddChild(instantiatedSimpleSoldier);
		}
	}
}
