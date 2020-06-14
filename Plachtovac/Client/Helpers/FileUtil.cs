using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Plachtovac.Client.Helpers
{
    public static class FileUtil
    {
        public static async Task SaveAs(IJSRuntime js, string filename, string data, string contentType = "application/json")
        {
            await js.InvokeAsync<object>(
                "Plachtovac.saveAsFile",
                filename,
                Converters.ToBase64(data),
                contentType);
        }
    }
}
