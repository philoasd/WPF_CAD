using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_CAD.Utils;

namespace WPF_CAD.Modes
{
    public class ProcessMode
    {
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
    }
}
