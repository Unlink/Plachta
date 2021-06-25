using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using Plachtovac.Shared.BO.GraphicsBlocks;

namespace Plachtovac.Shared.BO
{
    public class Aktivita : INotifyPropertyChanged, IAktivita
    {
        private TimeSpan _time;
        private AktivitaSablona _sablona;

        public AktivitaSablona Sablona
        {
            get => _sablona;
            set
            {
                if (_sablona != value)
                {
                    _sablona = value;
                    _sablona.PropertyChanged += (sender, args) =>
                    {
                        OnPropertyChanged(nameof(Sablona));
                        _design = null;
                    };
                }
            }
        }

        public string Nazov => Sablona.Nazov;

        public TimeSpan Time
        {
            get => _time;
            set
            {
                OnPropertyChanged();
                _time = value;
            }
        }

        public TimeSpan Trvanie { get; set; }

        public List<AktivitaVeduci> Veduci
        {
            get => _veduci;
            set
            {
                if (Equals(value, _veduci)) return;
                _veduci = value;
                _design = null;
            }
        }

        public bool ZobrazCas { get; set; }
        public bool ZobrazCasKonca { get; set; }

        private string _design = null;
        private List<AktivitaVeduci> _veduci;

        public string Design
        {
            get
            {
                if (Veduci == null) return Sablona.Design;
                if (_design != null) return _design;

                _design = Sablona.PrerenderujDesign(_veduci);
                return _design;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private AktivitaVeduci GetAktivitaVeduci(Veduci veduci)
        {
            return (
                Veduci?.FirstOrDefault(v => v.Veduci == veduci) ??
                Sablona.Veduci?.FirstOrDefault(v => v.Veduci == veduci)
                );
        }

        public bool JeVeduci(Veduci veduci)
        {
            return GetAktivitaVeduci(veduci) != null;
        }

        public string GetPopisVeduceho(Veduci veduci)
        {
            return GetAktivitaVeduci(veduci)?.Popis ?? "";
        }
    }
}