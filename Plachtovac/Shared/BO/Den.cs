using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Plachtovac.Shared.BO
{
    public class Den : INotifyPropertyChanged
    {
        private Veduci _veduci;
        public int Poradie { get; internal set; }
        public bool MaAktivity => Aktivity.Any();
        public List<Aktivita> Aktivity { get; } = new List<Aktivita>();

        public Veduci Veduci
        {
            get => _veduci;
            set
            {
                if (Equals(value, _veduci)) return;
                _veduci = value;
                OnPropertyChanged();
            }
        }

        public Den(int poradie)
        {
            Poradie = poradie;
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
            return Koliduje(aktivita.Time, null, aktivita.Trvanie);
        }

        public bool Koliduje(TimeSpan novyZaciatok, Aktivita aktivita, TimeSpan noveTrvanie = default)
        {
            if (noveTrvanie == default)
            {
                noveTrvanie = aktivita.Trvanie;
            }
            foreach (var a in Aktivity)
            {
                if (a != aktivita && a.Time < novyZaciatok.Add(noveTrvanie) && novyZaciatok < a.Time.Add(a.Trvanie))
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