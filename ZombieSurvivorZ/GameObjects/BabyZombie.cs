/********************************************
Course : TGD3351 Game Algorithms
Session: Trimester 1, 2022/23
ID and Name #1 : 1191101213 RavenLimZheXuan
Contacts #1 : 601155873318 1191101213@student.mmu.edu.my
ID and Name #2 : 1181103109 EuwernYongChernJun
Contacts #2 : 60163371078 1181103109@student.mmu.edu.my
********************************************/
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