using System;

namespace RapidXaml.CodeGen;

public interface ITaskLogWrapper
{
	void LogNormalMessage(string message);
	void LogImportantMessage(string message);
	void LogWarning(string message);
	void LogError(string message);
	void LogErrorFromException(Exception ex, bool showStackTrace);
}
