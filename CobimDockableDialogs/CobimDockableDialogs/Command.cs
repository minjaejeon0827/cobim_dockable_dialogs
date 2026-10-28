using Serilog;

using System.Windows;
using System.Reflection;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using CobimDockableDialogs.Common.CobimBase;
using CobimDockableDialogs.Common.LogBase;
using CobimDockableDialogs.Settings;

namespace CobimDockableDialogs
{

    #region CmdTest

    // TODO : TransactionAttribute - TransactionMode.Manual 설정 구현 (2024.08.20 minjae)
    // 참고 URL - https://manzoo.tistory.com/107
    [Transaction(TransactionMode.Manual)]       // Transaction 사용이 필요한 경우 (Document 내에서 수정/삭제/추가 등의 작업이 필요한 경우)
    [Regeneration(RegenerationOption.Manual)]
    internal class CmdTest : IExternalCommand
    {
        #region 프로퍼티

        #endregion 프로퍼티

        #region 기본 메소드

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                Log.Information(Logger.GetMethodPath(currentMethod) + "Test Command - Execute 시작");

                MessageBox.Show("테스트 작업 중...", CobimHelper.TestTitle);

                Log.Information(Logger.GetMethodPath(currentMethod) + "Test Command - Execute 종료");

                return Result.Succeeded;
            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
                return Result.Failed;
            }
        }

        #endregion 기본 메소드
    }

    #endregion CmdTest

    #region CmdWebExplorer

    [Transaction(TransactionMode.Manual)]       // Transaction 사용이 필요한 경우 (Document 내에서 수정/삭제/추가 등의 작업이 필요한 경우)
    [Regeneration(RegenerationOption.Manual)]
    /// <summary>
    /// WebExplorer Command 
    /// </summary>
    internal class CmdWebExplorer : IExternalCommand
    {
        #region 프로퍼티 

        #endregion 프로퍼티 

        #region 기본 메소드

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                Log.Information(Logger.GetMethodPath(currentMethod) + "WebExplorer Command - Execute 시작");

                // DockableDialog - CefSharp 탐색기 화면 출력
                AppSetting.Default.DockableDialogBase.ShowDockableDialog(CobimHelper.CefSharpExpTitle, commandData.Application);
                //AppSetting.Default.DockableDialogBase.ShowDockableDialog(CobimHelper.CefSharpExpTitle, commandData.Application, CobimHelper.CefSharpExplorerGuId);

                Log.Information(Logger.GetMethodPath(currentMethod) + "WebExplorer Command - Execute 종료");

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
                return Result.Failed;
            }
        }

        #endregion 기본 메소드
    }

    #endregion CmdWebExplorer

    // TODO : 아래 주석친 코드 필요시 참고 (2024.12.09 minjae)

    #region CmdCefSharpCDE

    // [Transaction(TransactionMode.Manual)]       // Transaction 사용이 필요한 경우 (Document 내에서 수정/삭제/추가 등의 작업이 필요한 경우)
    // [Regeneration(RegenerationOption.Manual)]
    /// <summary>
    /// 성과품(CDE) Command 
    /// </summary>
    //internal class CmdCefSharpCDE : IExternalCommand
    //{
    //    #region 프로퍼티 

    //    #endregion 프로퍼티 

    //    #region 기본 메소드

    //    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    //    {
    //        var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

    //        try
    //        {
    //            Log.Information(Logger.GetMethodPath(currentMethod) + "성과품(CDE) Command - Execute 시작");

    //            // DockableDialog - CefSharp 탐색기 화면 출력
    //            AppSetting.Default.DockableDialogBase.ShowDockableDialog(CobimHelper.CefSharpExpTitle, commandData.Application);
    //            //AppSetting.Default.DockableDialogBase.ShowDockableDialog(CobimHelper.CefSharpExpTitle, commandData.Application, CobimHelper.CefSharpExplorerGuId);

    //            Log.Information(Logger.GetMethodPath(currentMethod) + "성과품(CDE) Command - Execute 종료");

    //            return Result.Succeeded;
    //        }
    //        catch (Exception ex)
    //        {
    //            Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
    //            MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
    //            return Result.Failed;
    //        }
    //    }

    //    #endregion 기본 메소드
    //}

    #endregion CmdCefSharpCDE

    #region CmdHubBox

    // TODO : HubBox Command 클래스 CmdHubBox 필요시 참고 (2024.09.26 minjae)
    //[Transaction(TransactionMode.Manual)]       // Transaction 사용이 필요한 경우 (Document 내에서 수정/삭제/추가 등의 작업이 필요한 경우)
    //[Regeneration(RegenerationOption.Manual)]
    ///// <summary>
    ///// HubBox Command 
    ///// </summary>
    //internal class CmdHubBox : IExternalCommand
    //{
    //    #region 프로퍼티 

    //    #endregion 프로퍼티 

    //    #region 기본 메소드

    //    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    //    {
    //        var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

    //        try
    //        {
    //            Log.Information(Logger.GetMethodPath(currentMethod) + "HubBox Command - Execute 시작");

    //            // DockableDialog - HubBox 화면 출력 
    //            AppSetting.Default.DockableDialogBase.ShowDockableDialog(CobimHelper.HubBoxTitle, commandData.Application, CobimHelper.HubBoxGuId);

    //            Log.Information(Logger.GetMethodPath(currentMethod) + "HubBox Command - Execute 종료");

    //            return Result.Succeeded;
    //        }
    //        catch(Exception ex)
    //        {
    //            Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
    //            MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
    //            return Result.Failed;
    //        }
    //    }

    //    #endregion 기본 메소드
    //}

    #endregion CmdHubBox

    #region CmdImagineBuilder

    // [Transaction(TransactionMode.Manual)]       // Transaction 사용이 필요한 경우 (Document 내에서 수정/삭제/추가 등의 작업이 필요한 경우)
    // [Regeneration(RegenerationOption.Manual)]
    /// <summary>
    /// (주)상상진화 기업 홈페이지 Command 
    /// </summary>
    //internal class CmdImagineBuilder : IExternalCommand
    //{
    //    #region 프로퍼티 

    //    #endregion 프로퍼티 

    //    #region 기본 메소드

    //    // TODO : 콜백 함수 Execute 구현 (2024.08.20 minjae)
    //    // 콜백(CallBack) 함수란? 시스템이 사용자가 요청한 처리를 하다가 특정 이벤트를 발생시켜 해당 이벤트를 처리해달라고 역으로 전달해 오는 함수
    //    // 참고 URL - https://nephrolepis.tistory.com/12
    //    // 참고 2 URL - https://todaycode.tistory.com/24
    //    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    //    {
    //        var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

    //        try
    //        {
    //            Log.Information(Logger.GetMethodPath(currentMethod) + "(주)상상진화 기업 홈페이지 Command - Execute 시작");

    //            // (주)상상진화 기업 홈페이지 URL 주소로 연동
    //            AppSetting.Default.ImagineBuilderBase.ConnectImagineBuilder(CobimHelper.ImagineBuilderURL);

    //            Log.Information(Logger.GetMethodPath(currentMethod) + "(주)상상진화 기업 홈페이지 Command - Execute 종료");

    //            return Result.Succeeded;
    //        }
    //        catch (Exception ex)
    //        {
    //            Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
    //            MessageBox.Show(ex.Message, CobimHelper.ErrorTitle);
    //            return Result.Failed;
    //        }
    //    }

    //    #endregion 기본 메소드
    //}

    #endregion CmdImagineBuilder

    #region Sample

    #endregion Sample
}
