using System.Reflection;

new TestCaseTest("TestRunning").Run();

public class TestCaseTest(string name) : TestCase(name)
{
    public void TestRunning()
    {
        var test = new WasRun("TestMethod");
        
        Assert(!test.wasRun);
        test.Run();
        Assert(test.wasRun);
    }
}

public class TestCase(string name)
{
    public void Run()
    {
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

public class WasRun(string name) : TestCase(name)
{
    public bool wasRun { get; private set; } 

    public void TestMethod()
    {
        wasRun = true;
    }
}