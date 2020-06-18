using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blazor.FileReader;
using Blazored.Modal;
using Blazored.Modal.Services;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Plachtovac.Client.Components.Forms;
using Plachtovac.Client.Helpers;
using Plachtovac.Client.Services;
using Plachtovac.Shared.BO.GraphicsBlocks;

namespace Plachtovac.Client.Components.Plachta
{
    public class CanvasEditor : ComponentBase, IDisposable
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        private IModalService ModalService { get; set; }

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

        public virtual async Task InsertFile(Blazorise.IFileEntry file, int width = 0, int height = 0)
        {
            using (var stream = new MemoryStream())
            {
                await file.WriteToStreamAsync(stream);

                var bytes = stream.ToArray();
                var base64 = Convert.ToBase64String(bytes);
                var type = file.Type;

                Console.WriteLine("Conversion started");
                var imgData = await JSRuntime.InvokeAsync<ImgConversionResult>("FabricJSBindings.resizeImage",
                    $"data:{type};base64,{base64}", width, height);
                Console.WriteLine("Conversion done");
                Console.WriteLine(imgData.Data);

                var imageItem = new ObrazokGraphicsItem
                {
                    Height = imgData.Height,
                    Left = 10,
                    Top = 10,
                    Width = imgData.Width,
                    ScaleX = 1,
                    ScaleY = 1,
                    Image = imgData.Data
                };

                await InsertItem(imageItem);
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
