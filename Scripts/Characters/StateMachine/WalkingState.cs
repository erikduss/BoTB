using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Erikduss
{
	public partial class WalkingState : State
	{
        //probably need a reference to the animation that needs to play.

        /* 
         * In the idle state we shouldnt do much, the idle state should only be reached after we attacked.
         * The only function of idling would be to wait for your attack cooldown.
         */

        //if in team 1 we only need to check for layer 3 (bit 2) otherwise check for layer 2 (bit 1)
        public uint raycastCollisionMask = 0b1100110;


        public override void StateEnter(BaseCharacter character)
        {
            base.StateEnter(character);

            raycastCollisionMask = character.GetCollisionMask();

            //if(character.characterOwner == Enums.TeamOwner.TEAM_01)
            //{
            //    raycastCollisionMask = 0b10;
            //}
            //else
            //{
            //    raycastCollisionMask = 0b100;
            //}

            character.currentAnimatedSprite.Play("Walking");
        }

        public override void StateExit(BaseCharacter character)
        {
            base.StateExit(character);

            //reset any needed values
        }

        public override void TickState(float delta, BaseCharacter character)
        {
            if (character.IsDeadOrDestroyed) return;

            base.TickState(delta, character);

            /*if (!enableTimer) return;

            idleTimer += delta;

            if (idleTimer > idleDuration)
            {
                //Switch to the new state
                EmitSignal(SignalName.Transitioned, this, "WalkingState");
            }*/
        }

        public override void PhysicsTickState(float delta, BaseCharacter character)
        {
            base.PhysicsTickState(delta, character);

            #region Raycast

            var spaceState = character.GetWorld2D().DirectSpaceState;

            //movement speed is either - or +, this determines the direction of the raycast.
            float direction = character.movementSpeed >= 0 ? 1 : -1;

            //The final raycast position based on detectionrange
            Vector2 raycastDetectPosition = new Vector2(character.GlobalPosition.X + (direction * character.detectionRange), character.GlobalPosition.Y);

            // Binary - set the bit corresponding to the layers you want to enable (1, 3, and 4) to 1, set all other bits to 0.
            // Note: Layer 32 is the first bit, layer 1 is the last. The mask for layers 4,3 and 1 is therefore
            // (This can be shortened to 0b1101)

            var query = PhysicsRayQueryParameters2D.Create(character.GlobalPosition, raycastDetectPosition, raycastCollisionMask);
            //GD.Print("Start: " + character.GlobalPosition + " _ End: " + raycastDetectPosition);
            var result = spaceState.IntersectRay(query);

            if (result.Count > 0)
            {
                result.TryGetValue("collider", out Godot.Variant output);

                //we check if the output as characterbody is valid
                /*
                 * if it is, than we have hit another character
                 * if it isnt than either we hit the enemy base or there's something wrong
                 * So now we check if the output is a staticbody2d to check for enemy base collision
                 */
                bool passedCheckForReachedEnemyBase = false;

                //Casting gives errors due to this being able to be 2 different components. So we solve this by doing a name string check.
                string outputString = output.ToString();

                bool charBodyCheck = outputString.Contains("CharacterBody2D");
                bool staticBodyCheck = outputString.Contains("StaticBody2D");

                //We are not detecting a character collision.
                if(!charBodyCheck)
                {
                    if (staticBodyCheck) //which means we collide with a static body, which is the enemy base.
                    {
                        passedCheckForReachedEnemyBase = true;
                        character.unitHasReachedEnemyHomeBase = true;

                        if(!character.checkForAlliesRaycastInstead && character.unitAttackDamage != 0)
                        {
                            //also check if there is an enemy that spawned there, for example a ranger doesnt move and will never die otherwise.
                            //If there is a unit alive, we attack it instead of the home base.
                            if (character.characterOwner == Enums.TeamOwner.TEAM_01)
                            {
                                if (GameManager.Instance.unitsSpawner.team02AliveUnitDictionary.Count > 0)
                                {
                                    character.CurrentTarget = GameManager.Instance.unitsSpawner.team02AliveUnitDictionary.First().Value;
                                    if (character.canAttack)
                                    {
                                        EmitSignal(SignalName.Transitioned, this, "AttackState");
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                if (GameManager.Instance.unitsSpawner.team01AliveUnitDictionary.Count > 0)
                                {
                                    character.CurrentTarget = GameManager.Instance.unitsSpawner.team01AliveUnitDictionary.First().Value;
                                    if (character.canAttack)
                                    {
                                        EmitSignal(SignalName.Transitioned, this, "AttackState");
                                        return;
                                    }
                                }
                            }

                            StaticBody2D baseStaticBody2D = output.As<StaticBody2D>();
                            float distance = baseStaticBody2D.GlobalPosition.X - character.GlobalPosition.X;
                            if (distance < 0) distance = -distance;

                            //We dont need to check if we can actually attack it from this range cus we already hit it wiht the raycast.
                            //we need to go to the idle state if we do have a cooldown and are close enough to the enemy base.
                            if (!character.canAttack && distance < (GameManager.unitStoppingDistance + 5))
                            {
                                EmitSignal(SignalName.Transitioned, this, "IdleState");
                                return;
                            }
                            else if (character.canAttack)
                            {
                                //Switch to the new state
                                EmitSignal(SignalName.Transitioned, this, "AttackState");
                                return;
                            }
                        }
                        else
                        {
                            if(CheckForCharactersToHeal(character, character.characterOwner))
                            {                          
                                if (character.canAttack)
                                {
                                    //if we find a character that needs to be healed within range, and we can heal atm, we should
                                    EmitSignal(SignalName.Transitioned, this, "AttackState");
                                    return;
                                }
                            }

                            StaticBody2D baseStaticBody2D = output.As<StaticBody2D>();
                            float distance = baseStaticBody2D.GlobalPosition.X - character.GlobalPosition.X;
                            if (distance < 0) distance = -distance;

                            //we still need to check if we are close enough to the base to stop.
                            if (!character.canAttack && distance < (GameManager.unitStoppingDistance + 5))
                            {
                                EmitSignal(SignalName.Transitioned, this, "IdleState");
                                return;
                            }
                        }
                    }
                    else
                    {
                        GD.PrintErr("COLLISION OF UNIT IS WITH SOMETHING ELSE THAN STATICBODY2D OR CHARACTERBODY2D");
                    }
                }

                //if instead of a home base, we hit a character (can be friendly and enemy)
                if(!passedCheckForReachedEnemyBase)
                {
                    CharacterBody2D enemyCharacterBody2D = output.As<CharacterBody2D>();

                    if (enemyCharacterBody2D != null)
                    {
                        float distance = enemyCharacterBody2D.GlobalPosition.X - character.GlobalPosition.X;
                        if (distance < 0) distance = -distance;

                        BaseCharacter enemyChar = enemyCharacterBody2D.GetNode<BaseCharacter>(enemyCharacterBody2D.GetPath());

                        //we are colliding with an ally.
                        if (enemyChar.characterOwner == character.characterOwner)
                        {
                            if(!character.checkForAlliesRaycastInstead && character.unitAttackDamage > 0)
                            {
                                //Friendly stop distance is a bit bigger than the stopping distance with the enemy.
                                if (distance < GameManager.unitStoppingDistance) //chose a number due to the ranged units having a bigger attack range, but they dont have to stop further away from friendly units.
                                {
                                    if (CheckRangedCharacterTarget(character, character.characterOwner))
                                    {
                                        if (character.canAttack)
                                        {
                                            EmitSignal(SignalName.Transitioned, this, "AttackState");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        //Switch to the new state
                                        EmitSignal(SignalName.Transitioned, this, "IdleState");
                                        return;
                                    }
                                }
                                else if (character.isRangedCharacter && character.canAttack)
                                {
                                    if (CheckRangedCharacterTarget(character, character.characterOwner))
                                    {
                                        EmitSignal(SignalName.Transitioned, this, "AttackState");
                                        return;
                                    }
                                }
                            }
                            else //if we heal instead, we need to check for targets to heal within range, otherwise we just check for stopping.
                            {
                                if (CheckForCharactersToHeal(character, character.characterOwner))
                                {
                                    if (character.canAttack)
                                    {
                                        //if we find a character that needs to be healed within range, and we can heal atm, we should
                                        EmitSignal(SignalName.Transitioned, this, "AttackState");
                                        return;
                                    }
                                }

                                if (distance < GameManager.unitStoppingDistance)
                                {
                                    EmitSignal(SignalName.Transitioned, this, "IdleState");
                                    return;
                                }
                            }
                        }
                        else
                        {
                            character.CurrentTarget = enemyChar;

                            //we need to go to the idle state if we do have a cooldown and are close enough to the enemy.
                            if (!character.canAttack && distance <= GameManager.unitStoppingDistance)
                            {
                                EmitSignal(SignalName.Transitioned, this, "IdleState");
                                return;
                            }
                            else if (character.canAttack)
                            {
                                if(character.isRangedCharacter || distance <= GameManager.unitStoppingDistance)
                                {
                                    //Switch to the new state
                                    EmitSignal(SignalName.Transitioned, this, "AttackState");
                                    return;
                                }
                            }
                            else if (!character.canMove)
                            {
                                EmitSignal(SignalName.Transitioned, this, "IdleState");
                                return;
                            }
                        }
                    }
                }
                else
                {
                    StaticBody2D baseBody2D = output.As<StaticBody2D>();
                }
            }

            #endregion

            if (CheckDruidNeedToTransform(character))
            {
                Archdruid druid = (Archdruid)character;

                if (druid.isTransformed)
                {
                    if (!druid.isTransforming)
                    {
                        druid.TransformBack();
                    }
                    return;
                }
            }

            //character.characterBody.Velocity = new Vector2(character.movementSpeed, 0) * delta;
            //character.characterBody.MoveAndSlide();
            character.MoveAndCollide(new Vector2(character.movementSpeed, 0) * delta);
        }

        private bool CheckForCharactersToHeal(BaseCharacter character, Enums.TeamOwner team)
        {
            if (!character.isRangedCharacter) return false;

            List<BaseCharacter> listToSearch;

            if (team == Enums.TeamOwner.TEAM_01)
            {
                listToSearch = GameManager.Instance.unitsSpawner.team01DamagedUnits;
            }
            else
            {
                listToSearch = GameManager.Instance.unitsSpawner.team02DamagedUnits;
            }

            foreach (BaseCharacter frienlyUnit in listToSearch)
            {
                float distance = frienlyUnit.GlobalPosition.X - character.GlobalPosition.X;

                if (distance < 0) distance = -distance;

                if (distance > character.detectionRange) continue; //chose 144 due to characters being 64x64, plus keeping some margin of error.

                if (frienlyUnit.CurrentHealth >= frienlyUnit.MaxHealth) continue;

                return true;
            }

            return false;
        }

        private bool CheckRangedCharacterTarget(BaseCharacter character, Enums.TeamOwner team)
        {
            //We need to also check to see if we can attack the base!

            if (!character.isRangedCharacter) return false;

            System.Collections.Generic.Dictionary<string, BaseCharacter> dictionaryToSearch;

            if (team == Enums.TeamOwner.TEAM_01)
            {
                dictionaryToSearch = GameManager.Instance.unitsSpawner.team02AliveUnitDictionary;
            }
            else
            {
                dictionaryToSearch = GameManager.Instance.unitsSpawner.team01AliveUnitDictionary;
            }


            bool setNewRangedTarget = false;
            BaseCharacter newTarget = null;
            BaseCharacter backupTarget = null; //in case the main target dies before it sets this new one.
            float closestUnitDistance = 1000;

            foreach (BaseCharacter oppositeTeamUnit in dictionaryToSearch.Values)
            {
                float distance = oppositeTeamUnit.GlobalPosition.X - character.GlobalPosition.X;

                if (distance < 0) distance = -distance;

                if (distance > character.detectionRange) continue; //chose 144 due to characters being 64x64, plus keeping some margin of error.

                if (team == Enums.TeamOwner.TEAM_01)
                {
                    //This should not be possible, but we check if the unit is behind the unit or not.
                    if (oppositeTeamUnit.GlobalPosition.X < character.GlobalPosition.X) continue;
                }
                else
                {
                    if (oppositeTeamUnit.GlobalPosition.X > character.GlobalPosition.X) continue;
                }

                setNewRangedTarget = true;

                if(distance < closestUnitDistance)
                {
                    backupTarget = newTarget;
                    closestUnitDistance = distance;
                    newTarget = oppositeTeamUnit;
                }
            }

            if (setNewRangedTarget)
            {
                if(newTarget != null && !newTarget.IsDeadOrDestroyed)
                {
                    character.CurrentTarget = newTarget;
                }
                else
                {
                    character.CurrentTarget = backupTarget;
                }

                return true;
            }

            //we need to check if we can hit the enemy base
            if(team == Enums.TeamOwner.TEAM_01)
            {
                float distance = GameManager.Instance.team02HomeBase.GlobalPosition.X - character.GlobalPosition.X;

                if (distance < 0) distance = -distance;

                if(distance < (character.detectionRange))
                {
                    character.unitHasReachedEnemyHomeBase = true;
                    return true;
                }
            }
            else
            {
                float distance = GameManager.Instance.team01HomeBase.GlobalPosition.X - character.GlobalPosition.X;

                if (distance < 0) distance = -distance;

                if (distance < (character.detectionRange))
                {
                    character.unitHasReachedEnemyHomeBase = true;
                    return true;
                }
            }

            return false;
        }

        private bool CheckDruidNeedToTransform(BaseCharacter character)
        {
            if (character.unitType != Enums.UnitTypes.Archdruid) return false;

            Dictionary<string, BaseCharacter> dictionaryToCheck = character.characterOwner == Enums.TeamOwner.TEAM_01 ? GameManager.Instance.unitsSpawner.team02AliveUnitDictionary : GameManager.Instance.unitsSpawner.team01AliveUnitDictionary;

            if (dictionaryToCheck.Count > 0)
            {
                BaseCharacter characterToCheck = dictionaryToCheck.First().Value;

                float distance = characterToCheck.GlobalPosition.X - character.GlobalPosition.X;

                if (distance < 0) distance = -distance;

                //if there's an enemy close enough, we dont transform back.
                if (distance < (character.detectionRange + 15)) return false;
            }

            //We dont want to transform when we still have an attack cooldown. This is cus we move during this period.
            if(!character.canAttack) return false;

            return true;
        }
    }
}
