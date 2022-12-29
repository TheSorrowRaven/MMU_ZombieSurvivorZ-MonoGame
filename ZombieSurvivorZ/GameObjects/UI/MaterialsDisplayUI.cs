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
    public class MaterialsDisplayUI : UIText
    {

        public MaterialsDisplayUI(UIBase parent, Vector2 pos, Vector2 size) : base(parent, pos, size)
        {
            Text = "000";
            Align = new(0.5f, 0f);
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
            Text = text;
        }

    }
}