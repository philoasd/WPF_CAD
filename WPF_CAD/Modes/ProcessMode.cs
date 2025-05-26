using DrawingCanvasLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_CAD.Utils;

namespace WPF_CAD.Modes
{
    public class ProcessMode
    {
        private static string MachineConfigFilePath => "machine_config.json"; // 机器配置文件路径

        /// <summary>
        /// 进入自动模式
        /// </summary>
        public void EnteringAutoMode()
        {
            MsgBoxClass.ShowMsg("Entering Auto Mode", MsgBoxClass.MsgBoxType.Information);
        }

        /// <summary>
        /// 退出自动模式
        /// </summary>
        public void ExitingAutoMode()
        {
            MsgBoxClass.ShowMsg("Exiting Auto Mode", MsgBoxClass.MsgBoxType.Information);
        }

        /// <summary>
        /// 加载机器配置
        /// </summary>
        public ObservableCollection<MachineConfigClass> LoadMachineConfig()
        {
            string json = File.ReadAllText(MachineConfigFilePath);
            var machineConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<ObservableCollection<MachineConfigClass>>(json);
            if (machineConfig == null)
            {
                MsgBoxClass.ShowMsg("Failed to load machine configuration.", MsgBoxClass.MsgBoxType.Error);
                return [];
            }

            return machineConfig;
        }

        /// <summary>
        /// 保存机器配置
        /// </summary>
        /// <param name="machineConfig"></param>
        private void SaveMachineConfig(ObservableCollection<MachineConfigClass> machineConfig)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(machineConfig, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(MachineConfigFilePath, json);
            MsgBoxClass.ShowMsg("Machine configuration saved successfully.", MsgBoxClass.MsgBoxType.Information);
        }
    }
}
