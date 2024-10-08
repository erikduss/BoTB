using Godot;
using System;

namespace Erikduss
{
	public partial class State : Node
	{
		[Signal]
		public delegate void TransitionedEventHandler(State newState, string newStateName);

		public virtual void StateEnter()
		{

		}

		public virtual void StateExit()
		{

		}

		//Update
		public virtual void TickState()
		{

		}
	}
}
