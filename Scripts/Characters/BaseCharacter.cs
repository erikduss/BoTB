using Godot;
using System;
using System.Collections.Generic;

namespace Erikduss
{
	public partial class BaseCharacter : Node2D
	{
		public Enums.TeamOwner characterOwner = Enums.TeamOwner.NONE; //this will be set, this should NEVER be none.

		public List<AnimatedSprite2D> animatedSpritesAgeBased = new List<AnimatedSprite2D>();
		private AnimatedSprite2D currentAnimatedSprite;

		public Enums.Ages currentAge = Enums.Ages.AGE_01;

		public float movementSpeed = 2f;
		public float detectionRange = 2f;

        #region State Machine

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

            foreach (Node state in this.GetChildren())
            {
                if (state is State)
                {
                    State fetchedState = (State)state;
                    characterStates.Add(fetchedState.Name.ToString().ToLower(), fetchedState);
                    fetchedState.Transitioned += OnStateTransition;
                }
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
                currentState.TickState();
            }
		}

        public void OnStateTransition(State newState, string newStateName)
        {
            if (newState == currentState) return;

            //https://youtu.be/ow_Lum-Agbs?si=DD68-O0Y3B8puAAR&t=210
        }
    }
}
