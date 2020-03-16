using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Plachta.Shared.Annotations;

namespace Plachta.Shared.BO
{
    public class Aktivita : INotifyPropertyChanged
    {
        private TimeSpan _time;
        public AktivitaSablona Sablona { get; set; }

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

        public List<AktivitaVeduci> Veduci { get; set; }

        public bool ZobrazCas { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public abstract class AktivitaItem
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double ScaleX { get; set; }
        public double ScaleY { get; set; }
        public double Angle { get; set; }
        public bool FlipX { get; set; }
        public bool FlipY { get; set; }
    }

    public class TextAktivitaItem : AktivitaItem
    {
        public string Text { get; set; }
        public string Fill { get; set; }
        public string FontFamily { get; set; }
    }

    public class ObrazokAktivitaItem : AktivitaItem
    {
        public byte[] Image { get; set; }

    }

}