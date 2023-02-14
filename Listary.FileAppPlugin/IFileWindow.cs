using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listary.FileAppPlugin
{
    /// <summary>
    /// A file window that can display files of different folders.
    /// Each folder is displayed in a container called a file tab (<see cref="IFileTab"/>).
    /// 
    /// <para>
    ///   This interface can be implemented with:
    ///   <list type="bullet">
    ///     <item><term><see cref="IGetFileTabs"/></term>
    ///       <description>Get all file tabs of this window.</description>
    ///     </item>
    ///     <item><term><see cref="IOpenFolder"/></term>
    ///       <description>Open a folder in a new file tab of this window.</description>
    ///     </item>
    ///     <item><term><see cref="IOpenFolderAndSelectFile"/></term>
    ///       <description>Open a folder in a new file tab and select a specified file in it.</description>
    ///     </item>
    ///     <item><term><see cref="IDisposable"/></term>
    ///       <description>Provides a mechanism for releasing unmanaged resources.</description>
    ///     </item>
    ///   </list>
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

    /// <summary>
    /// Get all file tabs.
    /// </summary>
    public interface IGetFileTabs
    {
        /// <summary>
        /// Get all file tabs.
        /// </summary>
        Task<IEnumerable<IFileTab>> GetTabs();
    }
}
