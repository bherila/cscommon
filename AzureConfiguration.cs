using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Common
{
	public static class AzureConfiguration
	{
		public static CloudStorageAccount StorageAccount { get; set; }
	}
}