using System.Collections.Generic;
using System.IO;

namespace RapidXaml.CodeGen;

public interface IFileSystem
{
	bool DirectoryExists(string path);

	IEnumerable<string> DirectoryEnumerateFiles(string path, string searchPattern, SearchOption searchOption);
	
	string GetFileName(string path);

	string GetFilePath(string path);

	string FileReadAllText(string path);

	void FileWriteAllBytes(string path, byte[] bytes);

	string PathGetFileName(string path);

	string PathGetFileNameWithoutExtension(string path);

	string PathChangeExtension(string path, string extension);
}
