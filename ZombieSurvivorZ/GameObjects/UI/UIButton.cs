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
    public class UIButton : UIBase
    {

        public UIBorder border;
        public UIBox fill;

        public UIText text;

        public bool ClickHeld = false;
        public bool FirstClickInBounds = false;
        public bool Hovering = false;

        public float HoverAlpha = 0.2f;
        public float HeldAlpha = 0.6f;

        public event Action OnClick = () => { };

        private UIButton(UIBase parent, UIBorder border, UIBox fill) : base(parent, fill.Position, fill.Scale)
        {
            this.border = border;
            this.fill = fill;

            Color = Color.DarkGray;
        }
        private UIButton(UIBase parent, UIBorder border, UIBox fill, UIText text) : this(parent, border, fill)
        {
            this.text = text;
        }

        //Create using this to ensure button is created last in queue so it overlays border & fill
        public static UIButton Create(UIBase parent, Vector2 pos, Vector2 size)
        {
            UIBox fill = new(parent, pos, size);
            UIBorder border = new(fill, Vector2.Zero, size);
            UIButton button = new(parent, border, fill);
            return button;
        }

        public static UIButton CreateWithText(UIBase parent, Vector2 pos, Vector2 size, string text)
        {
            UIBox fill = new(parent, pos, size);
            UIBorder border = new(fill, Vector2.Zero, size);
            UIText uiText = new(fill, Vector2.Zero, size)
            {
                Text = text
            };
            UIButton button = new(parent, border, fill, uiText);
            return button;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update()
        {
            base.Update();

            if (MouseWithinBounds())
            {
                Hovering = true;

                if (Input.IsLMouseDownUI())
                {

                    if (Input.IsLMouseFirstDownUI())
                    {
                        FirstClickInBounds = true;
                    }

                    if (FirstClickInBounds)
                    {
                        ClickHeld = true;
                    }
                }
                else
                {
                    if (Input.IsLMouseFirstUpUI() && ClickHeld)
                    {
                        OnClick.Invoke();
                    }
                    FirstClickInBounds = false;
                    ClickHeld = false;
                }
            }
            else
            {
                Hovering = false;
            }
            if (Input.IsLMouseUpUI())
            {
                FirstClickInBounds = false;
                ClickHeld = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (ClickHeld)
            {
                Alpha = HeldAlpha;
            }
            else if (Hovering)
            {
                Alpha = HoverAlpha;
            }
            else
            {
                return;
            }
            base.Draw(spriteBatch);
        }

    }
}