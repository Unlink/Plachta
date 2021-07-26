using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using Plachtovac.Shared.BO.GraphicsBlocks;

namespace Plachtovac.Shared.BO
{
    public class AktivitaSablona : INotifyPropertyChanged, IAktivita
    {
        public string Nazov { get; set; }

        public string Typ { get; set; }

        public TimeSpan Trvanie { get; set; }

        public bool VeduciSPopisom { get; set; }

        public List<AktivitaVeduci> Veduci
        {
            get => _veduci;
            set
            {
                if (value != _veduci)
                {
                    _veduci = value;
                    Design = PrerenderujDesign(_veduci);
                }
            }
        }

        public List<GraphicsItem> AktivitaItems { get; } = new List<GraphicsItem>();

        private string _design;
        private List<AktivitaVeduci> _veduci = new List<AktivitaVeduci>();

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

        public string PrerenderujDesign(List<AktivitaVeduci> veduci)
        {
            var zoznamVeducich = AktivitaItems.Select(i => i as ZoznamVeducichTextGraphicsItem)
                .Where(i => i != null);
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(Design);
                var root = doc.DocumentElement;
                foreach (var item in zoznamVeducich)
                {
                    var riadky = item.VygenerujZoznamVeducich(veduci, VeduciSPopisom);
                    foreach (var node in root.ChildNodes)
                    {
                        if (node is XmlElement el && el.Name == "g" &&
                            el.Attributes.GetNamedItem("id")?.Value == item.Id)
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

                return doc.InnerXml;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return Design;
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