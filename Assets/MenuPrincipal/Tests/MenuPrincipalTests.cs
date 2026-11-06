using System.Collections;
using System.Linq;
using NUnit.Framework;
using Parkour.Menu;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Parkour.MenuTests
{
    public sealed class MenuPrincipalTests
    {
        [Test]
        public void SceneHasThreeWiredButtonsAndEmbeddedFonts()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/MenuPrincipal.unity");
            var controller = Object.FindFirstObjectByType<MenuPrincipalController>();
            Assert.That(controller.LevelScene, Is.EqualTo("Assets/Scenes/SampleScene.unity"));
            Assert.That(Object.FindFirstObjectByType<InputSystemUIInputModule>(), Is.Not.Null);
            var artwork = Object.FindFirstObjectByType<RawImage>();
            Assert.That(artwork.texture, Is.Not.Null);
            Assert.That(artwork.raycastTarget, Is.False);
            var buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
            Assert.That(buttons.Select(button => button.name), Is.EquivalentTo(new[] { "Jugar", "Opciones", "Salir" }));
            foreach (var button in buttons)
            {
                Assert.That(button.onClick.GetPersistentEventCount(), Is.EqualTo(1));
                Assert.That(button.onClick.GetPersistentTarget(0), Is.EqualTo(controller));
                Assert.That(button.GetComponent<MenuButtonHighlight>(), Is.Not.Null);
                Assert.That(button.GetComponent<CanvasRenderer>(), Is.Not.Null);
                Assert.That(button.navigation.selectOnDown, Is.Not.Null);
            }
            var sources = controller.GetComponents<AudioSource>();
            Assert.That(sources.Any(source => source.loop && source.clip != null), Is.True);
            Assert.That(scene.GetRootGameObjects().Length, Is.EqualTo(4));
            foreach (var text in Object.FindObjectsByType<Text>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                Assert.That(text.font, Is.Not.Null, text.name);
                Assert.That(UnityEditor.AssetDatabase.GetAssetPath(text.font), Is.EqualTo("Assets/MenuPrincipal/Fonts/Lato-Regular.ttf"));
            }
        }

        [UnityTest]
        public IEnumerator PanelsBlockBackgroundAndRestoreFocus()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/MenuPrincipal.unity");
            yield return new EnterPlayMode();
            yield return null;
            var buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
            var main = Object.FindFirstObjectByType<MenuButtonHighlight>().GetComponentInParent<CanvasGroup>();
            var input = EventSystem.current.GetComponent<InputSystemUIInputModule>();
            Assert.That(input.point.action.enabled, Is.True);
            Assert.That(input.leftClick.action.enabled, Is.True);
            Assert.That(input.move.action.enabled, Is.True);
            foreach (var name in new[] { "Opciones", "Salir" })
            {
                var button = buttons.Single(item => item.name == name);
                EventSystem.current.SetSelectedGameObject(button.gameObject);
                button.onClick.Invoke();
                yield return null;
                Assert.That(main.interactable, Is.False, name);
                Assert.That(main.blocksRaycasts, Is.False, name);
                Assert.That(GameObject.Find(name + " Panel"), Is.Not.Null, name);
                Assert.That(EventSystem.current.currentSelectedGameObject, Is.Not.Null);
                Canvas.ForceUpdateCanvases();
                foreach (var text in GameObject.Find(name + " Panel").GetComponentsInChildren<Text>())
                {
                    Assert.That(text.font, Is.Not.Null, text.name);
                    if (!string.IsNullOrEmpty(text.text)) Assert.That(text.cachedTextGenerator.vertexCount, Is.GreaterThan(0), text.text);
                }
                var back = Object.FindObjectsByType<Button>(FindObjectsSortMode.None).Single(item => item.name == (name == "Opciones" ? "Guardar y volver" : "Seguir jugando"));
                back.onClick.Invoke();
                Assert.That(main.interactable, Is.True);
                Assert.That(EventSystem.current.currentSelectedGameObject, Is.EqualTo(button.gameObject));
            }
            yield return new ExitPlayMode();
        }

        [UnityTest]
        public IEnumerator OptionsPersistAndMissingLevelShowsRecoverableMessage()
        {
            var hadVolume = PlayerPrefs.HasKey(MenuPrincipalController.VolumeKey);
            var oldVolume = PlayerPrefs.GetFloat(MenuPrincipalController.VolumeKey);
            var hadMusic = PlayerPrefs.HasKey(MenuPrincipalController.MusicKey);
            var oldMusic = PlayerPrefs.GetInt(MenuPrincipalController.MusicKey);
            EditorSceneManager.OpenScene("Assets/Scenes/MenuPrincipal.unity");
            yield return new EnterPlayMode();
            yield return null;
            var controller = Object.FindFirstObjectByType<MenuPrincipalController>();
            controller.ShowOptions();
            var slider = Object.FindFirstObjectByType<Slider>();
            slider.value = 0.23f;
            var toggle = Object.FindObjectsByType<Toggle>(FindObjectsSortMode.None).Single(item => item.name == "Música de fondo");
            toggle.isOn = false;
            Assert.That(PlayerPrefs.GetFloat(MenuPrincipalController.VolumeKey), Is.EqualTo(0.23f).Within(0.001));
            Assert.That(controller.GetComponents<AudioSource>().Single(item => item.loop).mute, Is.True);
            controller.ClosePanel();
            controller.ShowOptions();
            Assert.That(slider.value, Is.EqualTo(0.23f).Within(0.001));
            controller.ClosePanel();
            var serialized = new UnityEditor.SerializedObject(controller);
            serialized.FindProperty("levelScene").stringValue = "Assets/MissingLevel.unity";
            serialized.ApplyModifiedPropertiesWithoutUndo();
            controller.Play();
            Assert.That(controller.IsLoading, Is.False);
            Assert.That(GameObject.Find("Información Panel"), Is.Not.Null);
            controller.ClosePanel();
            Assert.That(SceneManager.GetActiveScene().path, Is.EqualTo("Assets/Scenes/MenuPrincipal.unity"));
            yield return new ExitPlayMode();
            if (hadVolume) PlayerPrefs.SetFloat(MenuPrincipalController.VolumeKey, oldVolume);
            else PlayerPrefs.DeleteKey(MenuPrincipalController.VolumeKey);
            if (hadMusic) PlayerPrefs.SetInt(MenuPrincipalController.MusicKey, oldMusic);
            else PlayerPrefs.DeleteKey(MenuPrincipalController.MusicKey);
            PlayerPrefs.Save();
        }
    }
}
