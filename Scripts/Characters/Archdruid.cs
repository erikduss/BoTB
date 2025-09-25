using Erikduss;
using Godot;
using System;
using System.Collections;
using System.Linq;

namespace Erikduss
{
	public partial class Archdruid : BaseCharacter
	{
        //This unit is called "Archdruid"

        public bool isTransformed = false;

        [Export] protected SpriteFrames[] transformedAnimatedSpriteFramesAgeBased = new SpriteFrames[2];

        public bool isTransforming = false;
        private float tranformationTimer = 0;
        private float transformationDuration = 0.7f;

        private static float tranformToCombatModeDuration = 0.1f;
        private static float tranformToNormalModeDuration = 0.5f;

        public override void _Ready()
        {
            if (GameManager.Instance.isMultiplayerMatch)
            {
                GDSync.ExposeFunction(new Callable(this, "SetNewSpriteFrameMultiplayer"));

                if (!GameManager.Instance.isHostOfMultiplayerMatch)
                {
                    foreach (Node childNode in this.GetChildren())
                    {
                        //this is needed, otherwise the characteraniumatedsprite is null and its needed to multiplayer switch
                        if (childNode is AnimatedSprite2D)
                        {
                            AnimatedSprite2D spriteComponent = childNode.GetNode<AnimatedSprite2D>(childNode.GetPath());

                            //spriteComponent.Visible = false;
                            characterAnimatedSprite = spriteComponent;
                        }
                    }
                }
            }

            if (GameManager.Instance.isMultiplayerMatch && !GameManager.Instance.isHostOfMultiplayerMatch)
            {
                base._Ready();
                return;
            }

            //Load Unit Stats

            UnitSettingsConfig loadedUnitSettings;

            //select the correct config file.
            switch (unitCreatedAge)
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

            if (GameManager.Instance.isMultiplayerMatch && !GameManager.Instance.isHostOfMultiplayerMatch) return;

                if (!IsDeadOrDestroyed)
            {
                //we need to handle transforming back
                if (isTransforming)
                {
                    if(tranformationTimer >= transformationDuration)
                    {
                        if(GameManager.Instance.isMultiplayerMatch && GameManager.Instance.isHostOfMultiplayerMatch)
                        {
                            int otherClient = MultiplayerManager.Instance.playersInLobby.Where(a => a != GDSync.GetHost()).First();
                            GDSync.CallFuncOn(otherClient, new Callable(this, "SetNewSpriteFrameMultiplayer"), [isTransformed]);
                        }

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

        public void SetNewSpriteFrameMultiplayer(bool isCurrentlyTransformed)
        {
            characterAnimatedSprite.SpriteFrames = isCurrentlyTransformed ? animatedSpriteFramesAgeBased[(int)unitCreatedAge] : transformedAnimatedSpriteFramesAgeBased[(int)unitCreatedAge];
        }

        public void ActivateTransformation()
        {
            //switch the sprite frames.
            characterAnimatedSprite.SpriteFrames = isTransformed ? animatedSpriteFramesAgeBased[(int)unitCreatedAge] : transformedAnimatedSpriteFramesAgeBased[(int)unitCreatedAge];

            isTransformed = !isTransformed; //update bool

            characterAnimatedSprite.Play("Idle");

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
            if (!GameManager.Instance.isHostOfMultiplayerMatch)
            {
                GD.Print("we get in here as non host.");
                return;
            }

            isTransforming = true;
            tranformationTimer = 0;
            transformationDuration = tranformToNormalModeDuration;
            characterAnimatedSprite.Play("Transform_Back");

            SetNewAttackCooldownTimer(transformationDuration);
        }
    }
}
