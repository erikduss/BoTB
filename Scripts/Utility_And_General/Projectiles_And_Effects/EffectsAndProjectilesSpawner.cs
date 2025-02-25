using Godot;
using System;
using System.Collections.Generic;
using System.Xml;
using static Godot.TextServer;
using static System.Net.Mime.MediaTypeNames;

namespace Erikduss
{
	public partial class EffectsAndProjectilesSpawner : Node2D
	{
        public static EffectsAndProjectilesSpawner Instance { get; private set; }

        #region Unit Effects And Projectiles
        public PackedScene warriorAttackVisualEffect = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/Effets_And_Projectiles/SimpleSoldierShockwave.tscn");

        public PackedScene rangerAge1Projectile = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/Effets_And_Projectiles/RangerAge1Projectile.tscn");

        public PackedScene assassinBleedingEffect = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/Effets_And_Projectiles/AssassinBleedingEffect.tscn");

        public PackedScene enforcerStunEffect = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/Effets_And_Projectiles/EnforcerStunEffect.tscn");

        public PackedScene tankBuffEffect = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/Effets_And_Projectiles/TankBuffEffect.tscn");

        public PackedScene battleMageAge1Projectile = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/Effets_And_Projectiles/BattlemageAge1Projectile.tscn");
        public PackedScene battleMageFireball = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/Effets_And_Projectiles/BattlemageFireball.tscn");

        public PackedScene healingEffect = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/Effets_And_Projectiles/HealingEffect.tscn");

        public PackedScene shamanAge1Projectile = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/Effets_And_Projectiles/ShamanAge1Projectile.tscn");
        #endregion

        #region Age Abilities And Effects

        public int baseAmountOfMeteorsToSpawn = 10; //this will be loaded in elsewere.
        public int team01AbilityEmpowerAmount = 0;
        public int team02AbilityEmpowerAmount = 0;

        public PackedScene meteorAbilyObjectPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Spawnable_Objects/Age01_Ability_Meteors/basic_Meteor.tscn");
        public PackedScene meteorImpactObjectPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Spawnable_Objects/Age01_Ability_Meteors/Age01_Meteor_Impact.tscn");

        #endregion

        private int lastUsedVisualEffectID = 0;

        public PackedScene trainingDummyFloatingDamage = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Spawnable_Objects/FloatingDamageNumber.tscn");

        public bool processingHealingEffects = false;

        // Called when the node enters the scene tree for the first time.
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            //We need to set the instance to null to reset the game variables.
            Instance = null;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
		{
		}

        public void SpawnWarriorShockwave(BaseCharacter unitOwner, float xMultiplier)
        {
            WarriorShockwave instantiatedShockwave = (WarriorShockwave)warriorAttackVisualEffect.Instantiate();

            float offSetX = 60f * xMultiplier;
            float addedXValue = unitOwner.movementSpeed >= 0 ? offSetX : -offSetX;
            float addedYValue = 2f; //make sure its on the ground.

            Vector2 fixedPosition = new Vector2(unitOwner.GlobalPosition.X + addedXValue, unitOwner.GlobalPosition.Y + addedYValue);
            instantiatedShockwave.GlobalPosition = fixedPosition;

            instantiatedShockwave.flipSpite = unitOwner.movementSpeed >= 0 ? false : true;

            instantiatedShockwave.Name = unitOwner.uniqueID + "_InstantiatedShockwave_" + lastUsedVisualEffectID;

            AddChild(instantiatedShockwave);

            lastUsedVisualEffectID++;
        }

        public void SpawnRangerProjectile(BaseCharacter unitOwner)
        {
            RangerAge1ProjectilePhysics instantiatedProjectile = (RangerAge1ProjectilePhysics)rangerAge1Projectile.Instantiate();

            instantiatedProjectile.attachedProjectileScript.projectileOwner = unitOwner.characterOwner;
            instantiatedProjectile.attachedProjectileScript.SetNewOwner(unitOwner.characterOwner);
            instantiatedProjectile.attachedProjectileScript.projectileOwnerChar = unitOwner;

            float offSetX = 30f;
            float addedXValue = unitOwner.movementSpeed >= 0 ? offSetX : -offSetX;
            float addedYValue = 2f; //make sure its on the ground.

            Vector2 fixedPosition = new Vector2(unitOwner.GlobalPosition.X + addedXValue, unitOwner.GlobalPosition.Y + addedYValue);
            instantiatedProjectile.GlobalPosition = fixedPosition;

            instantiatedProjectile.attachedProjectileScript.flipSpite = unitOwner.movementSpeed >= 0 ? false : true;

            instantiatedProjectile.Name = unitOwner.uniqueID + "_InstantiatedProjectile_" + lastUsedVisualEffectID;

            AddChild(instantiatedProjectile);

            lastUsedVisualEffectID++;
        }

        public void SpawnAssassinBleedingEffect(BaseCharacter unitOwner)
        {
            AssassinBleedingEffect instantiatedBleedingEffect = (AssassinBleedingEffect)assassinBleedingEffect.Instantiate();

            //Note: This effect is applied on the TARGET, keep this in mind.

            instantiatedBleedingEffect.characterThisEffectIsAttachedTo = unitOwner.currentTarget;
            instantiatedBleedingEffect.unitThisIsDamaging = unitOwner.currentTarget;

            instantiatedBleedingEffect.flipSpite = unitOwner.currentTarget.movementSpeed >= 0 ? false : true;

            instantiatedBleedingEffect.Name = unitOwner.uniqueID + "_InstantiatedBleedEffect_" + lastUsedVisualEffectID;

            unitOwner.currentTarget.AddChild(instantiatedBleedingEffect);

            lastUsedVisualEffectID++;
        }

        public void SpawnTestingBleedingEffect(BaseCharacter bleedTarget)
        {
            AssassinBleedingEffect instantiatedBleedingEffect = (AssassinBleedingEffect)assassinBleedingEffect.Instantiate();

            //Note: This effect is applied on the TARGET, keep this in mind.

            instantiatedBleedingEffect.characterThisEffectIsAttachedTo = bleedTarget;
            instantiatedBleedingEffect.unitThisIsDamaging = bleedTarget;

            instantiatedBleedingEffect.flipSpite = bleedTarget.movementSpeed >= 0 ? false : true;

            instantiatedBleedingEffect.Name = bleedTarget.uniqueID + "_InstantiatedBleedEffect_" + lastUsedVisualEffectID;

            bleedTarget.AddChild(instantiatedBleedingEffect);

            lastUsedVisualEffectID++;
        }

        public void SpawnEnforcerStunEffect(BaseCharacter unitOwner)
        {
            EnforcerStunEffect instantiatedStunEffect = (EnforcerStunEffect)enforcerStunEffect.Instantiate();

            instantiatedStunEffect.characterThisEffectIsAttachedTo = unitOwner.currentTarget;
            instantiatedStunEffect.flipSpite = unitOwner.currentTarget.movementSpeed >= 0 ? false : true;

            instantiatedStunEffect.Name = unitOwner.uniqueID + "_InstantiatedStunEffect_" + lastUsedVisualEffectID;

            unitOwner.currentTarget.ApplyStunEffect();
            unitOwner.currentTarget.AddChild(instantiatedStunEffect);

            lastUsedVisualEffectID++;
        }

        public void SpawnTankBuffEffect(BaseCharacter unitOwner)
        {
            //We need to check if there are actually units behind us within a certain range.
            System.Collections.Generic.Dictionary<string, BaseCharacter> dictionaryToSearch;

            //check which team this is, so which dictionary we need to check.
            if (unitOwner.characterOwner == Enums.TeamOwner.TEAM_01)
            {
                dictionaryToSearch = GameManager.Instance.unitsSpawner.team01AliveUnitDictionary;
            }
            else
            {
                dictionaryToSearch = GameManager.Instance.unitsSpawner.team02AliveUnitDictionary;
            }


            foreach (BaseCharacter friendlyTeamUnit in dictionaryToSearch.Values)
            {
                //we cannot stack tank buffs.
                if (friendlyTeamUnit.hasActiveTankBuff) continue;

                float distance = friendlyTeamUnit.GlobalPosition.X - unitOwner.GlobalPosition.X;

                if (distance < 0) distance = -distance;

                if (distance > 144) continue; //chose 144 due to characters being 64x64, plus keeping some margin of error.

                if (unitOwner.characterOwner == Enums.TeamOwner.TEAM_01)
                {
                    //This should not be possible, but we check if the unit is in front the unit or not.
                    if (friendlyTeamUnit.GlobalPosition.X > unitOwner.GlobalPosition.X) continue;
                }
                else
                {
                    if (friendlyTeamUnit.GlobalPosition.X < unitOwner.GlobalPosition.X) continue;
                }

                TankBuffEffect instantiatedBuffEffect = (TankBuffEffect)tankBuffEffect.Instantiate();

                instantiatedBuffEffect.characterThisEffectIsAttachedTo = friendlyTeamUnit;
                instantiatedBuffEffect.flipSpite = friendlyTeamUnit.movementSpeed >= 0 ? false : true;

                instantiatedBuffEffect.Name = unitOwner.uniqueID + "_InstantiatedStunEffect_" + lastUsedVisualEffectID;

                friendlyTeamUnit.ApplyTankBuffEffect(instantiatedBuffEffect.destroyTime);
                friendlyTeamUnit.AddChild(instantiatedBuffEffect);

                lastUsedVisualEffectID++;
            }
        }

        public void SpawnMass_Healer_HealingEffect(BaseCharacter unitOwner, bool checkForMaxDistance = true)
        {
            List<BaseCharacter> listToSearch;

            //check which team this is, so which list we need to check.
            if (unitOwner.characterOwner == Enums.TeamOwner.TEAM_01)
            {
                listToSearch = GameManager.Instance.unitsSpawner.team01DamagedUnits;
            }
            else
            {
                listToSearch = GameManager.Instance.unitsSpawner.team02DamagedUnits;
            }

            processingHealingEffects = true;

            foreach (BaseCharacter friendlyTeamUnit in listToSearch)
            {
                //we cannot heal if they are dead.
                if (friendlyTeamUnit.isDead) continue;
                if (friendlyTeamUnit.currentHealth == friendlyTeamUnit.maxHealth) continue; //this could possibly happen with a double heal, we want to prevent adding the icon twice.

                if (checkForMaxDistance)
                {
                    float distance = friendlyTeamUnit.GlobalPosition.X - unitOwner.GlobalPosition.X;

                    if (distance < 0) distance = -distance;

                    if (distance > (unitOwner.detectionRange + 10)) continue; //can only heal units in range (with slight extra margin of error.
                }

                HealingEffect instantiatedHealingEffect = (HealingEffect)healingEffect.Instantiate();

                instantiatedHealingEffect.characterThisEffectIsAttachedTo = friendlyTeamUnit;
                instantiatedHealingEffect.flipSpite = friendlyTeamUnit.movementSpeed >= 0 ? false : true;

                instantiatedHealingEffect.Name = unitOwner.uniqueID + "_InstantiatedStunEffect_" + lastUsedVisualEffectID;

                friendlyTeamUnit.HealDamage(GameManager.Instance.massHealerHealAmount);
                friendlyTeamUnit.AddChild(instantiatedHealingEffect);

                lastUsedVisualEffectID++;
            }

            processingHealingEffects = false;
        }

        public void SpawnBattlemageProjectile(BaseCharacter unitOwner)
        {
            BattlemageAge1ProjectilePhysics instantiatedProjectile = (BattlemageAge1ProjectilePhysics)battleMageAge1Projectile.Instantiate();

            instantiatedProjectile.attachedProjectileScript.projectileOwner = unitOwner.characterOwner;
            instantiatedProjectile.attachedProjectileScript.SetNewOwner(unitOwner.characterOwner);
            instantiatedProjectile.attachedProjectileScript.projectileOwnerChar = unitOwner;

            float offSetX = 30f;
            float addedXValue = unitOwner.movementSpeed >= 0 ? offSetX : -offSetX;
            float addedYValue = 2f; //make sure its on the ground.

            Vector2 fixedPosition = new Vector2(unitOwner.GlobalPosition.X + addedXValue, unitOwner.GlobalPosition.Y + addedYValue);
            instantiatedProjectile.GlobalPosition = fixedPosition;

            instantiatedProjectile.attachedProjectileScript.flipSpite = unitOwner.movementSpeed >= 0 ? false : true;

            instantiatedProjectile.Name = unitOwner.uniqueID + "_InstantiatedProjectile_" + lastUsedVisualEffectID;

            AddChild(instantiatedProjectile);

            lastUsedVisualEffectID++;
        }

        public void SpawnShamanProjectile(BaseCharacter unitOwner)
        {
            ShamanAge1ProjectilePhysics instantiatedProjectile = (ShamanAge1ProjectilePhysics)shamanAge1Projectile.Instantiate();

            instantiatedProjectile.attachedProjectileScript.projectileOwner = unitOwner.characterOwner;
            instantiatedProjectile.attachedProjectileScript.SetNewOwner(unitOwner.characterOwner);
            instantiatedProjectile.attachedProjectileScript.projectileOwnerChar = unitOwner;

            float offSetX = 30f;
            float addedXValue = unitOwner.movementSpeed >= 0 ? offSetX : -offSetX;
            float addedYValue = 2f; //make sure its on the ground.

            Vector2 fixedPosition = new Vector2(unitOwner.GlobalPosition.X + addedXValue, unitOwner.GlobalPosition.Y + addedYValue);
            instantiatedProjectile.GlobalPosition = fixedPosition;

            instantiatedProjectile.attachedProjectileScript.flipSpite = unitOwner.movementSpeed >= 0 ? false : true;

            instantiatedProjectile.Name = unitOwner.uniqueID + "_InstantiatedProjectile_" + lastUsedVisualEffectID;

            AddChild(instantiatedProjectile);

            lastUsedVisualEffectID++;
        }

        public void SpawnBattlemageFireball(BaseCharacter unitOwner)
        {
            BattlemageFireballLogic instantiatedFireball = (BattlemageFireballLogic)battleMageFireball.Instantiate();

            instantiatedFireball.fireballOwner = unitOwner.characterOwner;
            instantiatedFireball.characterThisEffectIsAttachedTo = unitOwner;

            float offSetX = 90f;
            float addedXValue = unitOwner.movementSpeed >= 0 ? offSetX : -offSetX;
            float addedYValue = 2f; //make sure its on the ground.

            Vector2 fixedPosition = new Vector2(unitOwner.GlobalPosition.X + addedXValue, unitOwner.GlobalPosition.Y + addedYValue);
            instantiatedFireball.GlobalPosition = fixedPosition;

            instantiatedFireball.flipSpite = unitOwner.movementSpeed >= 0 ? false : true;

            instantiatedFireball.Name = unitOwner.uniqueID + "_InstantiatedFireball_" + lastUsedVisualEffectID;

            AddChild(instantiatedFireball);

            lastUsedVisualEffectID++;
        }

        public void SpawnMeteorsAgeAbilityProjectiles(Enums.TeamOwner meteorShowerOwner)
        {
            //set it to the correct one for the team.
            int currentAmountOfMeteorsToSpawn = baseAmountOfMeteorsToSpawn + (meteorShowerOwner == Enums.TeamOwner.TEAM_01 ? team01AbilityEmpowerAmount : team02AbilityEmpowerAmount);

            for (int i = 0; i < currentAmountOfMeteorsToSpawn; i++)
            {
                IndividualMeteorLogic instantiatedMeteor = (IndividualMeteorLogic)meteorAbilyObjectPrefab.Instantiate();

                //position should be between:
                //x -> -904 (-904 + 900 -> -4)
                //x -> 2824 (2824 - 900 -> 1924)
                //y should be a random between 0 & 100

                instantiatedMeteor.meteorOwner = meteorShowerOwner;

                float randXValue = (float)(GD.Randi() % (4 + 1924));
                randXValue -= 4;
                float randYValue = (float)(GD.Randi() % (1000));
                randYValue -= 500;

                instantiatedMeteor.GlobalPosition = new Vector2(randXValue, randYValue);

                this.AddChild(instantiatedMeteor);
            }
        }

        public void SpawnMeteorImpactAtPosition(Vector2 impactPosition, Enums.TeamOwner meteorOwner)
        {
            MeteorImpactLogic instantiatedMeteorImpact = (MeteorImpactLogic)meteorImpactObjectPrefab.Instantiate();

            instantiatedMeteorImpact.meteorImpactOwner = meteorOwner;
            instantiatedMeteorImpact.GlobalPosition = impactPosition;

            CallDeferred("add_child", instantiatedMeteorImpact);

            //this.AddChild(instantiatedMeteorImpact);
        }

        public void SpawnDummyTakenDamageNumber(BaseCharacter unitOwner, int damageTaken)
        {
            FloatingDamageNumber instantiatedDamageNumber = (FloatingDamageNumber)trainingDummyFloatingDamage.Instantiate();

            instantiatedDamageNumber.SetHealthLabelValue(damageTaken);
            instantiatedDamageNumber.characterThisEffectIsAttachedTo = unitOwner;

            float addedXValue = (float)(GD.Randi() % 20);
            addedXValue -= 20;
            float addedYValue = -50f;

            Vector2 fixedPosition = new Vector2(unitOwner.GlobalPosition.X + addedXValue, unitOwner.GlobalPosition.Y + addedYValue);
            instantiatedDamageNumber.GlobalPosition = fixedPosition;

            instantiatedDamageNumber.Name = unitOwner.uniqueID + "_InstantiatedDamageNumber_" + lastUsedVisualEffectID;

            AddChild(instantiatedDamageNumber);

            lastUsedVisualEffectID++;
        }
    }
}
