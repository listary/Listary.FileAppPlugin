# Getting Started
In this tutorial, we will create a plugin for WinRAR (a file archiver) to learn how to develop a Listary file application plugin. You are assumed to have basic knowledge of [C\#](https://learn.microsoft.com/en-us/dotnet/csharp/tour-of-csharp/) and [Win32 windows](https://learn.microsoft.com/en-us/windows/win32/winmsg/about-windows). The complete source code for this plugin can be found at [Listary.FileAppPlugin.WinRAR](https://github.com/listary/Listary.FileAppPlugin.WinRAR).

WinRAR has an "Extraction path and options" dialog. It is not a system file dialog, and therefore Listary does not support it without integration. To support this dialog, we can create a file application plugin for WinRAR. Selecting a folder in Listary will populate the destination path.

This is how to open the extract dialog:

1. Open WinRAR or [download and install it](https://www.win-rar.com/start.html?&L=0) if you haven’t.  
2. Go to any .zip or .rar file  
3. Select it and click the Extract To button.  
4. Ensure the Listary window pops up below the WinRAR extract dialog

![](images/extract-dialog.png)

## Setting up the project
Before beginning to write the code, we need to create a class library project targeting the .NET Framework downloaded through the Visual Studio Installer:

Before beginning to write the code, we need to create a class library project targeting the .NET Framework downloaded through the Visual Studio Installer:

1. Open [Visual Studio Installer](https://visualstudio.microsoft.com/downloads/).  
2. Go to the installed tab and click Modify. If you don’t have a version installed yet, go to the Available tab and click Install.

![](images/install-enviroment.png)

3. Select .NET desktop development as the workload.  
4. Under the individual components tab, select C++ Core Features and C++ Profiling Tools to gain access to Spy++ [used in finding the WinRAR window handle]

![](images/find-addons.png)

5. Open Visual Studio.  
6. Click “Create a New Project” or going through File → New → Project  
7. Select Class Library (.NET Framework) and click Next

![](images/new-project.png)

8. Fill in the project name, set the framework version to .NET Framework 4.8, and click Create

![](images/project-name-screen.png)

Project names should follow the naming convention: `Listary.FileAppPlugin.\[ApplicationName\]`, for example, `Listary.FileAppPlugin.WinRAR`. But here we are not developing a plugin for the main window of WinRAR, but for the extract dialog, hence `Listary.FileAppPlugin.WinRAR.ExtractDialog` would be a more appropriate name for our plugin.

9. Add the plugin interface package [Listary.FileAppPlugin](https://www.nuget.org/packages/Listary.FileAppPlugin) to our project: Tools → NuGet Package Manager → Package Manager Console  
10.  In the console, run the command `NuGet\\Install-Package Listary.FileAppPlugin \-Version 1.0.0.` 
    ![](images/open-pmc.png)

## Adding the plugin manifest
A manifest file is required for the plugin to be recognized. The manifest file should be a JSON file named `ListaryPluginManifest.json`. You can add it to your project:

1. Go to the to menu: File → New → File  
2. Choose a text file, and click Open

![](images/new-text-file.png)

4. Save the new file as ListaryPluginManifest.json.

![](images/save-manifest-name.png)

6. Copy and paste this text to the ListaryPluginManifest.json manifest file:

```json
{
  "ManifestVersion": 1,

  "Id": "WinRAR.ExtractDialog",
  "PluginType": "FileAppPlugin",
  "PluginVersion": "0.1.0",
  "DisplayName": "WinRAR Extract Dialog",
  "Authors": [ "Listary" ],
  "Description": "Listary file application plugin for WinRAR's extract dialog.",
  "Homepage": "https://github.com/listary/Listary.FileAppPlugin.WinRAR",
  "Icon": "Icon.png",

  "Api": "NetFramework",
  "ApiVersion": 1,
  "Entry": "Listary.FileAppPlugin.WinRAR.ExtractDialog.dll"
}
```


- Id

  Id is an identifier used to uniquely identify our plugin. You can use the project name stripped of the common parts as your Id. You **must not** change your Id after the first release, as doing so will break configuration compatibility on users' computers.

- DisplayName

  The name of your plugin to be display o the user interface.

- Homepage

  Homepage of your plugin. We recommend using the GitHub repository as the plugin's homepage.

- Icon

  Acceptable image formats include `png` and `jpg`.

  To simplify the build process, you can add the icon file to the project and select `Copy if newer` in the `Copy to Output Directory` property, as shown below.

- Api

  Since we are making a .NET Framework plugin, it should be `NetFramework`.

- Entry
  
  The filename of the assembly containing your plugin classes, which would be `[ProjectName].dll` for a project with the default configuration.

5\. Instruct Visual Studio to automatically copy the manifest to the output directory after each build.

1. Right click on `ListaryPluginManifest.json`  
2. Go to the properties  
3. Select Copy if newer in the Copy to Output Directory property

![](images/manifest-file-properties.png)

## Implementing IFileAppPlugin
The entry point of the plugin is `IFileAppPlugin`. What follows is the template in which all Listary FileAppPlugins must follow:
```csharp
/// <summary>
/// A file application plugin. A file application can be a file manager or an application with its own file dialog.
/// 
/// <para>
///   This interface can be implemented with: ...
/// </para>
/// </summary>
public interface IFileAppPlugin
{
    /// <summary>
    /// Whether the folders opened by this file application should be shared with other applications.
    /// <para>
    ///   Currently, this property determines:
    ///   <list type="bullet">
    ///       <item>Whether this file application is used as a source for Quick Switch.</item>
    ///       <item>Whether the folders opened by this file application are displayed in the Currently Opened Folders menu.</item>
    ///   </list>
    ///   This property should be <c>true</c> for file managers and <c>false</c> for file dialogs.
    /// </para>
    /// </summary>
    bool IsOpenedFolderProvider { get; }

    /// <summary>
    /// Whether to automatically switch to the folder opened in other file applications after
    /// pressing hotkeys or switching folders in other file applications.
    /// <para>
    ///   This property should be <c>true</c> for file dialogs and <c>false</c> for file managers.
    /// </para>
    /// </summary>
    bool IsQuickSwitchTarget { get; }

    /// <summary>
    /// Whether the file window of this file application is used by other applications.
    /// This property is used to determine the configuration scope of the plugin.
    /// <para>
    ///   This property should be <c>false</c> for file managers and most non-system file dialogs.
    /// </para>
    /// </summary>
    bool IsSharedAcrossApplications { get; }

    /// <summary>
    /// Which type of search bar to use for this file application.
    /// <para>
    ///   Commonly, <see cref="SearchBarType.Floating"/> is used for file managers and
    ///   <see cref="SearchBarType.Fixed"/> is used for file dialogs.
    /// </para>
    /// </summary>
    SearchBarType SearchBarType { get; }

    /// <summary>
    /// Initialize the plugin.
    /// </summary>
    Task<bool> Initialize(IFileAppPluginHost host);

    /// <summary>
    /// Bind an external window to <see cref="IFileWindow"/>.
    /// </summary>
    /// <returns><c>null</c> if the window cannot be bound (i.e. is an unknown window for the plugin)</returns>
    IFileWindow BindFileWindow(IntPtr hWnd);
}
```

In accordance to the template, our plugin is coded as:

```csharp
namespace Listary.FileAppPlugin.WinRAR.ExtractDialog
{
    public class ExtractDialogPlugin : IFileAppPlugin
    {
        private IFileAppPluginHost _host;

        public bool IsOpenedFolderProvider => false;
        
        public bool IsQuickSwitchTarget => true;
        
        public bool IsSharedAcrossApplications => false;

        public SearchBarType SearchBarType => SearchBarType.Fixed;
        
        public async Task<bool> Initialize(IFileAppPluginHost host)
        {
            _host = host;
            return true;
        }

        public IFileWindow BindFileWindow(IntPtr hWnd)
        {
            // ...
        }
    }
}
```

### Finding the WinRAR window handle

To implement `BindFileWindow`, we need to find a way to identify WinRAR's extract dialog. We can use Visual Studio's [Spy++](https://learn.microsoft.com/en-us/visualstudio/debugger/introducing-spy-increment) to do this. You can start Spy++ from the tools menu:

![](images/acess-spy.png)
Spy++ is included in C++ core features and C++ profiling tools which we [installed earlier](#setting-up-the-project).

After starting it, you should open the [WinRAR's extract dialog](#getting-started), refresh the windows in Spy++ and find the window of WinRAR's extract dialog:

1. Click Search in the toolbar and then Find Window  
2. Fill the Caption box with Extraction path and options  
3. Fill the Class box with \#32770 (Dialog)

Alternatively, you can use the Finder Tool and drag it over the window title of the extraction dialog.

![](images/spy_find.png)
### Implementing BindFileWindow()

How to identify the currently opened window as WinRAR's extract dialog:

```csharp
public IFileWindow BindFileWindow(IntPtr hWnd)
{
    // It is a Win32 dialog box?
    if (Win32Utils.GetClassName(hWnd) == "#32770")
    {
        // It is from WinRAR?
        if (Win32Utils.GetProcessPathFromHwnd(hWnd).EndsWith("\\WinRAR.exe", StringComparison.OrdinalIgnoreCase))
        {
            // It is WinRAR's extract dialog?
            // GetDlgItem() is used to retrieve the handle to a control with the specified ID
            IntPtr tab = Win32Utils.GetDlgItem(hWnd, 0x3020);
            if (Win32Utils.GetClassName(tab) == "SysTabControl32")
            {
                return new ExtractDialogWindow(_host, hWnd);
            }
        }
    }
    return null;
}
```
Remember to implement this in `ExtractDialogPlugin`.

In this process, you will usually need to call some Win32 APIs. You can get P/Invoke signatures of the APIs you want to use from [pinvoke.net: the interop wiki](https://pinvoke.net/). You can also use the [C\#/Win32 P/Invoke Source Generator](https://github.com/microsoft/CsWin32), but this requires you to change the language version of the project to at least C\# 9\. For the sake of simplicity, [this project uses the former](https://github.com/listary/Listary.FileAppPlugin.WinRAR/blob/main/Listary.FileAppPlugin.WinRAR.ExtractDialog/Win32Utils.cs); if you need an example using the latter, please see [Listary.FileAppPlugin.DirectoryOpus](https://github.com/listary/Listary.FileAppPlugin.DirectoryOpus/blob/main/Listary.FileAppPlugin.DirectoryOpus/Win32Utils.cs).

## Implementing IFileWindow and IFileTab
`IFileWindow` and `IFileTab` are the interfaces Listary uses to manipulate the file window. How to implement them depends on the GUI framework used by the application you want to target:

* For Win32, MFC and Windows Forms, you can manipulate their windows using window messages. See [Common Controls](https://learn.microsoft.com/en-us/windows/win32/controls/individual-control-info) for more information.  
* For other GUI frameworks, including Qt, XAML-based GUI (WPF, UWP, WinUI and Avalonia UI) and web-based GUI (Electron and CEF), you can manipulate their windows using [UI Automation](https://learn.microsoft.com/dotnet/framework/ui-automation/ui-automation-overview) (successor to Microsoft Active Accessibility).  
* If the application is extensible or you are just the developer, you can implement them using IPC.

WinRAR uses MFC, so here we will implement `IFileWindow` and `IFileTab` using window messages.

`IFileWindow’s` outline is defined as:
```csharp
/// <summary>
/// A file window that can display files of different folders.
/// Each folder is displayed in a container called a file tab (<see cref="IFileTab"/>).
/// 
/// <para>
///   This interface can be implemented with: ...
/// </para>
/// </summary>
public interface IFileWindow
{
    IntPtr Handle { get; }

    /// <summary>
    /// Get the current file tab.
    /// </summary>
    Task<IFileTab> GetCurrentTab();
}
```

Our implementation of the new class is very simple:

```csharp
namespace Listary.FileAppPlugin.WinRAR.ExtractDialog
{
    public class ExtractDialogWindow : IFileWindow
    {
        private IFileAppPluginHost _host;

        public IntPtr Handle { get; }

        public ExtractDialogWindow(IFileAppPluginHost host, IntPtr hWnd)
        {
            Handle = hWnd;
            _host = host;
        }

        public async Task<IFileTab> GetCurrentTab()
        {
            return new ExtractDialogTab(_host, Handle);
        }
    }
}
```
You may be confused about `GetCurrentTab` because the extract dialog does not contain a folder tab. The key is to understand that a "file tab" in a file application plugin is actually "a container that can display a folder", it is not necessary to have a real tab control.

`IFileTab's` outline is defined as:

```csharp
/// <summary>
/// A file tab that displays files from the same folder.
/// <para>
/// This interface can be implemented with:
///   <list type="bullet">
///     <item><term><see cref="IGetFolder"/></term>
///       <description>Get the folder displayed in this tab.</description>
///     </item>
///     <item><term><see cref="IOpenFolder"/></term>
///       <description>Jump to a folder in this tab.</description>
///     </item>
///     <item><term><see cref="IOpenFolderAndSelectFile"/></term>
///       <description>Jump to a folder in this tab and select a specified file in it.</description>
///     </item>
///     <item><term><see cref="IOpenFile"/></term>
///       <description>Open a file in this tab. Mainly used for file dialogs.</description>
///     </item>
///   </list>
/// </para>
/// </summary>
public interface IFileTab
{
}
```

And our implementation:

```csharp
namespace Listary.FileAppPlugin.WinRAR.ExtractDialog
{
    public class ExtractDialogTab : IFileTab, IGetFolder, IOpenFolder
    {
        private IFileAppPluginHost _host;
        private IntPtr _pathEditor = IntPtr.Zero;
        private IntPtr _displayButton = IntPtr.Zero;

        public ExtractDialogTab(IFileAppPluginHost host, IntPtr dialog)
        {
            _host = host;

            IntPtr general = Win32Utils.FindWindowEx(dialog, IntPtr.Zero, "#32770", null);
            if (general == IntPtr.Zero)
            {
                _host.Logger.LogError("Failed to find General dialog");
                return;
            }

            _displayButton = Win32Utils.GetDlgItem(general, 0x66);
            if (_displayButton == IntPtr.Zero)
            {
                _host.Logger.LogWarning("Failed to find Display button");
            }

            IntPtr comboBox = Win32Utils.GetDlgItem(general, 0x65);
            if (comboBox == IntPtr.Zero)
            {
                _host.Logger.LogError("Failed to find the path combo box");
                return;
            }

            _pathEditor = Win32Utils.GetDlgItem(comboBox, 0x3E9);
            if (_pathEditor == IntPtr.Zero)
            {
                _host.Logger.LogError("Failed to find the path editor");
            }
        }

        public async Task<string> GetCurrentFolder()
        {
            if (_pathEditor != IntPtr.Zero)
            {
                return _host.GetWindowText(_pathEditor);
            }
            return string.Empty;
        }

        public async Task<bool> OpenFolder(string path)
        {
            if (_pathEditor != IntPtr.Zero)
            {
                if (_host.SetWindowText(_pathEditor, path))
                {
                    _host.PostMessage(_displayButton, Win32Utils.BM_CLICK, IntPtr.Zero, IntPtr.Zero);
                    return true;
                }
                else
                {
                    _host.Logger.LogError("Failed to set the text of the path editor");
                }
            }
            return false;
        }
    }
}
```
Note: These magic hex numbers are derived from the control Id’s of the parts of the extract dialog box. Control Id can be determined in Spy++ by:

1\. Right clicking the window you wish to see the Id of.  
2\. Select Properties.  
![](images/spy-winRAR-found.png)

Finally, you should make sure that all your classes implementing a plugin interface are **public**, otherwise the plugin will not load properly.

In the end you should have three separate classes in the same namespace.

## Testing the plugin
After we have completed our plugin, we can debug it with breakpoints using File Application Plugin DevTools.

You can set the project's start action to launch DevTools:

1. Right Click the Project in the Solution Manager  
2. Click Properties  
3. Click Debug

![](images/activate-debugger.png)

There is no need to specify the command line arguments as DevTools will automatically load the plugin from the working directory.

The path to the DevTools is `..\\Listary\\Listary.FileAppPlugin.DevTools.exe`

You may also open the DevTools using launch profiles:

1. Go to the Launch Profiles screen.  
2. Click the Create a New Profile button in the top left.  
3. Set the profile to be executable.  
4. Direct the executable path to the DevTools

![](images/launch-profiles.png)

### Testing BindFileWindow()

1\. Open the [WinRAR extract dialog](#getting-started).

2\. Set a breakpoint in `BindFileWindow()` (press F9)

3\. Launch the Visual Studio debugger (press F5). DevTools will load the plugin and display the plugin information for you to check. 

![](images/debugger-menu.png)

4\. Switch to the File windows tab. It will show all the opened WinRAR's extract dialog if `BindFileWindow` is working correctly.

### Testing IOpenFolder()

1\. Set a breakpoint in `IOpenFolder`

2\. Right click the target item and select `Jump to C:\\Program Files`.

![](images/jump-to-c-test.png)

## Build and Deploy the plugin

First we need to build the plugin to create the necessary dlls.

1. From the Visual Studio menu: `Build → Build Solution`  
2. Copy the output directory to Listary's plugins directory   
   (`..\\Listary\\FileAppPlugins`) and test the plugin with Listary.  
   ![](images/example-path.png)
3. Restart Listary  
4. Disable Listary’s existing WinRAR plugin and enable yours through the `Listary options → Integration`.

![](images/listary-settings-path.png)

The output directory is `\<The folder you put your original project in\>\\bin\\debug` OR `\<The folder you put your original project in\>\\\<project name\>\\bin\\debug\\net48`.

5. Ensure the Listary window pops up under the WinRAR extraction dialog (ie: `BindFileWindow` works) and that choosing a destination folder in Listary fills the Destination path (ie: `IOpenFolder` works)

**Listary working for the WinRAR extract dialog.**

![](images/working-for-winRAR.png)

## **Submitting the plugin**

After you have finished the plugin, you can submit it by creating a pull request to [Listary.FileAppPlugin.Repository](https://github.com/listary/Listary.FileAppPlugin.Repository). Please note that for security reasons, we do not accept any closed-source plugins.  
