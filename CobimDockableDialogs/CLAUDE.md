# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

`CobimDockableDialogs` is a **Revit 2025 add-in** (WPF, .NET 8, `net8.0-windows`) migrated from the MOLIT (국토교통부) COBIM explorer application. It docks an embedded Chromium browser (CefSharp via `ricaun.Revit.CefSharp`) inside Revit that loads the COBIM CDE web UI (`https://cobim.io`). The web page calls back into C# to download / check-out / check-in / cancel work on CDE containers and to open `.rvt` files in Revit.

## Build & Run

```powershell
# from the solution directory (the one containing CobimDockableDialogs.sln)
dotnet build CobimDockableDialogs.sln -c Debug            # or -p:Platform=x64
dotnet build CobimDockableDialogs\CobimDockableDialogs.csproj
```

- Revit 2025 must be installed: `RevitAPI.dll` / `RevitAPIUI.dll` are referenced by absolute `HintPath` (`C:\Program Files\Autodesk\Revit 2025\`) with `Private=False`.
- Debugging: `Properties/launchSettings.json` has a `Revit2025` profile that launches `Revit.exe`.
- There is **no `.addin` manifest in the repo**; one must exist in Revit's Addins folder pointing to the built `CobimDockableDialogs.dll` with `FullClassName` `CobimDockableDialogs.App` (application). The ribbon button targets an `IExternalCommand` in `Command.cs` (`CmdWebExplorer`, `CmdTest`) via `RibbonHelper.Path_COBIM`.
- `CopyLocalLockFileAssemblies=true` so NuGet dependencies (CefSharp, Serilog, …) are copied next to the DLL; `App.LoadFromCobimDockableDialogs` (an `AppDomain.AssemblyResolve` hook) loads them from the add-in folder at runtime.
- No test projects and no lint configuration exist.

## Solution structure (dependency direction)

```
CobimDockableDialogs (add-in, WPF UI)
 ├─> CobimDockableDialogs.Rest.Api   (static HTTP calls to cobim.io)
 │     ├─> CobimDockableDialogs.Data.Models
 │     └─> CobimUtil                (UtilHelper, Serilog Logger helper)
 ├─> CobimDockableDialogs.Data.Models (request/response "Pack" models, CefSharpExplorerView etc.)
 │     └─> CobimNet
 └─> CobimNet                        (BindableBase / INotifyPropertyChanged infrastructure)
```

## Architecture

**Startup (`App.cs`, `IExternalApplication.OnStartup`)**
1. Registers the assembly resolver, configures Serilog (`Common/LogBase/Logger.cs`) — logs go to `%MyDocuments%\COBIM\CobimDockableDialogs\Logs`, daily rolling, 30 files retained.
2. Initializes the singleton `AppSetting.Default` sub-settings (`Settings/`): directories, a shared `HttpClient`, windows (loading spinner), and dockable dialogs.
3. Registers the dockable pane (`DockableDialogSetting.RegisterDockableDialog`) — **must happen during OnStartup**; Revit forbids registering panes later.
4. Creates the ribbon (`Common/RibbonBase`), and subscribes to `DocumentCreated`/`DocumentOpened` to auto-show the pane.

**Global state** lives in `AppSetting.Default` (lazy singleton of `*Setting` classes). String constants, pane titles, GUIDs (`CefSharpExplorerGuId`, `HubBoxGuId`, `ExplorerGuId`), loading messages and the JS bound-object name (`boundAsync`) are in `Common/CobimBase/CobimHelper.cs`. Pane identity is keyed by these title/type strings in `DockableDialogSetting` switch statements — adding a new dockable page means adding constants there plus cases in `CreateDockableDialog` / `GetDockableDialog` / `RegisterDockableDialog`.

**Web ↔ C# bridge (main feature)**
- `Views/.../CefSharpExplorerV.xaml.cs` sets the browser address to the HubBox login URL and, on `JavascriptObjectRepository.ResolveObject` for `boundAsync`, registers its ViewModel (`CefSharpExplorerVM`) as the JS object (`NameConverter = null`, so JS calls use exact C# method names).
- Public async methods on `CefSharpExplorerVM` (`DownLoadAsync`, `CheckOutAsync`, `CheckInAsync`, `CancelWorkAsync`, `OpenFileAsync`) are invoked from JavaScript with a dictionary payload. Each one: guards with an `Is*Working` flag, fills `InputParamData` (a `CefSharpExplorerView` from Data.Models) with `RequestHeader` + `ContainerPack`/`FilePack`/…, shows the common loading window, calls a static method in `Rest.Api/.../CefSharpExplorerRestServer`, and returns a result string (`"success"` or the error message) back to JS.
- Payload keys are defined as enums with `[Description]` attributes and read via `.ToDescription()`.

**Revit API calls from non-Revit threads** go through the ExternalEvent pattern in `RequestBase/`: the VM calls `MakeRequest(EnumCefSharpExplorerRequestId.X)` → `CefSharpExplorerRequest.Make` (Interlocked) → `ExternalEvent.Raise()` → `CefSharpExplorerRequestHandler.Execute(UIApplication)` dispatches on the request id (currently `OPENFILE` → `OpenAndActivateDocument`). Any new Revit-document operation triggered from the web UI must be added as a new enum value + handler case, not called directly from the VM.

**MVVM base classes**: ViewModels derive from `Services/Page/PageBase<TData>` → `CobimScreen` → `CobimNet.BindableBase` (`NotifyOfPropertyChange`, `SetAndNotify`). `Commands/ButtonCommand` distinguishes sync vs async handlers using `CobimHelper.VoidMethodType` / `AsyncMethodType`. Views/ViewModels are mirrored by path under `Views/` and `ViewModels/` (`Pages/DockableDialogs/Project/...`, `Part/...`, `Windows/...`) with `V` / `VM` suffixes.

## Code conventions

- Comments, log messages and UI strings are in **Korean**; `TODO` comments carry a date and author (`(2024.09.19 minjae)`) plus reference URLs — keep this style.
- Every method follows the same pattern: `var currentMethod = MethodBase.GetCurrentMethod();`, `try { … Log.Information(Logger.GetMethodPath(currentMethod) + "…"); } catch (Exception ex) { Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message); … }`. Lower layers rethrow (`throw;`); top-level entry points (commands, event handlers, JS-invoked methods) show `MessageBox.Show(ex.Message, CobimHelper.ErrorTitle)`.
- Comparisons are written explicitly: `if(true == x)` / `if(false == x)`.
- Code is organized with `#region` blocks (`프로퍼티`, `생성자`, `기본 메소드`, …).
- Data.Models types use explicit `GetX()` / `SetX()` accessor methods rather than auto-properties.
