/********************************************
Course : TGD3351 Game Algorithms
Session: Trimester 1, 2022/23
ID and Name #1 : 1191101213 RavenLimZheXuan
Contacts #1 : 601155873318 1191101213@student.mmu.edu.my
ID and Name #2 : 1181103109 EuwernYongChernJun
Contacts #2 : 60163371078 1181103109@student.mmu.edu.my
********************************************/
using Microsoft.Xna.Framework;

namespace ZombieSurvivorZ
{
    public class AmmoDisplayUI : UIText
    {

        private int lastAmmoInClip;
        private int lastAmmoReserve;

        public AmmoDisplayUI(UIBase parent, Vector2 pos, Vector2 size) : base(parent, pos, size)
        {
            Text = "-";
            Align = new(0.5f, 1f);
        }

        public void UpdateAmmoCount(int ammoInClip, int ammoReserve)
        {
            if (ammoInClip == lastAmmoInClip && ammoReserve == lastAmmoReserve)
            {
                return;
            }
            lastAmmoInClip = ammoInClip;
            lastAmmoReserve = ammoReserve;
            Text = $"{ammoInClip} / {ammoReserve}";
        }

        public void WeaponNotActive()
        {
            Text = "-";
        }
        public void WeaponActive()
        {
            Text = $"{lastAmmoInClip} / {lastAmmoReserve}";
        }

    }
}