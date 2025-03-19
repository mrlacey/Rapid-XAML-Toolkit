using System.Collections.Generic;
using System.IO;

namespace RapidXaml.CodeGen;

public partial class DefaultFileSystem : IFileSystem
{
	public IEnumerable<string> DirectoryEnumerateFiles(string path, string searchPattern, SearchOption searchOption)
		=> Directory.EnumerateFiles(path, searchPattern, searchOption);

	public bool DirectoryExists(string path)
		=> Directory.Exists(path);

	public string FileReadAllText(string path)
		=> File.ReadAllText(path);

	public void FileWriteAllBytes(string path, byte[] bytes)
		=> File.WriteAllBytes(path, bytes);

	public string GetFileName(string path)
		=> new FileInfo(path).Name;

	public string GetFilePath(string path)
		=> new FileInfo(path).FullName;

	public string PathChangeExtension(string path, string extension)
		=> Path.ChangeExtension(path, extension);

	public string PathGetFileName(string path)
		=> Path.GetFileName(path);

	public string PathGetFileNameWithoutExtension(string path)
		=> Path.GetFileNameWithoutExtension(path);
}
