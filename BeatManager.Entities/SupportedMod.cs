namespace BeatManager.Entities
{
    public class SupportedMod
    {
        public string Name { get; set; }
        public int Supported { get; set; }

        public SupportedMod(string name, int supported)
        {
            Name = name;
            Supported = supported;
        }
    }
}
