#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.TestTools.TestRunner.Api;

public class AutomateBuild : MonoBehaviour
{
    [MenuItem("Deployment/Test and Build Development")]
    public static void RunTestsAndBuild()
    {
        var api = ScriptableObject.CreateInstance<TestRunnerApi>();
        var filter = new Filter() { testMode = TestMode.EditMode };
        api.RegisterCallbacks(new TestCallbacks());
        api.Execute(new ExecutionSettings(filter));

    }
}

public class TestCallbacks : ICallbacks
{
    public void RunStarted(ITestAdaptor testsToRun)
    {
        Debug.Log("Tests started...");
    }

    public void RunFinished(ITestResultAdaptor result)
    {

        Debug.Log("Tests finished...");

        if (result.FailCount > 0)
        {
            throw new BuildFailedException("Tests Failed");
        }

        BuildPlayer();
    }

    private void BuildPlayer()
    {
        string[] scenes = { "Assets/_Project/Scenes/Main.unity" };
        DateTime now = DateTime.Now;

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions()
        {
            scenes = scenes,
            locationPathName = $"Builds/{now.Day}-{now.Month}-{now.Year}/{PlayerSettings.productName}.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.Development
        };

        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Debug.Log("Build Succeeded");
    }

    public void TestStarted(ITestAdaptor test)
    {
        Debug.Log("Test started: " + test.Name);
    }

    public void TestFinished(ITestResultAdaptor result)
    {
        if (result.TestStatus == TestStatus.Failed)
        {
            Debug.LogError("Test failed: " + result.Name);
        }
    }
}
#endif  
