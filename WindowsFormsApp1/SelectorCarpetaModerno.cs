using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public static class SelectorCarpetaModerno
    {
        public static string SeleccionarCarpeta(IntPtr ownerHandle, string titulo = "Seleccionar Carpeta", string rutaInicial = null)
        {
            try
            {
                var dialog = (IFileOpenDialog)new FileOpenDialogRCW();
                try
                {
                    uint options;
                    dialog.GetOptions(out options);
                    options |= FOS_PICKFOLDERS | FOS_FORCEFILESYSTEM;
                    dialog.SetOptions(options);

                    if (!string.IsNullOrEmpty(titulo))
                    {
                        dialog.SetTitle(titulo);
                    }

                    if (!string.IsNullOrEmpty(rutaInicial) && Directory.Exists(rutaInicial))
                    {
                        IShellItem itemInicial;
                        if (SHCreateItemFromParsingName(rutaInicial, IntPtr.Zero, typeof(IShellItem).GUID, out itemInicial) == 0 && itemInicial != null)
                        {
                            dialog.SetFolder(itemInicial);
                        }
                    }

                    int hr = dialog.Show(ownerHandle);
                    if (hr == 0) // S_OK
                    {
                        IShellItem resultado;
                        dialog.GetResult(out resultado);
                        if (resultado != null)
                        {
                            string ruta;
                            resultado.GetDisplayName(SIGDN_FILESYSPATH, out ruta);
                            return ruta;
                        }
                    }
                    return null;
                }
                finally
                {
                    Marshal.ReleaseComObject(dialog);
                }
            }
            catch
            {
                // Fallback de seguridad al diálogo clásico en caso de error COM imprevisto
                using (var fbd = new FolderBrowserDialog())
                {
                    fbd.Description = titulo;
                    if (!string.IsNullOrEmpty(rutaInicial) && Directory.Exists(rutaInicial))
                        fbd.SelectedPath = rutaInicial;
                    if (fbd.ShowDialog() == DialogResult.OK)
                        return fbd.SelectedPath;
                }
                return null;
            }
        }

        private const uint FOS_PICKFOLDERS = 0x00000020;
        private const uint FOS_FORCEFILESYSTEM = 0x00000040;
        private const uint SIGDN_FILESYSPATH = 0x80028000;

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        private static extern int SHCreateItemFromParsingName(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszPath,
            [In] IntPtr pbc,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [Out, MarshalAs(UnmanagedType.Interface)] out IShellItem ppv);

        [ComImport]
        [Guid("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7")]
        [ClassInterface(ClassInterfaceType.None)]
        private class FileOpenDialogRCW { }

        [ComImport]
        [Guid("42f85109-75e4-4ca0-ab24-764799303b7f")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IFileOpenDialog
        {
            [PreserveSig] int Show([In] IntPtr parent);
            void SetFileTypes();
            void SetFileTypeIndex();
            void GetFileTypeIndex();
            void Advise();
            void Unadvise();
            void SetOptions([In] uint fos);
            void GetOptions(out uint fos);
            void SetDefaultFolder([In] IShellItem psi);
            void SetFolder([In] IShellItem psi);
            void GetFolder(out IShellItem ppsi);
            void GetCurrentSelection(out IShellItem ppsi);
            void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);
            void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);
            void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);
            void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);
            void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);
            void GetResult(out IShellItem ppsi);
            void AddPlace();
            void SetDefaultExtension();
            void Close();
            void SetClientGuid();
            void ClearClientData();
            void SetFilter();
            void GetResults();
            void GetSelectedItems();
        }

        [ComImport]
        [Guid("43826d1e-e718-42ee-bc55-a63e2647e3b4")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellItem
        {
            void BindToHandler();
            void GetParent();
            void GetDisplayName([In] uint sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);
            void GetAttributes();
            void Compare();
        }
    }
}
