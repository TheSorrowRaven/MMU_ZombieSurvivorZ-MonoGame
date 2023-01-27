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
using MonoGame.Extended;
using System.Collections.Generic;

namespace ZombieSurvivorZ
{
    public class FiringLines : GameObject
    {

        private readonly List<(Line, float)> firingLines = new();


        public FiringLines()
        {

        }

        public void CreateFiringLine(Line line, float time)
        {
            firingLines.Add((line, time));
        }

        public override void Update()
        {
            for (int i = 0; i < firingLines.Count; i++)
            {
                (Line, float) item = firingLines[i];
                float timeCount = item.Item2 - Time.deltaTime;
                if (timeCount < 0)
                {
                    firingLines.RemoveAt(i);
                    i--;
                    continue;
                }
                item.Item2 = timeCount;
                firingLines[i] = item;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            for (int i = 0; i < firingLines.Count; i++)
            {
                Line line = firingLines[i].Item1;
                spriteBatch.DrawLine(line.start, line.end, Color.Yellow);
            }
        }

    }
}