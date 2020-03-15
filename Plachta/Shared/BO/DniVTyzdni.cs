using System;
using System.Collections.Generic;
using System.Text;

namespace Plachta.Shared.BO
{
    public class DniVTyzdni
    {
        private static string[] _dniVTyzdni = new[]
            {"Pondelok", "Utorok", "Streda", "Štvrtok", "Piatok", "Sobota", "Neďeľa"};

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
