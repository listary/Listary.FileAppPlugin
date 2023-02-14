using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listary.FileAppPlugin
{
    /// <summary>
    /// A file tab that displays files from the same folder.
    /// <para>
    /// This interface can be implemented with:
    ///   <list type="bullet">
    ///     <item><term><see cref="IGetFolder"/></term>
    ///       <description>Get the folder displayed in this tab.</description>
    ///     </item>
    ///     <item><term><see cref="IOpenFolder"/></term>
    ///       <description>Open a folder in this tab.</description>
    ///     </item>
    ///     <item><term><see cref="IOpenFolderAndSelectFile"/></term>
    ///       <description>Open a folder in this tab and select a specified file in it.</description>
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

    /// <summary>
    /// Get the current opened folder.
    /// </summary>
    public interface IGetFolder
    {
        /// <summary>
        /// Get the current opened folder.
        /// </summary>
        Task<string> GetCurrentFolder();
    }

    /// <summary>
    /// Open a file.
    /// </summary>
    public interface IOpenFile
    {
        /// <summary>
        /// Open a file.
        /// </summary>
        Task<bool> OpenFile(string path);
    }
}
