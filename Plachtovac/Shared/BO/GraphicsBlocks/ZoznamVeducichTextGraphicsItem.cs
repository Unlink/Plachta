using System;
using System.Collections.Generic;
using System.Linq;

namespace Plachtovac.Shared.BO.GraphicsBlocks
{
    public class ZoznamVeducichTextGraphicsItem : TextGraphicsItem
    {
        private List<AktivitaVeduci> _zoznamVeducich;

        private int _pocetRiadkov;
        private bool _veduciSPopisom;

        public int PocetRiadkov
        {
            get => _pocetRiadkov;
            set
            {
                if (value == _pocetRiadkov) return;
                _pocetRiadkov = value;
                Text = string.Join('\n', VygenerujZoznamVeducich(NormalizujPocetVeducich(_zoznamVeducich), _veduciSPopisom));

                OnPropertyChanged();
            }
        }

        public void NastavZoznamVeducich(List<AktivitaVeduci> veduci, bool popis = false)
        {
            _zoznamVeducich = veduci;
            _veduciSPopisom = popis;
            Text = string.Join('\n', VygenerujZoznamVeducich(NormalizujPocetVeducich(_zoznamVeducich), popis));
        }

        public string[] VygenerujZoznamVeducich(List<AktivitaVeduci> veduci, bool popis = false)
        {

            var veduciText = new List<string>();
            if (popis)
            {
                foreach (var grouping in veduci.GroupBy(v => v.Popis?.Trim() ?? ""))
                {
                    if (grouping.Key == "")
                    {
                        veduciText.AddRange(grouping.Select(v => v.Veduci.Prezyvka));
                    }
                    else
                    {
                        var mena = string.Join(" + ", grouping.Select(v => v.Veduci.Prezyvka));
                        veduciText.Add($"{mena} ({grouping.Key})");
                    }
                    
                }
            }
            else
            {
                veduciText = veduci.Select(v => v.Veduci.Prezyvka).ToList();
            }


            var veducichNaRiadok = (int)Math.Ceiling(veduciText.Count / (double)PocetRiadkov);
            var riadky = new string[PocetRiadkov];
            for (int i = 0; i < PocetRiadkov; i++)
            {
                riadky[i] = "";
            }

            for (int i = 0; i < veduciText.Count; i++)
            {
                riadky[i / veducichNaRiadok] += veduciText[i] + ", ";
            }

            for (int i = 0; i < PocetRiadkov; i++)
            {
                riadky[i] = riadky[i].Trim(',', ' ');
            }

            return riadky;
        }

        private List<AktivitaVeduci> NormalizujPocetVeducich(List<AktivitaVeduci> veduci)
        {
            var result = veduci?.ToList() ?? new List<AktivitaVeduci>();
            var cnt = 1;
            while (result.Count < PocetRiadkov)
            {
                result.Add(new AktivitaVeduci {Veduci = new Veduci {Prezyvka = $"Veduci{cnt++}"}});
            }

            return result;
        }
    }
}