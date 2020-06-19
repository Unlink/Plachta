using System.Collections.Generic;

namespace Plachtovac.Shared.BO
{
    public interface IAktivita
    {
        List<AktivitaVeduci> Veduci { get; set; }
        string Nazov { get; }
    }
}