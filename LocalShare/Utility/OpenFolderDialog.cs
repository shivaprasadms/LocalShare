using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LocalShare.Utility
{
    internal class OpenFolderDialog
    {
        #region Variables

        /// <summary>
        /// Gets/sets folder in which dialog will be open.
        /// </summary>
        public string InitialFolder { get; set; }

        /// <summary>
        /// Gets/sets directory in which dialog will be open if there is no recent directory available.
        /// </summary>
        public string DefaultFolder { get; set; }

        /// <summary>
        /// Gets selected folders paths.
        /// </summary>
        public List<string> FoldersPaths = new List<string>();

        /// <summary>
        /// Gets selected folders names.
        /// </summary>
        public List<string> FoldersNames = new List<string>();

        /// <summary>
        /// Gets selected folder path.
        /// </summary>
        public string FolderPath => FoldersPaths.FirstOrDefault();

        /// <summary>
        /// Gets selected folders name.
        /// </summary>
        public string FolderName => FoldersNames.FirstOrDefault();

        /// <summary>
        /// Enable the option to select multiple folders.
        /// </summary>
        public bool Multiselect { get; set; }

        /// <summary>
        /// Sets text for dialog title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Sets text for dialog ok button.
        /// </summary>
        public string OkButtonLabel { get; set; }

        /// <summary>
        /// Sets text for dialog file name label.
        /// </summary>
        public string FileNameLabel { get; set; }

        #endregion

        internal DialogResult ShowDialog()
        {
            if (Environment.OSVersion.Version.Major >= 6)   //ShowVistaDialog
            {
                var dialogFrm = (NativeMethods.IFileDialog)new NativeMethods.FileOpenDialogRCW();
                uint options;
                dialogFrm.GetOptions(out options);
                options |= NativeMethods.FOS_PICKFOLDERS | NativeMethods.FOS_FORCEFILESYSTEM | NativeMethods.FOS_NOVALIDATE | NativeMethods.FOS_NOTESTFILECREATE | NativeMethods.FOS_DONTADDTORECENT;

                if (Multiselect)
                {
                    options |= NativeMethods.FOS_ALLOWMULTISELECT;
                }
                dialogFrm.SetOptions(options);

                if (Title != null)
                {
                    dialogFrm.SetTitle(Title);
                }

                if (OkButtonLabel != null)
                {
                    dialogFrm.SetOkButtonLabel(OkButtonLabel);
                }

                if (FileNameLabel != null)
                {
                    dialogFrm.SetFileName(FileNameLabel);
                }

                if (!string.IsNullOrEmpty(InitialFolder))
                {
                    NativeMethods.IShellItem directoryShellItem;
                    var riid = new Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"); //IShellItem
                    if (NativeMethods.SHCreateItemFromParsingName(this.InitialFolder, IntPtr.Zero, ref riid, out directoryShellItem) == NativeMethods.S_OK)
                    {
                        dialogFrm.SetFolder(directoryShellItem);
                    }
                }
                if (!string.IsNullOrEmpty(DefaultFolder))
                {
                    NativeMethods.IShellItem directoryShellItem;
                    var riid = new Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"); //IShellItem
                    if (NativeMethods.SHCreateItemFromParsingName(this.DefaultFolder, IntPtr.Zero, ref riid, out directoryShellItem) == NativeMethods.S_OK)
                    {
                        dialogFrm.SetDefaultFolder(directoryShellItem);
                    }
                }

                if (dialogFrm.Show() == NativeMethods.S_OK)
                {
                    NativeMethods.IShellItemArray shellItemArray;
                    if (dialogFrm.GetResults(out shellItemArray) == NativeMethods.S_OK)
                    {
                        uint count;
                        if (shellItemArray.GetCount(out count) == NativeMethods.S_OK)
                        {
                            for (uint i = 0; i < count; i++)
                            {
                                shellItemArray.GetItemAt(i, out var item);
                                string path;
                                item.GetDisplayName(NativeMethods.SIGDN_FILESYSPATH, out path);
                                string name;
                                item.GetDisplayName(NativeMethods.SIGDN_PARENTRELATIVE, out name);
                                if (!string.IsNullOrEmpty(path) || !string.IsNullOrEmpty(name))   //path != null || name != null
                                {
                                    //FoldersPaths.Add(Marshal.PtrToStringAuto(path));
                                    //FoldersNames.Add(Marshal.PtrToStringAuto(name));
                                    FoldersPaths.Add(path);
                                    FoldersNames.Add(name);

                                    if (FoldersNames.Count == count)
                                    {
                                        return DialogResult.OK;
                                    }
                                }
                            }
                        }
                    }
                }
                return DialogResult.Cancel;
            }
            else   //ShowLegacyDialog
            {
                using (var dialogFrm = new FolderBrowserDialog())
                {
                    dialogFrm.Description = "OS is not compatible with Windows Vista IFileDialog to support multiple folder selection";
                    if (dialogFrm.ShowDialog() == DialogResult.OK)
                    {
                        FoldersPaths.Add(dialogFrm.SelectedPath);
                        FoldersNames.Add(System.IO.Path.GetFileName(dialogFrm.SelectedPath));
                        return DialogResult.OK;
                    }
                    else
                    {
                        return DialogResult.Cancel;
                    }
                }

                // A hack to use SaveFileDialog to select folder
                //using (var dialogFrm = new SaveFileDialog())
                //{
                //    dialogFrm.CheckFileExists = false;
                //    dialogFrm.CheckPathExists = true;
                //    dialogFrm.CreatePrompt = false;
                //    dialogFrm.Filter = "|" + Guid.Empty.ToString();
                //    dialogFrm.FileName = "any";
                //    if (this.InitialFolder != null) { dialogFrm.InitialDirectory = this.InitialFolder; }
                //    dialogFrm.OverwritePrompt = false;
                //    dialogFrm.Title = "Select Folder (OS is not compatible with Windows Vista IFileDialog to support multiple folder selection)";
                //    dialogFrm.ValidateNames = false;
                //    if (dialogFrm.ShowDialog(owner) == DialogResult.OK)
                //    {
                //        FoldersPaths.Add(Path.GetDirectoryName(dialogFrm.FileName));
                //        FoldersNames.Add(Path.GetFileName(FoldersPaths[0]));
                //        return DialogResult.OK;
                //    }
                //    else
                //    {
                //        return DialogResult.Cancel;
                //    }
                //}
            }
        }

        public void Dispose() { } //just to have possibility of Using statement.
    }

    internal static class NativeMethods
    {
        #region Constants

        //public const uint FOS_OVERWRITEPROMPT = 0x00000002;
        //public const uint FOS_STRICTFILETYPES = 0x00000004;
        //public const uint FOS_NOCHANGEDIR = 0x00000008;
        public const uint FOS_PICKFOLDERS = 0x00000020;
        public const uint FOS_FORCEFILESYSTEM = 0x00000040;
        //public const uint FOS_ALLNONSTORAGEITEMS = 0x00000080;
        public const uint FOS_NOVALIDATE = 0x00000100;
        public const uint FOS_ALLOWMULTISELECT = 0x00000200;
        //public const uint FOS_PATHMUSTEXIST = 0x00000800;
        //public const uint FOS_FILEMUSTEXIST = 0x00001000;
        //public const uint FOS_CREATEPROMPT = 0x00002000;
        //public const uint FOS_SHAREAWARE = 0x00004000;
        //public const uint FOS_NOREADONLYRETURN = 0x00008000;
        public const uint FOS_NOTESTFILECREATE = 0x00010000;
        //public const uint FOS_HIDEMRUPLACES = 0x00020000;
        //public const uint FOS_HIDEPINNEDPLACES = 0x00040000;
        //public const uint FOS_NODEREFERENCELINKS = 0x00100000;
        //public const uint FOS_OKBUTTONNEEDSINTERACTION = 0x00200000;
        public const uint FOS_DONTADDTORECENT = 0x02000000;
        //public const uint FOS_FORCESHOWHIDDEN = 0x10000000;
        //public const uint FOS_DEFAULTNOMINIMODE = 0x20000000;
        //public const uint FOS_FORCEPREVIEWPANEON = 0x40000000;
        //public const uint FOS_SUPPORTSTREAMABLEITEMS = unchecked(0x80000000);

        public const uint S_OK = 0x00000000;

        //public const uint SIGDN_NORMALDISPLAY = 0x00000000;   //Name
        //public const uint SIGDN_PARENTRELATIVEPARSING = 0x80018001;   //Name
        //public const uint SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000;   //Full Name
        //public const uint SIGDN_PARENTRELATIVEEDITING = 0x80031001;   //Name
        //public const uint SIGDN_DESKTOPABSOLUTEEDITING = 0x8004c000;   //Full Name
        public const uint SIGDN_FILESYSPATH = 0x80058000;   //Full Name
        //public const uint SIGDN_URL = 0x80068000;   //Name like a URL
        //public const uint SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8007c001;   //Name
        public const uint SIGDN_PARENTRELATIVE = 0x80080001;   //Name

        #endregion

        #region COM

        [ComImport, ClassInterface(ClassInterfaceType.None), TypeLibType(TypeLibTypeFlags.FCanCreate), Guid("DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7")]
        internal class FileOpenDialogRCW { }

        [ComImport, Guid("d57c7288-d4ad-4768-be02-9d969532d960"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IFileDialog
        {
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            [PreserveSig()]
            uint Show([In, Optional] IntPtr hwndOwner); //IModalWindow 

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetFileTypes([In] uint cFileTypes, [In, MarshalAs(UnmanagedType.LPArray)] IntPtr rgFilterSpec);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetFileTypeIndex([In] uint iFileType);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetFileTypeIndex(out uint piFileType);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint Advise([In, MarshalAs(UnmanagedType.Interface)] IntPtr pfde, out uint pdwCookie);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint Unadvise([In] uint dwCookie);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetOptions([In] uint fos);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetOptions(out uint fos);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, uint fdap);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint Close([MarshalAs(UnmanagedType.Error)] uint hr);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetClientGuid([In] ref Guid guid);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint ClearClientData();

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetResults([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppenum);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetSelectedItems([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppsai);
        }

        [ComImport, Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IShellItem
        {
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint BindToHandler([In] IntPtr pbc, [In] ref Guid rbhid, [In] ref Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out IntPtr ppvOut);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetDisplayName([In] uint sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);   //, out IntPtr ppszName   //, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetAttributes([In] uint sfgaoMask, out uint psfgaoAttribs);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint Compare([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In] uint hint, out int piOrder);
        }

        [ComImport, Guid("b63ea76d-1f85-456f-a19c-48159efa858b"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IShellItemArray
        {
            // Not supported: IBindCtx
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint BindToHandler([In, MarshalAs(UnmanagedType.Interface)] IntPtr pbc, [In] ref Guid rbhid, [In] ref Guid riid, out IntPtr ppvOut);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetPropertyStore([In] int Flags, [In] ref Guid riid, out IntPtr ppv);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetPropertyDescriptionList([In] ref Guid riid, out IntPtr ppv);   //[In] ref PROPERTYKEY keyType, [In] ref Guid riid

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetAttributes([In] uint sfgaoMask, out uint psfgaoAttribs);   //[In] SIATTRIBFLAGS dwAttribFlags, [In] uint sfgaoMask

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetCount(out uint pdwNumItems);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetItemAt([In] uint dwIndex, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

            // Not supported: IEnumShellItems (will use GetCount and GetItemAt instead)
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint EnumItems([MarshalAs(UnmanagedType.Interface)] out IntPtr ppenumShellItems);
        }

        #endregion

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IntPtr pbc, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppv);
    }
}
