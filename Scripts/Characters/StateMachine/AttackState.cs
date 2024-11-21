using Erikduss;
using Godot;
using System;

namespace Erikduss
{
	public partial class AttackState : State
	{
        [Export] public float attackDuration = 1f;
        [Export] public float deathAttackThreshold = 0.2f;

        public float attackTimer = 0f;
        private bool enableTimer = false;
        private bool unitHasAttacked = false;

        private bool executedEffect = false;

        public override void StateEnter(BaseCharacter character)
        {
            base.StateEnter(character);

            character.currentAnimatedSprite.Play("Attack");

            enableTimer = true;
        }

        public override void StateExit(BaseCharacter character)
        {
            base.StateExit(character);

            //reset any needed values
            enableTimer = false;
            attackTimer = 0f;
            unitHasAttacked = false;
            executedEffect = false;
        }

        public override void TickState(float delta, BaseCharacter character)
        {
            if (character.isDead)
            {
                if (unitHasAttacked) return;

                //GD.Print("Unit has died with: " + (attackDuration - attackTimer) + " Time left");

                if(attackDuration - attackTimer <= deathAttackThreshold)
                {
                    unitHasAttacked = true;

                    character.DealDamage();
                }

                return;
            }

            base.TickState(delta, character);

            if (!enableTimer) return;

            attackTimer += delta;

            ExecuteUnitProjectileAndVisualEffects(character);

            if (attackTimer > attackDuration)
            {
                if (unitHasAttacked) return;

                unitHasAttacked = true;

                //we deal damage through the projectile
                if(!character.isRangedCharacter) character.DealDamage();

                if (character.isRangedCharacter)
                {
                    if(character.unitType == Enums.UnitTypes.Ranger)
                    {
                        if(character.currentAttackCooldownDuration < 0)
                        {
                            character.SetNewAttackCooldownTimer();
                            EmitSignal(SignalName.Transitioned, this, "WalkingState");
                        }
                        else
                        {
                            //we attack a lot faster now, but we cant move.
                            character.SetNewAttackCooldownTimer(character.currentAttackCooldownDuration);
                            EmitSignal(SignalName.Transitioned, this, "IdleState");
                        }
                        return;
                    }


                    character.SetNewAttackCooldownTimer();
                    EmitSignal(SignalName.Transitioned, this, "WalkingState");
                    return;
                }

                //Switch to the new state
                EmitSignal(SignalName.Transitioned, this, "IdleState");
            }
        }

        public override void PhysicsTickState(float delta, BaseCharacter character)
        {
            base.PhysicsTickState(delta, character);
        }

        private void ExecuteUnitProjectileAndVisualEffects(BaseCharacter character)
        {
            switch (character.unitType)
            {
                case Enums.UnitTypes.Warrior:
                    if((attackDuration - attackTimer) < 0.1f && !executedEffect)
                    {
                        executedEffect = true;
                        EffectsAndProjectilesSpawner.Instance.SpawnWarriorShockwave(character, 1);
                        EffectsAndProjectilesSpawner.Instance.SpawnWarriorShockwave(character, 2);
                    }
                    break;
                case Enums.UnitTypes.Ranger:
                    if ((attackDuration - attackTimer) < 0.1f && !executedEffect)
                    {
                        executedEffect = true;
                        EffectsAndProjectilesSpawner.Instance.SpawnRangerProjectile(character);
                    }
                    break;
                default:
                    GD.PrintErr("UNIT TYPE EFFECT NOT IMPLEMENTED, ATTACK STATE");
                    break;
            }
        }
    }
}
