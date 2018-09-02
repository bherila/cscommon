using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace Common
{
	public class AzureHelper
	{

		public AzureHelper(string connString)
		{
			_azureStorage = new Lazy<CloudStorageAccount>(() => {
				return CloudStorageAccount.Parse(connString);
			});
		}

		public static async Task<T> RetrieveAsync<T>(CloudTable table, string partitionKey, string rowKey) where T : ITableEntity
		{
			var op = TableOperation.Retrieve<T>(partitionKey, rowKey);
			var result = await table.ExecuteAsync(op);
			return (T)result.Result;
		}

		public static async Task<T> InsertOrMergeAsync<T>(CloudTable table, T entity) where T : ITableEntity
		{
			var op = TableOperation.InsertOrMerge(entity);
			var result = await table.ExecuteAsync(op);
			return (T)result.Result;
		}

		public static async Task<T> InsertOrReplaceAsync<T>(CloudTable table, T entity) where T : ITableEntity
		{
			var op = TableOperation.InsertOrReplace(entity);
			var result = await table.ExecuteAsync(op);
			return (T)result.Result;
		}

		public static async Task<T> DeleteAsync<T>(CloudTable table, T entity) where T : ITableEntity
		{
			var op = TableOperation.Delete(entity);
			var result = await table.ExecuteAsync(op);
			return (T)result.Result;
		}

		public async Task<List<string>> UploadToBucket(IEnumerable<HttpContent> multipartContent, string container, string replaceFileBaseName = null)
		{
			var result = new List<string>();
			foreach (var content in multipartContent) {

				try {
					var fileName = content?.Headers?.ContentDisposition?.FileName?.ToLower() ?? "file.dat";
					fileName = fileName.StripNonAlphaNumericDashDot();
					var baseName = Path.GetFileNameWithoutExtension(fileName);
					var extension = Path.GetExtension(fileName);

					if (!string.IsNullOrEmpty(replaceFileBaseName)) {
						fileName = replaceFileBaseName + extension;
					}

					result.Add(UploadToAzure(fileName, content.ReadAsStreamAsync().Result, container));
					if (fileName.EndsWith(".jpg") || fileName.EndsWith(".png")) {
						// it's an image
						using (var filesStream = await content.ReadAsStreamAsync()) {
							using (var src = Image.FromStream(filesStream)) {
								result.Add(UploadScaled(container, src, $"{baseName}{extension}", 2000));
								result.Add(UploadScaled(container, src, $"{baseName}-1024{extension}", 1024));
								result.Add(UploadScaled(container, src, $"{baseName}-512{extension}", 512));
							}
						}
					}
				} catch (Exception x) {
					throw new ValidationException("Failed to read file, " + x.Message);
				}
			}

			return result;
		}

		public string UploadScaled(string container, Image src, string fileName, float maxLength = 2000f)
		{
			// decide if we need to resize the image
			var scaleFactor = 1f / Math.Max(1.0f, Math.Max(src.Width, src.Height) / maxLength);

			if (scaleFactor < 1.0f) {
				// yes we need to scale it down (never scale it up)
				var size = new RectangleF(0f, 0f, src.Width * scaleFactor, src.Height * scaleFactor);
				using (var dst = new Bitmap((int)size.Width, (int)size.Height))
				using (var gfx = Graphics.FromImage(dst)) {
					gfx.InterpolationMode = InterpolationMode.HighQualityBilinear;
					gfx.DrawImage(src, size, new RectangleF(0, 0, src.Width, src.Height), GraphicsUnit.Pixel);

					var dstOutput = new MemoryStream();
					dst.Save(dstOutput, ImageFormat.Jpeg);
					dstOutput.Seek(0, SeekOrigin.Begin);
					return UploadToAzure(fileName, dstOutput, container);
				}
			}
			return null;
		}

		public byte[] ToByteArray(System.Web.HttpPostedFileBase value)
		{
			if (value == null)
				return null;
			var array = new byte[value.ContentLength];
			value.InputStream.Position = 0;
			value.InputStream.Read(array, 0, value.ContentLength);
			return array;
		}


		/// <summary>
		///     Uploads a file name to Azure.
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="data"></param>
		/// <param name="container"></param>
		/// <returns></returns>
		public string UploadToAzure(string filename, Stream data, string container)
		{
			filename = filename.StripNonAlphaNumericDashDot().Trim().ToLower();
			UploadFile(filename, data, container);
			return filename;
		}


		private Lazy<CloudStorageAccount> _azureStorage;

		public CloudStorageAccount GetCloudStorage()
		{
			return _azureStorage.Value;
		}

		public string UploadFile(string filename, Stream data, string myContainer)
		{
			// Retrieve storage account from connection string.
			CloudStorageAccount storageAccount = _azureStorage.Value;

			// Create the blob client.
			CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

			// Retrieve reference to a previously created container.
			CloudBlobContainer container = blobClient.GetContainerReference(myContainer);

			// Retrieve reference to a blob named "myblob".
			CloudBlockBlob blockBlob = container.GetBlockBlobReference(filename);

			// Create or overwrite the "myblob" blob with contents from a local file.
			blockBlob.UploadFromStream(data);
			var imageUrl = "https://ucassets.blob.core.windows.net/" + myContainer + "/" + filename;
			return imageUrl;
		}

	}
}
