# AzureBlobTool
A CLI tool used to filter Azure Blob Storage files, with the ability to download individual files.

It supports filtering on file name and modified date.

## What does it do?

Use this tool to quickly filter your Azure Blob Storage and easily download documents. The tool will display all filtered documents in the command line showing the name, modified date and size of the files.

Use the name in the output to easily download the file.

## How to compile

It is straightforward to compile, just make sure that your Nuget has the setting Include prerelease on as it is using the preview of Azure.Storage.Files.DataLake.

## Help

Please read the output of the tool for more details by just running AzureBlobTool.exe and specifying no input arguments, you should seem the following help output:

Glossary:
storageAccountName: Can be found in your Azure Storage Account -> Access Keys -> Storage account name
storageAccountKey: Can be found in your Azure Storage Account -> Access Keys -> key1 or key2 -> key, copy the whole key
fileSystemName: Name of the Blob container you want to search, can be found Azure Storage Account -> Storage Explorer -> FILE SYSTEMS
directoryName: The sub-directory which contains your files (if any)

Commands:
filterNameContains: Filter files on the file name containing the specified value
filterNameExact: Filter files on the file name being exactly the specified value
filterNameStartsWith: Filter files on the file name starting with the specified value
filterNameEndsWith: Filter files on the file name ending with the specified value
filterDate: Filter files on the modified date being equal to the specified date value

Examples:
Please specify a command in the format: AzureBlobTool.exe <storageAccountName> <storageAccountKey> <fileSystemName> <directoryName> -<command> <additionalInfo>.
To filter on name (contains): AzureBlobTool.exe myStorageAccount UB98ahXbIDOoTVE7jOVkhlI6ryOcZXJMftaC6SRskCw+mh8lrsliLdV0Qz7+YXoOVOAsAg== myFiles myDirectory -filterNameContains foo
To filter on date: AzureBlobTool.exe myStorageAccount UB98ahXbIDOoTVE7jOVhlI6ryZXJWjMftaC6SRlskCw+mh8lrsliLdV0Qz7+YXGOAsAg== myFiles myDirectory -filterDate 2020-01-20
To download a document: AzureBlobTool.exe myStorageAccount UB98ahXbIDOoTVE7jOVkhlI6ryXJWjMftckCw+mh8lrsliLdV0Qz7+YXoOVOAsAg== myFiles myDirectory -download myDocumentName.json

### Reason for existing

I created this tool as there is currently no way to filter Azure Blob Storage using Azure Storage Explorer. My Blob Storage has over 16000 files in there and it baecame a chore to load them all into cache to find one file. That is why the tool exists. It was a quick and dirty implementation and should be by no means used for anything other than to quickly filter and download files from Azure Blob Storage. 


## Contributions

Please feel free to extend functionality. 