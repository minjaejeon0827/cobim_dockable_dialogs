using Serilog;
using System.Reflection;
using System.Windows.Controls;

using CobimDockableDialogs.Common.LogBase;
using CobimDockableDialogs.Common.CobimBase;
using CobimDockableDialogs.Common.Extensions;

//using CobimDockableDialogs.Views.Pages.DockableDialogs.Project.CDE.Explorer;
using CobimDockableDialogs.Views.Pages.DockableDialogs.Project.CefSharpCDE.CefSharpExplorer;
using CobimDockableDialogs.Views.Pages.DockableDialogs.Project.BIM360.HubBox;

using Autodesk.Revit.UI;
using static CobimDockableDialogs.Common.Extensions.DockableExtensions;
using System.Windows.Documents.DocumentStructures;
//using CefSharp.DevTools.Debugger;

namespace CobimDockableDialogs.Settings
{
    /// <summary>
    /// DockableDialog 기본 설정 
    /// </summary>
    public class DockableDialogSetting
    {
        #region 프로퍼티

        /// <summary>
        /// Revit UI 애플리케이션 객체
        /// </summary>
        private UIApplication _RevitUIApp { get; set; }

        // TODO : 필요시 DockableDialog - 탐색기 화면 프로퍼티 "_ExplorerV" 사용 예정 (2024.09.25 jbh)
        /// <summary>
        /// DockableDialog - 탐색기 화면 뷰
        /// </summary>
        //private ExplorerV _ExplorerV { get; set; }

        /// <summary>
        /// DockableDialog - CefSharp 탐색기 화면 뷰 
        /// </summary>
        private CefSharpExplorerV _CefSharpExplorerV { get; set; }

        /// <summary>
        /// DockableDialog - CefSharp 탐색기 화면 뷰
        /// 활성화된 Revit UI 애플리케이션 객체
        /// </summary>
        // private UIApplication _CefSharpExplorerUIApp { get; set; }

        /// <summary>
        /// DockableDialog - HubBox 화면 뷰
        /// 활성화된 Revit UI 애플리케이션 객체
        /// </summary>
        private HubBoxV _HubBoxV { get; set; }

        /// <summary>
        /// DockableDialog 형식으로 출력할 DockablePaneId 객체
        /// </summary>
        private DockablePaneId _DockablePaneId { get; set; }

        /// <summary>
        /// DockableDialog 화면 객체 
        /// </summary>
        private DockablePane _DockablePane { get; set; }

        /// <summary>
        /// DockableDialog - HubBox 화면 뷰
        /// Revit UI 애플리케이션 객체
        /// </summary>
        // private UIApplication _HubBoxUIApp { get; set; }

        #endregion 프로퍼티

        #region 생성자

        // TODO : 필요시 생성자 "DockableDialogSetting" 구현 예정 (2024.10.07 jbh)
        public DockableDialogSetting() 
        {

        }

        #endregion 생성자

        #region CreateDockableDialog

        /// <summary>
        /// DockableDialog 화면 싱글톤 객체 생성
        /// </summary>
        public void CreateDockableDialog()
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // 싱글톤 패턴 사용해서 처음 한번만 프로퍼티 "_ExplorerV", "_CefSharpExplorerV", "_HubBoxV" 객체 생성
                // if(_ExplorerV is null) _ExplorerV = new ExplorerV();   // TODO : 필요시 DockableDialog - 탐색기 화면 프로퍼티 "_ExplorerV" 사용 예정 (2024.09.25 jbh)
                if(_CefSharpExplorerV is null) _CefSharpExplorerV = new CefSharpExplorerV();
                if(_HubBoxV is null) _HubBoxV = new HubBoxV();
            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                throw;   // 오류 발생시 상위 호출자 예외처리 전달
            }
        }

        #endregion CreateDockableDialog

        #region GetDockableDialog

        /// <summary>
        /// DockableDialog 화면 객체 가져오기 
        /// 메서드 파라미터 pDockableDialogName - DockableDialog 화면 뷰 이름
        /// </summary>
        public Page GetDockableDialog(string pDockableDialogName) 
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                switch(pDockableDialogName)
                {
                    case CobimHelper.TypeOfCefSharpExplorer:   // DockableDialog - CefSharp 탐색기 화면 뷰인 경우 
                        return _CefSharpExplorerV;
                    case CobimHelper.TypeOfHubBox:             // DockableDialog - HubBox 화면 뷰인 경우 
                        return _HubBoxV;
                    default:
                        throw new Exception($"[DockableDialog] {pDockableDialogName} 화면 가져오기 오류!\r\n관리자에게 문의하세요.");
                        // break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                throw;   // 오류 발생시 상위 호출자 예외처리 전달
            }
        }
        #endregion GetDockableDialog

        #region RegisterDockableDialog

        /// <summary>
        /// Revit 응용 프로그램 UI 화면에 DockableDialog 객체 생성 및 DockableDialog 화면 등록 및 출력 
        /// </summary>
        public void RegisterDockableDialog(string pDockableDialogTitle, UIControlledApplication rvUIControlledApp, Guid pDockableGuId)
        {
            DockablePaneState dockablePaneState = null;          // DockableDialog 형식으로 출력시 DockPosition 상태 객체 초기화 

            string dockablePaneTitle = string.Empty;             // DockableDialog 화면 제목 

            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                //Log.Information(Logger.GetMethodPath(currentMethod) + "[DockableDialog] 화면 등록 및 출력 시작");

                _DockablePaneId = new DockablePaneId(pDockableGuId);

                // 특정 dockablePaneId 값을 가진 DockableDialog 객체가 등록된 적이 없는 경우 
                // if(false == DockablePane.PaneExists(dockablePaneId))
                if(false == DockablePane.PaneIsRegistered(_DockablePaneId))
                {
                    dockablePaneState = new DockablePaneState { DockPosition = DockPosition.Right };
                    dockablePaneTitle = pDockableDialogTitle;

                    switch(dockablePaneTitle)
                    {
                        case CobimHelper.CefSharpExpTitle:   // DockableDialog - CefSharp 탐색기 화면 뷰인 경우 
                            rvUIControlledApp.RegisterDockablePane(_DockablePaneId, dockablePaneTitle, _CefSharpExplorerV, dockablePaneState);
                            break;
                        case CobimHelper.HubBoxTitle:   // DockableDialog - HubBox 화면 뷰인 경우 
                            rvUIControlledApp.RegisterDockablePane(_DockablePaneId, dockablePaneTitle, _HubBoxV, dockablePaneState);
                            break;
                        // TODO : 아래 주석친 case 레이블(case CobimHelper.TypeOfExplorer:) 필요시 사용 예정 (2024.09.25 jbh)
                        // case CobimHelper.ExpTitle:
                        //     rvUIControlledApp.RegisterDockablePane(dockablePaneId, RibbonHelper.PanName_CDE, _ExplorerV, dockablePaneState);
                        //     rvUIControlledApp.RegisterDockablePane(dockablePaneId, dockablePaneTitle, _ExplorerV, dockablePaneState);
                        //     break;
                        default:
                            throw new Exception($"[DockableDialog] {dockablePaneTitle} 화면 등록 및 출력 오류!\r\n관리자에게 문의하세요.");
                            // break;
                    }
                }

                Log.Information(Logger.GetMethodPath(currentMethod) + $"[DockableDialog] {dockablePaneTitle} 화면 등록 및 출력 완료");
            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                throw;   // 오류 발생시 상위 호출자 예외처리 전달
            }
        }

        #endregion RegisterDockableDialog

        #region ShowDockableDialog

        // Revit API DockablePane 클래스
        // 참고 URL - https://www.revitapidocs.com/2017/671f5ed0-09af-face-532f-72d214131cda.htm

        /// <summary>
        /// Revit 응용 프로그램 UI 화면에 이미 종료한 DockableDialog 화면 출력 
        /// </summary>
        //public void ShowDockableDialog(string pDockableDialogTitle, Guid pDockableGuId)
        public void ShowDockableDialog(string pDockableDialogTitle, UIApplication rvUIApp)
        {
            bool isShown = false;                                // DockableDialog 화면 출력 여부 (출력 - true / 출력안함 - false)

            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                // Log.Information(Logger.GetMethodPath(currentMethod) + "[DockableDialog] 화면 출력 시작");

                // TODO : 오류 메시지 "Object reference not set to an instance of an object." 출력 해결하기 위해 
                //        아래처럼 Revit UI 애플리케이션 객체 값 셋팅 코드 _RevitUIApp = rvUIApp; 구현 (2024.10.24 jbh)
                // Revit UI 애플리케이션 객체 값 셋팅 
                _RevitUIApp = rvUIApp;

                // if(_DockablePane is null) 
                _DockablePane = _RevitUIApp.GetDockablePane(_DockablePaneId);


                // 특정 dockablePaneId 값을 가진 DockableDialog 객체가 기존에 등록되어 있고
                // Revit 응용 프로그램 UI 화면에 DockableDialog 객체가 존재하지 않는 경우(종료 상태) 
                // bool result = DockablePane.PaneIsRegistered(_DockablePaneId);
                // bool result2 = DockablePane.PaneExists(dockablePaneId);
                // bool result3 = _DockablePane.IsShown();
                // bool result4 = _CefSharpExplorerV.IsVisible;

                isShown = (true == DockablePane.PaneIsRegistered(_DockablePaneId)) && (true == _DockablePane.IsShown());

                if(false == isShown) 
                {
                    _DockablePane.Show();
                    Log.Information(Logger.GetMethodPath(currentMethod) + $"[DockableDialog] {pDockableDialogTitle} 화면 출력 완료");
                }

                // TODO : 아래 주석친 조건절 if(dockablePane is not null) 은 사용 안 해도 됨. (2024.09.25 jbh)
                //        왜냐하면 try - catch 문 사용으로 인해 
                //        dockablePane에 null 값이 할당 되면 null Exception 이 발생되어 오류 로그 남기기 때문
                // if(dockablePane is not null)
                //if (true == isNotShown) 
                //{
                //    //dockablePane.Show();   // 이미 종료한 DockableDialog 화면 객체 dockablePane 출력 
                //    _DockablePane.Show();
                //    Log.Information(Logger.GetMethodPath(currentMethod) + $"[DockableDialog] {dockablePaneTitle} 화면 출력 완료");
                //} 
                // TODO : 아래 주석친 else 절 로직 필요시 참고 (2024.09.25 jbh)
                //else throw new Exception($"[DockableDialog] {dockablePaneTitle} 화면 출력 오류!\r\n관리자에게 문의하세요.");
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                throw;   // 오류 발생시 상위 호출자 예외처리 전달
            }
        }

        #endregion ShowDockableDialog

        #region SetRevitUIApp

        /// <summary>
        /// Revit UI 애플리케이션 객체 값 셋팅
        /// 주의사항 - Revit UI 애플리케이션 객체의 경우 실행되는 메인 프로그램(Revit 2025)에서는 오직 하나만 존재한다.(싱글톤 패턴)
        /// 메서드 파라미터 pDockableDialogName - DockableDialog 화면 뷰 이름
        /// </summary>
        public void SetRevitUIApp(UIApplication revitUIApp)
        {
            _RevitUIApp = revitUIApp;
        }

        #endregion SetRevitUIApp

        #region GetRevitUIApp

        /// <summary>
        /// Revit UI 애플리케이션 객체 가져오기 
        /// 주의사항 - Revit UI 애플리케이션 객체의 경우 실행되는 메인 프로그램(Revit 2025)에서는 오직 하나만 존재한다.(싱글톤 패턴)
        /// 메서드 파라미터 pDockableDialogName - DockableDialog 화면 뷰 이름
        /// </summary>
        public UIApplication GetRevitUIApp()
        {
            return _RevitUIApp;
        }

        #endregion GetRevitUIApp

        #region Sample

        #endregion Sample
    }
}
