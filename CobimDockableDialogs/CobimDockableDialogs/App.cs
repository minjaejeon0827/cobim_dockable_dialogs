using Serilog;

using System.IO;
using System.Windows;
using System.Reflection;

using CobimDockableDialogs.Settings;
using CobimDockableDialogs.Common.CobimBase;
using CobimDockableDialogs.Common.LogBase;
using CobimDockableDialogs.Common.RibbonBase;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;


namespace CobimDockableDialogs
{
    // TODO : TransactionAttribute - TransactionMode.Manual 설정 구현 (2024.09.19 jbh)
    // 참고 URL - https://manzoo.tistory.com/107
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class App : IExternalApplication   // Revit이 실행될 때, Ribbon(메뉴, 탭, 버튼)이 등록되어야 하기 때문에 인터페이스 "IExternalApplication"를 상속 받아야 함.
    {
        #region 프로퍼티 

        #endregion 프로퍼티

        #region 기본 메소드

        /// <summary>
        /// Revit 시작 (OnStartup)
        /// </summary>
        public Result OnStartup(UIControlledApplication application)
        {
            // TODO : AppDomain.CurrentDomain 속성 사용하여 현재 실행 중인 주 응용 프로그램(Revit 2025 버전) 도메인(어셈블리 + 메인 스레드) 가져오기 구현 (2024.10.10 jbh)
            // 참고 URL - https://learn.microsoft.com/en-us/dotnet/api/system.appdomain.currentdomain?view=net-8.0
            // 참고 2 URL - https://developer-kus.tistory.com/11
            AppDomain currentDomain = AppDomain.CurrentDomain;   // 현재 실행 중인 주 응용 프로그램(Revit 2025 버전) 도메인(어셈블리 + 스레드) 가져오기
            //string logDirectoryPath = string.Empty;              // 로그(Logs) 폴더(디렉토리) 경로 초기화  
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // TODO : 오류 메시지 "IOException: Assembly.GetEntryAssembly()에서 null을 반환합니다. Application.ResourceAssembly 속성을 설정하거나
                //        pack://application:,,,/assemblyname;component/ 구문을 사용하여 리소스를 로드할 어셈블리를 지정하십시오."
                //        해결하기 위해 아래 주석친 코드 "System.Windows.Application.ResourceAssembly = Assembly.GetExecutingAssembly();"
                //        직접 사용하지 않고 로그인, 체크아웃, 체크인, 다운로드 시 처리 중 화면 비하인드 코드 (CommonLoadingWorkV.xaml.cs)
                //        -> 메서드 "InitSetting"안에 아래처럼 속성 "UriSource"에 할당할 이미지 파일 경로를 절대경로 사용해서 구현 (2024.10.10 jbh)  
                //        commonProgressImg.UriSource = new Uri("pack://application:,,,/CobimDockableDialogs;component/Image/CommonProgress.gif"); -->
                //        commonProgressImg.UriSource = new Uri("pack://application:,,,/CobimDockableDialogs;component/Resources/CommonProgress.gif"); -->
                // 참고 URL - https://github.com/XamlAnimatedGif/WpfAnimatedGif  -->
                // 참고 2 URL - https://github.com/XamlAnimatedGif/XamlAnimatedGif?tab=readme-ov-file  -->
                // 참고 3 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.windows.media.imaging.bitmapimage.urisource?view=windowsdesktop-8.0  -->
                // 참고 4 URL - https://stackoverflow.com/questions/22576588/invalid-uri-the-format-of-the-uri-could-not-be-determined  -->(2024.10.10 jbh)


                // TODO : 아래 주석친 코드 방식으로 개발하면 주 응용 프로그램(Revit 2025 버전)의 리소스도 같이 건드리게 되므로 
                //        아래 방식처럼 개발하지 말고 참고만 할 것 (2024.10.10 jbh)
                // 참고 URL - https://learn.microsoft.com/en-us/dotnet/api/system.windows.application.resourceassembly?view=windowsdesktop-8.0
                // 참고 2 URL - https://stackoverflow.com/questions/31468183/change-application-resourceassembly-in-wpf
                // 참고 3 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.reflection.assembly.getentryassembly?view=net-8.0
                // Assembly resourceAssembly = System.Windows.Application.ResourceAssembly;
                // Assembly test = Assembly.GetEntryAssembly();
                //System.Windows.Application.ResourceAssembly = Assembly.GetExecutingAssembly();


                // TODO : Revit 응용 프로그램 자체가 아래 애니메이션 이미지 파일 "CommonProgress.gif" 화면으로 출력할 때,
                //        CommonLoadingWorkV.xaml 비하인드 코드(CommonLoadingWorkV.xaml.cs)에서 생성자 호출시
                //        참조된 Nuget 패키지 파일 "AnimatedImage.Wpf.dll" 읽지 못하고
                //        아래처럼 오류 메시지가 출력되서 주석 처리 진행 (2024.10.02 jbh)
                // 오류 메시지 "System.Windows.Markup.XamlParseException: 'Could not load file or assembly 'AnimatedImage.Wpf, Culture=neutral, PublicKeyToken=75a6352eaa886748'. 지정된 파일을 찾을 수 없습니다.'
                //               FileNotFoundException: Could not load file or assembly 'AnimatedImage.Wpf, Culture=neutral, PublicKeyToken=75a6352eaa886748'.지정된 파일을 찾을 수 없습니다."   
                // 참고 URL - https://stackoverflow.com/questions/62835691/filenotfoundexception-wpfanimatedgif  
                // 참고 2 URL - https://github.com/whistyun/AnimatedImage  
                // 
                // TODO : 오류 메시지 "System.Windows.Markup.XamlParseException: 'Could not load file or assembly 'AnimatedImage.Wpf, Culture=neutral, PublicKeyToken=75a6352eaa886748'. 지정된 파일을 찾을 수 없습니다.'
                //                       FileNotFoundException: Could not load file or assembly 'AnimatedImage.Wpf, Culture=neutral, PublicKeyToken=75a6352eaa886748'.지정된 파일을 찾을 수 없습니다."  
                //        오류 원인 파악 결과
                //        Revit 2024 버전 까지는 이 문제가 그렇게 발생한 것을 본 적이 없는데,
                //        Revit 2025 버전 부터는 DotNet Core로 바뀐 여파인지,
                //        이런저런 상황에서 위와 같은 오류 메시지가 꽤 발생하는 것으로 확인됨.
                //        해당 오류 메세지가 출력되는 주요 원인은 3rdparty dll(CobimDockableDialogs.dll) 파일을
                //        주 응용 프로그램(Revit.exe) 실행시 바로 읽지 않고,
                //        주 응용 프로그램(Revit.exe) 실행 후 도중에 3rdparty dll(CobimDockableDialogs.dll) 파일을 읽게 되는 상황에서는
                //        주 응용 프로그램(Revit.exe) 자체에 관련한 폴더만 검색하게 되는 상황이 발생하기 때문이다.  
                // 오류 개선 방법 참고 URL -https://stackoverflow.com/questions/1373100/how-to-add-folder-to-assembly-search-path-at-runtime-in-net  
                currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromCobimDockableDialogs);

                //logDirectoryPath = CobimHelper.LogDirectoryPath;

                Logger.ConfigureLogger(CobimHelper.LogFileCountLimit, CobimHelper.AssemblyName, CobimHelper.LogDirectoryPath);   // Serilog 로그 초기 설정 

                Log.Information(Logger.GetMethodPath(currentMethod) + "Serilog 로그 초기 설정 완료");

                ConfigureAppSetting(application);   // 애드인 프로그램 CobimDockableDialogs 기본 설정에 필요한 객체 구성(생성 또는 초기화)  

                
                Log.Information(Logger.GetMethodPath(currentMethod) + $"{CobimHelper.AssemblyName} 프로그램 시작");

                Ribbon.CreateRibbonControl(application);   // 리본 메뉴 등록

                // TODO : 위의 메서드 "RegisterDockableDialog" 호출 결과 
                //        오류 메시지 "'The requested dockable pane has not been created yet." 출력 되며
                //        Revit 응용 프로그램 UI 화면에 DockableDialog 화면 출력 안 되는 오류 발생.
                //        해당 오류 해결하기 위해 아래처럼 이벤트 메소드 "App_DocumentCreated", "App_DocumentOpened" 구현하여 
                //        Revit 응용 프로그램에서 Revit 문서(템플릿 문서 또는 빈문서) 새로 생성 또는 기존 문서 다시 열었을 때, 
                //        이벤트 메소드 "App_DocumentCreated", "App_DocumentOpened" 실행 되고, 
                //        해당 이벤트 메소드 2가지 실행 결과 
                //        DockableDialog 화면 객체가 생성되어 Revit 응용 프로그램 UI 화면에 DockableDialog 화면이 출력 되도록 구현 (2024.09.26 jbh)
                // TODO : DocumentCreated 이벤트 메소드 App_DocumentCreated 가입 구현 (2024.04.16 jbh)
                // 참고 URL - https://www.revitapidocs.com/2015/89f514bf-f067-308d-0518-f050450bde72.htm
                application.ControlledApplication.DocumentCreated += new EventHandler<DocumentCreatedEventArgs>(App_DocumentCreated);

                // TODO : DocumentOpened 이벤트 메소드 App_DocumentOpened 가입 구현 (2024.04.16 jbh)
                // 참고 URL - https://www.revitapidocs.com/2015/7e5bc7a1-0475-b2ec-0aec-c410015737fe.htm
                application.ControlledApplication.DocumentOpened += new EventHandler<DocumentOpenedEventArgs>(App_DocumentOpened);

                // TODO : 아래 주석친 테스트 코드 필요시 참고 (2024.09.26 jbh)
                // application.ViewActivating += App_ViewActivating;
                // application.ViewActivated += App_ViewActivated;

                return Result.Succeeded;
            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
                return Result.Failed;
            }
        }

        /// <summary>
        /// Revit 종료 (OnShutdown)
        /// </summary>
        public Result OnShutdown(UIControlledApplication application)
        {
            // TODO : DocumentCreated 이벤트 메소드 App_DocumentCreated 해제 구현 (2024.04.16 jbh)
            // 참고 URL - https://www.revitapidocs.com/2015/89f514bf-f067-308d-0518-f050450bde72.htm
            application.ControlledApplication.DocumentCreated -= new EventHandler<DocumentCreatedEventArgs>(App_DocumentCreated);

            // TODO : DocumentOpened 이벤트 메소드 App_DocumentOpened 해제 구현 (2024.04.16 jbh)
            // 참고 URL - https://www.revitapidocs.com/2015/7e5bc7a1-0475-b2ec-0aec-c410015737fe.htm
            application.ControlledApplication.DocumentOpened -= new EventHandler<DocumentOpenedEventArgs>(App_DocumentOpened);

            // TODO : 아래 주석친 테스트 코드 필요시 참고 (2024.09.26 jbh)
            // application.ViewActivating -= App_ViewActivating;
            // application.ViewActivated -= App_ViewActivated;

            return Result.Succeeded;
        }

        #endregion 기본 메소드

        #region ConfigureAppSetting

        /// <summary>
        /// 애드인 프로그램 CobimDockableDialogs 기본 설정에 필요한 객체 구성(생성 또는 초기화)
        /// </summary>
        private void ConfigureAppSetting(UIControlledApplication rvUIControlledApp)
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                AppSetting.Default.DirectoryBase.SetCommonDirectoryPath(CobimHelper.CommonDirectoryPath);

                // TODO : 메서드 "AppSetting.Default.DirectoryBase.SetParentDirectoryPath" 호출 -> 메서드 Directory.GetParent 호출시 변수 parentDirectoryPath에 담긴 파일의 루트(부모) 폴더가 없는 경우 오류가 발생함.
                //        하여 해당 메서드 "AppSetting.Default.DirectoryBase.SetParentDirectoryPath" 호출문 주석 처리 완료 (2024.10.11 jbh)
                // AppSetting.Default.DirectoryBase.SetParentDirectoryPath(CobimHelper.AssemblyFilePath);   // dll 파일(CobimDockableDialogs.dll)이 속한 부모 폴더 경로 가져오기 
                AppSetting.Default.DirectoryBase.SetLogDirectoryPath(CobimHelper.LogDirectoryPath);

                // HttpClient 싱글톤 객체 생성 
                AppSetting.Default.HttpClientBase.CreateHttpClient();

                // TODO : 필요시 Revit 애드인 프로그램(CobimDockableDialogs) 로그인 화면 출력 메서드 OpenLogin() 구현 예정 (2024.08.20 jbh)
                // AppSetting.Default.LoginBase.OpenLogin();

                // Window 화면 객체 생성
                AppSetting.Default.WindowBase.CreateWindow();

                // DockableDialog 화면 싱글톤 객체 생성
                AppSetting.Default.DockableDialogBase.CreateDockableDialog();

                // DockableDialog (탐색기 화면) 등록 
                // AppSetting.Default.DockableDialogBase.RegisterDockableDialog(CobimHelper.ExpTitle, rvUIControlledApp, CobimHelper.ExplorerGuId);

                // DockableDialog (CefSharp 탐색기 화면) 등록
                AppSetting.Default.DockableDialogBase.RegisterDockableDialog(CobimHelper.CefSharpExpTitle, rvUIControlledApp, CobimHelper.CefSharpExplorerGuId);

                // TODO : 아래 주석친 DockableDialog (HubBox 화면) 등록 코드 필요시 참고 (2024.09.26 jbh)
                // DockableDialog (HubBox 화면) 등록 
                // AppSetting.Default.DockableDialogBase.RegisterDockableDialog(CobimHelper.HubBoxTitle, rvUIControlledApp, CobimHelper.HubBoxGuId);
            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                throw;   // 오류 발생시 상위 호출자 예외처리 전달 throw
            }
        }

        #endregion ConfigureAppSetting

        #region App_DocumentCreated

        /// <summary>
        /// Revit 응용 프로그램에서 사용자가 새로 생성한 문서(Document)를 열고 난 직후 실행되는 이벤트 메소드
        /// 전용 문서(Document) 수정 작업도 가능
        /// </summary>
        private void App_DocumentCreated(object sender, DocumentCreatedEventArgs e)
        {
            UIApplication createdUIApp = null;                   // Revit 응용 프로그램에서 사용자가 새로 생성한 문서(Document) 객체 초기화

            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // TODO : Revit 응용 프로그램에서 DockableDialog 화면을 출력하려면
                //        선행으로 Revit 응용 프로그램에서 사용자가 새로 생성한 문서(Document) 객체 createdUIApp를 생성 해야 하므로
                //        아래처럼 UIApplication 클래스 객체 createdUIApp 생성문 구현 (2024.09.26 jbh)
                // 참고 URL - https://forums.autodesk.com/t5/revit-api-forum/dockable-pane-can-t-be-created/td-p/8509741
                createdUIApp = new UIApplication(sender as Application);

                // DockableDialog - CefSharp 탐색기 화면 출력
                AppSetting.Default.DockableDialogBase.ShowDockableDialog(CobimHelper.CefSharpExpTitle, createdUIApp);
            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
            }
        }

        #endregion App_DocumentCreated

        #region App_DocumentOpened

        /// <summary>
        /// Revit 응용 프로그램에서 사용자가 기존에 생성한 문서(Document)를 열고 난 직후 실행되는 이벤트 메소드 
        /// 전용 문서(Document) 수정 작업도 가능
        /// </summary>
        private void App_DocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            UIApplication openedUIApp = null;                    // Revit 응용 프로그램에서 사용자가 기존에 생성한 문서(Document) 객체 초기화

            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // TODO : Revit 응용 프로그램에서 DockableDialog 화면을 출력하려면
                //        선행으로 Revit 응용 프로그램에서 사용자가 기존에 생성한 문서(Document) 객체 openedUIApp를 생성 해야 하므로
                //        아래처럼 UIApplication 클래스 객체 openedUIApp 생성문 구현 (2024.09.26 jbh)
                // 참고 URL - https://forums.autodesk.com/t5/revit-api-forum/dockable-pane-can-t-be-created/td-p/8509741
                openedUIApp = new UIApplication(sender as Application);
                // openedUIApp = AppSetting.Default.DockableDialogBase.GetRevitUIApp();
                // Document testDoc = openedUIApp.ActiveUIDocument.Document;
                // UIApplication testUIApp = new UIApplication(e.Document.Application);

                // DockableDialog - CefSharp 탐색기 화면 출력
                AppSetting.Default.DockableDialogBase.ShowDockableDialog(CobimHelper.CefSharpExpTitle, openedUIApp);
                // AppSetting.Default.DockableDialogBase.ShowDockableDialog(CobimHelper.CefSharpExpTitle, testUIApp, CobimHelper.CefSharpExplorerGuId);

                // TODO : 아래 주석친 테스트 코드 필요시 참고 (2024.10.16 jbh)
                // DockablePaneId dockablePaneId = new DockablePaneId(CobimHelper.CefSharpExplorerGuId);

                // DockablePane _DockablePane = null;

                // // dockablePane = rvUIApp.GetDockablePane(dockablePaneId);
                // // if(_DockablePane is null)
                // var app = sender as Application;
                // var uiapp = new UIApplication(app);
                // _DockablePane = uiapp.GetDockablePane(dockablePaneId);

                // bool isShown = (true == DockablePane.PaneIsRegistered(dockablePaneId))
                //                 && (true == _DockablePane.IsShown());

                // if (false == isShown)
                // {
                //     bool testResult = DockablePane.PaneIsRegistered(dockablePaneId);
                //     _DockablePane.Show();
                //     // Log.Information(Logger.GetMethodPath(currentMethod) + $"[DockableDialog] {dockablePaneTitle} 화면 출력 완료");
                // }
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
            }
        }

        #endregion App_DocumentOpened

        #region LoadFromCobimDockableDialogs

        /// <summary>
        /// 애드인 프로그램 CobimDockableDialogs에 속한 
        /// dll 파일(Nuget 패키지 참조된 dll 파일 포함) 읽어오기
        /// </summary>
        private Assembly LoadFromCobimDockableDialogs(object sender, ResolveEventArgs args)
        {
            string folderPath = string.Empty;     // 애드인 프로그램 dll 파일(CobimDockableDialogs.dll) 속한 상위 폴더 경로 
            string assemblyPath = string.Empty;   // 애드인 프로그램 dll 파일(CobimDockableDialogs.dll) 경로 
            Assembly assembly = null;             // 애드인 프로그램 dll 파일(CobimDockableDialogs.dll)

            //Assembly loadingAssembly = null;

            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
                // assemblyPath = Path.Combine(folderPath, Assembly.GetExecutingAssembly().GetName().Name + ".dll");
                // if (!File.Exists(assemblyPath)) return null;
                if(false == File.Exists(assemblyPath)) return null;
                assembly = Assembly.LoadFrom(assemblyPath);

                // TODO : 아래 주석친 테스트 코드 필요시 참고 (2024.10.10 jbh)
                //Assembly test = Assembly.GetEntryAssembly();
                //Assembly executingAssembly = Assembly.GetExecutingAssembly();

                //string applicationDirectory = Path.GetDirectoryName(executingAssembly.Location);

                //string[] fields = args.Name.Split(',');
                //string assemblyName = fields[0];
                //string assemblyCulture;
                //if (fields.Length < 2)
                //    assemblyCulture = null;
                //else
                //    assemblyCulture = fields[2].Substring(fields[2].IndexOf('=') + 1);


                //string assemblyFileName = assemblyName + ".dll";
                //string assemblyPath;

                //if (assemblyName.EndsWith(".resources"))
                //{
                //    // Specific resources are located in app subdirectories
                //    string resourceDirectory = Path.Combine(applicationDirectory, assemblyCulture);

                //    assemblyPath = Path.Combine(resourceDirectory, assemblyFileName);
                //}
                //else
                //{
                //    assemblyPath = Path.Combine(applicationDirectory, assemblyFileName);
                //}



                //if (File.Exists(assemblyPath))
                //{
                //    //Load the assembly from the specified path.                    
                //    //Assembly loadingAssembly = Assembly.LoadFrom(assemblyPath);
                //    loadingAssembly = Assembly.LoadFrom(assemblyPath);

                //    //Return the loaded assembly.
                //    //return loadingAssembly;
                //}
                //else
                //{
                //    return null;
                //}
            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
            }
            return assembly;
            // return loadingAssembly;
        }

        #endregion LoadFromCobimDockableDialogs

        #region Sample

        // TODO : 필요시 이벤트 메소드 "App_ViewActivated" 구현 예정 (2024.09.26 jbh)
        //private void App_ViewActivated(object sender, ViewActivatedEventArgs e)
        //{
        //    var test = sender as UIApplication;
        //}

        // TODO : 필요시 이벤트 메소드 "App_ViewActivating" 구현 예정 (2024.09.26 jbh)
        //private void App_ViewActivating(object sender, ViewActivatingEventArgs e)
        //{
        //    var test = sender as UIApplication;
        //}

        #endregion Sample
    }
}
