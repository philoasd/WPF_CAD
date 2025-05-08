using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrawingCanvasLib.DrawTool;

namespace WPF_CAD.ViewModes
{
    public class DrawingPropertiesWindowViewMode : ObservableObject
    {
        #region Command

        /// <summary>
        /// 确定按钮命令
        /// </summary>
        public RelayCommand<Window> OKCommand => new((obj) =>
        {
            if (obj == null) { return; }


            // 关闭窗口
            obj.Close();
        });

        /// <summary>
        /// 取消按钮命令
        /// </summary>
        public RelayCommand<Window> CancelCommand => new((obj) =>
        {
            if (obj == null) { return; }
            // 关闭窗口
            obj.Close();
        });

        /// <summary>
        /// 更新绘图属性的窗口信息命令
        /// </summary>
        public RelayCommand<BaseDrawingClass> UpdateDrawingPropertiesCommand => new((obj) =>
        {
            if (obj == null) { return; }
        });

        #endregion

        #region Line page

        private Visibility _isShowLinePage = Visibility.Collapsed;
        public Visibility IsShowLinePage
        {
            get => _isShowLinePage;
            set => SetProperty(ref _isShowLinePage, value);
        }

        #endregion

        #region Data page

        private Visibility _isShowDataPage = Visibility.Collapsed;
        public Visibility IsShowDataPage
        {
            get => _isShowDataPage;
            set => SetProperty(ref _isShowDataPage, value);
        }

        #endregion

        #region Text page

        private Visibility _isShowTextPage = Visibility.Collapsed;
        public Visibility IsShowTextPage
        {
            get => _isShowTextPage;
            set => SetProperty(ref _isShowTextPage, value);
        }

        #endregion

        #region Barcode page

        private Visibility _isShowBarcodePage = Visibility.Collapsed;
        public Visibility IsShowBarcodePage
        {
            get => _isShowBarcodePage;
            set => SetProperty(ref _isShowBarcodePage, value);
        }

        #endregion

        #region Serialization

        private Visibility _isShowSerializePage = Visibility.Collapsed;
        public Visibility IsShowSerializePage
        {
            get => _isShowSerializePage;
            set => SetProperty(ref _isShowSerializePage, value);
        }

        #endregion

        #region Format

        private Visibility _isShowFormatPage = Visibility.Collapsed;
        public Visibility IsShowFormatPage
        {
            get => _isShowFormatPage;
            set => SetProperty(ref _isShowFormatPage, value);
        }

        #endregion

        #region Hatch

        private Visibility _isShowHatchPage = Visibility.Collapsed;
        public Visibility IsShowHatchPage
        {
            get => _isShowHatchPage;
            set => SetProperty(ref _isShowHatchPage, value);
        }

        #endregion

        #region Marking

        #endregion
    }
}
