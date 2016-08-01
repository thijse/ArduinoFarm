using System.IO;
using System.Windows.Forms;

namespace Utilities
{
    class DialogUtils
    {
        public static string OpenFileDialog(string fileName, bool useDialog=true)
        {
            if (!useDialog) return fileName;
            var ext = Path.GetExtension(fileName);
            var dialog = new OpenFileDialog
            {
                FileName = Path.GetFileName(fileName),
                RestoreDirectory = true,
                InitialDirectory = Path.GetDirectoryName(fileName),
                Filter = ext + " files (*" + ext + ")|*" + ext + "|All files (*.*)|*.*",
                FilterIndex = 0
            };
            if (dialog.ShowDialog() == DialogResult.OK) fileName = dialog.FileName;
            return fileName;
        }
    }
}
