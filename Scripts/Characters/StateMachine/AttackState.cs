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
        private bool playedAttackAnimation = false;

        private string currentAttackAnimationName = "Attack";

        public override void StateEnter(BaseCharacter character)
        {
            base.StateEnter(character);

            playedAttackAnimation = false;
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
            playedAttackAnimation = false;
        }

        private void PlayAttackAnimation(BaseCharacter character)
        {
            playedAttackAnimation = true;
            character.currentAnimatedSprite.Play(currentAttackAnimationName);
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

            if (!playedAttackAnimation) PlayAttackAnimation(character);

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


                    if (character.currentAttackCooldownDuration < 0)
                    {
                        character.SetNewAttackCooldownTimer();
                    }
                    else
                    {
                        character.SetNewAttackCooldownTimer(character.currentAttackCooldownDuration);
                    }

                    EmitSignal(SignalName.Transitioned, this, "WalkingState");
                    return;
                }

                if (character.currentAttackCooldownDuration < 0)
                {
                    character.SetNewAttackCooldownTimer();
                }
                else
                {
                    character.SetNewAttackCooldownTimer(character.currentAttackCooldownDuration);
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
                case Enums.UnitTypes.Asssassin:
                    if ((attackDuration - attackTimer) < 0.1f && !executedEffect)
                    {
                        executedEffect = true;

                        //chance of applying a bleeding effect = 35 - enemy unit armor
                        //the more armor the unit has, the lower chance of bleeding.

                        if (character.currentTarget == null || character.currentTarget.isDead) return;

                        int fixedNumber = GameSettingsLoader.Instance.assassinBleedApplyChance - character.currentTarget.unitArmor;
                        int randChance = (int)(GD.Randi() % (100));

                        if(randChance <= fixedNumber)
                        {
                            EffectsAndProjectilesSpawner.Instance.SpawnAssassinBleedingEffect(character);
                        }
                    }
                    break;
                case Enums.UnitTypes.Enforcer:
                    if ((attackDuration - attackTimer) < 0.1f && !executedEffect)
                    {
                        executedEffect = true;

                        //chance of applying a stun effect = 70 - enemy unit armor
                        //the more armor the unit has, the lower chance of bleeding.

                        if (character.currentTarget == null || character.currentTarget.isDead) return;

                        int fixedNumber = GameSettingsLoader.Instance.enforcerStunApplyChance - character.currentTarget.unitArmor;
                        int randChance = (int)(GD.Randi() % (100));

                        if (randChance <= fixedNumber)
                        {
                            EffectsAndProjectilesSpawner.Instance.SpawnEnforcerStunEffect(character);
                        }
                    }
                    break;
                case Enums.UnitTypes.Tank:
                    if ((attackDuration - attackTimer) < 0.1f && !executedEffect)
                    {
                        executedEffect = true;

                        //chance of applying a Buff effect = 70

                        int fixedNumber = GameSettingsLoader.Instance.tankBuffApplyChance;
                        int randChance = (int)(GD.Randi() % (100));

                        if (randChance <= fixedNumber)
                        {
                            EffectsAndProjectilesSpawner.Instance.SpawnTankBuffEffect(character);
                        }
                    }
                    break;
                case Enums.UnitTypes.Battlemage:
                    if ((attackDuration - attackTimer) < 0.1f && !executedEffect)
                    {
                        executedEffect = true;

                        //Every 4th attack we need to instead do a powered up attack.

                        Battlemage bmScript = (Battlemage)character;

                        bmScript.amountOfBasicAttacksPerformed++;

                        if(bmScript.amountOfBasicAttacksPerformed == 3)
                        {
                            currentAttackAnimationName = "Alternative_Attack";
                        }
                        else if(bmScript.amountOfBasicAttacksPerformed > 3)
                        {
                            bmScript.amountOfBasicAttacksPerformed = 0;
                            currentAttackAnimationName = "Attack";

                            //here we need to spawn the fireball projectile
                            EffectsAndProjectilesSpawner.Instance.SpawnBattlemageFireball(character);

                            return;
                        }

                        EffectsAndProjectilesSpawner.Instance.SpawnBattlemageProjectile(character);
                    }
                    break;
                case Enums.UnitTypes.Mass_Healer:
                    if ((attackDuration - attackTimer) < 0.1f && !executedEffect)
                    {
                        executedEffect = true;

                        EffectsAndProjectilesSpawner.Instance.SpawnMass_Healer_HealingEffect(character);
                    }
                    break;
                case Enums.UnitTypes.Shaman:
                    if ((attackDuration - attackTimer) < 0.1f && !executedEffect)
                    {
                        executedEffect = true;
                        EffectsAndProjectilesSpawner.Instance.SpawnShamanProjectile(character);
                    }
                    break;
                default:
                    GD.PrintErr("UNIT TYPE EFFECT NOT IMPLEMENTED, ATTACK STATE");
                    break;
            }
        }
    }
}
