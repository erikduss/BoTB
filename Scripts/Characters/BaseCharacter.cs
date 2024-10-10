using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Erikduss
{
	public partial class BaseCharacter : CharacterBody2D
	{
		public Enums.TeamOwner characterOwner = Enums.TeamOwner.NONE; //this will be set, this should NEVER be none.

		public List<AnimatedSprite2D> animatedSpritesAgeBased = new List<AnimatedSprite2D>();
		public AnimatedSprite2D currentAnimatedSprite;

		public Enums.Ages currentAge = Enums.Ages.AGE_01;

		public float movementSpeed = 50f;
		public float detectionRange = 2f;

        #region State Machine

        [Export] public State initialStartingState;

		public Dictionary<string, State> characterStates = new Dictionary<string, State>();
        public State currentState = null;

        #endregion

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
            foreach (Node animatedSprite in this.GetChildren())
            {
                if (animatedSprite is AnimatedSprite2D)
                {
                    AnimatedSprite2D spriteComponent = animatedSprite.GetNode<AnimatedSprite2D>(animatedSprite.GetPath());

                    spriteComponent.Visible = false;
                    animatedSpritesAgeBased.Add(spriteComponent);
                }
            }

            //Get the correct animated sprite to enable.
            currentAnimatedSprite = animatedSpritesAgeBased[((int)currentAge)];
            currentAnimatedSprite.Visible = true;

            if (characterOwner == Enums.TeamOwner.TEAM_02)
			{
				movementSpeed = -movementSpeed; // this one needs to go the other direction.
				currentAnimatedSprite.FlipH = true;
			}

            #region State Machine

            Node2D statesParentNode = GetNode<Node2D>("StateMachine_States");

            //Get all the states from the fetched parent node.
            if (statesParentNode != null)
            {
                foreach (Node state in statesParentNode.GetChildren())
                {
                    if (state is State)
                    {
                        State fetchedState = (State)state;
                        characterStates.Add(fetchedState.Name.ToString().ToLower(), fetchedState);
                        fetchedState.Transitioned += OnStateTransition;
                    }
                }
            }

            if(initialStartingState != null)
            {
                initialStartingState.StateEnter(this);
                currentState = initialStartingState;
            }

            #endregion
        }

        public override void _Notification(int what)
        {
            base._Notification(what);

            if(what == NotificationWMCloseRequest)
            {
                foreach(State state in characterStates.Values)
                {
                    state.Transitioned -= OnStateTransition;
                }
            }
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
		{
			//detect other team's characters in range

            if(currentState != null)
            {
                currentState.TickState((float)delta, this);
            }
		}

        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);

            if (currentState != null)
            {
                currentState.PhysicsTickState((float)delta, this);
            }
        }

        public void OnStateTransition(State transitionFromThisState, string newStateName)
        {
            if (transitionFromThisState != currentState) return; //we can only transition from the state we are currently in.

            State stateToTransitionTo = characterStates.First(a => a.Key == newStateName.ToLower()).Value;

            if (stateToTransitionTo == null) return;

            if (currentState != null) currentState.StateExit(this);

            stateToTransitionTo.StateEnter(this);

            currentState = stateToTransitionTo;

            //https://youtu.be/ow_Lum-Agbs?si=DD68-O0Y3B8puAAR&t=210
        }
    }
}
