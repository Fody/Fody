using Fody.Verification;

namespace Fody.Tests.Verification
{
    public class VerifierFixture : IVerifier
    {
        private readonly bool _expectedResult;

        public VerifierFixture(bool expectedResult)
        {
            _expectedResult = expectedResult;
        }

        public string Name
        {
            get { return "VerifierFixture"; }
        }

        public bool Verify(string assemblyFileName)
        {
            return _expectedResult;
        }
    }
}