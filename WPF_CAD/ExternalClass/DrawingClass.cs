using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WPF_CAD.ExternalClass
{
    public class DrawingClass : ObservableObject
    {
        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private float _width = 0;
        public float Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        private float _height = 0;
        public float Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        private float _centerX = 0;
        public float CenterX
        {
            get => _centerX;
            set => SetProperty(ref _centerX, value);
        }

        private float _centerY = 0;
        public float CenterY
        {
            get => _centerY;
            set => SetProperty(ref _centerY, value);
        }
    }
}
