using System;
using System.Collections.Generic;
using Plachta.Shared.Annotations;

namespace Plachta.Shared.BO
{
    public class Aktivita
    {
        public AktivitaSablona Sablona { get; set; }

        public string Nazov => Sablona.Nazov;

        public TimeSpan Time { get; set; }
        public TimeSpan Trvanie { get; set; }

        public List<String> Veduci { get; } = new List<string>();

        public List<AktivitaItem> AktivitaItems { get; } = new List<AktivitaItem>();
    }

    public abstract class AktivitaItem
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }

    public class TextAktivitaItem : AktivitaItem
    {
        public string Text { get; set; }
        public string Color { get; set; }

    }

    public class ObrazokAktivitaItem : AktivitaItem
    {
        public byte[] Image { get; set; }

    }

}