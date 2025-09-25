using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Erikduss
{
	public partial class BaseCharacter : CharacterBody2D, IDamageable
	{
        public int uniqueID = -1;

        protected bool loadDefaultValues = true;

		public Enums.TeamOwner characterOwner = Enums.TeamOwner.NONE; //this will be set, this should NEVER be none.
        public Enums.UnitTypes unitType = Enums.UnitTypes.Warrior;

        [Export] public SpriteFrames[] animatedSpriteFramesAgeBased = new SpriteFrames[2];
		public AnimatedSprite2D characterAnimatedSprite;
        public CollisionShape2D unitCollisionShape;

		public Enums.Ages unitCreatedAge = Enums.Ages.AGE_01;

        public BaseCharacter CurrentTarget
        {
            get { return currentTarget; }
            set
            {
                //Before we switch targets we need to clear that the old target is the target of this.
                if (currentTarget != null)
                {
                    if (currentTarget.clearTargetOnTheseUnits.Contains(this))
                    {
                        currentTarget.clearTargetOnTheseUnits.Remove(this);
                    }
                }

                currentTarget = value;

                //it's possible we set it to null, we cant add to null
                if (currentTarget != null)
                {
                    currentTarget.clearTargetOnTheseUnits.Add(this);
                }
            }
        }

        protected BaseCharacter currentTarget;

        //we need this to prevent dispose issues with currenttarget
        public List<BaseCharacter> clearTargetOnTheseUnits = new List<BaseCharacter>();

        public List<BaseCharacter> unitsThatNeedToBeSignaledOnDeath = new List<BaseCharacter>();

        public bool unitHasReachedEnemyHomeBase = false;
        public bool isRangedCharacter = false;
        public bool checkForAlliesRaycastInstead = false;

        public bool hasActiveTankBuff = false;
        private float tankBuffDuration = 1.5f;
        private float tankBuffTimer = 0f;


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

        public bool isStunned = false;
        private float stunnedTimer = 0f;
        private float defaultStunDuration = 2f;
        private float currentStunDuration = 2f;

        public bool canMove = true;

        #region State Machine

        [Export] public State initialStartingState;

		public Dictionary<string, State> characterStates = new Dictionary<string, State>();
        public State currentState = null;

        #endregion

        #region Unit personal Stats

        public float movementSpeed = 50f; //default 50f
        public float detectionRange = 30f; //pixels

        #region Interface Implementation
        public bool IsDeadOrDestroyed 
        { 
            get { return isDeadOrDestroyed; } 
        }
        protected bool isDeadOrDestroyed = false;

        public int CurrentHealth 
        { 
            get { return currentHealth; }
        }
        protected int currentHealth = 20;

        public int MaxHealth
        {
            get { return maxHealth; }
        }
        protected int maxHealth = 20;
        #endregion

        public int unitArmor = 20;
        public int unitAttackDamage = 10;

        #endregion

        bool reloadedReady = false;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
            GDSync.ExposeFunction(new Callable(this, "DestroyOnMultiplayerClient"));

            if (GameManager.Instance.isMultiplayerMatch && !GameManager.Instance.isHostOfMultiplayerMatch) return;

            if (GameManager.Instance.isHostOfMultiplayerMatch && GameManager.Instance.isMultiplayerMatch)
            {
                if (!reloadedReady && uniqueID == -1)
                {
                    GD.Print("We reload");
                    ReApplyReadyOnMultiplayer();
                    return;
                }
            }

            foreach (Node childNode in this.GetChildren())
            {
                if (childNode is AnimatedSprite2D)
                {
                    AnimatedSprite2D spriteComponent = childNode.GetNode<AnimatedSprite2D>(childNode.GetPath());

                    spriteComponent.Visible = false;
                    characterAnimatedSprite = spriteComponent;
                }

                if(childNode is CollisionShape2D)
                {
                    unitCollisionShape = childNode.GetNode<CollisionShape2D>(childNode.GetPath());
                }
            }

            //Get the correct animated sprite to enable.
            characterAnimatedSprite.SpriteFrames = animatedSpriteFramesAgeBased[((int)unitCreatedAge)];
            characterAnimatedSprite.Visible = true;

            if(loadDefaultValues && unitType != Enums.UnitTypes.TrainingDummy)
            {
                //Set the default values
                UnitSettingsConfig defaultUnitValues = UnitsDefaultValues.defaultUnitValuesDictionary.Where(a => a.Key == (unitCreatedAge.ToString() + "_" + unitType.ToString())).FirstOrDefault().Value;

                currentHealth = defaultUnitValues.unitHealth;
                maxHealth = defaultUnitValues.unitHealth;
                unitArmor = defaultUnitValues.unitArmour;
                unitAttackDamage = defaultUnitValues.unitAttack;

                detectionRange = defaultUnitValues.unitDetectionRange;
                movementSpeed = defaultUnitValues.unitMovementSpeed;
            }

            if (characterOwner == Enums.TeamOwner.TEAM_02)
            {
                GD.Print("We are from team 2!");

                movementSpeed = -movementSpeed; // this one needs to go the other direction.
                characterAnimatedSprite.FlipH = true;
                CollisionLayer = 0b100;
                CollisionMask = 0b100111; //This is needed to make sure we dont collide with out own base, but we do with the enemy base.
            }
            else
            {
                GD.Print("Our team is: " + characterOwner.ToString());

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

        async void ReApplyReadyOnMultiplayer()
        {
            while (uniqueID < 0)
            {
                await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
            }
            
            reloadedReady = true;
            GD.Print("Is ID set? " + uniqueID);

            _Ready();
        }

        //for some reason, without variables, this throws an error. We dont need any, so we give it a random one.
        public void DestroyOnMultiplayerClient(bool randombool)
        {
            GD.Print("Random bool: " + randombool);

            QueueFree();
        }

        protected override void Dispose(bool disposing)
        {
            if(clearTargetOnTheseUnits.Count > 0)
            {
                foreach(BaseCharacter unit in clearTargetOnTheseUnits)
                {
                    unit.ClearCurrentTarget();
                }
            }

            base.Dispose(disposing);
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
            if (GameManager.Instance.isMultiplayerMatch && !GameManager.Instance.isHostOfMultiplayerMatch) return;

            if (GameManager.Instance.gameIsPaused || GameManager.Instance.gameIsFinished) return;

            if (IsDeadOrDestroyed)
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
                    if (GameManager.Instance.isMultiplayerMatch)
                    {
                        int otherClient = MultiplayerManager.Instance.playersInLobby.Where(a => a != GDSync.GetClientId()).First();
                        GDSync.CallFuncOn(otherClient, new Callable(this, "DestroyOnMultiplayerClient"), [true]);
                    }

                    QueueFree();
                }
                else
                {
                    deathTimer += (float)delta;
                }
            }

            if (hasActiveTankBuff)
            {
                if (tankBuffTimer > tankBuffDuration)
                {
                    hasActiveTankBuff = false;
                    currentAttackCooldownDuration = -1;
                }
                else
                {
                    tankBuffTimer += (float)delta;
                }
            }

            //add stun check.
            if (isStunned)
            {
                if (stunnedTimer > currentStunDuration)
                {
                    isStunned = false;
                }
                else
                {
                    stunnedTimer += (float)delta;
                    return;
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
            if (GameManager.Instance.gameIsPaused || IsDeadOrDestroyed) return;

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

        protected virtual void SetNewMaxHealthBasedOnDamageTaken(int damage)
        {
            //Units cannot be healed back to their full potential after taking wounds in battle.
            //This prevents infinite fights (for example, tank vs tank with both a healer behind them)

            int maxHealthReduction = Mathf.RoundToInt(damage * 0.5f);

            if (maxHealthReduction <= 0)
            {
                maxHealthReduction = 1;
            }

            maxHealth = maxHealth - maxHealthReduction;

            //this can happen with the enforcer stun effect mechanic.
            if(maxHealth == 0 && currentHealth > 0)
            {
                currentHealth = 0;
                processDeath();
            }
        }

        public virtual void TakeDamage(int rawDamage)
        {
            //raw damage is the damage before any armour damage reduction.

            float damageReductionPercentageFromArmour = (float)(unitArmor / 100f); //armor is percentage damage reduction

            float fixedDamageReduction = rawDamage * damageReductionPercentageFromArmour;

            int fixedDamage = Mathf.RoundToInt(rawDamage - fixedDamageReduction);

            if (fixedDamage <= 0) fixedDamage = 1; //We always do at least some damage

            currentHealth -= fixedDamage;

            SetNewMaxHealthBasedOnDamageTaken(fixedDamage);

            EffectsAndProjectilesSpawner.Instance.SpawnFloatingDamageNumber(this, fixedDamage);

            //award teams with power up progress.
            #region PowerUp progress reward
            int powerUpProgressAmountRewardedSelf = fixedDamage * GameSettingsLoader.powerUpProgressMultiplierOwnUnitDamage;
            int powerUpProgressAmountRewardedEnemy = (int)MathF.Floor((float)fixedDamage * GameSettingsLoader.powerUpProgressMultiplierOtherUnitDamage);

            GameManager.Instance.UpdatePlayerPowerUpProgress(characterOwner, powerUpProgressAmountRewardedSelf);
            Enums.TeamOwner enemyTeamOwner = characterOwner == Enums.TeamOwner.TEAM_01 ? Enums.TeamOwner.TEAM_02 : Enums.TeamOwner.TEAM_01;

            if (powerUpProgressAmountRewardedEnemy < 0) powerUpProgressAmountRewardedEnemy = 1;

            GameManager.Instance.UpdatePlayerPowerUpProgress(enemyTeamOwner, powerUpProgressAmountRewardedEnemy);
            #endregion

            if (currentHealth <= 0) 
            {
                processDeath();
            }
            else
            {
                if (characterOwner == Enums.TeamOwner.TEAM_01)
                {
                    if (!GameManager.Instance.unitsSpawner.team01DamagedUnits.Contains(this))
                    {
                        GameManager.Instance.unitsSpawner.team01DamagedUnits.Add(this);
                    }

                    //if (GameManager.Instance.unitsSpawner.team01AliveUnitDictionary.Values.Any(a => a.unitType == (Enums.UnitTypes.Mass_Healer)))
                    //{
                    //    GD.Print("We have a healer in the house");


                    //}
                    //else
                    //{
                    //    GD.Print("We dont have healers, reported.");
                    //}
                }
                else
                {
                    //check if for team 2
                    if (!GameManager.Instance.unitsSpawner.team02DamagedUnits.Contains(this))
                    {
                        GameManager.Instance.unitsSpawner.team02DamagedUnits.Add(this);
                    }
                }
            }
        }

        public async virtual void HealDamage(int healAmount)
        {
            if(IsDeadOrDestroyed) return;

            currentHealth += healAmount;

            if(currentHealth > maxHealth) currentHealth = maxHealth;

            EffectsAndProjectilesSpawner.Instance.SpawnFloatingDamageNumber(this, healAmount, true);

            int powerUpProgressAmountRewardedSelf = (int)MathF.Floor((float)healAmount * GameSettingsLoader.powerUpProgressMultiplierOwnUnitHealing);

            GameManager.Instance.UpdatePlayerPowerUpProgress(characterOwner, powerUpProgressAmountRewardedSelf);

            //we need to wait till the effects function is done processing due to changes in the list structure that will give errors.
            while (EffectsAndProjectilesSpawner.Instance.processingHealingEffects)
            {
                await Task.Yield();
            }

            //the entry to the list is made when we are damaged, we cant get healed if we are not damaged so there is always an entry.
            if(characterOwner == Enums.TeamOwner.TEAM_01)
            {
                if (currentHealth == maxHealth) GameManager.Instance.unitsSpawner.team01DamagedUnits.Remove(this);
            }
            else
            {
                if (currentHealth == maxHealth) GameManager.Instance.unitsSpawner.team02DamagedUnits.Remove(this);
            }
        }

        public virtual void processDeath()
        {
            if(IsDeadOrDestroyed) return;

            if(characterOwner == Enums.TeamOwner.TEAM_01)
            {
                if (GameManager.Instance.unitsSpawner.team01DamagedUnits.Contains(this))
                {
                    GameManager.Instance.unitsSpawner.team01DamagedUnits.Remove(this);
                }
            }
            else
            {
                if (GameManager.Instance.unitsSpawner.team02DamagedUnits.Contains(this))
                {
                    GameManager.Instance.unitsSpawner.team02DamagedUnits.Remove(this);
                }
            }

            isDeadOrDestroyed = true;

            CollisionLayer = 0b00;
            CollisionMask = 0b00;

            unitCollisionShape.SetDeferred("Disabled", true);

            SignalUnitsThatThisOneDied();

            GameManager.Instance.unitsSpawner.RemoveAliveUnitFromTeam(characterOwner, unitType, uniqueID);

            characterAnimatedSprite.Play("Death");
        }

        public virtual void DealDamage()
        {
            //IF CHANGING ANYTHING HERE KEEP IN MIND SOME UNITS USE THEIR CUSTOM DEAL DAMAGE FUNCTION: Simplesoldier.cs (warrior)

            if (currentTarget == null && !unitHasReachedEnemyHomeBase)
            {
                GD.PrintErr("CURRENT TARGET NOT SET WHEN DEALING DAMAGE");
                return;
            }

            if(IsDeadOrDestroyed && !canStillDamage) return; //we cant deal damage if we are dead.

            //add any multipliers here
            int damage = unitAttackDamage;

            if(unitHasReachedEnemyHomeBase && currentTarget == null)
            {
                if(characterOwner == Enums.TeamOwner.TEAM_01)
                {
                    GameManager.Instance.team02HomeBase.TakeDamage(damage);
                }
                else
                {
                    GameManager.Instance.team01HomeBase.TakeDamage(damage);
                }
            }
            else currentTarget.TakeDamage(damage);
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

        //public virtual void SignalUnitHasTakenDamage(BaseCharacter unitThatTookDamage)
        //{

        //}

        public virtual void SetNewAttackCooldownTimer(float overrideAttackCooldown = -1)
        {
            canAttack = false;

            if(overrideAttackCooldown > 0) attackCooldownTimer = overrideAttackCooldown;
            else attackCooldownTimer = attackCooldownDuration;
        }

        public virtual void ApplyStunEffect(float overrideStunDuration = -1)
        {
            float stunDuration = defaultStunDuration;

            if (overrideStunDuration > 0) stunDuration = overrideStunDuration;

            //passive buff for enforecer, every stun, reduce the max health of the target by 1.
            SetNewMaxHealthBasedOnDamageTaken(1);

            //we dont want to be stunned permanently
            if (isStunned) return;

            currentStunDuration = stunDuration;

            stunnedTimer = 0;
            isStunned = true;
        }

        public virtual void ApplyTankBuffEffect(float buffDuration)
        {
            //Tank buff effect:
            //Give Damage reduction to units behind.
            //Give Ranged Characters improved attack speed.

            if (hasActiveTankBuff) return;

            tankBuffDuration = buffDuration;
            tankBuffTimer = 0;

            SetTankBuffedAttackSpeedValue();

            hasActiveTankBuff = true;
        }

        public virtual void SetTankBuffedAttackSpeedValue(bool justLostOtherAttackSpeedBuff = false)
        {
            if (currentAttackCooldownDuration > 0 && !justLostOtherAttackSpeedBuff)
            {
                float previousValue = currentAttackCooldownDuration;
                currentAttackCooldownDuration = currentAttackCooldownDuration * 0.5f;
            }
            else currentAttackCooldownDuration = attackCooldownDuration * 0.5f;
        }

        public void ClearCurrentTarget()
        {
            currentTarget = null;
        }
    }
}
