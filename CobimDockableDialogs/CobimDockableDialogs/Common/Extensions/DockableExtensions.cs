using System.Windows;

using Autodesk.Revit.UI;

namespace CobimDockableDialogs.Common.Extensions
{
    public static class DockableExtensions
    {
        #region PaneExists

        /// <summary>
        /// DockablePane 존재 여부 확인
        /// </summary>
        public static bool PaneExists(this UIControlledApplication application, Guid id, out DockablePane pane)
        {
            var dPid = new DockablePaneId(id);
            if(true == DockablePane.PaneExists(dPid))
            {
                pane = application.GetDockablePane(dPid);
                return true;
            }
            pane = null;

            return false;
        }

        /// <summary>
        /// DockablePane 존재 여부 확인
        /// </summary>
        public static bool PaneExists(this UIApplication application, Guid id, out DockablePane pane)
        {
            var dPid = new DockablePaneId(id);
            if(true == DockablePane.PaneExists(dPid))
            {
                pane = application.GetDockablePane(dPid);
                return true;
            }
            pane = null;

            return false;
        }

        #endregion PaneExists

        #region RegisterDockablePane

        /// <summary>
        /// DockablePane 등록
        /// </summary>
        public static void RegisterDockablePane(this UIControlledApplication application,
            DockablePaneId dockablePaneId, string title, FrameworkElement dockElement, DockablePaneState state)
        {
            var dataProvider = new DockablePaneProviderData { FrameworkElement = dockElement, InitialState = state };
            IDockablePaneProvider provider = new DockPageProvider(dataProvider);
            application.RegisterDockablePane(dockablePaneId, title, provider);
        }

        // TODO : 아래 주석친 DockablePane 등록 static 메서드 RegisterDockablePane 필요시 참고 (2024.09.25 minjae)
        /// <summary>
        /// DockablePane 등록
        /// </summary>
        //public static void RegisterDockablePane(this UIControlledApplication application,
        //    Guid id, string title, FrameworkElement dockElement, DockablePaneState state)
        //{
        //    var dPid = new DockablePaneId(id);

        //    var dataProvider = new DockablePaneProviderData { FrameworkElement = dockElement, InitialState = state };
        //    IDockablePaneProvider provider = new DockPageProvider(dataProvider);
        //    application.RegisterDockablePane(dPid, title, provider);
        //}

        #endregion RegisterDockablePane

        #region DockPageProvider

        /// <summary>
        /// Revit 응용프로그램에서 호출되며, 
        /// Revit 응용프로그램 화면(UI 인터페이스)에 추가될 
        /// DockableDialog(Dockable Window / Dockable Page)에 대한 정보(데이터)를 제공하는 제공자 클래스  
        /// Revit API 
        /// 인터페이스 IDockablePaneProvider
        /// 참고 URL - https://www.revitapidocs.com/2020/cde36571-ccf1-f628-9e34-6a720388d348.htm
        /// 클래스 DockablePaneProviderData
        /// 참고 URL - https://www.revitapidocs.com/2017/25c4224d-bc54-f2ed-589d-881a6ccbda87.htm
        /// </summary>
        public class DockPageProvider : IDockablePaneProvider
        {
            private readonly DockablePaneProviderData _data;   // Revit 응용 프로그램 화면(UI 인터페이스)에 추가되는 새로운 DockableDialog(Dockable Window / Dockable Page)에 대한 정보(데이터)

            public DockPageProvider(DockablePaneProviderData data)
            {
                _data = data;
            }

            public void SetupDockablePane(DockablePaneProviderData data)
            {
                data.FrameworkElement = _data.FrameworkElement;
                data.InitialState = _data.InitialState;
            }
        }

        #endregion DockPageProvider

        #region Sample

        #endregion Sample
    }
}
