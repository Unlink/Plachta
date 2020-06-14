﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Plachtovac.Shared.BO.GraphicsBlocks;

namespace Plachtovac.Shared.BO
{
    public class AktivitaSablona : INotifyPropertyChanged
    {
        public string Nazov { get; set; }

        public string Typ { get; set; }

        public TimeSpan Trvanie { get; set; }

        public List<AktivitaVeduci> Veduci { get; set; } = new List<AktivitaVeduci>();

        public List<GraphicsItem> AktivitaItems { get; } = new List<GraphicsItem>();

        private string _design;
        public string Design
        {
            get => _design;
            set
            {
                if (value == _design) return;
                _design = value;
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