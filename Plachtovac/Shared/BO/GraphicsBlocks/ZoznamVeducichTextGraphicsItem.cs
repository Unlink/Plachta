using System;
using System.Collections.Generic;

namespace Plachtovac.Shared.BO.GraphicsBlocks
{
    public class ZoznamVeducichTextGraphicsItem : TextGraphicsItem
    {
        private List<AktivitaVeduci> _zoznamVeducich;

        private int _pocetRiadkov;

        public int PocetRiadkov
        {
            get => _pocetRiadkov;
            set
            {
                if (value == _pocetRiadkov) return;
                _pocetRiadkov = value;
                if (_zoznamVeducich != null)
                {
                    Text = string.Join('\n', VygenerujZoznamVeducich(_zoznamVeducich));
                }

                OnPropertyChanged();
            }
        }

        public void NastavZoznamVeducich(List<AktivitaVeduci> veduci)
        {
            _zoznamVeducich = veduci;
            Text = string.Join('\n', VygenerujZoznamVeducich(_zoznamVeducich));
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