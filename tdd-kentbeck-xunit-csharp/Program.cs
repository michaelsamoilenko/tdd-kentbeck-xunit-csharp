using System.Reflection;

new TestCaseTest("TestTemplateMethod").Run();

public class TestCase(string name)
{
    public virtual void SetUp(){ }
    public virtual void TearDown() { }
    public void Run()
    {
        SetUp();
        var method = GetType().GetMethod(name, 
            BindingFlags.Instance | 
            BindingFlags.Public | 
            BindingFlags.NonPublic);
        method.Invoke(this, null);
        TearDown();
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
    public void TestTemplateMethod()
    {
        var test = new WasRun("TestMethod");
        test.Run();
        Assert("SetUp TestMethod TearDown " == test.Log);
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
}