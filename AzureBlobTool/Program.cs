namespace AzureBlobTool
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Threading.Tasks;
  using Azure;
  using Azure.Storage;
  using Azure.Storage.Files.DataLake;
  using Azure.Storage.Files.DataLake.Models;

  class Program
  {
    private static DataLakeServiceClient _dataLakeClient;

    static async Task Main(string[] args)
    {
      try
      {
        if (args == null || args.Length < 6)
        {
          Console.WriteLine("\nGlossary:");
          Console.WriteLine("storageAccountName: Can be found in your Azure Storage Account -> Access Keys -> Storage account name");
          Console.WriteLine("storageAccountKey: Can be found in your Azure Storage Account -> Access Keys -> key1 or key2 -> key, copy the whole key");
          Console.WriteLine("fileSystemName: Name of the Blob container you want to search, can be found Azure Storage Account -> Storage Explorer -> FILE SYSTEMS");
          Console.WriteLine("directoryName: The sub-directory which contains your files (if any)");
          Console.WriteLine("\nCommands:");
          Console.WriteLine("filterNameContains: Filter files on the file name containing the specified value");
          Console.WriteLine("filterNameExact: Filter files on the file name being exactly the specified value");
          Console.WriteLine("filterNameStartsWith: Filter files on the file name starting with the specified value");
          Console.WriteLine("filterNameEndsWith: Filter files on the file name ending with the specified value");
          Console.WriteLine("filterDate: Filter files on the modified date being equal to the specified date value");
          Console.WriteLine("\nExamples:");
          Console.WriteLine(
           "Please specify a command in the format: AzureBlobTool.exe <storageAccountName> <storageAccountKey> <fileSystemName> <directoryName> -<command> <additionalInfo>.");
          Console.WriteLine(
           "To filter on name (contains): AzureBlobTool.exe myStorageAccount UB11ahXbIDOoTVE7jOVkhlI6ryOcZXJWjMftaC6SRskCw+mh8lrsliLdV0Qz7+YYoOVEQGOAsAa== myFiles myDirectory -filterNameContains foo");
          Console.WriteLine(
           "To filter on date: AzureBlobTool.exe myStorageAccount UB11ahXbIDOoTVE7jOVkhlI6ryOcZXJWjMftaC6SRlCajKqaKrsliLdV0Qz7+YYoOVEQGOAsAa== myFiles myDirectory -filterDate 2020-01-20");
          Console.WriteLine(
           "To download a document: AzureBlobTool.exe myStorageAccount UB11ahXbIDOoTVE7jOVkhlI6ryOcZXJWjMftaC6SRlCajKqaKsliLdV0Qz7+YYoOVEQGOAsAa== myFiles myDirectory -download myDocumentName.json");
          Console.WriteLine();
          return;
        }

        _dataLakeClient = CreateGetDataLakeServiceClient(args[0], args[1]);

        switch (args[4].ToLower())
        {
          case "-filterdate":
            if (!DateTime.TryParse(args[5], out DateTime dateFilter))
            {
              Console.WriteLine($"The date {args[5]} is not valid.");
              return;
            }
            await FilterFilesAsync(args[2], args[3], (pathItem) => pathItem.LastModified.Date == dateFilter.Date);
            break;
          case "-filternamecontains":
            await FilterFilesAsync(args[2], args[3], (pathItem) => pathItem.Name.Contains(args[5], StringComparison.OrdinalIgnoreCase));
            break;
          case "-filternameexact":
            await FilterFilesAsync(args[2], args[3], (pathItem) => pathItem.Name.Equals(args[5], StringComparison.OrdinalIgnoreCase));
            break;
          case "-filternamestartswith":
            await FilterFilesAsync(args[2], args[3], (pathItem) => pathItem.Name.StartsWith(args[5], StringComparison.OrdinalIgnoreCase));
            break;
          case "-filternameendswith":
            await FilterFilesAsync(args[2], args[3], (pathItem) => pathItem.Name.EndsWith(args[5], StringComparison.OrdinalIgnoreCase));
            break;
          case "-download":
            await DownloadAsync(args[2], args[3], args[5]);
            break;
          default:
            Console.WriteLine("The command was not valid, please specify any of the valid commands.");
            break;
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
    }

    private static async Task FilterFilesAsync(string fileSystem, string directory, Predicate<PathItem> filter)
    {
      DataLakeFileSystemClient fileSystemClient = _dataLakeClient.GetFileSystemClient(fileSystem);

      IAsyncEnumerator<PathItem> enumerator =
       fileSystemClient.GetPathsAsync(directory).GetAsyncEnumerator();

      Console.WriteLine("\nStarting to filter...");
      Console.WriteLine($"All filtered documents in {fileSystem}\\{directory}:");
      await enumerator.MoveNextAsync();

      PathItem item = enumerator.Current;

      while (item != null)
      {
        if (filter(item))
        {
          Console.WriteLine($"Name: {item.Name.Remove(0, directory.Length + 1)}, LastModified: {item.LastModified}, Size: {item.ContentLength} bytes");
        }

        if (!await enumerator.MoveNextAsync())
        {
          break;
        }

        item = enumerator.Current;
      }

      Console.WriteLine("Documents filtered successfully.\n");
    }

    private static async Task DownloadAsync(string fileSystem, string directory, string fileName)
    {
      DataLakeFileSystemClient fileSystemClient = _dataLakeClient.GetFileSystemClient(fileSystem);

      DataLakeDirectoryClient directoryClient =
       fileSystemClient.GetDirectoryClient(directory);

      DataLakeFileClient fileClient =
       directoryClient.GetFileClient(fileName);

      Console.WriteLine("\nStarting download...");

      Response<FileDownloadInfo> downloadResponse = await fileClient.ReadAsync();

      var reader = new BinaryReader(downloadResponse.Value.Content);

      FileStream fileStream =
       File.OpenWrite($"{fileName}");

      const int bufferSize = 4096;

      var buffer = new byte[bufferSize];

      int count;

      while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
      {
        fileStream.Write(buffer, 0, count);
      }

      await fileStream.FlushAsync();

      fileStream.Close();

      Console.WriteLine($"File downloaded to {fileName}\n");
    }

    private static DataLakeServiceClient CreateGetDataLakeServiceClient(string accountName, string accountKey)
    {
      var sharedKeyCredential =
       new StorageSharedKeyCredential(accountName, accountKey);

      string dfsUri = "https://" + accountName + ".dfs.core.windows.net";

      return new DataLakeServiceClient(new Uri(dfsUri), sharedKeyCredential);
    }
  }
}