using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Plachta.Shared.Annotations;

namespace Plachta.Shared.BO
{
    public class Den : INotifyPropertyChanged
    {
        public int Poradie { get; internal set; }
        public bool MaAktivity => Aktivity.Any();
        public List<Aktivita> Aktivity { get; } = new List<Aktivita>();

        public Den(int poradie)
        {
            Poradie = poradie;
            Aktivity.Add(new Aktivita(){Trvanie = new TimeSpan(1, 0, 0), Time = new TimeSpan(12,10,0), Sablona = new AktivitaSablona(){Nazov = "Test"}});
        }


        public void PridajAktivitu(Aktivita aktivita)
        {
            if (Koliduje(aktivita))
            {
                throw new InvalidOperationException("Nemozem pridat aktivitu, lebo koliduje s inou aktivitou");
            }
            Aktivity.Add(aktivita);
        }

        public bool Koliduje(Aktivita aktivita)
        {
            return false;
        }

        public bool Koliduje(TimeSpan novyZaciatok, Aktivita aktivita)
        {
            foreach (var a in Aktivity)
            {
                if (a != aktivita && a.Time < novyZaciatok.Add(aktivita.Trvanie) && novyZaciatok < a.Time.Add(a.Trvanie))
                {
                    return true;
                }
            }
            return false;
        }

        public bool OdstranAktivitu(Aktivita aktivita)
        {
            if (Aktivity.Remove(aktivita))
            {
                OnPropertyChanged(nameof(Aktivity));
                return true;
            }

            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}