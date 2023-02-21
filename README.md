# <img style="height: 1em;" src="images/icon.png"/> Listary.FileAppPlugin

<!--
This README is also included in the NuGet package. Remember to use absolute links to maintain compatibility.

NuGet does not support HTML elements, so there is no need to use absolute links in <img>, because even then they will not be displayed properly.
-->

[![NuGet](http://img.shields.io/nuget/v/Listary.FileAppPlugin.svg)](https://www.nuget.org/packages/Listary.FileAppPlugin)

[Listary](https://www.listary.com/) file application plugin interfaces.

- [Getting Started](https://github.com/listary/Listary.FileAppPlugin/blob/master/docs/Getting%20Started.md)
- [Plugin Repository](https://github.com/listary/Listary.FileAppPlugin.Repository)

## Architecture diagrams
```mermaid
classDiagram
    direction TB

    note for IFileAppPlugin "A file application plugin.\nA file application can be a file manager or an application with its own file dialog."
    class IFileAppPlugin{
        <<interface>>
        IsOpenedFolderProvider: bool
        IsQuickSwitchTarget: bool
        IsSharedAcrossApplications: bool
        SearchBarType: SearchBarType
        Initialize(host: IFileAppPluginHost): Task~bool~
        BindFileWindow(hWnd: IntPtr): IFileWindow
    }
    IFileAppPlugin *-- IFileWindow : BindFileWindow()

    note for IFileWindow "A file window that can display files of different folders.\nEach folder is displayed in a container called a file tab."
    class IFileWindow{
        <<interface>>
        Handle: IntPtr
        GetCurrentTab(): Task~IFileTab~
    }
    IFileWindow *-- IFileTab : GetCurrentTab()
    IFileWindow *.. IFileTab : IGetFileTabs.GetTabs()

    note for IFileTab "A file tab that displays files from the same folder."
    class IFileTab{
        <<interface>>
    }
```

### [IFileAppPlugin](https://github.com/listary/Listary.FileAppPlugin/blob/master/Listary.FileAppPlugin/IFileAppPlugin.cs)
```mermaid
classDiagram
    direction TB

    IFileAppPlugin ..|> IOpenFolder : can
    IFileAppPlugin ..|> IOpenFolderAndSelectFile : can
    IFileAppPlugin ..|> IDisposable : can
    class IFileAppPlugin{
        <<interface>>
        IsOpenedFolderProvider: bool
        IsQuickSwitchTarget: bool
        IsSharedAcrossApplications: bool
        SearchBarType: SearchBarType
        Initialize(host: IFileAppPluginHost): Task~bool~
        BindFileWindow(hWnd: IntPtr): IFileWindow
    }

    class IDisposable{
        <<interface>>
    }

    note for IOpenFolder "Open a folder in a new file window."
    class IOpenFolder{
        <<interface>>
        OpenFolder(path: string): Task~bool~
    }

    note for IOpenFolderAndSelectFile "Open a folder in a new file window and select a specified file in it."
    class IOpenFolderAndSelectFile{
        <<interface>>
        OpenFolderAndSelectFile(path: string): Task~bool~
    }
```

### [IFileWindow](https://github.com/listary/Listary.FileAppPlugin/blob/master/Listary.FileAppPlugin/IFileWindow.cs)
```mermaid
classDiagram
    direction TB

    IFileWindow ..|> IGetFileTabs : can
    IFileWindow ..|> IOpenFolder : can
    IFileWindow ..|> IOpenFolderAndSelectFile : can
    IFileWindow ..|> IDisposable : can
    class IFileWindow{
        <<interface>>
        Handle: IntPtr
        GetCurrentTab(): Task~IFileTab~
    }

    class IDisposable{
        <<interface>>
    }

    note for IGetFileTabs "Get all file tabs of this window."
    class IGetFileTabs{
        <<interface>>
        GetTabs(): Task~IEnumerable~IFileTab~~
    }

    note for IOpenFolder "Open a folder in a new file tab of this window."
    class IOpenFolder{
        <<interface>>
        OpenFolder(path: string): Task~bool~
    }

    note for IOpenFolderAndSelectFile "Open a folder in a new file tab and select a specified file in it."
    class IOpenFolderAndSelectFile{
        <<interface>>
        OpenFolderAndSelectFile(path: string): Task~bool~
    }
```

### [IFileTab](https://github.com/listary/Listary.FileAppPlugin/blob/master/Listary.FileAppPlugin/IFileTab.cs)
```mermaid
classDiagram
    direction TB

    IFileTab ..|> IGetFolder : can
    IFileTab ..|> IOpenFolder : can
    IFileTab ..|> IOpenFolderAndSelectFile : can
    IFileTab ..|> IOpenFile : can
    class IFileTab{
        <<interface>>
    }

    note for IGetFolder "Get the folder displayed in this tab."
    class IGetFolder{
        <<interface>>
        GetCurrentFolder(): Task~string~
    }

    note for IOpenFile "Open a file in this tab.\nMainly used for file dialogs."
    class IOpenFile{
        <<interface>>
        OpenFile(path: string): Task~bool~
    }

    note for IOpenFolder "Jump to a folder in this tab."
    class IOpenFolder{
        <<interface>>
        OpenFolder(path: string): Task~bool~
    }

    note for IOpenFolderAndSelectFile "Jump to a folder in this tab and select a specified file in it."
    class IOpenFolderAndSelectFile{
        <<interface>>
        OpenFolderAndSelectFile(path: string): Task~bool~
    }
```