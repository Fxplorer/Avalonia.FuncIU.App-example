namespace AvaApp

open Elmish
open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.FuncUI
open Avalonia.FuncUI.Elmish
open Avalonia.FuncUI.Hosts
open Avalonia.Themes.Fluent
open System
open System.Diagnostics

/// This is your application you can ose the initialize method to load styles
/// or handle Life Cycle events of your application
type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add (FluentTheme())
        this.RequestedThemeVariant <- Styling.ThemeVariant.Dark
        this.Styles.Load "avares://AvaApp/Styles.xaml"

    override this.OnFrameworkInitializationCompleted() =
        let init(this: 'T when 'T :> Controls.ContentControl and 'T :> IViewHost) (visualRoot: Rendering.IRenderRoot) =       
            Shell.program
            |> Program.withHost this
            |> Program.run
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as lifetime ->
            lifetime.MainWindow <- {
                new HostWindow() with
                    override this.OnInitialized() =
                        base.Title <- "Full App"
                        init this this.VisualRoot
            }
        | :? ISingleViewApplicationLifetime as lifetime ->
            lifetime.MainView <- {
                new HostControl() with
                    override this.OnInitialized() =
                        init this this.VisualRoot
            }
        | _ -> ()
        base.OnFrameworkInitializationCompleted()



module Program =
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [<STAThread; EntryPoint>]
    let Main(args: string array) =
        About.urlOpen <- fun url ->
            if OperatingSystem.IsWindows() then
                Process.Start(ProcessStartInfo("cmd", $"/c start %s{url}")) |> ignore
            elif OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD() then
                Process.Start("xdg-open", url) |> ignore
            elif OperatingSystem.IsMacOS() then
                Process.Start("open", url) |> ignore
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace(?level = None)
            .StartWithClassicDesktopLifetime(args)