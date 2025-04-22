using Erikduss;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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

            CheckAttackModeForHybridCharacters(character);

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

            if (character.currentAnimatedSprite.SpriteFrames.HasAnimation(currentAttackAnimationName))
            {
                character.currentAnimatedSprite.Play(currentAttackAnimationName);
            }
            else
            {
                character.currentAnimatedSprite.Play("Attack");
            }
        }

        public override void TickState(float delta, BaseCharacter character)
        {
            if (character.IsDeadOrDestroyed)
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
                else if(character.unitType == Enums.UnitTypes.Archdruid)
                {
                    //we also have a melee attack that should do damage with this.
                    if(currentAttackAnimationName == "Alternative_Attack")
                    {
                        character.DealDamage();
                    }
                }

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
                case Enums.UnitTypes.Assassin:
                    if ((attackDuration - attackTimer) < 0.1f && !executedEffect)
                    {
                        executedEffect = true;

                        //chance of applying a bleeding effect = 35 - enemy unit armor
                        //the more armor the unit has, the lower chance of bleeding.

                        if (character.CurrentTarget == null || character.CurrentTarget.IsDeadOrDestroyed) return;

                        int fixedNumber = GameSettingsLoader.Instance.assassinBleedApplyChance - character.CurrentTarget.unitArmor;
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

                        if (character.CurrentTarget == null || character.CurrentTarget.IsDeadOrDestroyed) return;

                        int fixedNumber = GameSettingsLoader.Instance.enforcerStunApplyChance - character.CurrentTarget.unitArmor;
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
                case Enums.UnitTypes.Archdruid:
                    if ((attackDuration - attackTimer) < 0.1f && !executedEffect)
                    {
                        executedEffect = true;

                        Archdruid currentDruidScript = (Archdruid)character;

                        GD.Print("Druid attack mode!");

                        if (!currentDruidScript.isTransforming)
                        {
                            if (!currentDruidScript.isTransformed)
                            {
                                currentDruidScript.TransformCombatMode();
                                return;
                            }
                        }
                        else if (currentDruidScript.isTransforming)
                        {
                            GD.Print("We are still transforming in attack state.");
                            return; //we dont wanna attack while transforming.
                        }

                        GD.Print("We passed all duid checks");

                        if(currentAttackAnimationName == "Attack")
                        {
                            IDamageable targetThatWeHit;
                            bool weCanActuallyHitIt = true;
                            float marginOfErrorValue = 5f; //this is in case a unit has moved or anything

                            if (currentDruidScript.CurrentTarget != null)
                            {
                                targetThatWeHit = currentDruidScript.CurrentTarget;

                                float distance = currentDruidScript.CurrentTarget.GlobalPosition.X - character.GlobalPosition.X;
                                if (distance < 0) distance = -distance; //normalize

                                if(distance > (currentDruidScript.detectionRange + marginOfErrorValue)) weCanActuallyHitIt = false;
                            }
                            else
                            {
                                HomeBaseManager baseThatWeHit = character.characterOwner == Enums.TeamOwner.TEAM_01 ? GameManager.Instance.team02HomeBase : GameManager.Instance.team01HomeBase;
                                targetThatWeHit = baseThatWeHit;

                                float distance = baseThatWeHit.GlobalPosition.X - character.GlobalPosition.X;
                                if (distance < 0) distance = -distance; //normalize

                                if (distance > (currentDruidScript.detectionRange + marginOfErrorValue)) weCanActuallyHitIt = false;
                            }

                            if(weCanActuallyHitIt && targetThatWeHit != null)
                            {
                                EffectsAndProjectilesSpawner.Instance.SpawnArchdruidRangedAttack(character, targetThatWeHit);
                            }
                            else
                            {
                                if (!weCanActuallyHitIt)
                                {
                                    GD.PrintErr("ERROR, The target of the druid's ranged attack is out of range");
                                }
                                if (targetThatWeHit != null)
                                {
                                    GD.PrintErr("ERROR, The target of the druid's ranged attack is null");
                                }


                                Dictionary<string, BaseCharacter> dictionaryToSearch = character.characterOwner == Enums.TeamOwner.TEAM_01 ? GameManager.Instance.unitsSpawner.team02AliveUnitDictionary : GameManager.Instance.unitsSpawner.team01AliveUnitDictionary;
                                if(dictionaryToSearch.Count > 0)
                                {
                                    BaseCharacter targetWeDoubleCheck = dictionaryToSearch.First().Value;
                                    float distance = targetWeDoubleCheck.GlobalPosition.X - character.GlobalPosition.X;

                                    if (distance < 0) distance = -distance; //normalize

                                    if (distance > (currentDruidScript.detectionRange + marginOfErrorValue))
                                    {
                                        GD.Print("We found a target close enough to damage anyways");
                                        EffectsAndProjectilesSpawner.Instance.SpawnArchdruidRangedAttack(character, targetThatWeHit);
                                    }
                                }
                            }
                        }
                    }
                    break;
                default:
                    GD.PrintErr("UNIT TYPE EFFECT NOT IMPLEMENTED, ATTACK STATE");
                    break;
            }
        }

        private void CheckAttackModeForHybridCharacters(BaseCharacter character)
        {
            //in this case we only need to do this for the archdruid

            if (character.unitType != Enums.UnitTypes.Archdruid) return; //add more to this if needed in the future

            if(character.unitType == Enums.UnitTypes.Archdruid)
            {
                Archdruid currentDruidScript = (Archdruid)character;

                //we can only do both melee and ranged attacks if we are transformed
                if (!currentDruidScript.isTransformed || currentDruidScript.isTransforming)
                {
                    //we need to prevent the druid from doing damage while transforming.
                    if (currentDruidScript.isTransforming)
                    {
                        unitHasAttacked = true;
                        playedAttackAnimation = true;
                    }

                    return;
                }

                if (currentDruidScript.CurrentTarget != null)
                {
                    float distance = character.CurrentTarget.GlobalPosition.X - character.GlobalPosition.X;
                    if (distance < 0) distance = -distance; //normalize

                    if (distance <= GameManager.unitStoppingDistance)
                    {
                        //we want to do a melee attack instead
                        currentAttackAnimationName = "Alternative_Attack";
                        attackDuration = 0.6f;
                    }
                    else
                    {
                        currentAttackAnimationName = "Attack";
                        attackDuration = 0.7f;
                    }
                }
                else
                {
                    //we are probably hitting the base
                    int enemyBasePosition = (int)(character.characterOwner == Enums.TeamOwner.TEAM_01 ? GameManager.Instance.team02HomeBase.GlobalPosition.X : GameManager.Instance.team01HomeBase.GlobalPosition.X);

                    float distance = enemyBasePosition - character.GlobalPosition.X;
                    if (distance < 0) distance = -distance; //normalize

                    if (distance <= GameManager.unitStoppingDistance)
                    {
                        //we want to do a melee attack instead
                        currentAttackAnimationName = "Alternative_Attack";
                        attackDuration = 0.6f;
                    }
                    else
                    {
                        currentAttackAnimationName = "Attack";
                        attackDuration = 0.7f;
                    }
                }
            }
        }
    }
}
