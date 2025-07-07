using Godot;
using System;

namespace Erikduss
{
	public partial class BaseProjectile : DestroyableObject
    {
        [Export] public BaseProjectilePhysics rigidBody;

        public BaseCharacter projectileOwnerChar;
        public Enums.TeamOwner projectileOwner = Enums.TeamOwner.TEAM_01;

        protected bool dealtDamage = false;

        public override void _Ready()
        {
            SetNewOwner(projectileOwner);

            destroyObjectOverride = this.GetParent() as Node2D;

            base._Ready();
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
        }

        public override void _PhysicsProcess(double delta)
        {
            if (GameManager.Instance.isMultiplayerMatch && !GameManager.Instance.isHostOfMultiplayerMatch) return;

            if (GameManager.Instance.gameIsPaused) return;

            base._PhysicsProcess(delta);

            //rigidBody.MoveAndCollide(new Vector2(1000, 0));
        }

        public virtual void SetNewOwner(Enums.TeamOwner owner)
        {
            projectileOwner = owner;

            if (owner == Enums.TeamOwner.TEAM_01)
            {
                rigidBody.CollisionMask = 0b1000101;
            }
            else
            {
                rigidBody.CollisionMask = 0b100011;
            }
        }

        public virtual void OnCollisionEnter(Node2D body)
        {
            if (GameManager.Instance.isMultiplayerMatch && !GameManager.Instance.isHostOfMultiplayerMatch) return;

            //we shouldnt hit ourselves, this can happen if we get this call too quickly before we set our collisionmasks and stuff.
            if (body.Name == projectileOwnerChar.Name) return;

            if(body == null) return;
            if (!body.IsInsideTree()) return;

            rigidBody.StopForces();

            rigidBody.CollisionMask = 0b1;

            if (dealtDamage) return;

            if (projectileOwnerChar.CurrentTarget != null && body.GetInstanceId() == projectileOwnerChar.CurrentTarget.GetInstanceId() && !projectileOwnerChar.IsDeadOrDestroyed)
            {
                dealtDamage = true;
                projectileOwnerChar.DealDamage();
            }
            else
            {
                if (projectileOwner == Enums.TeamOwner.TEAM_01)
                {
                    if (body.GetInstanceId() == GameManager.Instance.team02HomeBase.StaticBody.GetInstanceId())
                    {
                        //We hit the enemy's base. Possibly needs to change some variables still to make sure it works.
                        dealtDamage = true;
                        projectileOwnerChar.DealDamage();
                        DestroyObject();
                        destroyTimer = destroyTime;
                        return;
                    }
                }
                else
                {
                    if (body.GetInstanceId() == GameManager.Instance.team01HomeBase.StaticBody.GetInstanceId())
                    {
                        //We hit the enemy's base. Possibly needs to change some variables still to make sure it works.
                        dealtDamage = true;
                        projectileOwnerChar.DealDamage();
                        DestroyObject();
                        destroyTimer = destroyTime;
                        return;
                    }
                }

                //GD.Print(body.Name + " _ " + body.GetInstanceId());
                //GD.Print(body.GlobalPosition);
                try
                {
                    BaseCharacter enemyChar = body.GetNode<BaseCharacter>(body.GetPath());
                    if (enemyChar.characterOwner == projectileOwnerChar.characterOwner) return;

                    dealtDamage = true;
                    projectileOwnerChar.CurrentTarget = enemyChar;
                    projectileOwnerChar.DealDamage();
                }
                catch
                {
                    //we hit a ground tile.
                }
            }
        }

        public virtual void OnCollisionExit(Node2D body)
        {

        }
    }
}
