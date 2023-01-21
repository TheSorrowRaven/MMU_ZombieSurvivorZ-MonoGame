namespace ZombieSurvivorZ
{
    public class BabyZombie : Zombie
    {

        private const float BabyMovementSpeed = 400f;

        public override int MaxHealth => 10;
        public override float MovementSpeed => BabyMovementSpeed;
        public override float ColliderSize => 15f;
        public override float ScaleF => 0.15f;

    }
}