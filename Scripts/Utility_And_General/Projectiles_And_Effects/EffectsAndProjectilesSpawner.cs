using Godot;
using System;
using System.Xml;
using static Godot.TextServer;

namespace Erikduss
{
	public partial class EffectsAndProjectilesSpawner : Node2D
	{
        public static EffectsAndProjectilesSpawner Instance { get; private set; }

        public PackedScene warriorAttackVisualEffect = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/Effets_And_Projectiles/SimpleSoldierShockwave.tscn");

        public PackedScene rangerAge1Projectile = GD.Load<PackedScene>("res://Scenes_Prefabs/Prefabs/Characters/Effets_And_Projectiles/RangerAge1Projectile.tscn");

        private int lastUsedVisualEffectID = 0;

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
	}
}
