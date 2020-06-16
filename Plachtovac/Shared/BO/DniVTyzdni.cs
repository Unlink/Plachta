namespace Plachtovac.Shared.BO
{
    public class DniVTyzdni
    {
        private static string[] _dniVTyzdni = new[]
            {"Neďeľa", "Pondelok", "Utorok", "Streda", "Štvrtok", "Piatok", "Sobota"};

        public static string[] GetAll()
        {
            return _dniVTyzdni;
        }

        public static string GetDen(int poradie)
        {
            return _dniVTyzdni[poradie];
        }
    }
}
