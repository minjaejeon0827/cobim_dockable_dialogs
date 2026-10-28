namespace CobimDockableDialogs.Data.Models.CobimBase.CefSharpExplorer
{
    // TODO : 필요시 TCefSharpExplorerView.cs 클래스 파일 구현 예정 (2024.09.24 jbh)

    // 제네릭 타입 제약 문법 where T 
    // 참고 URL - http://www.csharpstudy.com/CSharp/CSharp-generics.aspx
    public class TCefSharpExplorerView<TData> : TCefSharpExplorerView where TData : TCefSharpExplorerView<TData> { }

    // TODO : 일단 임시로 아래 처럼 구현해서 사용 추후 필요시 CDEModelBase 클래스 구현 예정 (2024.09.24 jbh)    
    public class TCefSharpExplorerView : CDEModelBase<TCefSharpExplorerView>
    {
        #region 프로퍼티 

        #endregion 프로퍼티
    }
}
