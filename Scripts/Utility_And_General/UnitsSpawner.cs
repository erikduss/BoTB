using Godot;
using System;
using System.Collections.Generic;

namespace Erikduss
{
	public partial class UnitsSpawner : Node2D
	{
		[Export] public Node2D team01UnitsSpawnerLocation;
        [Export] public Node2D team02UnitsSpawnerLocation;

		public PackedScene simpleSoldierPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/SimpleSoldier.tscn");

		private int lastUsedUnitID = 0;

		//In this dictionary we save the units that we are queueing per team, we need to save the unit type and the age 
		public Dictionary<Enums.UnitTypes, Enums.Ages> team01UnitQueueDictionary = new Dictionary<Enums.UnitTypes, Enums.Ages>();
        public Dictionary<Enums.UnitTypes, Enums.Ages> team02UnitQueueDictionary = new Dictionary<Enums.UnitTypes, Enums.Ages>();

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
			//GameManager.Instance.unitsSpawner = this;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		public void OnCollisionEnterCheckTeam01Spawner(Node2D body)
		{
			GD.Print("Character entered");
		}

        public void OnCollisionExitCheckTeam01Spawner(Node2D body)
        {
            GD.Print("Character exited");
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

		private void AddUnitToQueue()
		{

		}

		private void GetAndRemoveUnitFromQueue()
		{

		}
	}
}
