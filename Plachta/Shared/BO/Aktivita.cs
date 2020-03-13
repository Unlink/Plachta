using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Plachta.Shared.Annotations;

namespace Plachta.Shared.BO
{
    public class Aktivita : INotifyPropertyChanged
    {
        private string _design;
        public AktivitaSablona Sablona { get; set; }

        public string Nazov => Sablona.Nazov;

        public TimeSpan Time { get; set; }
        public TimeSpan Trvanie { get; set; }

        public List<String> Veduci { get; } = new List<string>();

        public List<AktivitaItem> AktivitaItems { get; } = new List<AktivitaItem>();

        public string Design
        {
            get => _design;
            set
            {
                if (value == _design) return;
                _design = value;
                Console.WriteLine("Changed design val");
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

    public abstract class AktivitaItem
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
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