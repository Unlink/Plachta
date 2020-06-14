using System;
using Newtonsoft.Json;
using Plachtovac.Client.Helpers;
using Plachtovac.Shared.BO;

namespace Plachtovac.Client.Services
{
    public class PlachtaService
    {
        public event EventHandler<Rozvrh> RozvrhChanged;

        public Rozvrh AktualnyRozvrh { get; set; }

        public PlachtaService()
        {
        }

        public void CreateNew()
        {
            AktualnyRozvrh = new Rozvrh
            {
                Nazov = "Nepomenovaný rozvrh",
                ZaciatokDna = new TimeSpan(8,0,0),
                KoniecDna = new TimeSpan(22, 0, 0),
            };
            AktualnyRozvrh.NastavPocetDni(1);
            AktualnyRozvrh.RozvrhChanged += (sender, args) => OnRozvrhChanged(AktualnyRozvrh);
            OnRozvrhChanged(AktualnyRozvrh);
        }

        public string ExportRozvrh()
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            return JsonConvert.SerializeObject(AktualnyRozvrh, settings);
        }

        public void LoadFromJson(string json)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            Console.WriteLine(json);
            AktualnyRozvrh = JsonConvert.DeserializeObject<Rozvrh>(json, settings);
            OnRozvrhChanged(AktualnyRozvrh);
        }

        protected virtual void OnRozvrhChanged(Rozvrh e)
        {
            RozvrhChanged?.Invoke(this, e);
        }
    }
}
