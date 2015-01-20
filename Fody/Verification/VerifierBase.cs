using System;
using System.Diagnostics;

namespace Fody.Verification
{
    public abstract class VerifierBase : IVerifier
    {
        protected VerifierBase(ILogger logger)
        {
            Logger = logger;
        }

        public ILogger Logger { get; private set; }

        public abstract string Name { get; }

        public bool Verify(string assemblyFileName)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                Logger.LogInfo("  Verifying assembly");

                return VerifyAssembly(assemblyFileName);
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
                return false;
            }
            finally
            {
                Logger.LogInfo(string.Format("  Finished verification in {0}ms.", stopwatch.ElapsedMilliseconds));
            }
        }

        protected abstract bool VerifyAssembly(string assemblyFileName);
    }
}
