using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static ZombieSurvivorZ.Collision;

namespace ZombieSurvivorZ
{
    public class Zombie : SpriteColliderObject
    {

        public enum State
        {
            Chase,
            Attack,
            Die
        }

        private const float BaseColliderSize = 24;

        private const float AnimSpeed = 0.1f;

        private const float AttackDistance = 16;
        private const float AttackDistanceCompensation = 4;
        private const float AttackTime = AnimSpeed * 9;
        private const float AttackDealDamageTime = AnimSpeed * 5;

        private const float MovementSpeed = 100;
        private const int MaxHealth = 20;

        private const int Damage = 5;



        private int health;

        private static float ClosestDistanceToPlayer => Game1.Player.ColliderSize + BaseColliderSize;

        private readonly Texture2D[] ChasingAnim;
        private readonly Texture2D[] AttackingAnim;

        private Texture2D[] Anim;
        private int animIndex;
        private float animSpeedCount;
        private bool animRepeat;


        private readonly Stack<Vector2Int> Path = new();
        private bool hasNextTargetPos = false;
        private Vector2 nextTargetPos = new(float.MinValue, float.MinValue);

        private bool nextTargetIsDoor;
        private Vector2Int doorCell;

        private readonly GameObject[] IgnoredObjects = new GameObject[1];

        private bool wasSeeingPlayer = false;

        private Vector2Int lastCellPos = new(int.MinValue, int.MinValue);
        public Vector2Int CellPosition => lastCellPos;

        private State CurrentState = State.Chase;

        private Vector2 attackDirection;
        private float attackTimeCount = 0;
        private bool dealtDamageThisAttack;

        public Zombie()
        {
            health = MaxHealth;
            Scale = new(0.25f, 0.25f);
            CL = new CircleDynamicCollider(this, BaseColliderSize);
            IgnoredObjects[0] = this;

            ChasingAnim = new Texture2D[17];
            PopulateAnim(ChasingAnim, "Zombie/skeleton-move_");

            AttackingAnim = new Texture2D[9];
            PopulateAnim(AttackingAnim, "Zombie/skeleton-attack_");

            EnterChaseState(false);
            CurrentState = State.Chase;

            Texture = ChasingAnim[0];   //Safe
        }

        ~Zombie()
        {
            Console.WriteLine("Zombie released from memory");
        }

        private static void PopulateAnim(Texture2D[] anim, string name)
        {
            for (int i = 0; i < anim.Length; i++)
            {
                anim[i] = Game1.GetTexture(name + i);
            }
        }

        public override void Initialize()
        {

        }

        public void DealDamage(int damage, Vector2 direction)
        {
            Game1.BloodManager.AddBlood(Position, MathF.Atan2(direction.Y, direction.X));
            health -= damage;
            if (health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            CL.DestroyCollider();
            CL = null;
            ChangeState(State.Die);
            Game1.MapManager.ZombieSpawnLayer.ZombieDied(this);
            Destroy();
        }

        private void CalculatePathToPlayer()
        {
            Game1.MapManager.Pathfind(CellPosition, Game1.Player.CellPosition, Path);
            SetNextTargetPos();
        }

        private void SetAnim(Texture2D[] anim)
        {
            Anim = anim;
        }

        private void UpdateAnim()
        {
            animSpeedCount += Time.deltaTime;
            if (animSpeedCount < AnimSpeed)
            {
                return;
            }

            animSpeedCount = 0;
            animIndex++;

            if (animIndex >= Anim.Length)
            {
                if (!animRepeat)
                {
                    return;
                }
                animIndex = 0;
            }
            Texture = Anim[animIndex];
        }


        private void ChangeState(State state)
        {
            switch (CurrentState)
            {
                case State.Chase:
                    break;
                case State.Attack:
                    break;
                case State.Die:
                    break;
            }
            CurrentState = state;
            switch (state)
            {
                case State.Chase:
                    EnterChaseState();
                    break;
                case State.Attack:
                    EnterAttackState();
                    break;
                case State.Die:
                    EnterDieState();
                    break;
            }

            animSpeedCount = 0;
            animIndex = 0;
        }

        public override void Update()
        {
            base.Update();

            Vector2Int zombieCell = Game1.MapManager.PositionToLocal(Position);
            if (zombieCell != lastCellPos)
            {
                lastCellPos = zombieCell;
            }

            switch (CurrentState)
            {
                case State.Chase:
                    ChaseUpdate();
                    break;
                case State.Attack:
                    AttackUpdate();
                    break;
                case State.Die:
                    break;
            }
            UpdateAnim();
        }

        private void EnterChaseState(bool updatePath = true)
        {
            SetAnim(ChasingAnim);
            animRepeat = true;

            if (updatePath)
            {
                CalculatePathToPlayer();
            }
        }

        private void EnterAttackState()
        {
            SetAnim(AttackingAnim);
            animRepeat = false;
            dealtDamageThisAttack = false;
            attackTimeCount = 0;
            Heading = attackDirection;
        }

        private void EnterDieState()
        {
            Active = false;
            Game1.Player.ZombieKilledAddMaterials();
        }

        private void ChaseUpdate()
        {

            if (nextTargetIsDoor)
            {
                if (Game1.MapManager.DoorsLayer.TryGetClosedDoor(doorCell, out _))
                {
                    attackDirection = (Game1.MapManager.LocalToTileCenterPosition(doorCell) - Position).NormalizedCopy();
                    Console.WriteLine("Attack Door");
                    ChangeState(State.Attack);
                    return;
                }
            }

            Vector2 directionToPlayer = Game1.Player.Position - Position;
            float distance = directionToPlayer.Length();
            directionToPlayer /= distance;

            bool seesPlayer = PlayerCheckUpdate(directionToPlayer);
            if (seesPlayer)
            {

                float attackDistance = ClosestDistanceToPlayer + AttackDistance;
                if (distance < attackDistance)
                {
                    attackDirection = directionToPlayer;
                    Console.WriteLine("Attack Player");
                    ChangeState(State.Attack);
                    return;
                }

                CheckNextTargetIsDoor();
                Position += MovementSpeed * Time.deltaTime * directionToPlayer;
                Heading = directionToPlayer;
                wasSeeingPlayer = true;
            }
            else
            {
                if (wasSeeingPlayer)
                {
                    CalculatePathToPlayer();
                    wasSeeingPlayer = false;
                    return;
                }
                PathfindUpdate();
            }
        }

        private void AttackUpdate()
        {
            attackTimeCount += Time.deltaTime;

            if (!dealtDamageThisAttack && attackTimeCount > AttackDealDamageTime)
            {
                dealtDamageThisAttack = true;
                TryDealDamage();
            }

            if (attackTimeCount > AttackTime)
            {
                // Finish Attack
                ChangeState(State.Chase);
            }
        }

        private void TryDealDamage()
        {

            if (nextTargetIsDoor)
            {
                Vector2Int doorDistance = CellPosition - doorCell;
                if (Math.Abs(doorDistance.X * doorDistance.Y) <= 1)
                {
                    //Hit
                    if (Game1.MapManager.DoorsLayer.TryGetDoor(doorCell, out DoorsMap.Door door))
                    {
                        HitDoor(door);
                    }
                    else
                    {
                        //Impossible?
                        Console.WriteLine("HuH/?");
                    }
                }
            }

            if (Raycast(Position, attackDirection, IgnoredObjects, out Collider collider, out float hitDistance))
            {
                if (collider.Go == Game1.Player)
                {
                    float requiredDistance = AttackDistance + AttackDistanceCompensation + BaseColliderSize;
                    if (hitDistance < requiredDistance)
                    {
                        HitPlayer(attackDirection);
                    }
                    Console.WriteLine($"{hitDistance < requiredDistance}, Distance: {hitDistance}, Required: {requiredDistance}");
                }
            }

        }

        private void HitDoor(DoorsMap.Door door)
        {
            if (door.IsOpen)
            {
                //No damage when it's open
                return;
            }
            Game1.MapManager.DoorsLayer.DealDamage(doorCell, Damage);
        }

        private void HitPlayer(Vector2 direction)
        {
            Game1.Player.DealDamage(Damage, direction);
        }

        private bool PlayerCheckUpdate(Vector2 directionToPlayer)
        {
            if (RaycastAll(Position, directionToPlayer, IgnoredObjects, out Collider collider, out _))
            {
                if (collider.Go == Game1.Player)
                {
                    return true;
                }
            }
            return false;
        }

        private void PathfindUpdate()
        {
            if (hasNextTargetPos)
            {
                Vector2 direction = nextTargetPos - Position;
                float distance = direction.Length();
                if (distance == 0)
                {
                    SetNextTargetPos();
                    return;
                }
                direction /= distance;  //Normalize
                Heading = direction;

                if (distance > 100)
                {
                    CalculatePathToPlayer();
                    return;
                }

                float move = MovementSpeed * Time.deltaTime;
                if (distance < move)
                {
                    SetNextTargetPos();
                }

                Vector2 movement = direction * move;
                Position += movement;
            }
            else
            {
                CalculatePathToPlayer();
            }
        }

        private void SetNextTargetPos()
        {
            if (Path.TryPop(out Vector2Int cell))
            {
                if (cell == CellPosition)
                {
                    SetNextTargetPos();
                    return;
                }
                nextTargetIsDoor = Game1.MapManager.DoorsLayer.TryGetDoor(cell, out _);
                if (nextTargetIsDoor)
                {
                    doorCell = cell;
                }
                nextTargetPos = Game1.MapManager.LocalToTileCenterPosition(cell);
                hasNextTargetPos = true;
                return;
            }
            hasNextTargetPos = false;
        }

        private void CheckNextTargetIsDoor()
        {
            Vector2 checkPos = Position + Heading * (BaseColliderSize + 16);
            Vector2Int checkCell = Game1.MapManager.PositionToLocal(checkPos);
            nextTargetIsDoor = Game1.MapManager.DoorsLayer.TryGetDoor(checkCell, out _);
            if (nextTargetIsDoor)
            {
                doorCell = checkCell;
            }
        }

        public override void OnCollision(DynamicCollider current, Collider other, Vector2 penetrationVector)
        {
            OnCollision_PushBack(current, other, penetrationVector);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);


            if (Game1.CollisionDebugging && CL != null)
            {
                spriteBatch.DrawCircle((CircleF)CL.Bounds, 20, Color.Red);
            }

            if (Time.frameCount == 0)
            {
                return;
            }

            //Stack<Vector2Int> path = new(Path);
            //Vector2Int firstPos = Game1.MapManager.PositionToLocal(Position);
            //while (path.TryPop(out Vector2Int pos))
            //{
            //    spriteBatch.DrawLine(Game1.MapManager.LocalToTileCenterPosition(firstPos.X, firstPos.Y), Game1.MapManager.LocalToTileCenterPosition(pos.X, pos.Y), Color.Yellow, 2);
            //    firstPos = pos;
            //}
        }

    }
}