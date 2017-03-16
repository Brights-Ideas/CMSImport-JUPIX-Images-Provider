using System;
using System.IO;
using System.Net;
using System.Web;
using CMSImportLibrary.Config;
using CMSImportLibrary.Helpers;

namespace CMSImport.JupixImagesFieldProvider
{
    /// <summary>
    /// Holds info about images
    /// </summary>
    public class ImageFileInfo
    {
        private string ConvertUrlToRelativePath()
        {
            //First replace domain info
            var relativeUrl = MediaImportHelper.ParseDomain(Url, CMSImportConfig.AllowedMediaImportDomainsList);

            return relativeUrl;
        }

        /// <summary>
        /// Gets or sets the URL of the file.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the modifie date time.
        /// </summary>
        public DateTime ModifieDateTime { get; set; }

        public string RelativePath
        {
            get
            {
                return ConvertUrlToRelativePath();
            }
        }
    }
}
