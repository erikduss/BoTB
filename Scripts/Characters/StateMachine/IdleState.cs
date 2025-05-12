using Godot;
using System;

namespace Erikduss
{
    public partial class IdleState : State
    {
        [Export] public float idleDuration = 1f;
        private float currentIdleDuration = 1f;

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

            if (character.CurrentTarget != null || character.unitHasReachedEnemyHomeBase)
            {
                //checking for the tank buff causes it so the rangers can move while being buffed by their own attack speed passive. If this is too op, check for exception here.
                if (character.currentAttackCooldownDuration > 0f && !character.hasActiveTankBuff) currentIdleDuration = character.currentAttackCooldownDuration;
                else currentIdleDuration = idleDuration;
            }
            else
            {
                currentIdleDuration = 0.5f;
            }
            

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
            if(character.IsDeadOrDestroyed) return;

            base.TickState(delta, character);

            if (!enableTimer) return;

            idleTimer += delta;

            if(idleTimer > currentIdleDuration)
            {
                if (character.CurrentTarget != null) 
                {
                    if (character.CurrentTarget.CurrentHealth > 0)
                    {
                        EmitSignal(SignalName.Transitioned, this, "AttackState");
                        return;
                    }
                    else character.CurrentTarget = null;
                }

                //we need to attack right away too when we are at the enemy homebase.
                if (character.unitHasReachedEnemyHomeBase)
                {
                    if (!GameManager.Instance.gameIsFinished)
                    {
                        EmitSignal(SignalName.Transitioned, this, "AttackState");
                        return;
                    }
                    else
                    {
                        EmitSignal(SignalName.Transitioned, this, "IdleState");
                        return;
                    }
                }

                //Switch to the new state
                EmitSignal(SignalName.Transitioned, this, "WalkingState");
            }
        }
    }
}
