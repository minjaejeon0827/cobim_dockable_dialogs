using CobimDockableDialogs.ViewModels.Part.DockableDialogs.Project.CefSharpCDE.CefSharpExplorer;
using CobimDockableDialogs.ViewModels.Windows.LoadingWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CobimDockableDialogs.Views.Part.DockableDialogs.Project.CefSharpCDE.CefSharpExplorer
{
    /// <summary>
    /// CheckInPartV.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CheckInPartV : Window
    {
        public CheckInPartV()
        {
            InitializeComponent();

            // TODO : 체크인 팝업 화면 뷰(CheckInPartV.xaml) 호출시 DataContext 객체 생성 구현 (2024.10.10 jbh)
            // 참고 URL - https://yeko90.tistory.com/entry/wpf-DataContext-%EB%8B%A4%EC%96%91%ED%95%9C-%EB%B0%A9%EB%B2%95
            this.DataContext = new CheckInPartVM();

        }
    }
}
