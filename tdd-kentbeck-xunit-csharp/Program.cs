using System.Reflection;

var suite = new TestSuite(typeof(TestCaseTest));
var result = new TestResult();
suite.Run(result);
Console.WriteLine(result.Summary());

public class TestCaseTest(string name) : TestCase(name)
{
    private TestResult _result;
    public override void SetUp()
    {
        _result = new TestResult();
    }
    public void TestTemplateMethod()
    {
        var test = new WasRun("TestMethod");
        test.Run(_result);
        Assert("SetUp TestMethod TearDown " == test.Log);
    }
    public void TestResult()
    {
        var test = new WasRun("TestMethod");
        test.Run(_result);
        Assert("1 run, 0 failed" == _result.Summary());
    }
    public void TestFailedResult()
    {
        var test = new WasRun("TestBrokenMethod");
        test.Run(_result);
        Assert("1 run, 1 failed" == _result.Summary());
    }
    public void TestFailedResultFormatting()
    {
        _result.TestStarted();
        _result.TestFailed();
        Assert("1 run, 1 failed" == _result.Summary());
    }
    public void TestFailedSetup()
    {
        var test = new WasRunBrokenSetup("TestMethod");
        test.Run(_result);
        Assert("1 run, 1 failed" == _result.Summary());
    }
    public void TestSuite()
    {
        var suite = new TestSuite();
        suite.Add(new WasRun("TestMethod"));
        suite.Add(new WasRun("TestBrokenMethod"));
        suite.Run(_result);
        Assert("2 run, 1 failed" == _result.Summary());
    }
    public void TestExecuteTearDownOnFailure()
    {
        var test = new WasRun("TestBrokenMethod");
        test.Run(_result);
        Assert("SetUp TearDown " == test.Log);
    }
    public void TestSuiteFromTestClass()
    {
        var suite = new TestSuite(typeof(WasRun));
        suite.Run(_result);
        Assert("2 run, 1 failed" == _result.Summary());
    }
}

public interface ITest
{
    public void Run(TestResult result);
}

public class TestCase(string name): ITest
{
    public virtual void SetUp(){ }
    public virtual void TearDown() { }
    public void Run(TestResult result)
    {
        result.TestStarted();
        try {
            SetUp();
            var method = GetType().GetMethod(name, 
                BindingFlags.Instance | 
                BindingFlags.Public | 
                BindingFlags.NonPublic);
            method.Invoke(this, null);
        }
        catch (Exception e) {
           result.TestFailed();
        }
        finally {
            TearDown();
        }
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

public class TestSuite: ITest
{
    private readonly List<ITest> _tests;
    public TestSuite(Type? testClass = null)
    {
        _tests = [];
        if (testClass != null) {
            var tests =
                testClass
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    .Select(m => m.Name)
                    .Where(name => name.Contains("Test", StringComparison.OrdinalIgnoreCase))
                    .Select(name => (ITest)Activator.CreateInstance(testClass, name)) 
                    .ToList();
            _tests.AddRange(tests);
        }
    }
    public void Add(ITest test)
    {
        _tests.Add(test);
    }
    public void Run(TestResult result)
    {
        foreach (var test in _tests) {
            test.Run(result);
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
    private int ErrorCount { get; set; }

    public void TestStarted()
    {
        RunCount++;
    }
    public void TestFailed()
    {
        ErrorCount++;
    }

    public string Summary()
    {
        return $"{RunCount} run, {ErrorCount} failed";
    }
}

public class WasRun(string name) : TestCase(name)
{
    public string Log { get; private set; } = "";

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

public class WasRunBrokenSetup(string name): TestCase(name)
{
    public override void SetUp()
    {
        throw new Exception();
    }
}