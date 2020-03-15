using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plachta.Shared.BO
{
    public class Rozvrh
    {

        public int Zaciatok { get; set; }

        public string ZaciatokStr => DniVTyzdni.GetDen(Zaciatok);

        public List<AktivitaSablona> SablonyAktivit { get; } = new List<AktivitaSablona>();

        public LinkedList<Den> Dni { get; } = new LinkedList<Den>();

        public TimeSpan ZaciatokDna { get; set; }
        public TimeSpan KoniecDna { get; set; }

        public List<Veduci> Veduci { get; set; } = new List<Veduci>();

        public int PocetDni => Dni.Count;
        public string Nazov { get; set; }


        public void PresunAktivitu(Aktivita aktivita, Den den, TimeSpan timeSpan)
        {
            if (!den.Koliduje(timeSpan, aktivita)) { 
                OdstranAktivitu(aktivita);
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
        }

        public bool Koliduje(Aktivita aktivita, TimeSpan time, TimeSpan trvanie)
        {
            var den = Dni.FirstOrDefault(d => d.Aktivity.Contains(aktivita));
            if (den == null || den.Koliduje(time, aktivita, trvanie)) return true;
            return false;
        }
    }
}
