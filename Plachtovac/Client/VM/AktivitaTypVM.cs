using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plachtovac.Client.Services;
using Plachtovac.Shared.BO;

namespace Plachtovac.Client.VM
{
    public class AktivitaTypVM
    {
        public string Nazov { get; set; }
        public bool Collapsed { get; set; }
        public List<AktivitaVM> Aktivity { get; set; }
        public List<SablonaVM> Sablony { get; set; }

        public AktivitaTypVM(string nazov)
        {
            Nazov = nazov;
            Collapsed = true;
            Aktivity = new List<AktivitaVM>();
            Sablony = new List<SablonaVM>();
        }
    }

    public class AktivitaVM
    {
        public DateTime AktivitaTime { get; set; }
        public Aktivita Aktivita { get; set; }
        public string Datum => $"{DniVTyzdni.GetDen((int) AktivitaTime.DayOfWeek)} {AktivitaTime.Day}.{AktivitaTime.Month}";
        public string Nazov => Aktivita.Nazov;

        public AktivitaVM(DateTime aktivitaTime, Aktivita aktivita)
        {
            AktivitaTime = aktivitaTime;
            Aktivita = aktivita;
        }
    }

    public class SablonaVM
    {
        public AktivitaSablona Sablona { get; set; }
        public string Nazov => Sablona.Nazov;

        public SablonaVM(AktivitaSablona sablona)
        {
            Sablona = sablona;
        }
    }
}
