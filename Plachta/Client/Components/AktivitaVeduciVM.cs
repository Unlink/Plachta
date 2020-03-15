using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Plachta.Shared.BO;

namespace Plachta.Client.Components
{
    public class AktivitaVeduciVM
    {
        public AktivitaVeduciVM(Veduci veduci)
        {
            Veduci = veduci;
        }

        public Veduci Veduci { get; private set; }

        public bool Selected { get; set; }
        public string Poznamka { get; set; }
    }
}
