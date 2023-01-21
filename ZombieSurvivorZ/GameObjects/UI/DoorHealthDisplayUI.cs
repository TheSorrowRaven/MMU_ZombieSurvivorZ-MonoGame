using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZombieSurvivorZ
{
    public class DoorHealthDisplayUI : UIBox
    {

        private readonly UIBorderedBox[] Boxes = new UIBorderedBox[3];
        private readonly UIBorderedBox BarricadeProgress;

        private bool doorSet = false;
        private Vector2 targetPos;
        private DoorsMap.Door targetDoor;

        private bool barricading = false;
        private float barricadeProgress;

        public DoorHealthDisplayUI(UIBase parent, Vector2 pos, Vector2 size) : base(parent, pos, size)
        {
            Boxes[0] = new(this, new(0, 0), new(20, 20));
            Boxes[1] = new(this, new(0, 0), new(20, 20));
            Boxes[2] = new(this, new(0, 0), new(20, 20));

            BarricadeProgress = new(this, new(1, -6), new(62, 6));
            BarricadeProgress.Box.Color = Color.Blue;
            BarricadeProgress.Color = Color.Black;

            for (int i = 0; i < Boxes.Length; i++)
            {
                UIBorderedBox box = Boxes[i];
                box.Color = Color.Green;
                box.Box.Color = Color.Green;
            }

            Game1.Camera.OnCameraUpdated += Camera_OnCameraUpdated;
        }

        private void Camera_OnCameraUpdated()
        {
            if (!doorSet)
            {
                return;
            }

            doorSet = false;
            Vector2 uiPos = Game1.Camera.WorldToScreen(targetPos);
            Position = uiPos;

            int level = targetDoor.GetBarricadeLevel();

            int i = 0;
            for (; i < level; i++)
            {
                float scale = 1;
                if (i + 1 == level)
                {
                    float value = targetDoor.BarricadeHealth % DoorsMap.Door.BarricadeLevelMaxHealth;
                    if (value == 0)
                    {
                        scale = 1;
                    }
                    else
                    {
                        scale = value / DoorsMap.Door.BarricadeLevelMaxHealth;
                    }
                }
                Vector2 pos = new();
                Vector2 sca = new(20, 20);
                if (targetDoor.Rotated)
                {
                    pos.Y = 21 * i + 1;
                    sca.Y = 20 * scale;
                }
                else
                {
                    pos.X = 21 * i + 1;
                    sca.X = 20 * scale;
                }
                Boxes[i].Position = pos;
                Boxes[i].Box.Size = sca;
                Boxes[i].SetActive(true);
            }
            for (; i < Boxes.Length; i++)
            {
                Boxes[i].SetActive(false);
            }


            if (barricading)
            {
                BarricadeProgress.SetActive(true);
                BarricadeProgress.Box.Size = new(62 * barricadeProgress, 6);
                barricading = false;
            }
            else
            {
                BarricadeProgress.SetActive(false);
            }


        }

        public void SetDoor(DoorsMap.Door door, Vector2 doorPos)
        {
            targetDoor = door;
            targetPos = doorPos;
            doorSet = true;
        }

        public void SetBarricadeStatus(float progress, float max)
        {
            barricading = true;
            barricadeProgress = progress / max;
        }

        public override void SetActive(bool active)
        {
            if (Active == active)
            {
                return;
            }

            base.SetActive(active);
            for (int i = 0; i < Boxes.Length; i++)
            {
                Boxes[i].SetActive(active);
            }
            if (active)
            {
                BarricadeProgress.SetActive(barricading);
            }
            else
            {
                BarricadeProgress.SetActive(false);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

        }


        public override void Destroy()
        {
            base.Destroy();
            BarricadeProgress.Destroy();
            for (int i = 0; i < Boxes.Length; i++)
            {
                Boxes[i].Destroy();
            }
        }
    }
}