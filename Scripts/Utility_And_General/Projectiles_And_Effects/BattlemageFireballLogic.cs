using Godot;
using System;

namespace Erikduss
{
	public partial class BattlemageFireballLogic : DestroyableObject
	{
        public Enums.TeamOwner fireballOwner = Enums.TeamOwner.NONE;
        [Export] public AnimatedSprite2D animatedSprite;

        [Export] public Area2D attachedArea2D;

        private int fireballDamageMultiplier = 5;

        public override void _Ready()
        {
            if (fireballOwner == Enums.TeamOwner.TEAM_01)
            {
                attachedArea2D.CollisionMask = 0b1000100;
            }
            else
            {
                attachedArea2D.CollisionMask = 0b100010;
            }

            base._Ready();

            if (animatedSprite != null)
            {
                if (flipSpite) animatedSprite.FlipH = true;
            }
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
        }

        public void OnCollisionEnter(Node2D body)
        {
            int amountOfAliveUnitsOfEnemyTeam;

            if (fireballOwner == Enums.TeamOwner.TEAM_01)
            {
                amountOfAliveUnitsOfEnemyTeam = GameManager.Instance.unitsSpawner.team02AliveUnitDictionary.Count;
            }
            else
            {
                amountOfAliveUnitsOfEnemyTeam = GameManager.Instance.unitsSpawner.team01AliveUnitDictionary.Count;
            }

            if(amountOfAliveUnitsOfEnemyTeam == 0)
            {
                if (fireballOwner == Enums.TeamOwner.TEAM_01)
                {
                    if (body.GetInstanceId() == GameManager.Instance.team02HomeBase.StaticBody.GetInstanceId())
                    {
                        //we need to do TakeDamage instead of dealdamage on the basecharacter to apply the fireball damage multiplier.
                        GameManager.Instance.team02HomeBase.TakeDamage(characterThisEffectIsAttachedTo.unitAttackDamage * fireballDamageMultiplier);
                        return;
                    }
                }
                else
                {
                    if (body.GetInstanceId() == GameManager.Instance.team01HomeBase.StaticBody.GetInstanceId())
                    {
                        //we need to do TakeDamage instead of dealdamage on the basecharacter to apply the fireball damage multiplier.
                        GameManager.Instance.team01HomeBase.TakeDamage(characterThisEffectIsAttachedTo.unitAttackDamage * fireballDamageMultiplier);
                        return;
                    }
                }
            }
            else
            {
                //if we hit an enemy thats standing on the base, we also hit the base and get an error if we dont check this.
                if(body.GetInstanceId() == GameManager.Instance.team02HomeBase.StaticBody.GetInstanceId() || body.GetInstanceId() == GameManager.Instance.team01HomeBase.StaticBody.GetInstanceId())
                {
                    return;
                }

                BaseCharacter enemyChar = body.GetNode<BaseCharacter>(body.GetPath());

                //We should not deal damage to our own units.
                if (enemyChar.characterOwner == fireballOwner) return;

                if (enemyChar != null)
                {
                    enemyChar.TakeDamage(characterThisEffectIsAttachedTo.unitAttackDamage * fireballDamageMultiplier);
                }
            }
        }
    }
}
