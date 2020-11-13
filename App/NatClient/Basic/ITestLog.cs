namespace NatCore
{
    public interface ITestLog:IShare
    {
        void Log(object msg);
    }
    public class TestLog
    {
        public static void Log(object msg)
        {
            Shares.GetShare<ITestLog>()?.Log(msg);
        }
    }
}
