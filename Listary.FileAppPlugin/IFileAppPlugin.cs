using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listary.FileAppPlugin
{
    /// <summary>
    /// A file application plugin. A file application can be a file manager or an application with its own file dialog.
    /// <para>
    ///   This interface can be implemented with:
    ///   <list type="bullet">
    ///     <item><term><see cref="IOpenFolder"/></term>
    ///       <description>Open a folder in a new file window.</description>
    ///     </item>
    ///     <item><term><see cref="IOpenFolderAndSelectFile"/></term>
    ///       <description>Open a folder in a new file window and select a specified file in it.</description>
    ///     </item>
    ///     <item><term><see cref="IDisposable"/></term>
    ///       <description>Provides a mechanism for releasing unmanaged resources.</description>
    ///     </item>
    ///   </list>
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

    public enum SearchBarType
    {
        /// <summary>
        /// A floating search bar.
        /// Displayed when activated by keyboard input.
        /// </summary>
        Floating,

        /// <summary>
        /// A search bar fixed with the window.
        /// Displayed after the window is created and until it is closed.
        /// </summary>
        Fixed,
    }

    public interface IFileAppPluginHost
    {
        ILogger Logger { get; }

        /// <summary>
        /// Send the window message with a high integrity level.
        /// This method can work with windows created by a "Run as administrator" process.
        /// <para>
        ///   You should use this method instead of invoking SendMessage yourself.
        /// </para>
        /// </summary>
        /// <param name="wParam">Cannot be a pointer.</param>
        /// <param name="lParam">Cannot be a pointer.</param>
        IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Send the window message with a high integrity level.
        /// This method can work with windows created by a "Run as administrator" process.
        /// <para>
        ///   You should use this method instead of invoking SendMessage yourself.
        /// </para>
        /// </summary>
        /// <param name="wParam">Cannot be a pointer.</param>
        IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, byte[] lParam);

        /// <summary>
        /// Send the WM_COPYDATA message with a high integrity level.
        /// This method can work with windows created by a "Run as administrator" process.
        /// <para>
        ///   You should use this method instead of invoking SendMessage yourself.
        /// </para>
        /// </summary>
        IntPtr SendCopyData(IntPtr hWnd, IntPtr sourceWindow, IntPtr dwData, byte[] data, uint dataSize, uint timeout = 0);

        /// <summary>
        /// Post the window message with a high integrity level.
        /// This method can work with windows created by a "Run as administrator" process.
        /// <para>
        ///   You should use this method instead of invoking PostMessage yourself.
        /// </para>
        /// </summary>        
        /// <param name="wParam">Cannot be a pointer.</param>
        /// <param name="lParam">Cannot be a pointer.</param>
        bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    }

    /// <summary>
    /// Open a folder.
    /// </summary>
    public interface IOpenFolder
    {
        /// <summary>
        /// Open a folder.
        /// </summary>
        Task<bool> OpenFolder(string path);
    }

    /// <summary>
    /// Open a folder and select a specified file in it.
    /// </summary>
    public interface IOpenFolderAndSelectFile
    {
        /// <summary>
        /// Open a folder and select a file in it.
        /// </summary>
        Task<bool> OpenFolderAndSelectFile(string path);
    }
}
