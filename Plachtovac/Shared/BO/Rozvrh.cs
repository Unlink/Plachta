using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Plachtovac.Shared.BO.GraphicsBlocks;

namespace Plachtovac.Shared.BO
{
    public class Rozvrh
    {
        private DateTime? _zaciatokRozvrhu;

        public DateTime? ZaciatokRozvrhu
        {
            get => _zaciatokRozvrhu;
            set
            {
                _zaciatokRozvrhu = value;
                RozvrhChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int Zaciatok => (int) (_zaciatokRozvrhu?.DayOfWeek ?? 0);

        public string ZaciatokStr => DniVTyzdni.GetDen(Zaciatok);

        public List<AktivitaSablona> SablonyAktivit { get; } = new List<AktivitaSablona>();

        public LinkedList<Den> Dni { get; } = new LinkedList<Den>();

        public TimeSpan ZaciatokDna { get; set; }
        public TimeSpan KoniecDna { get; set; }

        public List<Veduci> Veduci { get; set; } = new List<Veduci>();

        public int PocetDni => Dni.Count;
        public string Nazov { get; set; }

        public List<GraphicsItem> CustomGraphicsItems { get; } = new List<GraphicsItem>();
        public string CustomGraphicsOverlay { get; set; } = "";
        public ElementSize CustomGraphicsCanvasSize { get; set; }
        public string Program { get; set; }

        public event EventHandler RozvrhChanged; 

        public void PresunAktivitu(Aktivita aktivita, Den den, TimeSpan timeSpan)
        {
            if (!den.Koliduje(timeSpan, aktivita)) { 
                OdstranAktivitu(aktivita);

                var zaokrulovaciePointy = new[] {30, 15, 5, 1,0};

                foreach (var roundTime in zaokrulovaciePointy)
                {
                    var time = timeSpan.RoundToNearest(TimeSpan.FromMinutes(roundTime));
                    if (time >= ZaciatokDna && (time + aktivita.Trvanie) <= KoniecDna && !den.Koliduje(time, aktivita))
                    {
                        aktivita.Time = time;
                        den.PridajAktivitu(aktivita);
                        return;
                    }
                }
                aktivita.Time = timeSpan;
                den.PridajAktivitu(aktivita);
            }
        }

        public bool OdstranAktivitu(Aktivita aktivita)
        {
            foreach (var d in Dni)
            {
                if (d.OdstranAktivitu(aktivita))
                {
                    return true;
                }
            }
            return false;
        }

        public void NastavPocetDni(int dni)
        {
            var rozdiel = this.PocetDni - dni;
            if (rozdiel > 0)
            {
                for (int i = 0; i < rozdiel; i++)
                {
                    Dni.RemoveLast(); //TODO remove also from DB?
                }
            }
            else if (rozdiel < 0)
            {
                for (int i = 0; i < (-1* rozdiel); i++)
                {
                    var poradie = PocetDni == 0 ? 0 : (Dni.Last().Poradie + 1) % 7;
                    Dni.AddLast(new Den(poradie));
                }
            }
            RozvrhChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool Koliduje(Aktivita aktivita, TimeSpan time, TimeSpan trvanie)
        {
            if (time < ZaciatokDna || time + trvanie > KoniecDna) return true;
            var den = Dni.FirstOrDefault(d => d.Aktivity.Contains(aktivita));
            if (den == null || den.Koliduje(time, aktivita, trvanie)) return true;
            return false;
        }

        public void OdstranSablonu(AktivitaSablona sablona)
        {
            foreach (var aktivita in Dni.SelectMany(d => d.Aktivity).ToList())
            {
                if (aktivita.Sablona == sablona)
                {
                    OdstranAktivitu(aktivita);
                }
            }

            SablonyAktivit.Remove(sablona);
            RozvrhChanged?.Invoke(this, EventArgs.Empty);
        }

        public void OdstranVeduceho(Veduci veduci)
        {
            foreach (var aktivita in Dni.SelectMany(d => d.Aktivity).ToList())
            {
                var veduciVSablone = aktivita.Sablona.Veduci?.SingleOrDefault(v => v.Veduci == veduci);
                if (veduciVSablone != null)
                {
                    aktivita.Sablona.Veduci.Remove(veduciVSablone);
                }

                var veduciVAktivite = aktivita.Veduci?.SingleOrDefault(v => v.Veduci == veduci);
                if (veduciVAktivite != null)
                {
                    aktivita.Veduci.Remove(veduciVAktivite);
                }
            }

            foreach (var den in Dni)
            {
                if (den.Veduci == veduci)
                {
                    den.Veduci = null;
                }
            }

            Veduci.Remove(veduci);
            RozvrhChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
