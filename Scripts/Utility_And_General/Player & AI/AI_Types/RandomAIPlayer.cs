using Godot;
using System;

namespace Erikduss
{
	public partial class RandomAIPlayer : BaseAIPlayer
	{
        /* The Random AI (we just want to press buttons, nothing else)
		 * 
		 * We buy units at random, meaning that we will try to buy a new unit every few seconds, which is a different one every time.
		 * We randomize the buy cooldown timer so we dont buy a unit at exact intervals everytime.
		 * Every unit buy attempt has a chance to instead be a refresh button press. (If we have 2 or more druids in our shop, we refresh guarenteed)
		 * 
		 * For future: we need a way to save money to age up.
		 */

        #region Player Ability Activation Attempt Timer
        private float abiltyActivationAttemptTimer = 0;
        private int activationAttemptCooldown = 7; //this will be + or - 2 based on a randomizer.
		private int randomActivationChance = 15; //in percentage
        #endregion

        #region Unit Buy Attempt Timer
        private float unitBuyAttemptTimer = 0;
        private int unitBuyAttemptCooldown = 7; //this will be + or - 4 based on a randomizer.
        private int refreshChance = 15; //in percentage
        private int comboBuyChance = 12;
        #endregion


        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
			base._Ready();

			SetTimerStartingValues();
        }

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			base._Process(delta);

            #region Ability Activation
			if(playerAbilityCurrentCooldown <= 0)
			{
				if (abiltyActivationAttemptTimer <= 0)
				{
					AttemptToActivateOurAbility();
				}
                else
                {
                    abiltyActivationAttemptTimer -= (float)delta;
                }
			}
            #endregion

            #region Unit Buying
            if (unitBuyAttemptTimer <= 0)
            {
                AttemptToBuyUnit();
            }
            else
            {
                unitBuyAttemptTimer -= (float)delta;
            }
            #endregion
        }

        private void SetTimerStartingValues()
		{
			SetNewAbilityActivationTimer();
            SetNewUnitBuyTimer();
        }

        private void AttemptToBuyUnit()
        {
            int randRefreshRoll = (int)(GD.Randi() % 100);

            SetNewUnitBuyTimer();

            //we hit the roll
            if (randRefreshRoll <= refreshChance)
            {
                RefreshUnitShop(); //We have a chance to fail this if we dont have enough gold.
                return;
            }

            int randUnitFromShophRoll = (int)(GD.Randi() % 3); //number is not inclusive.

            BuyUnitFromShop(randUnitFromShophRoll);
        }

		private void AttemptToActivateOurAbility()
		{
			//we will try doing this about every 5-9 seconds with a 10% chance for it to succeed.

			if (playerAbilityCurrentCooldown > 0) return;

			SetNewAbilityActivationTimer();

            int randActivateRoll = (int)(GD.Randi() % 100);

			//we hit the roll
			if(randActivateRoll <= randomActivationChance)
			{
				ActivatePlayerAbility();
			}
        }

		private void SetNewAbilityActivationTimer()
		{
            int randTimerRoll = (int)(GD.Randi() % 4);
			randTimerRoll -= 2;

			int newCooldownTime = activationAttemptCooldown - randTimerRoll;

			abiltyActivationAttemptTimer = newCooldownTime;
        }

        private void SetNewUnitBuyTimer()
        {
            int randTimerRoll = (int)(GD.Randi() % 8);
            randTimerRoll -= 4;

            int newCooldownTime = unitBuyAttemptCooldown - randTimerRoll;

            int randComboBuyRoll = (int)(GD.Randi() % 100);

            if(randComboBuyRoll <= comboBuyChance)
            {
                newCooldownTime = 1;
            }

            unitBuyAttemptTimer = newCooldownTime;
        }
    }
}
