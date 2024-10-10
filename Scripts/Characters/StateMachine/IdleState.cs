using Godot;
using System;

namespace Erikduss
{
    public partial class IdleState : State
    {
        [Export] public float idleDuration = 1f;

        public float idleTimer = 0f;
        private bool enableTimer = false;

        //probably need a reference to the animation that needs to play.

        /* 
         * In the idle state we shouldnt do much, the idle state should only be reached after we attacked.
         * The only function of idling would be to wait for your attack cooldown.
         */

        public override void StateEnter(BaseCharacter character)
        {
            base.StateEnter(character);

            character.currentAnimatedSprite.Play("Idle");

            enableTimer = true;
            idleTimer = 0f;
        }

        public override void StateExit(BaseCharacter character)
        {
            base.StateExit(character);

            //reset any needed values
            enableTimer = false;
            idleTimer = 0f;
        }

        public override void TickState(float delta, BaseCharacter character)
        {
            base.TickState(delta, character);

            if (!enableTimer) return;

            idleTimer += delta;

            if(idleTimer > idleDuration)
            {
                //Switch to the new state
                EmitSignal(SignalName.Transitioned, this, "WalkingState");
            }
        }
    }
}
