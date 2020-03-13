using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plachta.Shared.BO
{
    public class Rozvrh
    {

        public int Zaciatok { get; set; }

        public List<AktivitaSablona> SablonyAktivit { get; } = new List<AktivitaSablona>();

        public LinkedList<Den> Dni { get; } = new LinkedList<Den>();

        public TimeSpan ZaciatokDna { get; set; }
        public TimeSpan KoniecDna { get; set; }

        public int PocetDni => Dni.Count;

        public void PridajDen()
        {
            var poradie = PocetDni == 0 ? Zaciatok : (Dni.Last().Poradie + 1) % 7;
            Dni.AddLast(new Den(poradie));
        }

        public void OdoberDen()
        {
            if (PocetDni > 0)
            {
                if (Dni.Last().MaAktivity)
                {
                    throw new InvalidOperationException("Nemozem zmazat den, ktory ma aktivity");
                }
                Dni.RemoveLast();
            }
        }

        public void PresunAktivitu(Aktivita aktivita, Den den, TimeSpan timeSpan)
        {
            OdstranAktivitu(aktivita);
            den.PridajAktivitu(aktivita);
            aktivita.Time = timeSpan;
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
    }
}
