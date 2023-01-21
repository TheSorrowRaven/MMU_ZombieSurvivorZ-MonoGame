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