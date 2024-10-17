using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Erikduss
{
	public partial class UnitsSpawner : Node2D
	{
		[Export] public Node2D team01UnitsSpawnerLocation;
        [Export] public Node2D team02UnitsSpawnerLocation;

		public PackedScene simpleSoldierPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/SimpleSoldier.tscn");

		private int lastUsedUnitID = 0;

		//In this dictionary we save the units that we are queueing per team, we need to save the unit type and the age 
		public Dictionary<string, Enums.UnitTypes> team01UnitQueueDictionary = new Dictionary<string, Enums.UnitTypes>();
        public Dictionary<string, Enums.UnitTypes> team02UnitQueueDictionary = new Dictionary<string, Enums.UnitTypes>();

		private int team01UnitSpawnIDCurrentValue = 0;
        private int team02UnitSpawnIDCurrentValue = 0;

        private bool team01HasSpawnSpace = true;
        private bool team02HasSpawnSpace = true;

		private float unitSpawnAttamptCooldown = 1f;

		private float team01UnitSpawnAttemptTimer = 0f;
        private float team02UnitSpawnAttemptTimer = 0f;

		private int debugSpawnCounter = 0;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
			//GameManager.Instance.unitsSpawner = this;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			//Note: Both teams need their own timers and statements due to both players behaving differently, 1 player could have units queued while the other could have no queue.
			if(team01UnitSpawnAttemptTimer <= 0f)
			{
                if (team01HasSpawnSpace && team01UnitQueueDictionary.Count > 0)
                {
					//Add a cooldown to prevent multiple units from spawning at the same time.
					team01UnitSpawnAttemptTimer = unitSpawnAttamptCooldown;
					GetAndRemoveUnitFromQueue(Enums.TeamOwner.TEAM_01);
                }
            }
			else
			{
				team01UnitSpawnAttemptTimer -= (float)delta;
			}

			if(team02UnitSpawnAttemptTimer <= 0f)
			{
				if (team02HasSpawnSpace && team02UnitQueueDictionary.Count > 0)
				{
					//Add a cooldown to prevent multiple units from spawning at the same time.
					team02UnitSpawnAttemptTimer = unitSpawnAttamptCooldown;
					GetAndRemoveUnitFromQueue(Enums.TeamOwner.TEAM_02);
				}
				//debug, spawn some enemies
				else if (team02UnitQueueDictionary.Count <= 0 && debugSpawnCounter == 0)
				{
					debugSpawnCounter = 10000;
					AddUnitToQueue(Enums.TeamOwner.TEAM_02, Enums.UnitTypes.SimpleSoldier, Enums.Ages.AGE_01);
				}
				else if (team02UnitQueueDictionary.Count <= 0) debugSpawnCounter--;
            }
			else
			{
				team02UnitSpawnAttemptTimer -= (float)delta;
            }
		}

		public void OnCollisionEnterCheckTeam01Spawner(Node2D body)
		{
			//GD.Print("Character entered 01");
			team01HasSpawnSpace = false;

        }

        public void OnCollisionExitCheckTeam01Spawner(Node2D body)
        {
            //GD.Print("Character exited 01");
			team01HasSpawnSpace = true;
        }

        public void OnCollisionEnterCheckTeam02Spawner(Node2D body)
        {
            //GD.Print("Character entered 02");
			team02HasSpawnSpace = false;
        }

        public void OnCollisionExitCheckTeam02Spawner(Node2D body)
        {
            //GD.Print("Character exited 02");
			team02HasSpawnSpace = true;
        }

        public void ProcessBuyingSimpleSoldier(Enums.TeamOwner team)
		{
			//spawn prefab
			Enums.Ages currentAge = Enums.Ages.AGE_01; //This should first check the age of the specific team

			//Add to queue
			AddUnitToQueue(team, Enums.UnitTypes.SimpleSoldier, currentAge);
		}

		private void AddUnitToQueue(Enums.TeamOwner team, Enums.UnitTypes unitType, Enums.Ages unitAge)
		{
			if(team == Enums.TeamOwner.TEAM_01)
			{
				string uniqueIDString = team01UnitSpawnIDCurrentValue + "_" + ((uint)unitAge);
				//GD.Print(uniqueIDString);
				team01UnitSpawnIDCurrentValue++;

                team01UnitQueueDictionary.Add(uniqueIDString, unitType);
			}
			else
			{
                string uniqueIDString = team02UnitSpawnIDCurrentValue + "_" + ((uint)unitAge);
				team02UnitSpawnIDCurrentValue++;

                team02UnitQueueDictionary.Add(uniqueIDString, unitType);
			}
		}

		private void GetAndRemoveUnitFromQueue(Enums.TeamOwner team)
		{
			if(team == Enums.TeamOwner.TEAM_01)
			{
				//the age is saved in the unique string, this string is needed to give the unit a unique ID.
				string uniqueIDString = team01UnitQueueDictionary.FirstOrDefault().Key;
				string[] splitUniqueString = uniqueIDString.Split('_');

				GD.Print(splitUniqueString[1]);

				Enums.Ages unitAge = (Enums.Ages)splitUniqueString[1].ToInt();

				SpawnUnitFromQueue(team, team01UnitQueueDictionary.FirstOrDefault().Value, unitAge);

				//remove after passing the info
				team01UnitQueueDictionary.Remove(uniqueIDString);
			}
			else
			{
                //the age is saved in the unique string, this string is needed to give the unit a unique ID.
                string uniqueIDString = team02UnitQueueDictionary.FirstOrDefault().Key;
                string[] splitUniqueString = uniqueIDString.Split('_');

                Enums.Ages unitAge = (Enums.Ages)splitUniqueString[1].ToInt();

                SpawnUnitFromQueue(team, team02UnitQueueDictionary.FirstOrDefault().Value, unitAge);

                //remove after passing the info
                team02UnitQueueDictionary.Remove(uniqueIDString);
            }
		}

		private void SpawnUnitFromQueue(Enums.TeamOwner team, Enums.UnitTypes unitType, Enums.Ages unitAge)
		{
			//GD.Print("We are spawning: " + unitType.ToString() + " for team: " + team.ToString() + " in age: " + unitAge.ToString());

			if(unitType == Enums.UnitTypes.SimpleSoldier)
			{
                //NOTE: IF CAST TO NOTE2D DOESNT WORK, DOUBLE CHECK SCRIPTS ATTACHED TO PREFAB, MAKE SURE THEY INHERIT NOTE2D NOT NODE.
                SimpleSoldier instantiatedSimpleSoldier = (SimpleSoldier)simpleSoldierPrefab.Instantiate();

                //determine the position based on the team
                instantiatedSimpleSoldier.GlobalPosition = team == Enums.TeamOwner.TEAM_01 ? team01UnitsSpawnerLocation.Position : team02UnitsSpawnerLocation.Position;
                instantiatedSimpleSoldier.characterOwner = team;
                instantiatedSimpleSoldier.currentAge = unitAge;

                instantiatedSimpleSoldier.Name = "InstantiatedSimpleSoldier_" + lastUsedUnitID;
                lastUsedUnitID++;

                AddChild(instantiatedSimpleSoldier);
            }
        }
	}
}
