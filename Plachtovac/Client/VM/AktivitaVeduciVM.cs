using Plachtovac.Shared.BO;

namespace Plachtovac.Client.VM
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
