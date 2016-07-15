<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
	var toolsDiretory = Path.GetDirectoryName(Util.CurrentQueryPath);
	var codeDirectory = Path.GetFullPath(Path.Combine(toolsDiretory,"../../"));
	var nuget = Path.Combine(toolsDiretory, "nuget.exe");

	var solutionFiles = Directory.EnumerateFiles(codeDirectory, "*.sln", SearchOption.AllDirectories);

	Parallel.ForEach(solutionFiles,
	new ParallelOptions() { MaxDegreeOfParallelism = 10 },
	(solutionFile) =>
		{
			Debug.WriteLine(solutionFile);
			var packagesDirectory = Path.Combine(Path.GetDirectoryName(solutionFile), "packages");
			Directory.CreateDirectory(packagesDirectory);
			try
			{
				Execute(nuget, "restore " + solutionFile + " -packagesDirectory " + packagesDirectory);
				Execute(nuget, "update " + solutionFile + " -safe -NonInteractive -repositoryPath " + packagesDirectory);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(solutionFile + ": " + ex.ToString());
				if (ex.InnerException != null)
				{
					Debug.WriteLine(ex.InnerException.ToString());
				}
			}
		}
	);
}

void Execute(string file, string arguments)
{
	var startInfo = new ProcessStartInfo();
	startInfo.FileName = file;
	startInfo.CreateNoWindow = true;
	startInfo.UseShellExecute = false;
	startInfo.RedirectStandardOutput = true;
	startInfo.RedirectStandardError = true;
	startInfo.Arguments = arguments;
	var process = Process.Start(startInfo);
	process.WaitForExit();
	var error = process.StandardError.ReadToEnd();
	if (!string.IsNullOrWhiteSpace(error))
	{
		throw new Exception(error);
	}
	var result = process.StandardOutput.ReadToEnd();
	Debug.WriteLine(result);
}