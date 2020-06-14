namespace Plachtovac.Shared.BO.GraphicsBlocks
{
    public abstract class GraphicsItem
    {
        public string Id { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double ScaleX { get; set; }
        public double ScaleY { get; set; }
        public double Angle { get; set; }
        public bool FlipX { get; set; }
        public bool FlipY { get; set; }
    }
}