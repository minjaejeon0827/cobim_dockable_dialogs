# CobimDockableDialogs

## 개요

`CobimDockableDialogs`는 국토교통부 COBIM 연구과제에서 개발한 **Revit 2025 애드인 프로그램**이다. (WPF, .NET 8, `net8.0-windows`)

Revit 화면 안에 Chromium 웹 브라우저(`ricaun.Revit.CefSharp` 패키지의 CefSharp)를 도킹 창(DockableDialog)으로 띄우고,
그 안에서 COBIM CDE 웹 화면(`https://cobim.io`)을 불러온다.
웹 화면에서 C# 코드를 호출해 CDE 컨테이너의 파일 다운로드 / 체크아웃 / 체크인 / 작업 취소를 처리하고, Revit 파일(`.rvt`)을 Revit 응용 프로그램에서 연다.

## 빌드 및 실행

```powershell
# 솔루션 폴더(CobimDockableDialogs.sln이 있는 폴더)에서 실행
dotnet build CobimDockableDialogs.sln -c Debug            # x64 빌드는 -p:Platform=x64 추가
dotnet build CobimDockableDialogs\CobimDockableDialogs.csproj
```

- **Revit 2025 응용 프로그램이 설치되어 있어야 한다.** `RevitAPI.dll` / `RevitAPIUI.dll`을 절대 경로(`C:\Program Files\Autodesk\Revit 2025\`)로 참조하며, 출력 폴더로 복사하지 않는다(`Private=False`).
- **디버깅:** `Properties/launchSettings.json`의 `Revit2025` 프로필을 선택하면 `Revit.exe`가 실행된다.
- **`.addin` 매니페스트 파일은 저장소에 없다.** Revit의 Addins 폴더에 `.addin` 파일을 직접 만들어 빌드된 `CobimDockableDialogs.dll`을 가리키고, `FullClassName`을 `CobimDockableDialogs.App`(Application 타입)으로 지정해야 한다. 리본 버튼은 `RibbonHelper.Path_COBIM`을 통해 `Command.cs`의 `IExternalCommand` 클래스(`CmdWebExplorer`, `CmdTest`)를 실행한다.
- `CopyLocalLockFileAssemblies=true` 설정으로 NuGet 패키지(CefSharp, Serilog 등) DLL이 애드인 DLL과 같은 폴더에 복사된다.
애드인 프로그램 실행 시 `App.LoadFromCobimDockableDialogs`(`AppDomain.AssemblyResolve` 이벤트 핸들러)가 이 폴더에서 해당 DLL을 불러온다.
- 테스트 프로젝트와 린트(lint) 설정은 없다.

## 솔루션 구조 (프로젝트 참조 방향)

```
CobimDockableDialogs (애드인 본체, WPF 화면)
 ├─> CobimDockableDialogs.Rest.Api   (cobim.io 서버와 HTTP 통신하는 static 메서드)
 │     ├─> CobimDockableDialogs.Data.Models
 │     └─> CobimUtil                (UtilHelper, Serilog 로그 헬퍼)
 ├─> CobimDockableDialogs.Data.Models (요청/응답용 "Pack" 모델, CefSharpExplorerView 등)
 │     └─> CobimNet
 └─> CobimNet                        (BindableBase / INotifyPropertyChanged 기반 클래스)
```

## 아키텍처

### 시작 과정 (`App.cs`, `IExternalApplication.OnStartup`)

1. 어셈블리 로드 핸들러를 등록하고 Serilog 로그를 설정한다(`Common/LogBase/Logger.cs`).
   - 로그 저장 위치: `내 문서\COBIM\CobimDockableDialogs\Logs`
   - 하루 단위로 새 파일을 만들고, 최대 30개까지 보관한다.
2. 싱글톤 객체 `AppSetting.Default`의 하위 설정(`Settings/` 폴더)을 초기화한다. (폴더 경로, 공용 `HttpClient`, 로딩 화면 창, 도킹 창)
3. 도킹 창을 등록한다(`DockableDialogSetting.RegisterDockableDialog`).
   - **반드시 OnStartup 안에서 등록해야 한다.** Revit은 시작 이후의 도킹 창 등록을 허용하지 않는다.
4. 리본 메뉴를 만들고(`Common/RibbonBase`), `DocumentCreated` / `DocumentOpened` 이벤트를 구독해 문서를 만들거나 열 때 도킹 창이 자동으로 표시되도록 한다.

### 전역 상태와 상수

- 전역 상태는 `AppSetting.Default`(필요할 때 생성되는 `*Setting` 클래스들의 싱글톤)에서 관리한다.
- 문자열 상수, 도킹 창 제목, GUID(`CefSharpExplorerGuId`, `HubBoxGuId`, `ExplorerGuId`), 로딩 메시지, JavaScript 바인딩 객체 이름(`boundAsync`)은 `Common/CobimBase/CobimHelper.cs`에 모여 있다.
- `DockableDialogSetting`의 switch 문은 이 제목/타입 문자열로 도킹 창을 구분한다.
**새 도킹 창을 추가하려면** `CobimHelper`에 상수를 추가하고, `CreateDockableDialog` / `GetDockableDialog` / `RegisterDockableDialog`에 case를 추가해야 한다.

### 웹 화면 ↔ C# 연동 (핵심 기능)

- `Views/.../CefSharpExplorerV.xaml.cs`는 브라우저 주소를 HubBox 로그인 URL로 설정한다.
`JavascriptObjectRepository.ResolveObject` 이벤트에서 `boundAsync` 객체를 요청 받으면 ViewModel(`CefSharpExplorerVM`)을 JavaScript 객체로 등록한다.
  - `NameConverter = null`로 설정했기 때문에 JavaScript에서는 C# 메서드 이름을 그대로(대소문자 변환 없이) 호출한다.
- JavaScript는 `CefSharpExplorerVM`의 public async 메서드(`DownLoadAsync`, `CheckOutAsync`, `CheckInAsync`, `CancelWorkAsync`, `OpenFileAsync`)를 딕셔너리 형태의 데이터와 함께 호출합니다. 각 메서드는 다음 순서로 동작한다.
  1. `Is*Working` 플래그로 중복 실행을 막는다.
  2. `InputParamData`(Data.Models의 `CefSharpExplorerView`)에 `RequestHeader`, `ContainerPack`, `FilePack` 등을 채운다.
  3. 공용 로딩 화면을 표시한다.
  4. `Rest.Api/.../CefSharpExplorerRestServer`의 static 메서드를 호출한다.
  5. 결과 문자열(`"success"` 또는 오류 메시지)을 JavaScript로 돌려준다.
- 전달 데이터의 키(key)는 `[Description]` 특성이 붙은 enum으로 정의되어 있으며, `.ToDescription()`으로 읽는다.

### Revit 외부 스레드에서 Revit API 호출

Revit API는 Revit이 허용한 시점에만 호출할 수 있으므로, `RequestBase/` 폴더의 **ExternalEvent 패턴**을 사용한다.

```
VM.MakeRequest(EnumCefSharpExplorerRequestId.X)
  → CefSharpExplorerRequest.Make (Interlocked로 요청 ID 저장)
  → ExternalEvent.Raise()
  → CefSharpExplorerRequestHandler.Execute(UIApplication)  // 요청 ID별로 분기
```

현재는 `OPENFILE`(→ `OpenAndActivateDocument`로 파일 열기)만 있다. 웹 화면에서 요청하는 새로운 Revit 문서 작업은 VM에서 직접 호출하지 말고, **enum 값과 핸들러 case를 새로 추가**해서 구현해야 한다.

### MVVM 기반 클래스

- ViewModel 상속 구조: `Services/Page/PageBase<TData>` → `CobimScreen` → `CobimNet.BindableBase` (`NotifyOfPropertyChange`, `SetAndNotify` 제공)
- `Commands/ButtonCommand`는 `CobimHelper.VoidMethodType` / `AsyncMethodType` 값으로 일반(void) 메서드와 비동기 메서드를 구분해 실행한다.
- View와 ViewModel은 `Views/`와 `ViewModels/` 아래에 같은 경로 구조(`Pages/DockableDialogs/Project/...`, `Part/...`, `Windows/...`)로 배치되며, 이름 끝에 각각 `V` / `VM`을 붙인다.

## 코드 작성 규칙

- 주석, 로그 메시지, 화면 문구는 **한국어**로 작성한다. `TODO` 주석에는 날짜와 작성자(`(2024.09.19 minjae)`), 참고 URL을 함께 적는다.
- 모든 메서드는 같은 패턴을 따른다.

  ```csharp
  var currentMethod = MethodBase.GetCurrentMethod();

  try
  {
      // ...
      Log.Information(Logger.GetMethodPath(currentMethod) + "...");
  }
  catch (Exception ex)
  {
      Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
      // ...
  }
  ```

  - 하위 계층에서는 예외를 상위 호출자에게 다시 던진다(`throw;`).
  - 최상위 진입점(커맨드, 이벤트 핸들러, JavaScript에서 호출하는 메서드)에서는 `MessageBox.Show(ex.Message, CobimHelper.ErrorTitle)`로 오류를 표시한다.
- 조건 비교는 명시적으로 작성한다: `if(true == x)` / `if(false == x)`
- 코드는 `#region` 블록(`프로퍼티`, `생성자`, `기본 메소드` 등)으로 구분한다.
- Data.Models의 클래스는 자동 속성 대신 `GetX()` / `SetX()` 메서드로 값을 읽고 쓴다.

### 참고
- [COBIM BIM 기반 인프라 발주-설계 프로세스 디지털 협업 체계 개발](https://scienceon.kisti.re.kr/srch/selectPORSrchReport.do?cn=TRKO202500010351)