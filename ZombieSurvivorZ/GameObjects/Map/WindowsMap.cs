﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Timers;
using MonoGame.Extended;
using System;
using System.Runtime.CompilerServices;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace ZombieSurvivorZ
{
    public class WindowsMap : BaseMap
    {

        public WindowsMap(TiledMapTileLayer layer) : base(layer)
        {

        }

    }
}