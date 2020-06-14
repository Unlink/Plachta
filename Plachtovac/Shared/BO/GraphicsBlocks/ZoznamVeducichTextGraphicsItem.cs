using System;
using System.Collections.Generic;

namespace Plachtovac.Shared.BO.GraphicsBlocks
{
    public class ZoznamVeducichTextGraphicsItem : TextGraphicsItem
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
}