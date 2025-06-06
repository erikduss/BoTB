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

        public PackedScene archdruidRangedEffect = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/Effets_And_Projectiles/ArchdruidRangedEffect.tscn");
        public PackedScene archdruidRangedImpact = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/Effets_And_Projectiles/ArchdruidRanged_Impact.tscn");
        #endregion

        #region Age Abilities And Effects

        public int baseAmountOfMeteorsToSpawn = 10; //this will be loaded in elsewere.
        public int team01AbilityEmpowerAmount = 0;
        public int team02AbilityEmpowerAmount = 0;

        public PackedScene meteorAbilyObjectPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Spawnable_Objects/Age01_Ability_Meteors/basic_Meteor.tscn");
        public PackedScene meteorImpactObjectPrefab = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Spawnable_Objects/Age01_Ability_Meteors/Age01_Meteor_Impact.tscn");

        #endregion

        private int lastUsedVisualEffectID = 0;

        public PackedScene floatingDamageNumber = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Spawnable_Objects/FloatingDamageNumber.tscn");

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

            instantiatedBleedingEffect.characterThisEffectIsAttachedTo = unitOwner.CurrentTarget;
            instantiatedBleedingEffect.unitThisIsDamaging = unitOwner.CurrentTarget;

            instantiatedBleedingEffect.flipSpite = unitOwner.CurrentTarget.movementSpeed >= 0 ? false : true;

            instantiatedBleedingEffect.Name = unitOwner.uniqueID + "_InstantiatedBleedEffect_" + lastUsedVisualEffectID;

            unitOwner.CurrentTarget.AddChild(instantiatedBleedingEffect);

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

            instantiatedStunEffect.characterThisEffectIsAttachedTo = unitOwner.CurrentTarget;
            instantiatedStunEffect.flipSpite = unitOwner.CurrentTarget.movementSpeed >= 0 ? false : true;

            instantiatedStunEffect.Name = unitOwner.uniqueID + "_InstantiatedStunEffect_" + lastUsedVisualEffectID;

            unitOwner.CurrentTarget.ApplyStunEffect();
            unitOwner.CurrentTarget.AddChild(instantiatedStunEffect);

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
                if (friendlyTeamUnit.IsDeadOrDestroyed) continue;
                if (friendlyTeamUnit.CurrentHealth == friendlyTeamUnit.MaxHealth) continue; //this could possibly happen with a double heal, we want to prevent adding the icon twice.

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

                friendlyTeamUnit.HealDamage(GameManager.massHealerHealAmount);
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

        public void SpawnArchdruidRangedAttack(BaseCharacter unitOwner, IDamageable target)
        {
            ArchdruidRangedEffect instantiatedEffect = (ArchdruidRangedEffect)archdruidRangedEffect.Instantiate();

            instantiatedEffect.characterThisEffectIsAttachedTo = unitOwner;
            instantiatedEffect.targetThatWeHit = target;

            float addedXValue = 0;
            float addedYValue = 2f; //make sure its on the ground.

            Vector2 fixedPosition = new Vector2(unitOwner.GlobalPosition.X + addedXValue, unitOwner.GlobalPosition.Y + addedYValue);
            instantiatedEffect.GlobalPosition = fixedPosition;

            instantiatedEffect.flipSpite = unitOwner.movementSpeed >= 0 ? false : true;

            instantiatedEffect.Name = unitOwner.uniqueID + "_InstantiatedEffect_" + lastUsedVisualEffectID;

            AddChild(instantiatedEffect);

            lastUsedVisualEffectID++;
        }

        public void SpawnArchdruidRangedAttackImpact(BaseCharacter unitOwner, Vector2 position)
        {
            ArchdruidRangedEffectImpact instantiatedEffect = (ArchdruidRangedEffectImpact)archdruidRangedImpact.Instantiate();

            float addedXValue = 0;
            float addedYValue = 2f; //make sure its on the ground.

            Vector2 fixedPosition = new Vector2(position.X + addedXValue, GameManager.unitGroundYPosition);
            instantiatedEffect.GlobalPosition = fixedPosition;

            instantiatedEffect.flipSpite = unitOwner.movementSpeed >= 0 ? false : true;

            instantiatedEffect.Name = unitOwner.uniqueID + "_InstantiatedEffect_" + lastUsedVisualEffectID;

            AddChild(instantiatedEffect);

            lastUsedVisualEffectID++;
        }

        public void SpawnMeteorsAgeAbilityProjectiles(Enums.TeamOwner meteorShowerOwner)
        {
            //set it to the correct one for the team.
            int currentAmountOfMeteorsToSpawn = baseAmountOfMeteorsToSpawn + (meteorShowerOwner == Enums.TeamOwner.TEAM_01 ? team01AbilityEmpowerAmount : team02AbilityEmpowerAmount);

            GameManager.Instance.UpdatePlayerPowerUpProgress(meteorShowerOwner, GameSettingsLoader.powerUpProgressAmountAbilityUsed);

            for (int i = 0; i < currentAmountOfMeteorsToSpawn; i++)
            {
                IndividualMeteorLogic instantiatedMeteor = (IndividualMeteorLogic)meteorAbilyObjectPrefab.Instantiate();

                //set the meteor's velocity and decide if this is a vertical or diagonal meteor.
                float xVelocity = (float)(GD.Randi() % (500f));
                xVelocity -= 250f;
                instantiatedMeteor.projectileVelocity = xVelocity;

                if (Mathf.Abs(xVelocity) > 150)
                {
                    instantiatedMeteor.isDiagonalMeteor = true;
                }

                instantiatedMeteor.meteorOwner = meteorShowerOwner;

                #region old spawning positions
                //position should be between:
                //x -> -904 (-904 + 900 -> -4)
                //x -> 2824 (2824 - 900 -> 1924)
                //y should be a random between 0 & 100

                //updated map size values
                //Seems like the meteors can travel about 900 pixels?
                //x -> -300 (-300 + 900 -> 600)
                //x -> 1220 (1220 - 900 -> 320)

                //float randXValue = (float)(GD.Randi() % (4 + 1924));
                //randXValue -= 4;
                //float randYValue = (float)(GD.Randi() % (1000));
                //randYValue -= 500;
                #endregion


                //if the meteor goes straight down, it can be between: 0 and 1920

                float randXValue = (float)(GD.Randi() % (920));
                float randYValue = (float)(GD.Randi() % (1000));
                randYValue -= 500;

                randXValue += 500f;

                //GD.Print("Rand X: " + randXValue + " _ " + xVelocity);
                float xPosMultiplier = (randYValue / 1000f);

                if(xPosMultiplier < 0) xPosMultiplier = -xPosMultiplier;

                xPosMultiplier += 1f;
                //if(xPosMultiplier > 1.1f) xPosMultiplier = 1.1f;

                //if(xVelocity > 0)
                //{
                //    randXValue += 768;
                //}
                //else
                //{
                //    randXValue -= 768;
                //}

                float addedValue = xVelocity * xPosMultiplier;

                randXValue -= addedValue; //this makes it so it can come from outside of the map into the map.

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

        public void SpawnFloatingDamageNumber(BaseCharacter unitOwner, int damageTaken, bool isHealingDamage = false)
        {
            FloatingDamageNumber instantiatedDamageNumber = (FloatingDamageNumber)floatingDamageNumber.Instantiate();

            if (isHealingDamage)
            {
                instantiatedDamageNumber.isHealingDamageInstead = true;
            }

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

        public void SpawnHomeBaseFloatingDamageNumber(HomeBaseManager baseOwner, int damageTaken)
        {
            FloatingDamageNumber instantiatedDamageNumber = (FloatingDamageNumber)floatingDamageNumber.Instantiate();

            instantiatedDamageNumber.SetHealthLabelValue(damageTaken);

            float addedXValue = (float)(GD.Randi() % 20);
            addedXValue -= 20;
            float addedYValue = -100f;

            Vector2 fixedPosition = new Vector2(baseOwner.GlobalPosition.X + addedXValue, baseOwner.GlobalPosition.Y + addedYValue);
            instantiatedDamageNumber.GlobalPosition = fixedPosition;

            instantiatedDamageNumber.Name = "InstantiatedBaseDamageNumber_" + lastUsedVisualEffectID;

            AddChild(instantiatedDamageNumber);

            lastUsedVisualEffectID++;
        }
    }
}
