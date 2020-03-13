using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Plachta.Client
{
    public class JSInteropBOWrapper<T>
    {
        public T M { get; set; }

        public JSInteropBOWrapper(T m)
        {
            M = m;
        }

        [JSInvokable]
        public void Update(object data)
        {
            Console.WriteLine(data);
        }

        [JSInvokable]
        public void Selected()
        {
            Console.WriteLine("Object selected");
        }
    }
}
