using Godot;
using System;

namespace Erikduss
{
	public partial class BattlemageAge1Projectile : ProjectileAndEffect
	{
		[Export] public BattlemageAge1ProjectilePhysics rigidBody;

        public BaseCharacter projectileOwnerChar;
		public Enums.TeamOwner projectileOwner = Enums.TeamOwner.TEAM_01;

        bool dealtDamage = false;

        [Export] public AnimatedSprite2D animatedSprite;

        public override void _Ready()
        {
            base._Ready();

            SetNewOwner(projectileOwner);

            if (animatedSprite != null) 
            {
                if (flipSpite) animatedSprite.FlipH = true;
            }
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
        }

        public override void _PhysicsProcess(double delta)
        {
            if (GameManager.Instance.gameIsPaused) return;

            base._PhysicsProcess(delta);

           //rigidBody.MoveAndCollide(new Vector2(1000, 0));
        }

        public void SetNewOwner(Enums.TeamOwner owner)
		{
			projectileOwner = owner;

            if (owner == Enums.TeamOwner.TEAM_01)
            {
                rigidBody.CollisionMask = 0b1000111;
            }
            else
            {
                rigidBody.CollisionMask = 0b100111;
            }
        }

        public void OnCollisionEnter(Node2D body)
		{
            rigidBody.StopForces();

            if (dealtDamage) return;

            if (body.GetInstanceId() == projectileOwnerChar.currentTarget.GetInstanceId())
            {
                dealtDamage = true;
                projectileOwnerChar.DealDamage();
            }
            else 
            {
                if(projectileOwner == Enums.TeamOwner.TEAM_01)
                {
                    if(body.GetInstanceId() == GameManager.Instance.team02HomeBase.GetInstanceId())
                    {
                        //We hit the enemy's base. Possibly needs to change some variables still to make sure it works.
                        dealtDamage = true;
                        projectileOwnerChar.DealDamage();
                        return;
                    }
                }
                else
                {
                    if (body.GetInstanceId() == GameManager.Instance.team01HomeBase.GetInstanceId())
                    {
                        //We hit the enemy's base. Possibly needs to change some variables still to make sure it works.
                        dealtDamage = true;
                        projectileOwnerChar.DealDamage();
                        return;
                    }
                }

                BaseCharacter enemyChar = body.GetNode<BaseCharacter>(body.GetPath());
                if (enemyChar.characterOwner == projectileOwnerChar.characterOwner) return;

                dealtDamage = true;
                projectileOwnerChar.currentTarget = enemyChar;
                projectileOwnerChar.DealDamage();
            }
		}

		public void OnCollisionExit(Node2D body)
		{

        }
    }
}
