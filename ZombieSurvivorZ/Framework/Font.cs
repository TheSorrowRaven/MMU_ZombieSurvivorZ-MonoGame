using Microsoft.Xna.Framework.Graphics;

namespace ZombieSurvivorZ
{

    public static class Font
    {

        public static SpriteFont font6;
        public static SpriteFont font8;
        public static SpriteFont font10;
        public static SpriteFont font12;
        public static SpriteFont font14;
        public static SpriteFont font16;
        public static SpriteFont font18;

        public static SpriteFont[] fonts;

        public static SpriteFont Smallest => font6;
        public static SpriteFont Biggest => font18;

        public static void Load()
        {
            font6 = Game1.GetContent<SpriteFont>("font-6");
            font8 = Game1.GetContent<SpriteFont>("font-8");
            font10 = Game1.GetContent<SpriteFont>("font-10");
            font12 = Game1.GetContent<SpriteFont>("font-12");
            font14 = Game1.GetContent<SpriteFont>("font-14");
            font16 = Game1.GetContent<SpriteFont>("font-16");
            font18 = Game1.GetContent<SpriteFont>("font-18");

            fonts = new SpriteFont[]
            {
                font6, font8, font10, font12, font14, font16, font18
            };
        }

        public static SpriteFont Smaller(SpriteFont font)
        {
            for (int i = 0; i < fonts.Length; i++)
            {
                if (fonts[i] == font)
                {
                    if (i == 0)
                    {
                        return font;
                    }
                    return fonts[i - 1];
                }
            }
            return font;
        }

        public static SpriteFont Bigger(SpriteFont font)
        {
            for (int i = 0; i < fonts.Length; i++)
            {
                if (fonts[i] == font)
                {
                    if (i == fonts.Length - 1)
                    {
                        return font;
                    }
                    return fonts[i + 1];
                }
            }
            return font;
        }

    }

}