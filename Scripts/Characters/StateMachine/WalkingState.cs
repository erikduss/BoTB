using Godot;
using System;
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
        public uint raycastCollisionMask = 0b10;


        public override void StateEnter(BaseCharacter character)
        {
            base.StateEnter(character);

            if(character.characterOwner == Enums.TeamOwner.TEAM_01)
            {
                raycastCollisionMask = 0b10;
            }
            else
            {
                raycastCollisionMask = 0b100;
            }

            character.currentAnimatedSprite.Play("Walking");
        }

        public override void StateExit(BaseCharacter character)
        {
            base.StateExit(character);

            //reset any needed values
        }

        public override void TickState(float delta, BaseCharacter character)
        {
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

            var query = PhysicsRayQueryParameters2D.Create(character.GlobalPosition, raycastDetectPosition);
            //GD.Print("Start: " + character.GlobalPosition + " _ End: " + raycastDetectPosition);
            var result = spaceState.IntersectRay(query);

            if (result.Count > 0)
            {
                result.TryGetValue("collider", out Godot.Variant output);

                CharacterBody2D enemyCharacterBody2D = output.As<CharacterBody2D>();

                if(enemyCharacterBody2D != null)
                {
                    float distance = enemyCharacterBody2D.GlobalPosition.X - character.GlobalPosition.X;
                    if (distance < 0) distance = -distance;

                    BaseCharacter enemyChar = enemyCharacterBody2D.GetNode<BaseCharacter>(enemyCharacterBody2D.GetPath());

                    if(enemyChar.characterOwner == character.characterOwner)
                    {
                        GD.Print("Dist to friendly: " + distance);

                        if(distance < 50f)
                        {
                            character.currentTarget = enemyChar;

                            //Switch to the new state
                            EmitSignal(SignalName.Transitioned, this, "IdleState");
                        }
                    }
                    else
                    {
                        GD.Print("Dist to enemy: " + distance);

                        character.currentTarget = enemyChar;

                        //Switch to the new state
                        EmitSignal(SignalName.Transitioned, this, "IdleState");

                    }
                }
            }

            #endregion

            //character.characterBody.Velocity = new Vector2(character.movementSpeed, 0) * delta;
            //character.characterBody.MoveAndSlide();
            character.MoveAndCollide(new Vector2(character.movementSpeed, 0) * delta);
        }
    }
}
