using System.IO;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace Parkour.MenuTests
{
    public sealed class MenuValidationRunner : ScriptableObject, ICallbacks
    {
        [MenuItem("Parkour/Menú principal/Ejecutar pruebas")]
        public static void Run()
        {
            if (EditorApplication.isPlaying) return;
            var api = CreateInstance<TestRunnerApi>();
            api.RegisterCallbacks(CreateInstance<MenuValidationRunner>());
            api.Execute(new ExecutionSettings(new Filter { testMode = TestMode.EditMode, assemblyNames = new[] { "Parkour.Menu.Tests" } }));
        }

        public void RunStarted(ITestAdaptor testsToRun) { }
        public void TestStarted(ITestAdaptor test) { }
        public void TestFinished(ITestResultAdaptor result) { }

        public void RunFinished(ITestResultAdaptor result)
        {
            Directory.CreateDirectory("Artifacts");
            TestRunnerApi.SaveResultToFile(result, "Artifacts/MenuTests.xml");
            Debug.Log("PARKOUR_MENU_TESTS: " + result.ResultState + " · " + result.PassCount + " correctas · " + result.FailCount + " fallidas");
        }

        [MenuItem("Parkour/Menú principal/Previsualizar opciones")]
        public static void PreviewOptions()
        {
            if (EditorApplication.isPlaying) Object.FindAnyObjectByType<Parkour.Menu.MenuPrincipalController>().ShowOptions();
        }

        [MenuItem("Parkour/Menú principal/Previsualizar salir")]
        public static void PreviewExit()
        {
            if (EditorApplication.isPlaying) Object.FindAnyObjectByType<Parkour.Menu.MenuPrincipalController>().ShowExit();
        }
    }
}
