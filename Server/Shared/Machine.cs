namespace Server.Shared
{
    public class Machine
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public string Key { get; set; }

        public int Logs { get; set; }
        public bool Online { get; set; }
    }
}