namespace WildernessOverhaul
{
    public class TileObject {
        public byte Tile;
        public int Bl, Br, Tr, Tl;

        public TileObject(byte tile, int bl, int br, int tr, int tl) {
            Tile = tile;
            Bl = bl;
            Br = br;
            Tr = tr;
            Tl = tl;
        }
    }
}