using Godot;
using System;

namespace Erikduss
{
	public partial class State : Node
	{
		[Signal]
		public delegate void TransitionedEventHandler(State transitionedFromThisState, string newStateName);

		public virtual void StateEnter(BaseCharacter character)
		{
			//GD.Print("State entered");
		}

		public virtual void StateExit(BaseCharacter character)
		{
            //GD.Print("State exited");
        }

		//Update
		public virtual void TickState(float delta, BaseCharacter character)
		{
            //GD.Print("State Tick");
        }

		public virtual void PhysicsTickState(float delta, BaseCharacter character)
		{

		}
	}
}
