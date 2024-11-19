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

            if (attackTimer > attackDuration)
            {
                if (unitHasAttacked) return;

                unitHasAttacked = true;

                character.DealDamage();

                if (character.isRangedCharacter)
                {
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
    }
}
