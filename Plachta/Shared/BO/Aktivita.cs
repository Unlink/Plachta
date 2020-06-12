using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using Plachta.Shared.Annotations;

namespace Plachta.Shared.BO
{
    public class Aktivita : INotifyPropertyChanged
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
                    _sablona.PropertyChanged += (sender, args) => OnPropertyChanged(nameof(Sablona));
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

        private string _design = null;
        private List<AktivitaVeduci> _veduci;

        public string Design
        {
            get
            {
                if (Veduci == null) return Sablona.Design;
                if (_design != null) return _design;

                var zoznamVeducich = Sablona.AktivitaItems.Select(i => i as ZoznamVeducichTextAktivitaItem)
                    .Where(i => i != null);
                try
                {
                    var doc = new XmlDocument();
                    doc.LoadXml(Sablona.Design);
                    var root = doc.DocumentElement;
                    foreach (var item in zoznamVeducich)
                    {
                        var riadky = item.VygenerujZoznamVeducich(Veduci);
                        foreach (var node in root.ChildNodes)
                        {
                            if (node is XmlElement el && el.Name == "g" && el.Attributes.GetNamedItem("id")?.Value == item.Id)
                            {
                                var cnt = 0;
                                var textElements = el.GetElementsByTagName("tspan");
                                foreach (var f in textElements)
                                {
                                    var l = (XmlElement)f;
                                    l.InnerText = riadky[cnt++];
                                }
                            }
                        }
                    }

                    _design = doc.InnerXml;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }

                return _design;
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
        public string Id { get; set; }
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

    public class ZoznamVeducichTextAktivitaItem : TextAktivitaItem
    {
        private int _pocetRiadkov;

        public int PocetRiadkov
        {
            get => _pocetRiadkov;
            set
            {
                if (value == _pocetRiadkov) return;
                _pocetRiadkov = value;
                OnPropertyChanged();
            }
        }

        public void NastavZoznamVeducich(List<AktivitaVeduci> veduci)
        {
            Text = string.Join('\n', VygenerujZoznamVeducich(veduci));
            Console.WriteLine(Text);
        }

        public string[] VygenerujZoznamVeducich(List<AktivitaVeduci> veduci)
        {
            var veducichNaRiadok = (int)Math.Ceiling(veduci.Count / (double)PocetRiadkov);
            var riadky = new string[PocetRiadkov];
            for (int i = 0; i < PocetRiadkov; i++)
            {
                riadky[i] = "";
            }

            for (int i = 0; i < veduci.Count; i++)
            {
                riadky[i / veducichNaRiadok] += veduci[i].Veduci.Prezyvka + ", ";
            }

            for (int i = 0; i < PocetRiadkov; i++)
            {
                riadky[i] = riadky[i].Trim(',', ' ');
            }

            return riadky;
        }
    }

    public class TextAktivitaItem : AktivitaItem, INotifyPropertyChanged
    {
        private string _text;
        private string _fill;
        private string _fontFamily;

        public string Text
        {
            get => _text;
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged();
            }
        }

        public string Fill
        {
            get => _fill;
            set
            {
                if (value == _fill) return;
                _fill = value;
                OnPropertyChanged();
            }
        }

        public string FontFamily
        {
            get => _fontFamily;
            set
            {
                if (value == _fontFamily) return;
                _fontFamily = value;
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

    public class ObrazokAktivitaItem : AktivitaItem
    {
        public string Image { get; set; }

    }

}