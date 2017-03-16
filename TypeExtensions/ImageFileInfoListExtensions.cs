using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMSImportLibrary.TypeExtensions;

namespace CMSImport.JupixImagesFieldProvider.TypeExtensions
{
    public static class ImageFileInfoListExtensions
    {
        public static string FilenamesAsCsv(this List<ImageFileInfo> fileInfo)
        {
            return fileInfo.Select(s => s.RelativePath).ToList().ToCsv();
        }
    }
}
