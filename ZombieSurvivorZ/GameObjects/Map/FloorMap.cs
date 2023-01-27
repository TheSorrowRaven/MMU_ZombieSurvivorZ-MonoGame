/********************************************
Course : TGD3351 Game Algorithms
Session: Trimester 1, 2022/23
ID and Name #1 : 1191101213 RavenLimZheXuan
Contacts #1 : 601155873318 1191101213@student.mmu.edu.my
ID and Name #2 : 1181103109 EuwernYongChernJun
Contacts #2 : 60163371078 1181103109@student.mmu.edu.my
********************************************/
using MonoGame.Extended.Tiled;

namespace ZombieSurvivorZ
{
    public class FloorMap : BaseMap
    {

        public override World World => World.floor;

        public FloorMap(TiledMapTileLayer layer) : base(layer)
        {

        }

        public override bool IsTileWalkable(ushort x, ushort y, out float cost)
        {
            cost = 0;
            return true;
        }
    }
}