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
        public PackedScene rangerPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/Ranger.tscn");
        public PackedScene assassinPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/Assassin.tscn");
        public PackedScene enforcerPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/Enforcer.tscn");

        private int lastUsedUnitID = 0;

		//In this dictionary we save the units that we are queueing per team, we need to save the unit type and the age 
		public Dictionary<string, Enums.UnitTypes> team01UnitQueueDictionary = new Dictionary<string, Enums.UnitTypes>();
        public Dictionary<string, Enums.UnitTypes> team02UnitQueueDictionary = new Dictionary<string, Enums.UnitTypes>();

        //The saved string is the Unit type + unique ID
        public Dictionary<string, BaseCharacter> team01AliveUnitDictionary = new Dictionary<string, BaseCharacter>();
        public Dictionary<string, BaseCharacter> team02AliveUnitDictionary = new Dictionary<string, BaseCharacter>();

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
				else
				{
					//GD.Print("Has space? " + team01HasSpawnSpace + " Queue amount: " + team01UnitQueueDictionary.Count);
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
					debugSpawnCounter = 100;
					AddUnitToQueue(Enums.TeamOwner.TEAM_02, Enums.UnitTypes.Warrior, Enums.Ages.AGE_01);
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
			AddUnitToQueue(team, Enums.UnitTypes.Warrior, currentAge);
		}

        public void ProcessBuyingRanger(Enums.TeamOwner team)
        {
            //spawn prefab
            Enums.Ages currentAge = Enums.Ages.AGE_01; //This should first check the age of the specific team

            //Add to queue
            AddUnitToQueue(team, Enums.UnitTypes.Ranger, currentAge);
        }

        public void ProcessBuyingEnforcer(Enums.TeamOwner team)
        {
            //spawn prefab
            Enums.Ages currentAge = Enums.Ages.AGE_01; //This should first check the age of the specific team

            //Add to queue
            AddUnitToQueue(team, Enums.UnitTypes.Enforcer, currentAge);
        }

        public void ProcessBuyingAssassin(Enums.TeamOwner team)
        {
            //spawn prefab
            Enums.Ages currentAge = Enums.Ages.AGE_01; //This should first check the age of the specific team

            //Add to queue
            AddUnitToQueue(team, Enums.UnitTypes.Asssassin, currentAge);
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

        private void AddUnitToAliveDict(Enums.TeamOwner team, BaseCharacter unitChar, string uniqueIDString)
        {
            if (team == Enums.TeamOwner.TEAM_01)
            {
                team01AliveUnitDictionary.Add(uniqueIDString, unitChar);
            }
            else
            {
                team02AliveUnitDictionary.Add(uniqueIDString, unitChar);
            }
        }

        public void RemoveAliveUnitFromTeam(Enums.TeamOwner team, Enums.UnitTypes unitType, int unitID)
        {
            if (team == Enums.TeamOwner.TEAM_01)
            {
                //unique ID = unit type + unitID
                string uniqueIDString = (uint)unitType + "_" + unitID;

                team01AliveUnitDictionary.Remove(uniqueIDString);
            }
            else
            {
                string uniqueIDString = (uint)unitType + "_" + unitID;

                team02AliveUnitDictionary.Remove(uniqueIDString);
            }
        }

        private void SpawnUnitFromQueue(Enums.TeamOwner team, Enums.UnitTypes unitType, Enums.Ages unitAge)
		{
            //GD.Print("We are spawning: " + unitType.ToString() + " for team: " + team.ToString() + " in age: " + unitAge.ToString());

            string uniqueUnitName = "";

            switch (unitType)
			{
				case Enums.UnitTypes.Warrior:

                    //NOTE: IF CAST TO NOTE2D DOESNT WORK, DOUBLE CHECK SCRIPTS ATTACHED TO PREFAB, MAKE SURE THEY INHERIT NOTE2D NOT NODE.
                    SimpleSoldier instantiatedSimpleSoldier = (SimpleSoldier)simpleSoldierPrefab.Instantiate();

                    //determine the position based on the team
                    instantiatedSimpleSoldier.GlobalPosition = team == Enums.TeamOwner.TEAM_01 ? team01UnitsSpawnerLocation.Position : team02UnitsSpawnerLocation.Position;
                    instantiatedSimpleSoldier.characterOwner = team;
                    instantiatedSimpleSoldier.currentAge = unitAge;

                    instantiatedSimpleSoldier.Name = "InstantiatedSimpleSoldier_" + lastUsedUnitID;

                    instantiatedSimpleSoldier.uniqueID = lastUsedUnitID;

                    AddChild(instantiatedSimpleSoldier);

                    uniqueUnitName = (uint)unitType + "_" + lastUsedUnitID;
                    AddUnitToAliveDict(team, instantiatedSimpleSoldier, uniqueUnitName);

                    lastUsedUnitID++;

                    break;
				case Enums.UnitTypes.Ranger:

                    //NOTE: IF CAST TO NOTE2D DOESNT WORK, DOUBLE CHECK SCRIPTS ATTACHED TO PREFAB, MAKE SURE THEY INHERIT NOTE2D NOT NODE.
                    Ranger instantiatedRanger = (Ranger)rangerPrefab.Instantiate();

                    //determine the position based on the team
                    instantiatedRanger.GlobalPosition = team == Enums.TeamOwner.TEAM_01 ? team01UnitsSpawnerLocation.Position : team02UnitsSpawnerLocation.Position;
                    instantiatedRanger.characterOwner = team;
                    instantiatedRanger.currentAge = unitAge;

                    instantiatedRanger.Name = "InstantiatedRanger_" + lastUsedUnitID;

                    instantiatedRanger.uniqueID = lastUsedUnitID;

                    AddChild(instantiatedRanger);

                    uniqueUnitName = (uint)unitType + "_" + lastUsedUnitID;
                    AddUnitToAliveDict(team, instantiatedRanger, uniqueUnitName);

                    lastUsedUnitID++;

                    break;
                case Enums.UnitTypes.Asssassin:

                    //NOTE: IF CAST TO NOTE2D DOESNT WORK, DOUBLE CHECK SCRIPTS ATTACHED TO PREFAB, MAKE SURE THEY INHERIT NOTE2D NOT NODE.
                    Assassin instantiatedAssassin = (Assassin)assassinPrefab.Instantiate();

                    //determine the position based on the team
                    instantiatedAssassin.GlobalPosition = team == Enums.TeamOwner.TEAM_01 ? team01UnitsSpawnerLocation.Position : team02UnitsSpawnerLocation.Position;
                    instantiatedAssassin.characterOwner = team;
                    instantiatedAssassin.currentAge = unitAge;

                    instantiatedAssassin.Name = "instantiatedAssassin_" + lastUsedUnitID;

                    instantiatedAssassin.uniqueID = lastUsedUnitID;

                    AddChild(instantiatedAssassin);

                    uniqueUnitName = (uint)unitType + "_" + lastUsedUnitID;
                    AddUnitToAliveDict(team, instantiatedAssassin, uniqueUnitName);

                    lastUsedUnitID++;

                    break;
                case Enums.UnitTypes.Enforcer:

                    //NOTE: IF CAST TO NOTE2D DOESNT WORK, DOUBLE CHECK SCRIPTS ATTACHED TO PREFAB, MAKE SURE THEY INHERIT NOTE2D NOT NODE.
                    Enforcer instantiatedEnforcer = (Enforcer)enforcerPrefab.Instantiate();

                    //determine the position based on the team
                    instantiatedEnforcer.GlobalPosition = team == Enums.TeamOwner.TEAM_01 ? team01UnitsSpawnerLocation.Position : team02UnitsSpawnerLocation.Position;
                    instantiatedEnforcer.characterOwner = team;
                    instantiatedEnforcer.currentAge = unitAge;

                    instantiatedEnforcer.Name = "instantiatedEnforcer_" + lastUsedUnitID;

                    instantiatedEnforcer.uniqueID = lastUsedUnitID;

                    AddChild(instantiatedEnforcer);

                    uniqueUnitName = (uint)unitType + "_" + lastUsedUnitID;
                    AddUnitToAliveDict(team, instantiatedEnforcer, uniqueUnitName);

                    lastUsedUnitID++;

                    break;
                default:
					GD.PrintErr("UNIT NOT IMPLEMENTED, UNITSPAWNER, SPAWNUNITFROMQUEUE");

                    //NOTE: IF CAST TO NOTE2D DOESNT WORK, DOUBLE CHECK SCRIPTS ATTACHED TO PREFAB, MAKE SURE THEY INHERIT NOTE2D NOT NODE.
                    SimpleSoldier instantiatedNoType = (SimpleSoldier)simpleSoldierPrefab.Instantiate();

                    //determine the position based on the team
                    instantiatedNoType.GlobalPosition = team == Enums.TeamOwner.TEAM_01 ? team01UnitsSpawnerLocation.Position : team02UnitsSpawnerLocation.Position;
                    instantiatedNoType.characterOwner = team;
                    instantiatedNoType.currentAge = unitAge;

                    instantiatedNoType.Name = "InstantiatedNoTyoe_" + lastUsedUnitID;

                    instantiatedNoType.uniqueID = lastUsedUnitID;

                    AddChild(instantiatedNoType);

                    uniqueUnitName = (uint)unitType + "_" + lastUsedUnitID;
                    AddUnitToAliveDict(team, instantiatedNoType, uniqueUnitName);

                    lastUsedUnitID++;

                    break;
			}
        }
	}
}
