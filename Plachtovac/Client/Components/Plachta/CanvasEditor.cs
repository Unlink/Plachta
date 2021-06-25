using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Modal;
using Blazored.Modal.Services;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Plachtovac.Client.Components.Forms;
using Plachtovac.Client.Helpers;
using Plachtovac.Client.Services;
using Plachtovac.Shared.BO.GraphicsBlocks;
using Tewr.Blazor.FileReader;

namespace Plachtovac.Client.Components.Plachta
{
    public class CanvasEditor : ComponentBase, IDisposable
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected IModalService ModalService { get; set; }

        [Inject]
        private IFileReaderService FileReaderService { get; set; }

        [Inject]
        private GoogleDriveStorage CloudService { get; set; }

        [Parameter]
        public ElementReference CanvasElement { get; set; }

        public ElementSize CurrentCanvasSize { get; set; }

        public GraphicsItem SelectedItem { get; set; }
        protected Dictionary<string, DotNetObjectReference<JSInteropBOWrapper<GraphicsItem>>> Items = new Dictionary<string, DotNetObjectReference<JSInteropBOWrapper<GraphicsItem>>>();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                CurrentCanvasSize =
                    await JSRuntime.InvokeAsync<ElementSize>("Plachtovac.getBoundingClientRect", CanvasElement);
                await JSRuntime.InvokeVoidAsync("FabricJSBindings.create", CanvasElement);
            }
        }

        public virtual async Task InsertText(string text = "Textové pole")
        {
            var textAktivita = new TextGraphicsItem
            {
                Fill = "#000000",
                Height = 15,
                Left = 10,
                Text = text,
                Top = 10,
                Width = 40,
                ScaleX = 1,
                ScaleY = 1,
                FontFamily = "Comic Sans",
            };

            await InsertItem(textAktivita);
        }

        public virtual async Task InsertFile(ElementReference inputTypeFileElement, int width = 0, int height = 0)
        {
            foreach (var file in await FileReaderService.CreateReference(inputTypeFileElement).EnumerateFilesAsync())
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                //using (MemoryStream memoryStream = await file.CreateMemoryStreamAsync(80 * 1024))
                //{
                //    Convert.ToBase64String(memoryStream.ToArray());
                //    Console.WriteLine($"Done reading file - {memoryStream.Length} bytes in {stopwatch.ElapsedMilliseconds}ms.");
                //}


                var fileInfo = await file.ReadFileInfoAsync();
                Console.WriteLine($"{nameof(IFileInfo)}.{nameof(fileInfo.Name)}: {fileInfo.Name}");
                Console.WriteLine($"{nameof(IFileInfo)}.{nameof(fileInfo.Size)}: {fileInfo.Size}");
                Console.WriteLine($"{nameof(IFileInfo)}.{nameof(fileInfo.Type)}: {fileInfo.Type}");
                Console.WriteLine($"{nameof(IFileInfo)}.{nameof(fileInfo.LastModifiedDate)}: {fileInfo.LastModifiedDate?.ToString() ?? "(N/A)"}");

                var bytes = new byte[fileInfo.Size];
                var filePos = 0;
                using (var fs = await file.OpenReadAsync())
                {
                    var buffer = new byte[80*1024];
                    int count;
                    var lastAnnounce = 0m;
                    while ((count = await fs.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    {
                        Buffer.BlockCopy(buffer, 0, bytes, filePos, count);
                        filePos += count;
                        var progress = ((decimal)fs.Position * 100) / fs.Length;
                        if (progress > (lastAnnounce + 10))
                        {
                            Console.WriteLine($"Read {count} bytes ({progress:00}%). {fs.Position} / {fs.Length}");
                        }
                    }
                    Console.WriteLine($"Done reading file {fileInfo.Name} - {fs.Length} bytes in {stopwatch.ElapsedMilliseconds}ms.");
                }

                var imgUrl = await CloudService.StoreImage(fileInfo.Name, fileInfo.Type, bytes);
                Console.WriteLine($"Done uploading to gdrive {imgUrl}");

                InsertExternalImage(imgUrl);
            }
        }

        public virtual async Task InsertExternalImage(int width = 0, int height = 0)
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(BasicInputModal.Message), "Zadajte url adresu obrázku");
            parameters.Add(nameof(BasicInputModal.Title), "Url");
            var modal = ModalService.Show<BasicInputModal>($"Otvor obrázok", parameters);
            if (!(await modal.Result).Cancelled)
            {
                var imgUrl = modal.Result.Result.Data.ToString();
                InsertExternalImage(imgUrl);
            }
        }

        private async void InsertExternalImage(string imgUrl)
        {
            var imgData = await JSRuntime.InvokeAsync<ElementSize>("FabricJSBindings.getImageSize",
                imgUrl);

            var scale = 1.0;
            if (imgData.Width > CurrentCanvasSize.Width)
            {
                scale = CurrentCanvasSize.Width / imgData.Width;
            }

            if (imgData.Height * scale > CurrentCanvasSize.Height)
            {
                scale = CurrentCanvasSize.Height / imgData.Height;
            }

            var imageItem = new ObrazokGraphicsItem
            {
                Height = imgData.Height,
                Left = 10,
                Top = 10,
                Width = imgData.Width,
                ScaleX = scale,
                ScaleY = scale,
                Image = imgUrl
            };
            await InsertItem(imageItem);
        }

        protected virtual async Task<JSInteropBOWrapper<GraphicsItem>> InsertItem(GraphicsItem item)
        {
            var id = Guid.NewGuid().ToString();
            item.Id = id;
            var jsWrapper = new JSInteropBOWrapper<GraphicsItem>(id, item);
            jsWrapper.ItemSelectionChanged += JsWrapperOnItemSelectionChanged;
            jsWrapper.ItemRemoved += JsWrapperOnItemRemoved;
            Items.Add(id, DotNetObjectReference.Create(jsWrapper));
            if (item is TextGraphicsItem)
            {
                await JSRuntime.InvokeVoidAsync("FabricJSBindings.addText", CanvasElement, id, Items[id], Items[id].Value.M);
            }
            else if (item is ObrazokGraphicsItem)
            {
                await JSRuntime.InvokeVoidAsync("FabricJSBindings.addImage", CanvasElement, id, Items[id], Items[id].Value.M);
            }
            return jsWrapper;
        }

        private void JsWrapperOnItemSelectionChanged(object sender, EventArgs e)
        {
            var jsWrapper = sender as JSInteropBOWrapper<GraphicsItem>;
            if (jsWrapper?.M is TextGraphicsItem item)
            {
                if (jsWrapper.IsSelected)
                {
                    SelectedItem = item;
                    item.PropertyChanged += SelectedGraphicsItemOnPropertyChanged;
                }
                else
                {
                    if (SelectedItem != null)
                    {
                        item.PropertyChanged -= SelectedGraphicsItemOnPropertyChanged;
                    }
                    SelectedItem = null;
                }
            }
            else
            {
                SelectedItem = null;
            }
            StateHasChanged();
        }

        private void JsWrapperOnItemRemoved(object sender, EventArgs e)
        {
            var jsWrapper = sender as JSInteropBOWrapper<GraphicsItem>;
            Items.Remove(jsWrapper.Id);
            StateHasChanged();
        }

        public void SelectedGraphicsItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var id = Items.Where(x => x.Value.Value.M == SelectedItem).Select(x => x.Key).FirstOrDefault();
            if (id != null)
            {
                JSRuntime.InvokeVoidAsync("FabricJSBindings.changeProperty", CanvasElement, id, SelectedItem);
            }
        }

        public void Dispose()
        {
            foreach (var item in Items.Values)
            {
                item.Dispose();
            }
            JSRuntime.InvokeVoidAsync("FabricJSBindings.dispose", CanvasElement);
        }
    }
}
