using System.Diagnostics;
using System.Reflection;

new TestCaseTest("TestTemplateMethod").Run().Summary();
new TestCaseTest("TestResult").Run().Summary();
new TestCaseTest("TestFailedResult").Run().Summary();

public class TestCase(string name)
{
    public virtual void SetUp(){ }
    public virtual void TearDown() { }
    public TestResult Run()
    {
        var result = new TestResult();
        result.TestStarted();
        SetUp();
        var method = GetType().GetMethod(name, 
            BindingFlags.Instance | 
            BindingFlags.Public | 
            BindingFlags.NonPublic);
        method.Invoke(this, null);
        TearDown();
        return result;
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

public class TestResult
{
    public TestResult()
    {
        RunCount = 0;
    }
    private int RunCount { get; set; }

    public void TestStarted()
    {
        RunCount++;
    }

    public string Summary()
    {
        return $"{RunCount} run, 0 failed";
    }
}

public class TestCaseTest(string name) : TestCase(name)
{
    public void TestTemplateMethod()
    {
        var test = new WasRun("TestMethod");
        test.Run();
        Assert("SetUp TestMethod TearDown " == test.Log);
    }
    public void TestResult()
    {
        var test = new WasRun("TestMethod");
        var result = test.Run();
        Assert("1 run, 0 failed" == result.Summary());
    }
    public void TestFailedResult()
    {
        var test = new WasRun("TestBrokenMethod");
        var result = test.Run();
        Assert("1 run, 1 failed" == result.Summary());
    }
}

public class WasRun(string name) : TestCase(name)
{
    public string Log { get; private set; }

    public override void SetUp()
    {
        Log = "SetUp ";
    }
    public override void TearDown()
    {
        Log += "TearDown ";
    }
    public void TestMethod()
    {
        Log += "TestMethod ";
    }
    public void TestBrokenMethod()
    {
        throw new Exception();
    }
}