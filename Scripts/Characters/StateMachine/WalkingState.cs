using Godot;
using System;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

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
            if (character.isDead) return;

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

                if(!charBodyCheck)
                {
                    if (staticBodyCheck)
                    {
                        passedCheckForReachedEnemyBase = true;
                        character.unitHasReachedEnemyHomeBase = true;
                        
                        //also check if there is an enemy that spawned there, for example a ranger doesnt move and will never die otherwise.
                        //If there is a unit alive, we attack it instead of the home base.
                        if(character.characterOwner == Enums.TeamOwner.TEAM_01)
                        {
                            if (GameManager.Instance.unitsSpawner.team02AliveUnitDictionary.Count > 0)
                            {
                                character.currentTarget = GameManager.Instance.unitsSpawner.team02AliveUnitDictionary.First().Value;
                                EmitSignal(SignalName.Transitioned, this, "AttackState");
                                return;
                            }
                        }
                        else
                        {
                            if(GameManager.Instance.unitsSpawner.team01AliveUnitDictionary.Count > 0)
                            {
                                character.currentTarget = GameManager.Instance.unitsSpawner.team01AliveUnitDictionary.First().Value;
                                EmitSignal(SignalName.Transitioned, this, "AttackState");
                                return;
                            }
                        }

                        //We dont need to check if we can actually attack it from this range cus we already hit it wiht the raycast.
                        //we need to go to the idle state if we do have a cooldown and are close enough to the enemy base.
                        if (!character.canAttack)
                        {
                            EmitSignal(SignalName.Transitioned, this, "IdleState");
                            return;
                        }

                        //Switch to the new state
                        EmitSignal(SignalName.Transitioned, this, "AttackState");
                        return;

                        //we need to set the target to the enemy base
                        //we should probably do this by making the reachedEnemyBase variable public and checking this on the attack function and have a variable for the enemy base to deal damage to it.

                        //before we start attacking the enemy base, we still do need to be sure that there is no enemy unit standing at the base
                        //so we should probably move this to AFTER the enemy character check. Then we should check in the public list of units that are alive if there are any alive
                        //If the enemy does not have any units, we do attack the base.

                        //we need to switch to attack state now

                    }
                    else
                    {
                        GD.PrintErr("COLLISION OF UNIT IS WITH SOMETHING ELSE THAN STATICBODY2D OR CHARACTERBODY2D");
                    }
                }

                if(!passedCheckForReachedEnemyBase)
                {
                    CharacterBody2D enemyCharacterBody2D = output.As<CharacterBody2D>();

                    if (enemyCharacterBody2D != null)
                    {
                        float distance = enemyCharacterBody2D.GlobalPosition.X - character.GlobalPosition.X;
                        if (distance < 0) distance = -distance;

                        BaseCharacter enemyChar = enemyCharacterBody2D.GetNode<BaseCharacter>(enemyCharacterBody2D.GetPath());

                        if (enemyChar.characterOwner == character.characterOwner)
                        {
                            //GD.Print("Dist to friendly: " + distance);

                            //Friendly stop distance is a bit bigger than the stopping distance with the enemy.
                            if (distance < 42) //chose a number due to the ranged units having a bigger attack range, but they dont have to stop further away from friendly units.
                            {
                                //character.currentTarget = enemyChar;

                                if(CheckRangedCharacterTarget(character, character.characterOwner))
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
                        else
                        {
                            //GD.Print("Dist to enemy: " + distance);

                            character.currentTarget = enemyChar;

                            //we need to go to the idle state if we do have a cooldown and are close enough to the enemy.
                            if(character.isRangedCharacter && !character.canAttack)
                            {
                                EmitSignal(SignalName.Transitioned, this, "IdleState");
                                return;
                            }

                            //Switch to the new state
                            EmitSignal(SignalName.Transitioned, this, "AttackState");
                            return;

                        }
                    }
                }
                else
                {
                    StaticBody2D baseBody2D = output.As<StaticBody2D>();
                }
            }

            #endregion

            //character.characterBody.Velocity = new Vector2(character.movementSpeed, 0) * delta;
            //character.characterBody.MoveAndSlide();
            character.MoveAndCollide(new Vector2(character.movementSpeed, 0) * delta);
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

                character.currentTarget = oppositeTeamUnit;

                return true;
            }

            return false;
        }
    }
}
