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
        public PackedScene tankPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/Tank.tscn");
        public PackedScene battleMagePrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/Battlemage.tscn");
        public PackedScene massHealerPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/Mass_Healer.tscn");
        public PackedScene shamanPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/Shaman.tscn");
        public PackedScene archdruidPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/Archdruid.tscn");

        private int lastUsedUnitID = 0;

		//In this dictionary we save the units that we are queueing per team, we need to save the unit type and the age 
		public Dictionary<string, Enums.UnitTypes> team01UnitQueueDictionary = new Dictionary<string, Enums.UnitTypes>();
        public Dictionary<string, Enums.UnitTypes> team02UnitQueueDictionary = new Dictionary<string, Enums.UnitTypes>();

        //The saved string is the Unit type + unique ID
        public Dictionary<string, BaseCharacter> team01AliveUnitDictionary = new Dictionary<string, BaseCharacter>();
        public Dictionary<string, BaseCharacter> team02AliveUnitDictionary = new Dictionary<string, BaseCharacter>();

        public List<BaseCharacter> team01DamagedUnits = new List<BaseCharacter>();
        public List<BaseCharacter> team02DamagedUnits = new List<BaseCharacter>();

        private int team01UnitSpawnIDCurrentValue = 0;
        private int team01LastSpawnedUnitID = -1;

        private int team02UnitSpawnIDCurrentValue = 0;
        private int team02LastSpawnedUnitID = -1;

        private bool team01HasSpawnSpace = true;
        private bool team02HasSpawnSpace = true;

		private float unitSpawnAttamptCooldown = 1f;

		private float team01UnitSpawnAttemptTimer = 0f;
        private float team02UnitSpawnAttemptTimer = 0f;

		private int debugSpawnCounter = 0;

        #region Debug Stuff

        private bool spawnDummies = false;

        public PackedScene trainingDummyPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/TrainingDummy.tscn");

        private float dummyStartingXPosition = -740f; //-342
        private float dummyYPosition = 815f;

        private float dummyXPositionDifference = 41;

        private int trainingDummyCurrentCount = 0;

        private int spawnAmountOfTrainingDummies = 15;

        #endregion

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
            //GameManager.Instance.unitsSpawner = this;
            if (spawnDummies)
            {
                for (int i = 0; i < spawnAmountOfTrainingDummies; i++)
                {
                    AddUnitToQueue(Enums.TeamOwner.TEAM_02, Enums.UnitTypes.TrainingDummy, Enums.Ages.AGE_01);
                }
            }
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
				//else if (team02UnitQueueDictionary.Count <= 0 && debugSpawnCounter == 0)
				//{
				//	debugSpawnCounter = 100;
				//	AddUnitToQueue(Enums.TeamOwner.TEAM_02, Enums.UnitTypes.Warrior, Enums.Ages.AGE_01);
				//}
				//else if (team02UnitQueueDictionary.Count <= 0) debugSpawnCounter--;
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

        public void ProcessBuyingUnit(Enums.TeamOwner team, Enums.UnitTypes unitType)
        {
            Enums.Ages currentAge = team == Enums.TeamOwner.TEAM_01 ? GameManager.Instance.player01Script.currentAgeOfPlayer : GameManager.Instance.player02Script.currentAgeOfPlayer;

            //Add to queue
            AddUnitToQueue(team, unitType, currentAge);
        }

        private void AddUnitToQueue(Enums.TeamOwner team, Enums.UnitTypes unitType, Enums.Ages unitAge)
		{
			if(team == Enums.TeamOwner.TEAM_01)
			{
				string uniqueIDString = team01UnitSpawnIDCurrentValue + "_" + ((uint)unitAge);
				GD.Print("Adding to Queue: " + uniqueIDString);
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
                //we search the dictionary for the unit ID, the ID is saved in the dictionary Key of the unit together with the age its from.
                //So we try to search for the next index based on what was the last unit index that we spawned.
                //This is used to prevent getting the first dictionary index which is getting overritten due to the removal from old units that are already spawned.

                KeyValuePair<string, Enums.UnitTypes> nextUnitToSpawnDictionaryEntry = team01UnitQueueDictionary.Where(a => int.Parse(a.Key.Split('_')[0]) == (team01LastSpawnedUnitID+1)).FirstOrDefault();

                //the age is saved in the unique string, this string is needed to give the unit a unique ID.
                string uniqueIDString = nextUnitToSpawnDictionaryEntry.Key;
                string[] splitUniqueString = uniqueIDString.Split('_');

                Enums.Ages unitAge = (Enums.Ages)splitUniqueString[1].ToInt();

                //GD.Print("Spawning Unit: " + uniqueIDString);

				SpawnUnitFromQueue(team, nextUnitToSpawnDictionaryEntry.Value, unitAge);

                //we need to increase this so we get the correct unit next cycle.
                team01LastSpawnedUnitID++;

				//remove after passing the info
				team01UnitQueueDictionary.Remove(uniqueIDString);
			}
			else
			{
                //we search the dictionary for the unit ID, the ID is saved in the dictionary Key of the unit together with the age its from.
                //So we try to search for the next index based on what was the last unit index that we spawned.
                //This is used to prevent getting the first dictionary index which is getting overritten due to the removal from old units that are already spawned.

                KeyValuePair<string, Enums.UnitTypes> nextUnitToSpawnDictionaryEntry = team02UnitQueueDictionary.Where(a => int.Parse(a.Key.Split('_')[0]) == (team02LastSpawnedUnitID + 1)).FirstOrDefault();

                //the age is saved in the unique string, this string is needed to give the unit a unique ID.
                string uniqueIDString = nextUnitToSpawnDictionaryEntry.Key;
                string[] splitUniqueString = uniqueIDString.Split('_');

                Enums.Ages unitAge = (Enums.Ages)splitUniqueString[1].ToInt();

                SpawnUnitFromQueue(team, nextUnitToSpawnDictionaryEntry.Value, unitAge);

                //we need to increase this so we get the correct unit next cycle.
                team02LastSpawnedUnitID++;

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

        public int AddTrainingDummyToAliveDictionary(Enums.TeamOwner team, BaseCharacter unitChar)
        {
            string uniqueIDString;

            if (team == Enums.TeamOwner.TEAM_01)
            {
                uniqueIDString = lastUsedUnitID + "_" + ((uint)unitChar.currentAge);
                lastUsedUnitID++;
                team01AliveUnitDictionary.Add(uniqueIDString, unitChar);

                return lastUsedUnitID - 1;
            }
            else
            {
                uniqueIDString = lastUsedUnitID + "_" + ((uint)unitChar.currentAge);
                lastUsedUnitID++;
                team02AliveUnitDictionary.Add(uniqueIDString, unitChar);

                return lastUsedUnitID - 1;
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
                case Enums.UnitTypes.Assassin:

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
                case Enums.UnitTypes.Tank:

                    //NOTE: IF CAST TO NOTE2D DOESNT WORK, DOUBLE CHECK SCRIPTS ATTACHED TO PREFAB, MAKE SURE THEY INHERIT NOTE2D NOT NODE.
                    Tank instantiatedTank = (Tank)tankPrefab.Instantiate();

                    //determine the position based on the team
                    instantiatedTank.GlobalPosition = team == Enums.TeamOwner.TEAM_01 ? team01UnitsSpawnerLocation.Position : team02UnitsSpawnerLocation.Position;
                    instantiatedTank.characterOwner = team;
                    instantiatedTank.currentAge = unitAge;

                    instantiatedTank.Name = "instantiatedTank_" + lastUsedUnitID;

                    instantiatedTank.uniqueID = lastUsedUnitID;

                    AddChild(instantiatedTank);

                    uniqueUnitName = (uint)unitType + "_" + lastUsedUnitID;
                    AddUnitToAliveDict(team, instantiatedTank, uniqueUnitName);

                    lastUsedUnitID++;

                    break;
                case Enums.UnitTypes.Battlemage:

                    //NOTE: IF CAST TO NOTE2D DOESNT WORK, DOUBLE CHECK SCRIPTS ATTACHED TO PREFAB, MAKE SURE THEY INHERIT NOTE2D NOT NODE.
                    Battlemage instantiatedBattlemage = (Battlemage)battleMagePrefab.Instantiate();

                    //determine the position based on the team
                    instantiatedBattlemage.GlobalPosition = team == Enums.TeamOwner.TEAM_01 ? team01UnitsSpawnerLocation.Position : team02UnitsSpawnerLocation.Position;
                    instantiatedBattlemage.characterOwner = team;
                    instantiatedBattlemage.currentAge = unitAge;

                    instantiatedBattlemage.Name = "instantiatedBattlemage_" + lastUsedUnitID;

                    instantiatedBattlemage.uniqueID = lastUsedUnitID;

                    AddChild(instantiatedBattlemage);

                    uniqueUnitName = (uint)unitType + "_" + lastUsedUnitID;
                    AddUnitToAliveDict(team, instantiatedBattlemage, uniqueUnitName);

                    lastUsedUnitID++;

                    break;
                case Enums.UnitTypes.Mass_Healer:

                    //NOTE: IF CAST TO NOTE2D DOESNT WORK, DOUBLE CHECK SCRIPTS ATTACHED TO PREFAB, MAKE SURE THEY INHERIT NOTE2D NOT NODE.
                    Mass_Healer instantiatedMassHealer = (Mass_Healer)massHealerPrefab.Instantiate();

                    //determine the position based on the team
                    instantiatedMassHealer.GlobalPosition = team == Enums.TeamOwner.TEAM_01 ? team01UnitsSpawnerLocation.Position : team02UnitsSpawnerLocation.Position;
                    instantiatedMassHealer.characterOwner = team;
                    instantiatedMassHealer.currentAge = unitAge;

                    instantiatedMassHealer.Name = "instantiatedMassHealer_" + lastUsedUnitID;

                    instantiatedMassHealer.uniqueID = lastUsedUnitID;

                    AddChild(instantiatedMassHealer);

                    uniqueUnitName = (uint)unitType + "_" + lastUsedUnitID;
                    AddUnitToAliveDict(team, instantiatedMassHealer, uniqueUnitName);

                    lastUsedUnitID++;

                    break;
                case Enums.UnitTypes.Shaman:

                    //NOTE: IF CAST TO NOTE2D DOESNT WORK, DOUBLE CHECK SCRIPTS ATTACHED TO PREFAB, MAKE SURE THEY INHERIT NOTE2D NOT NODE.
                    Shaman instantiatedShaman = (Shaman)shamanPrefab.Instantiate();

                    //determine the position based on the team
                    instantiatedShaman.GlobalPosition = team == Enums.TeamOwner.TEAM_01 ? team01UnitsSpawnerLocation.Position : team02UnitsSpawnerLocation.Position;
                    instantiatedShaman.characterOwner = team;
                    instantiatedShaman.currentAge = unitAge;

                    instantiatedShaman.Name = "instantiatedShaman_" + lastUsedUnitID;

                    instantiatedShaman.uniqueID = lastUsedUnitID;

                    AddChild(instantiatedShaman);

                    uniqueUnitName = (uint)unitType + "_" + lastUsedUnitID;
                    AddUnitToAliveDict(team, instantiatedShaman, uniqueUnitName);

                    lastUsedUnitID++;

                    break;
                case Enums.UnitTypes.Archdruid:

                    //NOTE: IF CAST TO NOTE2D DOESNT WORK, DOUBLE CHECK SCRIPTS ATTACHED TO PREFAB, MAKE SURE THEY INHERIT NOTE2D NOT NODE.
                    Archdruid instantiatedArchdruid = (Archdruid)archdruidPrefab.Instantiate();

                    //determine the position based on the team
                    instantiatedArchdruid.GlobalPosition = team == Enums.TeamOwner.TEAM_01 ? team01UnitsSpawnerLocation.Position : team02UnitsSpawnerLocation.Position;
                    instantiatedArchdruid.characterOwner = team;
                    instantiatedArchdruid.currentAge = unitAge;

                    instantiatedArchdruid.Name = "instantiatedArchdruid_" + lastUsedUnitID;

                    instantiatedArchdruid.uniqueID = lastUsedUnitID;

                    AddChild(instantiatedArchdruid);

                    uniqueUnitName = (uint)unitType + "_" + lastUsedUnitID;
                    AddUnitToAliveDict(team, instantiatedArchdruid, uniqueUnitName);

                    lastUsedUnitID++;

                    break;
                case Enums.UnitTypes.TrainingDummy:
                    //NOTE: IF CAST TO NOTE2D DOESNT WORK, DOUBLE CHECK SCRIPTS ATTACHED TO PREFAB, MAKE SURE THEY INHERIT NOTE2D NOT NODE.
                    TrainingDummy instantiatedTrainingDummy = (TrainingDummy)trainingDummyPrefab.Instantiate();

                    //determine the position based on the team

                    instantiatedTrainingDummy.GlobalPosition =  new Vector2(dummyStartingXPosition + (dummyXPositionDifference * trainingDummyCurrentCount), dummyYPosition);
                    instantiatedTrainingDummy.characterOwner = team;
                    instantiatedTrainingDummy.currentAge = unitAge;

                    instantiatedTrainingDummy.Name = "instantiatedTrainingDummy_" + lastUsedUnitID;

                    instantiatedTrainingDummy.uniqueID = lastUsedUnitID;

                    AddChild(instantiatedTrainingDummy);

                    uniqueUnitName = (uint)unitType + "_" + lastUsedUnitID;
                    AddUnitToAliveDict(team, instantiatedTrainingDummy, uniqueUnitName);

                    //EffectsAndProjectilesSpawner.Instance.SpawnTestingBleedingEffect(instantiatedTrainingDummy);

                    trainingDummyCurrentCount++;
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
