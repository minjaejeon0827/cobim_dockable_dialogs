using Serilog;
using System.Reflection;

using Autodesk.Revit.UI;

using CobimDockableDialogs.Common.CobimBase;
using CobimDockableDialogs.Common.Converters;
using CobimDockableDialogs.Common.LogBase;

namespace CobimDockableDialogs.Common.RibbonBase
{
    /// <summary>
    /// 리본 메뉴 
    /// </summary>
    public class Ribbon
    {
        #region 프로퍼티 

        #endregion 프로퍼티 

        #region CreateRibbonControl

        /// <summary>
        /// 리본 컨트롤(메뉴) 등록(생성)
        /// </summary>
        public static void CreateRibbonControl(UIControlledApplication rvUIControlledApp)
        {
            var currentMethod = MethodBase.GetCurrentMethod();   // 로그 기록시 현재 실행 중인 메서드 위치 기록

            try
            {
                Log.Information(Logger.GetMethodPath(currentMethod) + "리본 메뉴 등록 시작");

                // TODO : 리본 탭, 리본 패널, 리본 버튼 구현하기 (2024.08.20 jbh)
                // 참고 URL - https://www.revitapidocs.com/2024/8ce17489-75ee-ae81-306d-58f9c505c80c.htm
                // 참고 2 URL - https://thebuildingcoder.typepad.com/blog/2013/02/adding-a-button-to-existing-ribbon-panel.html
                // 참고 3 URL - https://archi-lab.net/create-your-own-tab-and-buttons-in-revit/

                // 1 단계 : 리본 탭 - COBIM 생성
                rvUIControlledApp.CreateRibbonTab(RibbonHelper.TabName);

                // 2 단계 : 리본 패널 - COBIM 생성
                Dictionary<string, RibbonPanel> panelDic = new Dictionary<string, RibbonPanel>();
                panelDic.Add(RibbonHelper.PanName_COBIM, rvUIControlledApp.CreateRibbonPanel(RibbonHelper.TabName, RibbonHelper.PanName_COBIM));

                // TODO : 필요시 리본 버튼 - COBIM 텍스트 "WebExplorer" 숨김 처리 되도록 구현 예정 (2024.12.09 jbh)
                // 참고 URL - https://stackoverflow.com/questions/76601888/button-with-image-and-empty-string-in-revit-api

                // TODO : 리소스 이미지 파일 "COBIMLogo.png" 추가시 아래와 같은 오류 메시지가 출력하여
                //        리소스 파일(Resources.resx) 마우스 우클릭 -> 컨텍스트 메뉴 "다른 프로그램으로 열기(N)..." 클릭
                //        -> 팝업화면 "연결 프로그램 - Resources.resx" 출력
                //        -> 텍스트 "이 파일을 열 때 사용할 프로그램을 선택하세요(C)." 아래 연결할 수 있는 프로그램들 중에 "관리되는 리소스 편집기(레거시)" 클릭
                //        -> 버튼 "확인" 클릭 -> 예전에 리소스 이미지 파일 추가하는 방식으로 리소스 이미지 파일 "COBIMLogo.png" 추가하기 (2024.12.10 jbh)
                // 오류 코드 "MSB3554"
                // 오류 메시지 - 출력 파일 "D:\bhjeon\Cobim_DockableDialogs\CobimDockableDialogs\CobimDockableDialogs\obj\x64\Debug\net8.0-windows\CobimDockableDialogs.Properties.Resources.resources"에 쓸 수 없습니다.
                //               'D:\bhjeon\Cobim_DockableDialogs\CobimDockableDialogs\CobimDockableDialogs\Properties\Resources\COBIMLogo.png' 경로의 일부를 찾을 수 없습니다.
                // 참고 URL - https://stackoverflow.com/questions/79176337/visual-studio-2022-resource-explorer-add-file
                // 참고 2 URL - https://stackoverflow.com/questions/78873310/vs2022-updates-broke-resource-file/78924871#78924871
                // 3 단계 - 리본 패널 - COBIM 버튼 추가 
                // 버튼 - COBIM
                PushButtonData pdbCOBIM = new PushButtonData(RibbonHelper.BtnName_COBIM, RibbonHelper.BtnName_COBIM, CobimHelper.AssemblyFilePath, RibbonHelper.Path_COBIM);
                pdbCOBIM.LargeImage = BitmapConverter.ConvertFromBitmap(Properties.Resources.COBIMLogo);   // 아이콘 셋팅
                panelDic[RibbonHelper.PanName_COBIM].AddItem(pdbCOBIM);

                // TODO : 아래 주석친 코드 필요시 참고 (2024.12.09 jbh)
                // 4 단계 : 리본 패널 - 성과품, (주)상상진화, HubBox 생성
                // Dictionary<string, RibbonPanel> panelDic = new Dictionary<string, RibbonPanel>();
                // panelDic.Add(RibbonHelper.PanName_CefSharpCDE, rvUIControlledApp.CreateRibbonPanel(RibbonHelper.TabName, RibbonHelper.PanName_CefSharpCDE));
                // panelDic.Add(RibbonHelper.PanName_ImagineBuilder, rvUIControlledApp.CreateRibbonPanel(RibbonHelper.TabName , RibbonHelper.PanName_ImagineBuilder));
                // panelDic.Add(RibbonHelper.PanName_HubBox, rvUIControlledApp.CreateRibbonPanel(RibbonHelper.TabName, RibbonHelper.PanName_HubBox));

                // 5 단계 : 리본 패널 - 성과품 버튼 추가 
                // 버튼 - 성과품(CDE)
                // PushButtonData pdbCefSharpCDE = new PushButtonData(RibbonHelper.BtnName_CefSharpCDE, RibbonHelper.BtnName_CefSharpCDE, CobimHelper.AssemblyFilePath, RibbonHelper.Path_CefSharpCDE);
                // pdbCefSharpCDE.LargeImage = BitmapConverter.ConvertFromBitmap(Properties.Resources.CefSharpCDE);   // 아이콘 셋팅
                // panelDic[RibbonHelper.PanName_CefSharpCDE].AddItem(pdbCefSharpCDE);

                // 6 단계 : 리본 패널 - (주)상상진화 하단 버튼 추가 (물음표(?) 이미지 추가) 
                // 버튼 - Home Page
                // PushButtonData pdbImagineBuilder = new PushButtonData(RibbonHelper.BtnName_ImagineBuilder, RibbonHelper.BtnName_ImagineBuilder, CobimHelper.AssemblyFilePath, RibbonHelper.Path_ImagineBuilder);
                // pdbImagineBuilder.LargeImage = BitmapConverter.ConvertFromBitmap(Properties.Resources.ImagineBuilder); // 아이콘 셋팅
                // panelDic[RibbonHelper.PanName_ImagineBuilder].AddItem(pdbImagineBuilder);

                // 7 단계 : 리본 패널 - HubBox 버튼 추가 
                // 버튼 - HubBox
                // PushButtonData pdbHubBox = new PushButtonData(RibbonHelper.BtnName_HubBox, RibbonHelper.BtnName_HubBox, CobimHelper.AssemblyFilePath, RibbonHelper.Path_HubBox);
                // pdbHubBox.LargeImage = BitmapConverter.ConvertFromBitmap(Properties.Resources.HubBox);   // 아이콘 셋팅
                // panelDic[RibbonHelper.PanName_HubBox].AddItem(pdbHubBox);

                Log.Information(Logger.GetMethodPath(currentMethod) + "리본 메뉴 등록 완료");
            }
            catch(Exception ex)
            {
                Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
                throw;   // 오류 발생시 예외처리 throw 
            }
        }

        #endregion CreateRibbonControl

        #region Sample

        #endregion Sample
    }
}
