/********************************************
Course : TGD3351 Game Algorithms
Session: Trimester 1, 2022/23
ID and Name #1 : 1191101213 RavenLimZheXuan
Contacts #1 : 601155873318 1191101213@student.mmu.edu.my
ID and Name #2 : 1181103109 EuwernYongChernJun
Contacts #2 : 60163371078 1181103109@student.mmu.edu.my
********************************************/
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ZombieSurvivorZ
{
    public class TimerDisplayUI : UIBox
    {

        public readonly UIText timer;

        public TimerDisplayUI(UIBase parent, Vector2 pos, Vector2 size) : base(parent, pos, size)
        {
            timer = new(this, new(0, 0), size);
            timer.Align = new(0.5f, 0.25f);
        }

        public void SetTimer(TimeSpan time)
        {
            timer.Text = $"{PadInt(time.Seconds)} : {PadInt(time.Milliseconds / 10)}";
        }
        public void SetTimerText(string text)
        {
            timer.Text = text;
        }
        public void SetTimerColor(Color color)
        {
            timer.Color = color;
        }

        public static string PadInt(int value)
        {
            if (value >= 10)
            {
                return value.ToString();
            }
            return "0" + value.ToString();
        }

        public override void SetActive(bool active)
        {
            base.SetActive(active);
            timer.SetActive(active);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

        }


        public override void Destroy()
        {
            base.Destroy();
            timer.Destroy();
        }
    }
}