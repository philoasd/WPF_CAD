using System.Windows;

namespace WPF_CAD.Utils
{
    /// <summary>
    /// 显示消息框类
    /// </summary>
    public class MsgBoxClass
    {
        /// <summary>
        /// 显示信息框
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="type">0:info, 1:warning, 2:error</param>
        public static void ShowMsg(string msg, MsgBoxType type)
        {
            switch ((int)type)
            {
                case 0: // information
                    {
                        MessageBox.Show(msg, "Information", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None, MessageBoxOptions.DefaultDesktopOnly);
                        break;
                    }
                case 1: // warning
                    {
                        MessageBox.Show(msg, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.None, MessageBoxOptions.DefaultDesktopOnly);
                        break;
                    }
                case 2: // error
                    {
                        MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None, MessageBoxOptions.DefaultDesktopOnly);
                        break;
                    }
            }
        }

        /// <summary>
        /// 显示询问框
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static MessageBoxResult ShowQMsg(string msg)
        {
            MessageBoxResult result = MessageBox.Show(msg, "Question", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No, MessageBoxOptions.DefaultDesktopOnly);
            return result;
        }

        public enum MsgBoxType
        {
            Information,
            Warning,
            Error
        }
    }
}
