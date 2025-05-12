using Erikduss;
using Godot;
using System;
using System.Collections;

namespace Erikduss
{
	public partial class Archdruid : BaseCharacter
	{
        //This unit is called "Archdruid"

        public bool isTransformed = false;

        [Export] protected SpriteFrames defaultAnimatedSprite2D;
        [Export] protected SpriteFrames transformedAnimatedSprite2D;

        public bool isTransforming = false;
        private float tranformationTimer = 0;
        private float transformationDuration = 0.7f;

        private static float tranformToCombatModeDuration = 0.1f;
        private static float tranformToNormalModeDuration = 0.5f;

        public override void _Ready()
        {
            //Load Unit Stats

            UnitSettingsConfig loadedUnitSettings;

            //select the correct config file.
            switch (currentAge)
            {
                case Enums.Ages.AGE_01:
                    loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age01_ArchdruidSettingsConfig;
                    break;
                case Enums.Ages.AGE_02:
                    loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age02_ArchdruidSettingsConfig;
                    break;
                default:
                    loadedUnitSettings = GameSettingsLoader.Instance.unitSettingsManager.Age01_ArchdruidSettingsConfig;
                    break;
            }

            if (loadedUnitSettings.useCustomVariables)
            {
                loadDefaultValues = false;

                //Set the values
                currentHealth = loadedUnitSettings.unitHealth;
                maxHealth = loadedUnitSettings.unitHealth;

                unitArmor = loadedUnitSettings.unitArmour;
                unitAttackDamage = loadedUnitSettings.unitAttack;

                detectionRange = loadedUnitSettings.unitDetectionRange;
                movementSpeed = loadedUnitSettings.unitMovementSpeed;
            }

            unitType = Enums.UnitTypes.Archdruid;

            isRangedCharacter = true;

            base._Ready();
        }

        public override void _Process(double delta)
        {
            if (GameManager.Instance.gameIsPaused) return;

            if (!IsDeadOrDestroyed)
            {
                //we need to handle transforming back
                if (isTransforming)
                {
                    if(tranformationTimer >= transformationDuration)
                    {
                        ActivateTransformation();
                    }
                    else
                    {
                        tranformationTimer += (float)delta;
                    }
                }
            }

            base._Process(delta);
        }

        public void ActivateTransformation()
        {
            //switch the sprite frames.
            currentAnimatedSprite.SpriteFrames = isTransformed ? defaultAnimatedSprite2D : transformedAnimatedSprite2D;

            isTransformed = !isTransformed; //update bool

            currentAnimatedSprite.Play("Idle");

            isTransforming = false;
        }

        public void TransformCombatMode()
        {
            isTransforming = true;
            tranformationTimer = 0;
            transformationDuration = tranformToCombatModeDuration; //duration is very short, I'm quite sure that the regular attack cooldown is already applied. So making this too long will cause the unit to freeze.

            SetNewAttackCooldownTimer(transformationDuration);
        }

        public void TransformBack()
        {
            isTransforming = true;
            tranformationTimer = 0;
            transformationDuration = tranformToNormalModeDuration;
            currentAnimatedSprite.Play("Transform_Back");

            SetNewAttackCooldownTimer(transformationDuration);
        }
    }
}
