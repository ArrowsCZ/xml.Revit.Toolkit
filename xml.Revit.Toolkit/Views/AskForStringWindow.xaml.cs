using System.Windows;

namespace xml.Revit.Toolkit.Views
{
    /// <summary>
    /// 快速询问弹窗
    /// </summary>
    public partial class AskForStringWindow : Window
    {
        /// <summary>
        /// 输入内容
        /// </summary>
        public string Result { get; private set; } = string.Empty;

        /// <summary>
        /// 询问关键字窗口
        /// <para>通过 <see cref="AskForStringWindow.Result"/> 获取输入内容字符串</para>
        /// </summary>
        /// <param name="prompt">提示输入</param>
        /// <param name="title">窗口标题</param>
        public AskForStringWindow(string prompt, string title = "微信公众号:Revit二次开发教程")
        {
            InitializeComponent();
            this.prompt.Text = prompt;
            Title = title;
        }

        private void EnterClick(object sender, RoutedEventArgs e)
        {
            Result = input.Text;
            DialogResult = true;
            Close();
        }
    }
}
