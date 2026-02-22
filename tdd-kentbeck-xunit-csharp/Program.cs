using System.Reflection;

new TestCaseTest("TestRunning").Run();
new TestCaseTest("TestSetUp").Run();

public class TestCase(string name)
{
    public virtual void SetUp(){ }
    public void Run()
    {
        SetUp();
        var method = GetType().GetMethod(name, 
            BindingFlags.Instance | 
            BindingFlags.Public | 
            BindingFlags.NonPublic);
        method.Invoke(this, null);
    }
    // This helper method is basically simplified replication of what Python 'assert' does.
    // C#'s Debug.Assert doesn't suit us since it doesn't throw an exception but just reports an error.
    public static void Assert(bool assertion)
    {
        if (!assertion) {
            throw new Exception ("Assertion failed");
        }
    }
}

public class TestCaseTest(string name) : TestCase(name)
{
    private WasRun _test;

    public override void SetUp() => _test = new WasRun("TestMethod");
    public void TestRunning()
    {
        _test.Run();
        Assert(_test.wasRun);
    }
    public void TestSetUp()
    {
        _test.Run();
        Assert(_test.WasSetUp);
    }
}

public class WasRun(string name) : TestCase(name)
{
    public bool wasRun { get; private set; }
    public bool WasSetUp { get; private set; }

    public override void SetUp()
    {
        WasSetUp = true;
    }
    public void TestMethod()
    {
        wasRun = true;
    }
}