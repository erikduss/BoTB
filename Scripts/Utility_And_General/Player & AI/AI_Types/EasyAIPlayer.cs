using Godot;
using System;

namespace Erikduss
{
	public partial class EasyAIPlayer : BaseAIPlayer
	{
        /* The normal AI (we just want to press buttons, nothing else)
		 * 
		 * We buy units at random, meaning that we will try to buy a new unit every few seconds, which is a different one every time.
		 * We randomize the buy cooldown timer so we dont buy a unit at exact intervals everytime.
		 * Every unit buy attempt has a chance to instead be a refresh button press. (If we have 2 or more druids in our shop, we refresh guarenteed)
		 * 
		 */

        public override void _Ready()
        {
            abilityActivationChance = 15;
            ageUpgradeChance = 25;
            redeemPowerUpAttemptCooldown = 9;
            activationAttemptCooldown = 10;
            unitBuyAttemptCooldown = 14;
            comboBuyChance = 10;
            refreshChance = 20;

            minimumAmountOfEnemiesRequiredToUseAbility = 1;
            minimumAmountOfUnitDifferenctRequiredToSaveCurrency = 2;

            base._Ready();
        }
    }
}
