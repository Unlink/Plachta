using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Plachtovac.Shared.BO.GraphicsBlocks
{
    public class TextGraphicsItem : GraphicsItem, INotifyPropertyChanged
    {
        private string _text;
        private string _fill;
        private string _fontFamily;

        public string Text
        {
            get => _text;
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged();
            }
        }

        public string Fill
        {
            get => _fill;
            set
            {
                if (value == _fill) return;
                _fill = value;
                OnPropertyChanged();
            }
        }

        public string FontFamily
        {
            get => _fontFamily;
            set
            {
                if (value == _fontFamily) return;
                _fontFamily = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}