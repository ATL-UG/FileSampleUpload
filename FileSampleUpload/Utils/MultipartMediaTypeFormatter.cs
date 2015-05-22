using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace FileSampleUpload.Utils
{
    public class MultipartMediaTypeFormatter : FormUrlEncodedMediaTypeFormatter
    {
        //uploads forms with files and data..
        private const string StringMultipartMediaType = "multipart/form-data";

        //for uploading files only...
        private const string StringApplicationMediaType = "application/octet-stream";

        public MultipartMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(StringMultipartMediaType));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(StringApplicationMediaType));
        }

        public override bool CanReadType(Type type)
        {
            return true;
        }

        public override bool CanWriteType(Type type)
        {
            return true;
        }

        public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content,
            IFormatterLogger formatterLogger)
        {
            var parts = await content.ReadAsMultipartAsync();
            var obj = Activator.CreateInstance(type);
            var propertiesFromObj = obj.GetType().GetRuntimeProperties().ToList();

            foreach (var property in propertiesFromObj.Where(x => x.PropertyType == typeof(ContentModel)))
            {
                var file = parts.Contents.FirstOrDefault(x => x.Headers.ContentDisposition.Name.Contains(property.Name));

                if (file == null || file.Headers.ContentLength <= 0) continue;

                try
                {
                    var fileModel = new ContentModel(file.Headers.ContentDisposition.FileName,
                        Convert.ToInt32(file.Headers.ContentLength), ReadFully(file.ReadAsStreamAsync().Result));
                    property.SetValue(obj, fileModel);
                }
                catch (Exception e)
                {
                }
            }

            foreach (var property in propertiesFromObj.Where(x => x.PropertyType != typeof(ContentModel)))
            {
                var formData =
                    parts.Contents.FirstOrDefault(x => x.Headers.ContentDisposition.Name.Contains(property.Name));

                if (formData == null) continue;

                try
                {
                    var strValue = formData.ReadAsStringAsync().Result;
                    var valueType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                    var value = Convert.ChangeType(strValue, valueType);
                    property.SetValue(obj, value);
                }
                catch (Exception e)
                {
                }
            }

            return obj;
        }

        private byte[] ReadFully(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }

    public class ContentModel
    {
        public ContentModel(string filename, int contentLength, byte[] content)
        {
            Filename = filename;
            ContentLength = contentLength;
            Content = content;
        }

        public string Filename { get; set; }

        public int ContentLength { get; set; }

        public byte[] Content { get; set; }

    }
}