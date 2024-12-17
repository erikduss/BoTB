using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Erikduss
{
	public partial class BaseCharacter : CharacterBody2D
	{
        public int uniqueID = -1;

		public Enums.TeamOwner characterOwner = Enums.TeamOwner.NONE; //this will be set, this should NEVER be none.
        public Enums.UnitTypes unitType = Enums.UnitTypes.Warrior;

		public List<AnimatedSprite2D> animatedSpritesAgeBased = new List<AnimatedSprite2D>();
		public AnimatedSprite2D currentAnimatedSprite;
        public CollisionShape2D unitCollisionShape;

		public Enums.Ages currentAge = Enums.Ages.AGE_02;

        public BaseCharacter currentTarget;
        public List<BaseCharacter> unitsThatNeedToBeSignaledOnDeath = new List<BaseCharacter>();

        public bool isRangedCharacter = false;

        public bool isDead = false;
        private bool startedDeathTimer = false;
        private float deathTimerDuration = 1.5f;
        private float deathTimer = 0f;

        public bool canStillDamage = true;
        private float canStillDamageTimer = 0f;
        private float canStillDamageDuration = 0.2f;

        public bool canAttack = true;
        private float attackCooldownTimer = 0f;
        private float attackCooldownDuration = 1f;
        public float currentAttackCooldownDuration = -1f;

        #region State Machine

        [Export] public State initialStartingState;

		public Dictionary<string, State> characterStates = new Dictionary<string, State>();
        public State currentState = null;

        #endregion

        #region Unit personal Stats

        public float movementSpeed = 50f; //default 50f
        public float detectionRange = 30f; //pixels

        public int currentHealth = 20;
        public int maxHealth = 20;

        public int unitArmor = 20;
        public int unitAttackDamage = 10;

        #endregion

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
            foreach (Node childNode in this.GetChildren())
            {
                if (childNode is AnimatedSprite2D)
                {
                    AnimatedSprite2D spriteComponent = childNode.GetNode<AnimatedSprite2D>(childNode.GetPath());

                    spriteComponent.Visible = false;
                    animatedSpritesAgeBased.Add(spriteComponent);
                }

                if(childNode is CollisionShape2D)
                {
                    unitCollisionShape = childNode.GetNode<CollisionShape2D>(childNode.GetPath());
                }
            }

            //Get the correct animated sprite to enable.
            currentAnimatedSprite = animatedSpritesAgeBased[((int)currentAge)];
            currentAnimatedSprite.Visible = true;

            if (characterOwner == Enums.TeamOwner.TEAM_02)
			{
				movementSpeed = -movementSpeed; // this one needs to go the other direction.
				currentAnimatedSprite.FlipH = true;
                CollisionLayer = 0b100;
                CollisionMask = 0b100111; //This is needed to make sure we dont collide with out own base, but we do with the enemy base.
            }
            else
            {
                CollisionLayer = 0b10;
                CollisionMask = 0b1000111; //This is needed to make sure we dont collide with out own base, but we do with the enemy base.
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
            if (GameManager.Instance.gameIsPaused) return;

            if (isDead)
            {
                if (canStillDamageTimer >= canStillDamageDuration)
                {
                    canStillDamage = false;
                }
                else
                {
                    canStillDamageTimer += (float)delta;
                }

                if(deathTimer >= deathTimerDuration)
                {
                    QueueFree();
                }
                else
                {
                    deathTimer += (float)delta;
                }
            }

            if (!canAttack)
            {
                if(attackCooldownTimer >= 0)
                {
                    attackCooldownTimer -= (float)delta;
                }
                else
                {
                    canAttack = true;
                    attackCooldownTimer = 0;
                }
            }

            //detect other team's characters in range

            if (currentState != null)
            {
                currentState.TickState((float)delta, this);
            }
		}

        public override void _PhysicsProcess(double delta)
        {
            if (GameManager.Instance.gameIsPaused || isDead) return;

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
        }

        public virtual void TakeDamage(int rawDamage)
        {
            //raw damage is the damage before any armour damage reduction.

            float damageReductionPercentageFromArmour = (float)(unitArmor / 100f); //armor is percentage damage reduction

            float fixedDamageReduction = rawDamage * damageReductionPercentageFromArmour;

            int fixedDamage = Mathf.RoundToInt(rawDamage - fixedDamageReduction);

            if (fixedDamage <= 0) fixedDamage = 1; //We always do at least some damage

            currentHealth -= fixedDamage;

            if (currentHealth <= 0) 
            {
                processDeath();
            }
        }

        public virtual void processDeath()
        {
            if(isDead) return;

            isDead = true;

            CollisionLayer = 0b00;
            CollisionMask = 0b00;

            unitCollisionShape.SetDeferred("Disabled", true);

            SignalUnitsThatThisOneDied();

            GameManager.Instance.unitsSpawner.RemoveAliveUnitFromTeam(characterOwner, unitType, uniqueID);

            currentAnimatedSprite.Play("Death");
        }

        public virtual void DealDamage()
        {
            if (currentTarget == null)
            {
                GD.PrintErr("CURRENT TARGET NOT SET WHEN DEALING DAMAGE");
                return;
            }

            if(isDead && !canStillDamage) return; //we cant deal damage if we are dead.

            //add any multipliers here
            int damage = unitAttackDamage;


            currentTarget.TakeDamage(damage);
        }

        public virtual void SignalUnitsThatThisOneDied()
        {
            foreach(BaseCharacter unit in unitsThatNeedToBeSignaledOnDeath)
            {
                if (unit == null) continue;

                unit.UnitSignaledForDeathEvent();
            }
        }

        public virtual void UnitSignaledForDeathEvent()
        {

        }

        public virtual void SetNewAttackCooldownTimer(float overrideAttackCooldown = -1)
        {
            canAttack = false;

            if(overrideAttackCooldown > 0) attackCooldownTimer = overrideAttackCooldown;
            else attackCooldownTimer = attackCooldownDuration;
        }
    }
}
