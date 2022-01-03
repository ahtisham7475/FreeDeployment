namespace swift.api._2010.code.scrubber
{
    public class FakeResult
    {
        public string Log;

        public bool IsValid;

        public FakeResult()
        {

        }

        public FakeResult(string log, bool isValid)
        {
            Log = log;
            IsValid = isValid;
        }
    }
}