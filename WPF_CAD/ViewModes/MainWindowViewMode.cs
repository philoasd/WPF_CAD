using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using WPF_CAD.ExternalClass;
using WPF_CAD.Modes;
using WPF_CAD.Utils;

namespace WPF_CAD.ViewModes
{
    public class MainWindowViewMode : ObservableObject
    {
        private ProcessMode _processMode => App.ServiceProvider.GetRequiredService<ProcessMode>();

        public MainWindowViewMode()
        {
            Title = $"{_mianTitle} - {OpenFileName}";
        }

        public string _mianTitle => "WPF_CAD Software";

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _openFileName = string.Empty;
        public string OpenFileName
        {
            get => _openFileName;
            set => SetProperty(ref _openFileName, value);
        }

        private string _openFilePath = string.Empty;
        public string OpenFilePath
        {
            get => _openFilePath;
            set => SetProperty(ref _openFilePath, value);
        }

        private string _status = string.Empty;
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private string _dataTime = string.Empty;
        public string DataTime
        {
            get => _dataTime;
            set => SetProperty(ref _dataTime, value);
        }

        private bool _isAutoMode = false;
        public bool IsAutoMode
        {
            get => _isAutoMode;
            set
            {
                SetProperty(ref _isAutoMode, value);
                if (value)
                {
                    _processMode.EnteringAutoMode();
                }
                else
                {
                    _processMode.ExitingAutoMode();
                }
            }
        }

        private ObservableCollection<string> _drawingList = new();
        public ObservableCollection<string> DrawingList
        {
            get => _drawingList;
            set => SetProperty(ref _drawingList, value);
        }

        private ObservableCollection<DrawingClass> _selectedDrawingInfomation = new();
        public ObservableCollection<DrawingClass> SelectedDrawingInfomation
        {
            get => _selectedDrawingInfomation;
            set => SetProperty(ref _selectedDrawingInfomation, value);
        }

        #region menu commands
        public RelayCommand NewFileCommand => new(() =>
        {
            // 关闭当前文件，清空当前显示内容
            OpenFileName = string.Empty;
            Title = $"{_mianTitle} - {OpenFileName}";
        });

        public RelayCommand OpenFileCommand => new(() =>
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Font Files (*.json)|*.json",
                DefaultExt = ".json",
            };
            if (openFileDialog.ShowDialog() == true)
            {
                OpenFilePath = openFileDialog.FileName;
                OpenFileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                Title = $"{_mianTitle} - {OpenFileName}";

                string json = File.ReadAllText(OpenFilePath);
            }
        });

        public RelayCommand SaveFileCommand => new(() =>
        {
            if (string.IsNullOrEmpty(OpenFileName))
            {
                // 打开一个文件选择框
                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "Font Files (*.json)|*.json",
                    DefaultExt = ".json",
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    OpenFilePath = saveFileDialog.FileName;
                    OpenFileName = Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
                    Title = $"{_mianTitle} - {OpenFileName}";
                }
                else
                {
                    return;
                }
            }

            string json = "TEST";
            File.WriteAllText(OpenFilePath, json);
            MsgBoxClass.ShowMsg("Save File Successfully", MsgBoxClass.MsgBoxType.Information);
        });

        public RelayCommand SaveAsFileCommand => new(() =>
        {
            // 打开一个文件选择框
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Font Files (*.json)|*.json",
                DefaultExt = ".json",
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                OpenFilePath = saveFileDialog.FileName;
                OpenFileName = Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
                Title = $"{_mianTitle} - {OpenFileName}";

                string json = "TEST";
                File.WriteAllText("artwork.json", json);
                MsgBoxClass.ShowMsg("Save File Successfully", MsgBoxClass.MsgBoxType.Information);
            }
        });

        public RelayCommand SaveFileAsPltCommand => new(() =>
        {

        });

        public RelayCommand HardWareSetupCommand => new(() =>
        {
            // 打开hardware setup window
        });
        #endregion

        #region toolbar commands

        #endregion
    }
}
