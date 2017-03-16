using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Xml.Linq;
using CMSImport.Extensions.Providers.FieldProviders;
using CMSImport.Extensions.Providers.ImportProviders;
using CMSImport.Extensions.TypeExtensions;
using CMSImport.JupixImagesFieldProvider.TypeExtensions;

namespace CMSImport.JupixImagesFieldProvider
{
    [FieldProvider(PropertyEditorAlias = "Umbraco.MultipleMediaPicker", Priority = FieldProviderPrio.High)]
    public class TransformJupixImagesFieldProvider : IFieldProvider
    {
        public object Parse(object value, PropertyInfo property, FieldProviderOptions fieldProviderOptions)
        {
            var fileinfo = ParseXML(value.AsString().Trim());
            //Ensure all files exists
            DownloadFiles(fileinfo);

            //return filenames as csv
            return fileinfo.FilenamesAsCsv();
        }

        private List<ImageFileInfo> ParseXML(string value)
        {
            var fileInfo = new List<ImageFileInfo>();

            if (value.StartsWith("<") && value.EndsWith(">"))
            {
                //Value is XML
                var xmlDoc = XDocument.Parse($"<images>{value}</images>");
                foreach (var item in xmlDoc.Descendants("image"))
                {
                    var imgFile = new ImageFileInfo();

                    var xAttribute = item.Attribute("modified");
                    if (xAttribute != null)
                    {
                        imgFile.ModifieDateTime = DateTime.Parse(xAttribute.Value);
                    }
                    imgFile.Url = item.Value;

                    fileInfo.Add(imgFile);
                }
            }

            return fileInfo;
        }

        /// <summary>
        /// Takes the file collection and checks if file exists. If not or modifieddate is newer than original one files will be downloaded
        /// </summary>
        /// <param name="fileInfo">The file information.</param>
        private void DownloadFiles(IEnumerable<ImageFileInfo> fileInfo)
        {
            foreach (var item in fileInfo)
            {
                var localFile = HttpContext.Current.Server.MapPath(item.RelativePath);
                if (!File.Exists(localFile))
                {
                    DownloadFile(localFile, item);
                }
                else if(File.GetLastWriteTime(localFile) < item.ModifieDateTime)
                {
                    //File is newer
                    DownloadFile(localFile, item);
                }
            }
        }

        private static void DownloadFile(string localFile, ImageFileInfo item)
        {
            //Ensure Path exists
            var path = Path.GetDirectoryName(localFile);
            if (!Directory.Exists(path) &&  path != null)
            {
                Directory.CreateDirectory(path);
            }

            //Set remote hostname
            var remoteUri = new UriBuilder(item.Url);

            //Download file
            var webclient = new WebClient();
            webclient.DownloadFile(remoteUri.Uri, localFile);
        }
    }
}
