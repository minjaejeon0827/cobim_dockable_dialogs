using System.Windows.Controls;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CobimDockableDialogs.Data.Models.CobimBase.Revit
{
    // 제네릭 타입 제약 문법 where T 
    // 참고 URL - http://www.csharpstudy.com/CSharp/CSharp-generics.aspx
    public class RevitView<TData> : RevitView where TData : RevitView<TData> { }

    // TODO : 추후 필요시 Revit API 전용 데이터 모델 클래스 RevitView 구현 예정 (2024.10.30 jbh) 
    /// <summary>
    /// CobimDockableDialogs
    /// Revit API 전용 데이터 모델
    /// </summary>
    public class RevitView : TRevitView<RevitView>
    {
        #region 프로퍼티 

        #endregion 프로퍼티 

        #region 생성자

        public RevitView() : base() { }

        #endregion 생성자
    }

    #region Sample

    #endregion Sample
}
