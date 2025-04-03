namespace Interfaces
{
    public interface IPuzzleBlock
    {
        public void InitCat(int catNumber, int posX, int posY, bool horiBomb, bool vertiBomb, bool colorBomb);
        public void PlayDestroyed();

        public void SetBlock(int posX, int posY);
    }
}