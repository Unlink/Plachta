using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Plachta.Shared.Annotations;

namespace Plachta.Shared.BO
{
    public class AktivitaSablona : INotifyPropertyChanged
    {
        public string Nazov { get; set; }

        public string Typ { get; set; }

        public TimeSpan Trvanie { get; set; }

        public List<AktivitaVeduci> Veduci { get; set; } = new List<AktivitaVeduci>();

        public List<AktivitaItem> AktivitaItems { get; } = new List<AktivitaItem>();

        private string _design;
        public string Design
        {
            get => _design;
            set
            {
                if (value == _design) return;
                _design = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}