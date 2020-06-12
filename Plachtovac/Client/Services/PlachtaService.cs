using System;
using Plachtovac.Shared.BO;

namespace Plachtovac.Client.Services
{
    public class PlachtaService
    {
        public Rozvrh AktualnyRozvrh { get; set; } = new Rozvrh() { ZaciatokDna = new TimeSpan(8, 0, 0), KoniecDna = new TimeSpan(22, 0, 0) };
    }
}
