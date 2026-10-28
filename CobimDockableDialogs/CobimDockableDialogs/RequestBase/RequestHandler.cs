using Serilog;

using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using CobimDockableDialogs.Settings;
using CobimDockableDialogs.Common.LogBase;
using CobimDockableDialogs.Common.CobimBase;

using CobimDockableDialogs.Data.Models.CobimBase.CefSharpExplorer;
using CobimNet;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CobimDockableDialogs.Views.Windows.LoadingWork;

namespace CobimDockableDialogs.RequestBase
{
    /// <summary>
    /// Modeless 폼(.Show()) 형식에 의해 발생하는 외부 요청 핸들러
    /// </summary>
    public class CefSharpExplorerRequestHandler : BindableBase, IExternalEventHandler
    {
        #region 프로퍼티

        /// <summary>
        /// DockableDialog 화면 객체 (CefSharp 탐색기 뷰 - CefSharpExplorerV.xaml)
        /// </summary>
        private Page _DockableDialog { get; set; }

        // TODO : 필요시 프로퍼티 "_CommonLoadingWorkV" 참고 (2024.11.28 minjae)
        /// <summary>
        /// Window - 로그인, 체크아웃, 체크인, 다운로드 시 처리 중 화면
        /// </summary>
        // private CommonLoadingWorkV _CommonLoadingWorkV { get; set; }

        /// <summary>
        /// CobimDockableDialogs
        /// CefSharp 탐색기 화면 뷰모델(CefSharpExplorerVM.cs) 데이터 모델 객체
        /// </summary>
        private CefSharpExplorerView _CefSharpExplorerView { get; set; }

        /// <summary>
        /// Modeless 폼(.Show()) 최근 요청 값에 엑세스 하기위한 프로퍼티 (읽기 전용)
        /// </summary>
        public CefSharpExplorerRequest Request { get { return _Request; } }
        private CefSharpExplorerRequest _Request = new CefSharpExplorerRequest();

        /// <summary>
        /// Revit UI 애플리케이션 객체
        /// </summary>
        private UIApplication _RevitUIApp { get; set; }

        // TODO : 필요시 Revit 문서 프로퍼티 "_RevitDoc" 사용 예정 (2024.10.16 minjae)
        /// <summary>
        /// Revit 문서 
        /// </summary>
        // private Document _RevitDoc { get; set; }

        /// <summary>
        /// CefSharpExplorer Request 유형 enum 열거형 구조체 프로퍼티
        /// </summary>
        private EnumCefSharpExplorerRequestId _RequestIdValue { get; set; }

        /// <summary>
        /// 컨테이너에 속한 파일(확장자 .rvt) 열기 처리 진행 여부 
        /// </summary>
        public bool IsOpenFileWorking { get => _IsOpenFileWorking; set { _IsOpenFileWorking = value; NotifyOfPropertyChange(nameof(IsOpenFileWorking)); } }
        private bool _IsOpenFileWorking;

        #endregion 프로퍼티

        #region GetName

        /// <summary>
        /// 해당 외부 이벤트 핸들러(CefSharpExplorerRequestHandler)(이벤트 메서드) 이름으로 식별 
        /// </summary>
        public string GetName()
        {
            return CobimHelper.TypeOfCefSharpExplorer;
        }

        #endregion GetName

        #region SetCommonLoadingWorkV

        // TODO : 필요시 메서드 "SetCommonLoadingWorkV" 참고 (2024.11.28 minjae)
        /// <summary>
        /// CefSharp 탐색기 화면 뷰모델(CefSharpExplorerVM.cs) 
        /// Window - 로그인, 체크아웃, 체크인, 다운로드 시 처리 중 화면 객체 값 셋팅
        /// </summary>
        // public void SetCommonLoadingWorkV(CommonLoadingWorkV commonLoadingWorkV) 
        // {
        //     _CommonLoadingWorkV = commonLoadingWorkV;
        // }

        #endregion SetCommonLoadingWorkV

        #region SetCefSharpExplorerView

        /// <summary>
        /// CefSharp 탐색기 화면 뷰모델(CefSharpExplorerVM.cs) 데이터 모델 객체 값 셋팅
        /// </summary>
        public void SetCefSharpExplorerView(CefSharpExplorerView cefSharpExplorerView)
        {
            _CefSharpExplorerView = cefSharpExplorerView;
        }

        #endregion SetCefSharpExplorerView

        #region Execute

        /// <summary>
        /// Modeless 폼(.Show()) 형식에 의해 발생하는 외부 이벤트 메서드(외부 이벤트 핸들러 - CefSharpExplorerRequestHandler) 
        /// </summary>
        public void Execute(UIApplication rvUIApp)
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록 

            try
            {
                Log.Information(Logger.GetMethodPath(currentMethod) + "메서드 Execute 시작");

                _RevitUIApp = rvUIApp;

                // TODO : 필요시 아래 주석친 코드 참고 (2024.10.16 minjae)
                // Revit 문서 프로퍼티 "RevitDoc" 할당
                // _RevitDoc = _RevitUIApp.ActiveUIDocument.Document;    // 활성화된 Revit 문서 

                _RequestIdValue = Request.Take();

                switch (_RequestIdValue)
                {
                    case EnumCefSharpExplorerRequestId.NONE:      // 요청이 없는 경우 -> 즉시 종료 
                        break;
                    case EnumCefSharpExplorerRequestId.OPENFILE:  // 파일 열기 
                        // OpenFile(_RevitUIApp, _RevitDoc, _CefSharpExplorerView);
                        OpenFile(_RevitUIApp, _CefSharpExplorerView);
                        break;
                    default:
                        break;
                }

                Log.Information(Logger.GetMethodPath(currentMethod) + "메서드 Execute 완료");
            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);

                // TODO : 오류 발생시 메시지 박스 출력 구현 (2024.10.18 minjae)
                // 메시지 박스 출력 사유
                // - COBIM 웹 탐색기 JSP 파일에서 오류 메시지를 Alert 으로 대신 출력할 경우
                // Revit 응용 프로그램 -> Revit 문서의 백그라운드에서 돌고있는 UI쓰레드와 충돌이 발생하여
                // Revit 응용 프로그램 자체가 종료(ShutDown)되는 현상이 발생함.
                // 해당 오류 해결 방법
                // 1. Revit 2025 응용 프로그램 최신 버전 업데이트 (AutoDesk Revit 2025.3)
                // 2. 해당 오류를 개선하기 위해 JSP 파일에서 오류 메시지를 Alert 으로 출력 안 하고
                // 아래처럼 MessageBox.Show(ex.Message, CobimHelper.ErrorTitle); 형식으로 출력함.
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
            }
        }

        #endregion Execute

        #region OpenFile

        /// <summary>
        /// 파일열기
        /// </summary>
        public void OpenFile(UIApplication rvUIApp, CefSharpExplorerView cefSharpExplorerView)
        {
            string fileName = string.Empty;                      // 파일(확장자 .rvt) 열기 처리할 파일 이름
            string filePath = string.Empty;                      // 파일(확장자 .rvt) 열기 처리할 파일 전체 경로

            string resMsg = string.Empty;                        // 파일열기 처리 결과(성공 또는 실패) 문자열 초기화  

            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록
        
            try
            {
                if(true == IsOpenFileWorking)   // 파일열기 처리 중인 경우 
                    throw new Exception(CobimHelper.OpenFile_LoadingWork);

                IsOpenFileWorking = true;   // 파일열기 처리 중 시작

                Log.Information(Logger.GetMethodPath(currentMethod) + $"파일명 - {fileName} 열기 시작!");
        
                fileName = cefSharpExplorerView.GetOpenFilePack().GetFileName();
                filePath = cefSharpExplorerView.GetOpenFilePack().GetFilePath();
                ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(filePath);
                
                _DockableDialog = cefSharpExplorerView.GetOpenFilePack().GetDockableDialog();

                // 파일열기 처리 중 화면 출력 
                AppSetting.Default.WindowBase.ShowCommonLoadingWork(CobimHelper.OpenFile_LoadingWork, _DockableDialog);

                // 파일열기 처리
                rvUIApp.OpenAndActivateDocument(modelPath,
                                                new OpenOptions(),
                                                false);

                resMsg = "success";  // 파일열기 성공   
                Log.Information(Logger.GetMethodPath(currentMethod) + $"파일명 - {fileName} 열기 완료!");
                // TODO : 파일 열기 기능의 경우 아래 주석친 테스트 코드 메서드 _CommonLoadingWorkV.Dispatcher.Invoke() 실행시
                //        파일열기 처리 중 화면이 종료되지 않는 오류 발생!
                //        하여 해당 오류 해결하기 위해 메세지 박스 출력 메서드 MessageBox.Show 호출시 첫 번째 인자값으로
                //        메서드 AppSetting.Default.WindowBase.GetCommonLoadingWorkV() 호출 반환값인 
                //        Window - 로그인, 체크아웃, 체크인, 다운로드 시 처리 중 화면 객체 값으로 할당 구현 (2024.11.28 minjae) 
                MessageBox.Show(AppSetting.Default.WindowBase.GetCommonLoadingWorkV(), $"파일명 - {fileName}\r\n열기 완료!");

                // TODO : 아래 주석친 테스트 코드 필요시 참고 (2024.10.16 minjae)
                // ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(@"C:\Users\bhjeon\Desktop\상상진화 회사 업무\RevitBox\RevitBox 버전업\RevitBox 2025\Revitbox 템플릿\Angles.rfa");
                // rvUIApp.OpenAndActivateDocument(modelPath,
                //                                 new OpenOptions(),
                //                                 false);

                // _DockableDialog.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                // {
                //     // _RevitUIApp = AppSetting.Default.DockableDialogBase.GetRevitUIApp();
                //     // rvUIApp.OpenAndActivateDocument(@"C:\Users\bhjeon\Desktop\상상진화 회사 업무\RevitBox\RevitBox 버전업\RevitBox 2025\Revitbox 템플릿\Angles.rfa");
                //     // rvUIApp.OpenAndActivateDocument(filePath);
                // 
                //     rvUIApp.OpenAndActivateDocument(modelPath,
                //                                     new OpenOptions(),
                //                                     false);
                // 
                // }));
                // TODO : 아래 오류 메시지 해결하기 위해 Dispatcher.Invoke 사용 (2024.11.28 minjae)
                // 오류 메시지 - "다른 스레드가 이 개체를 소유하고 있어 호출한 스레드가 해당 개체에 액세스할 수 없습니다"
                // 참고 URL - https://learn.microsoft.com/ko-kr/dotnet/api/system.windows.threading.dispatcher.invoke?view=windowsdesktop-6.0
                // 참고 2 URL - https://insurang.tistory.com/entry/WPF-C-Error-%EB%8B%A4%EB%A5%B8-%EC%8A%A4%EB%A0%88%EB%93%9C%EA%B0%80-%EC%9D%B4-%EA%B0%9C%EC%B2%B4%EB%A5%BC-%EC%86%8C%EC%9C%A0%ED%95%98%EA%B3%A0-%EC%9E%88%EC%96%B4-%ED%98%B8%EC%B6%9C%ED%95%9C-%EC%8A%A4%EB%A0%88%EB%93%9C%EA%B0%80-%ED%95%B4%EB%8B%B9-%EA%B0%9C%EC%B2%B4%EC%97%90-%EC%95%A1%EC%84%B8%EC%8A%A4%ED%95%A0-%EC%88%98-%EC%97%86%EC%8A%B5%EB%8B%88%EB%8B%A4-%EC%97%90%EB%9F%AC-%EC%93%B0%EB%A0%88%EB%93%9C-Thread-DispatcherInvoke
                // _CommonLoadingWorkV.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                // {
                //     resMsg = "success";  // 파일열기 성공   
                //     Log.Information(Logger.GetMethodPath(currentMethod) + $"파일명 - {fileName} 열기 완료!");
                //     MessageBox.Show($"파일명 - {fileName}\r\n열기 완료!");
                // }));
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                resMsg = ex.Message;  // 파일열기 처리 결과 오류 메시지 발생시 string 클래스 객체 resMsg에 할당
                throw;   // 오류 발생시 상위 호출자 예외처리 전달 throw 
            }
            finally
            {
                // 파일열기 처리 중이 아닌 경우 
                if(resMsg != CobimHelper.OpenFile_LoadingWork)
                {
                    IsOpenFileWorking = false;   // 파일열기 처리 중 종료
                    AppSetting.Default.WindowBase.HideCommonLoadingWork(_DockableDialog);   // 파일(확장자 .rvt) 열기 처리 중 화면 숨기기
                } 
            }
        }

        #endregion OpenFile

        #region Sample

        #endregion Sample
    }

    #region Sample

    #endregion Sample
}
