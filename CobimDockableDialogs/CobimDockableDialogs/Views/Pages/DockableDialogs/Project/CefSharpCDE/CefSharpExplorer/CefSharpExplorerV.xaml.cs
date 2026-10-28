using Serilog;
using CefSharp;

using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using CobimDockableDialogs.Common.CobimBase;
using CobimDockableDialogs.Common.LogBase;
using CobimDockableDialogs.Rest.Api.CobimBase.HubBox;
using CobimDockableDialogs.ViewModels.Pages.DockableDialogs.Project.CefSharpCDE.CefSharpExplorer;
using CobimDockableDialogs.ViewModels.Part.DockableDialogs.Project.CefSharpCDE.CefSharpExplorer;
using Autodesk.Revit.DB;
using CefSharp.Handler;
using CobimDockableDialogs.Data.Models.CobimBase.CefSharpExplorer;
using CobimDockableDialogs.RequestBase;
using System.Text;
using CobimDockableDialogs.Utils.String;
using CobimDockableDialogs.Settings;
using System.IO;
using Autodesk.Revit.UI;

namespace CobimDockableDialogs.Views.Pages.DockableDialogs.Project.CefSharpCDE.CefSharpExplorer
{
    // TODO : 아래 오류 메시지 개선을 하기 위해 Nuget 패키지 "CefSharp.Wpf.NETCore" 기존 버전 "Version=128.4.90.0" 삭제 후 낮은 버전 "Version=119.4.30.0" 설치 진행함 (2024.09.27 jbh)
    // "cobimWebExplorer.Address error CS1705: ID가 'CobimDockableDialogs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'인 'CobimDockableDialogs' 어셈블리는 ID가 'CefSharp.Wpf, Version=119.4.30.0, Culture=neutral, PublicKeyToken=40c4b6fc221f4138'인 참조된 어셈블리 'CefSharp.Wpf' 이후 버전인 'CefSharp.Wpf, Version=128.4.90.0, Culture=neutral, PublicKeyToken=40c4b6fc221f4138'을(를) 사용합니다." 



    // *** CefSharpWpf 웹 브라우저 Revit - Cobim 애드인 프로그램 기능 설명 ***
    // 웹 브라우저에서 작성된 자바스크립트 코드와 Revit - Cobim 애드인 프로그램에서 작성된 C# 코드 둘 사이에
    // "DownLoadAsync", "CheckOutAsync", "CheckInAsync", "CancelWorkAsync" 라는 이름의 객체로 연결된 상태로
    // 자바스크립트 코드로 c# 쪽 함수를 그대로 호출할 수 있는 기능이다.

    // *** CefSharpWpf 웹 브라우저 Revit - Cobim 애드인 프로그램 실행 순서 ***
    // 1. Revit 응용 프로그램 실행 
    // 2. Revit - Cobim 애드인 프로그램(.dll) 실행 (예) 프로젝트 파일 "CobimDockableDialogs"
    // 3. Cefsharp 사용하면 화면 뷰 생성자 실행 (예) CefSharpExplorerV.xaml.cs -> 생성자 CefSharpExplorerV

    // 4. Cefsharp 객체("cobimWebExplorer")에 http URL 주소 연동 cobimWebExplorer.Address = "http://localhost:11977/";

    // 5. 4번에서 연동한 http URL 주소 호출시 JavaSpring 웹서비스에서 웹브라우저 화면으로 뿌려지는 html, CSS, 자바스크립트 코드 중
    //    자바스크립트 코드로 객체 "CefSharp"이 바인딩 성공되면 (예) await CefSharp.BindObjectAsync("DownLoadAsync");
    //    아래처럼 Cefsharp 객체("cobimWebExplorer")의 저장소(cobimWebExplorer.JavascriptObjectRepository)에
    //    C# 코드 Debug.WriteLine 호출하여 바인딩 처리 성공했다는 Trigger가 실행 되도록 이벤트 핸들러 등록하기 
    //    cobimWebExplorer.JavascriptObjectRepository.ObjectBoundInJavascript += (sender, e) => { ... }

    // 6. 4번에서 연동한 http URL 주소 호출시 JavaSpring 웹서비스에서 웹브라우저 화면으로 뿌려지는 html, CSS, 자바스크립트 코드 중
    //    자바스크립트 코드로 바인딩 성공한 CefSharp 객체가 (예) await CefSharp.BindObjectAsync("DownLoadAsync");
    //    C# 코드 사용한 Cefsharp 객체("cobimWebExplorer")의 저장소(cobimWebExplorer.JavascriptObjectRepository)에 아직 등록되지 않은 경우 
    //    바인딩 요청(bindingOptions = BindingOptions.DefaultBinder;) 및 Cefsharp 객체("cobimWebExplorer")의 저장소 객체(this._ObjectRepository)에 등록 처리(this._ObjectRepository.Register("DownLoadAsync", new CefSharpExplorerVM(), isAsync: true, options: bindingOptions);)

    // 7. Cobim 애드인 프로그램(.dll) Dockable Window 출력 
    // 8. Dockable Window에서 출력되는 Cefsharp 웹브라우저 화면 (예) index.html 에서 출력되는 버튼 "click" 클릭
    // 9. 비하인드 코드 MainWindow.xaml.cs -> 클래스 "CefSharpExplorerVM" -> 메서드 "Test" 호출 
    // 10. Cefsharp 웹브라우저 화면 (예) index.html 에서 마우스 우클릭 -> "검사" 클릭 -> "검사" 화면 출력 -> 상단 탭 "Console" 클릭 
    //     9번 메서드 "Test" 호출 결과로 Debug.WriteLine($"{a} {b}"); 이 출력된다.

    /// <summary>
    /// CefSharpExplorerV.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CefSharpExplorerV : Page
    {
        #region 프로퍼티

        /// <summary>
        /// 뷰모델 CefSharpExplorerVM에 매핑되는 CefSharp 탐색기 뷰 이름(CefSharpExplorerV)
        /// </summary>
        private string _DockableDialogName { get; set; }

        /// <summary>
        /// COBIM 웹 탐색기 브라우저에 존재하는 바인딩 할 메서드 객체 이름 
        /// </summary>
        private string _ObjectName { get; set; }

        /// <summary>
        /// 바인딩 성공한 메서드 객체의 저장소
        /// </summary>
        private IJavascriptObjectRepository _ObjectRepository { get; set; }

        #endregion 프로퍼티

        #region 생성자

        public CefSharpExplorerV()
        {
            InitializeComponent();
            InitSetting();
        }

        #endregion 생성자

        #region InitSetting

        /// <summary>
        /// CefSharp 기술 기반 
        /// 응용 프로그램 CobimDockableDialogs 
        /// 화면 초기 셋팅
        /// </summary>
        private void InitSetting()
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록
            
            try
            {
                // TODO : COBIM CefSharp 웹 탐색기 화면 뷰(CefSharpExplorerV.xaml) 호출시 DataContext 객체 생성 구현 (2024.10.10 jbh)
                // 참고 URL - https://yeko90.tistory.com/entry/wpf-DataContext-%EB%8B%A4%EC%96%91%ED%95%9C-%EB%B0%A9%EB%B2%95
                this.DataContext = new CefSharpExplorerVM();

                // TODO : 아래 주석친 테스트 웹 어플리케이션 URL 주소 "http://localhost:8080/src/index.html" 필요시 참고 (2024.09.30 jbh)
                // 테스트 웹 어플리케이션 Git URL 주소 - https://github.com/bonghakjeon/CefSharpWebApp
                // cobimWebExplorer.Address = "http://localhost:8080/src/index.html";

                cobimWebExplorer.Address = HubBoxHelper.requestLoginURL;   // HubBox 웹 탐색기 - 로그인 Rest API 주소 

                // COBIM 웹 탐색기 브라우저에 존재하는 메서드 객체 바인딩(ObjectBoundInJavascript) 처리하기 
                cobimWebExplorer.JavascriptObjectRepository.ObjectBoundInJavascript += (sender, e) =>
                {
                    Log.Information(Logger.GetMethodPath(currentMethod) + $"COBIM 웹 탐색기 메서드 객체명 : {this._ObjectName} 객체 바인딩(ObjectBoundInJavascript) 성공!");
                    Debug.WriteLine($"Object {this._ObjectName} was bound successfully.");
                };

                cobimWebExplorer.JavascriptObjectRepository.ResolveObject += (sender, e) =>
                {
                    // TODO : 아래 오류 메시지 해결하기 위해 Dispatcher.Invoke 사용 (2024.09.27 jbh)
                    // 오류 메시지 - "다른 스레드가 이 개체를 소유하고 있어 호출한 스레드가 해당 개체에 액세스할 수 없습니다"
                    // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.windows.threading.dispatcher.invoke?view=windowsdesktop-6.0
                    // 참고 2 URL - https://insurang.tistory.com/entry/WPF-C-Error-%EB%8B%A4%EB%A5%B8-%EC%8A%A4%EB%A0%88%EB%93%9C%EA%B0%80-%EC%9D%B4-%EA%B0%9C%EC%B2%B4%EB%A5%BC-%EC%86%8C%EC%9C%A0%ED%95%98%EA%B3%A0-%EC%9E%88%EC%96%B4-%ED%98%B8%EC%B6%9C%ED%95%9C-%EC%8A%A4%EB%A0%88%EB%93%9C%EA%B0%80-%ED%95%B4%EB%8B%B9-%EA%B0%9C%EC%B2%B4%EC%97%90-%EC%95%A1%EC%84%B8%EC%8A%A4%ED%95%A0-%EC%88%98-%EC%97%86%EC%8A%B5%EB%8B%88%EB%8B%A4-%EC%97%90%EB%9F%AC-%EC%93%B0%EB%A0%88%EB%93%9C-Thread-DispatcherInvoke
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        this._DockableDialogName = CobimHelper.TypeOfCefSharpExplorer;
                        this._ObjectName = e.ObjectName;
                        this._ObjectRepository = e.ObjectRepository;

                        // 바인딩할 메서드 객체 이름이 "boundAsync"인 경우  
                        if(this._ObjectName == CobimHelper.boundAsync)
                        {
                            BindingOptions bindingOptions = null;          // Binding options is an optional param, defaults to null
                            bindingOptions = BindingOptions.DefaultBinder; // Use the default binder to serialize values into complex objects
                            CefSharpExplorerVM cefSharpExplorerVM = (CefSharpExplorerVM)this.DataContext;
                            cefSharpExplorerVM.InitSetting(_DockableDialogName);

                            // bindingOptions = new BindingOptions { Binder = new MyCustomBinder() }); //Specify a custom binder
                            this._ObjectRepository.NameConverter = null;   // No CamelCase of Javascript Names
                                                                           // For backwards compatability reasons the default NameConverter doesn't change the case of the objects returned from methods calls.
                                                                           // https://github.com/cefsharp/CefSharp/issues/2442
                                                                           // Use the new name converter to bound object method names and property names of returned objects converted to camelCase

                            this._ObjectRepository.Register(CobimHelper.boundAsync, this.DataContext, options: bindingOptions);
                        }
                    }));
                };
            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
            }
        }

        #endregion InitSetting

        #region Test 

        /// <summary>
        /// Test 메서드 
        /// </summary>
        public void Test()
        {

        }

        #endregion Test

        #region Sample

        #endregion Sample
    }
}
