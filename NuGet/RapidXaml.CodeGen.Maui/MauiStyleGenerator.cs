using System;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace RapidXaml.CodeGen;

public class MauiStyleGenerator : Task, ITaskLogWrapper
{
	[Required]
	public ITaskItem[] InputFiles { get; set; } = [];

	[Required]
	public string GenerationNamespace { get; set; }

	[Required]
	public bool SupportResxGeneration { get; set; } = false;

	[Required]
	public ITaskItem[] ResxInputFiles { get; set; } = [];

	internal IFileSystem _file = new DefaultFileSystem();

	public static MauiStyleGenerator CreateForTesting(IFileSystem fileSystem)
	{
		var generator = new MauiStyleGenerator
		{
			_file = fileSystem,
		};

		return generator;
	}

	public override bool Execute()
	{
		try
		{
			string[] TaskItemsToString(ITaskItem[] taskItems)
			{
				List<string> result = [];
				foreach (var item in taskItems)
				{
					result.Add(item.ItemSpec);
				}
				return [.. result];
			}

			var executor = new MauiExecutionLogic(_file, this);

			return executor.Execute(TaskItemsToString(InputFiles), GenerationNamespace, SupportResxGeneration, TaskItemsToString(ResxInputFiles));
		}
		catch (Exception ex)
		{
			Log.LogErrorFromException(ex, showStackTrace: true);
			return false;
		}
	}

	public void LogWarning(string message)
		=> Log.LogWarning(message);

	public void LogError(string message)
		=> Log.LogError(message);

	public void LogErrorFromException(Exception ex, bool showStackTrace)
		=> Log.LogErrorFromException(ex, showStackTrace);

	public void LogNormalMessage(string message)
		=> Log.LogMessage(MessageImportance.Normal, message);

	public void LogImportantMessage(string message)
		=> Log.LogMessage(MessageImportance.High, message);
}

