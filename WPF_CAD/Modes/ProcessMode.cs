using DrawingCanvasLib;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_CAD.Utils;
using WPF_CAD.ViewModes;

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
        public MachineConfigClass LoadMachineConfig()
        {
            if(!File.Exists(MachineConfigFilePath))
            {
                return new MachineConfigClass();
            }

            string json = File.ReadAllText(MachineConfigFilePath);
            var machineConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<MachineConfigClass>(json);
            if (machineConfig == null)
            {
                MsgBoxClass.ShowMsg("Failed to load machine configuration.", MsgBoxClass.MsgBoxType.Error);
                return new MachineConfigClass();
            }

            return machineConfig;
        }

        /// <summary>
        /// 保存机器配置
        /// </summary>
        /// <param name="machineConfig"></param>
        public void SaveMachineConfig()
        {
            var machineConfig = App.ServiceProvider?.GetService<MainWindowViewMode>()?.MachineConfig;
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(machineConfig, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(MachineConfigFilePath, json);
            //MsgBoxClass.ShowMsg("Machine configuration saved successfully.", MsgBoxClass.MsgBoxType.Information);
        }
    }
}
