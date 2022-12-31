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
    public class MaterialsDisplayUI : UIBox
    {

        public UIText MaterialsText;
        public UIBox WarningBox;

        private bool isWarning;
        private float warningTimeCount;
        private float warningTime = 1f;
        private int warningIntervals = 8;

        public MaterialsDisplayUI(UIBase parent, Vector2 pos, Vector2 size) : base(parent, pos, size)
        {
            WarningBox = new(this, Vector2.Zero, size);
            MaterialsText = new(this, Vector2.Zero, size);

            MaterialsText.Text = "000";
            MaterialsText.Align = new(0.5f, 0f);

            WarningBox.Color = Color.Red;
            WarningBox.Alpha = 0;

            Active = false;
        }

        public override void SetActive(bool active)
        {
            base.SetActive(active);
            MaterialsText.SetActive(active);
        }

        public void UpdateMaterials(int materials)
        {
            string text = materials.ToString();
            if (materials < 10)
            {
                text = "00" + text;
            }
            else if (materials < 100)
            {
                text = "0" + text;
            }
            MaterialsText.Text = text;
        }

        public override void Update()
        {
            base.Update();
            if (!isWarning)
            {
                return;
            }
            warningTimeCount -= Time.deltaTime;
            if (warningTimeCount < 0)
            {
                isWarning = false;
                WarningBox.Alpha = 0;
                return;
            }
            float sinVal = warningTimeCount / (warningTime / warningIntervals);
            float alpha = (MathF.Sin(sinVal) + 1) / 2;
            WarningBox.Alpha = alpha;
        }

        public void WarnInsufficientMaterials()
        {
            isWarning = true;
            warningTimeCount = warningTime;
            RectangleF rect = MaterialsText.GetTextLocalBounds();
            WarningBox.Position = rect.TopLeft - new Vector2(5, 0);
            WarningBox.Size = (Vector2)rect.Size + new Vector2(10, 0);

        }

    }
}