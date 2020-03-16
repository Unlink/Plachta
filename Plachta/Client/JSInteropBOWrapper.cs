using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Plachta.Client
{
    public class JSInteropBOWrapper<T>
    {
        public T M { get; set; }

        private Dictionary<string, JsonElement> _elementData = new Dictionary<string, JsonElement>();

        public JSInteropBOWrapper(T m)
        {
            M = m;
        }

        [JSInvokable]
        public void Update(object data)
        {
            _elementData.Clear();
            var json = JsonDocument.Parse(Convert.ToString(data));
            foreach (var prop in json.RootElement.EnumerateObject())
            {
                _elementData.Add(prop.Name.ToLower(), prop.Value);
                Console.WriteLine(prop.Name + " = " + prop.Value);
            }
            ApplyChanges();

        }

        [JSInvokable]
        public void Selected()
        {
            Console.WriteLine("Object selected");
        }

        private void ApplyChanges()
        {
            var itemType = M.GetType();
            Console.WriteLine(itemType);
            foreach (var propertyInfo in itemType.GetProperties())
            {
                Console.WriteLine("Property " + propertyInfo.Name);
                if (propertyInfo.CanWrite && _elementData.ContainsKey(propertyInfo.Name.ToLower()))
                {
                    var jsonElement = _elementData[propertyInfo.Name.ToLower()];
                    switch (jsonElement.ValueKind)
                    {
                        case JsonValueKind.Number:
                            Console.WriteLine($" = {jsonElement.GetDouble()}");
                            propertyInfo.SetValue(M, jsonElement.GetDouble());
                            break;
                        case JsonValueKind.String:
                            propertyInfo.SetValue(M, jsonElement.GetString());
                            Console.WriteLine($" = {jsonElement.GetString()}");
                            break;
                        case JsonValueKind.False:
                        case JsonValueKind.True:
                            Console.WriteLine($" = {jsonElement.GetBoolean()}");
                            propertyInfo.SetValue(M, jsonElement.GetBoolean());
                            break;
                    }
                }
            }
        }
    }
}
