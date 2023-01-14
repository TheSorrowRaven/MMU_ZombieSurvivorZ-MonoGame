using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Timers;
using MonoGame.Extended;
using System;
using System.Runtime.CompilerServices;
using MonoGame.Extended.ViewportAdapters;

namespace ZombieSurvivorZ
{
    public class DoorHealthDisplayUI : UIBox
    {

        private readonly UIBorderedBox[] Boxes = new UIBorderedBox[3];

        private bool doorSet = false;
        private Vector2 targetPos;
        private DoorsMap.Door targetDoor;

        public DoorHealthDisplayUI(UIBase parent, Vector2 pos, Vector2 size) : base(parent, pos, size)
        {
            Boxes[0] = new(this, new(0, 0), new(20, 20));
            Boxes[1] = new(this, new(0, 0), new(20, 20));
            Boxes[2] = new(this, new(0, 0), new(20, 20));

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
        }

        public void SetDoor(DoorsMap.Door door, Vector2 doorPos)
        {
            targetDoor = door;
            targetPos = doorPos;
            doorSet = true;
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
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

        }

    }
}