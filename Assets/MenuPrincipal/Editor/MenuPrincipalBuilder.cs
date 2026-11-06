using System;
using System.IO;
using Parkour.Menu;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Parkour.MenuEditor
{
    public static class MenuPrincipalBuilder
    {
        private const string Root = "Assets/MenuPrincipal/";
        private static readonly Color Cream = new Color(0.96f, 0.9f, 0.75f);
        private static Font font;

        [MenuItem("Parkour/Menú principal/Regenerar escena")]
        public static void Generate()
        {
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            ConfigureTexture();
            CreateTone("Hover", 660, 0.06f);
            CreateTone("Click", 440, 0.11f);
            font = AssetDatabase.LoadAssetAtPath<Font>(Root + "Fonts/Lato-Regular.ttf");
            if (font == null) throw new InvalidOperationException("Falta la fuente del menú: Lato-Regular.ttf");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var camera = new GameObject("Menu Camera", typeof(Camera), typeof(AudioListener)).GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            camera.cullingMask = 0;
            camera.tag = "MainCamera";
            var canvas = new GameObject("Menu Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1672, 941);
            scaler.matchWidthOrHeight = 0.5f;
            var composition = Rect("Composición original 1672x941", canvas.transform, Vector2.zero, new Vector2(1672, 941));
            composition.gameObject.AddComponent<MenuCompositionFit>();
            var artwork = Rect("Fondo PARKOUR sin botones", composition, Vector2.zero, new Vector2(1672, 941)).gameObject.AddComponent<RawImage>();
            artwork.texture = AssetDatabase.LoadAssetAtPath<Texture2D>(Root + "Art/ParkourBackground.png");
            artwork.raycastTarget = false;
            var controller = new GameObject("MenuPrincipal Controller").AddComponent<MenuPrincipalController>();
            var music = controller.gameObject.AddComponent<AudioSource>();
            music.clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Hollow Knight OST - White Palace.mp3");
            music.loop = true;
            music.playOnAwake = false;
            music.spatialBlend = 0;
            var effects = controller.gameObject.AddComponent<AudioSource>();
            effects.playOnAwake = false;
            var events = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            events.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
            var buttons = Rect("Botones principales", composition, Vector2.zero, new Vector2(1672, 941)).gameObject.AddComponent<CanvasGroup>();
            var labels = new[] { "Jugar", "Opciones", "Salir" };
            var centers = new[] { 420f, 535f, 650f };
            UnityAction[] actions = { controller.Play, controller.ShowOptions, controller.ShowExit };
            var main = new Button[labels.Length];
            for (var index = 0; index < main.Length; index++)
            {
                var target = Rect(labels[index], buttons.transform, new Vector2(355 - 836, 470.5f - centers[index]), new Vector2(410, 78));
                var button = target.gameObject.AddComponent<Button>();
                var highlight = target.gameObject.AddComponent<MenuButtonHighlight>();
                button.targetGraphic = highlight;
                button.transition = Selectable.Transition.None;
                UnityEventTools.AddPersistentListener(button.onClick, actions[index]);
                Set(highlight, "controller", controller);
                Label(labels[index], target, new Vector2(20, 0), new Vector2(310, 76), 34);
                Label("0" + (index + 1), target, new Vector2(-145, 0), new Vector2(65, 76), 21);
                main[index] = button;
            }
            for (var index = 0; index < main.Length; index++)
            {
                var navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnUp = main[(index + main.Length - 1) % main.Length], selectOnDown = main[(index + 1) % main.Length] };
                main[index].navigation = navigation;
            }
            events.GetComponent<EventSystem>().firstSelectedGameObject = main[0].gameObject;

            var options = Panel("Opciones", composition);
            Label("Ajusta el ambiente de tu aventura", options.transform, new Vector2(0, 140), new Vector2(570, 45), 23);
            Label("Volumen", options.transform, new Vector2(-170, 72), new Vector2(210, 50), 25);
            var volumeValue = Label("65%", options.transform, new Vector2(245, 72), new Vector2(80, 50), 23);
            var volume = MakeSlider(options.transform, new Vector2(45, 72));
            UnityEventTools.AddPersistentListener(volume.onValueChanged, controller.SetVolume);
            var musicToggle = MakeToggle("Música de fondo", options.transform, new Vector2(0, -10));
            UnityEventTools.AddPersistentListener(musicToggle.onValueChanged, controller.SetMusic);
            var screenToggle = MakeToggle("Pantalla completa", options.transform, new Vector2(0, -85));
            UnityEventTools.AddPersistentListener(screenToggle.onValueChanged, controller.SetFullscreen);
            Label("Los cambios se guardan automáticamente", options.transform, new Vector2(0, -155), new Vector2(570, 40), 19);
            MakeButton("Guardar y volver", options.transform, new Vector2(0, -220), controller.ClosePanel);
            var exit = Panel("Salir", composition);
            Label("¿Terminamos por ahora?", exit.transform, new Vector2(0, 100), new Vector2(570, 70), 32);
            Label("Volverás al escritorio.\nPodrás regresar cuando quieras.", exit.transform, new Vector2(0, 15), new Vector2(570, 95), 24);
            MakeButton("Seguir jugando", exit.transform, new Vector2(0, -105), controller.ClosePanel);
            MakeButton("Salir del juego", exit.transform, new Vector2(0, -200), controller.Quit);
            var messagePanel = Panel("Información", composition);
            var message = Label("", messagePanel.transform, new Vector2(0, 25), new Vector2(570, 220), 27);
            MakeButton("Volver", messagePanel.transform, new Vector2(0, -180), controller.ClosePanel);

            Set(controller, "music", music);
            Set(controller, "effects", effects);
            Set(controller, "hoverSound", AssetDatabase.LoadAssetAtPath<AudioClip>(Root + "Audio/Hover.wav"));
            Set(controller, "clickSound", AssetDatabase.LoadAssetAtPath<AudioClip>(Root + "Audio/Click.wav"));
            Set(controller, "mainButtons", buttons);
            Set(controller, "optionsPanel", options.transform.parent.gameObject);
            Set(controller, "exitPanel", exit.transform.parent.gameObject);
            Set(controller, "messagePanel", messagePanel.transform.parent.gameObject);
            Set(controller, "message", message);
            Set(controller, "volume", volume);
            Set(controller, "volumeValue", volumeValue);
            Set(controller, "musicEnabled", musicToggle);
            Set(controller, "fullscreen", screenToggle);
            Set(controller, "playButton", main[0]);
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/MenuPrincipal.unity");
            AssetDatabase.SaveAssets();
            Debug.Log("PARKOUR_MENU_GENERATED");
        }

        private static GameObject Panel(string title, Transform parent)
        {
            var overlay = Rect(title + " Panel", parent, Vector2.zero, new Vector2(1672, 941));
            var dim = overlay.gameObject.AddComponent<Image>();
            dim.color = new Color(0.02f, 0.03f, 0.06f, 0.55f);
            var box = Rect(title, overlay, new Vector2(280, 0), new Vector2(660, 640));
            var image = box.gameObject.AddComponent<Image>();
            image.color = new Color(0.035f, 0.065f, 0.09f, 0.97f);
            var outline = box.gameObject.AddComponent<Outline>();
            outline.effectColor = Cream;
            outline.effectDistance = new Vector2(2, -2);
            Label("P A R K O U R", box, new Vector2(0, 275), new Vector2(570, 40), 18);
            Label(title, box, new Vector2(0, 215), new Vector2(570, 70), 42);
            var rule = Rect("Separador dorado", box, new Vector2(0, 175), new Vector2(520, 1)).gameObject.AddComponent<Image>();
            rule.color = new Color(0.86f, 0.78f, 0.59f, 0.6f);
            rule.raycastTarget = false;
            overlay.gameObject.SetActive(false);
            return box.gameObject;
        }

        private static RectTransform Rect(string name, Transform parent, Vector2 position, Vector2 size)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
            return rect;
        }

        private static Text Label(string text, Transform parent, Vector2 position, Vector2 size, int fontSize)
        {
            var label = Rect(text.Length < 30 ? text : "Texto", parent, position, size).gameObject.AddComponent<Text>();
            label.font = font;
            label.text = text;
            label.fontSize = fontSize;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Cream;
            label.raycastTarget = false;
            return label;
        }

        private static Button MakeButton(string text, Transform parent, Vector2 position, UnityAction action)
        {
            var target = Rect(text, parent, position, new Vector2(420, 65));
            var image = target.gameObject.AddComponent<Image>();
            image.color = new Color(0.33f, 0.27f, 0.15f);
            var button = target.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            var colors = button.colors;
            colors.highlightedColor = new Color(1.5f, 1.35f, 0.9f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;
            var outline = target.gameObject.AddComponent<Outline>();
            outline.effectColor = Cream;
            outline.effectDistance = Vector2.one;
            Label(text, target, Vector2.zero, new Vector2(410, 60), 28);
            UnityEventTools.AddPersistentListener(button.onClick, action);
            return button;
        }

        private static Slider MakeSlider(Transform parent, Vector2 position)
        {
            var target = Rect("Volumen", parent, position, new Vector2(260, 24));
            var background = target.gameObject.AddComponent<Image>();
            background.color = new Color(0.25f, 0.25f, 0.23f);
            var slider = target.gameObject.AddComponent<Slider>();
            var fill = Rect("Fill", target, Vector2.zero, Vector2.zero).gameObject.AddComponent<Image>();
            fill.color = new Color(0.7f, 0.55f, 0.23f);
            slider.fillRect = fill.rectTransform;
            var handle = Rect("Handle", target, Vector2.zero, new Vector2(18, 12)).gameObject.AddComponent<Image>();
            handle.color = Cream;
            slider.handleRect = handle.rectTransform;
            slider.targetGraphic = handle;
            slider.value = 0.65f;
            return slider;
        }

        private static Toggle MakeToggle(string text, Transform parent, Vector2 position)
        {
            var target = Rect(text, parent, position, new Vector2(500, 60));
            var toggle = target.gameObject.AddComponent<Toggle>();
            var background = Rect("Casilla", target, new Vector2(-210, 0), new Vector2(44, 44)).gameObject.AddComponent<Image>();
            background.color = new Color(0.35f, 0.31f, 0.22f);
            var check = Rect("Activo", background.transform, Vector2.zero, new Vector2(26, 26)).gameObject.AddComponent<Image>();
            check.color = Cream;
            toggle.targetGraphic = background;
            toggle.graphic = check;
            Label(text, target, new Vector2(30, 0), new Vector2(400, 60), 27);
            toggle.isOn = true;
            return toggle;
        }

        private static void Set(UnityEngine.Object target, string property, UnityEngine.Object value)
        {
            var serialized = new SerializedObject(target);
            serialized.FindProperty(property).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConfigureTexture()
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(Root + "Art/ParkourBackground.png");
            importer.textureType = TextureImporterType.Default;
            importer.maxTextureSize = 2048;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }

        private static void CreateTone(string name, float frequency, float duration)
        {
            Directory.CreateDirectory(Root + "Audio");
            var path = Root + "Audio/" + name + ".wav";
            if (File.Exists(path)) return;
            const int rate = 22050;
            var count = (int)(rate * duration);
            using (var writer = new BinaryWriter(File.Create(path)))
            {
                writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
                writer.Write(36 + count * 2);
                writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt "));
                writer.Write(16);
                writer.Write((short)1);
                writer.Write((short)1);
                writer.Write(rate);
                writer.Write(rate * 2);
                writer.Write((short)2);
                writer.Write((short)16);
                writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
                writer.Write(count * 2);
                for (var sample = 0; sample < count; sample++)
                {
                    var envelope = Mathf.Sin(Mathf.PI * sample / count);
                    writer.Write((short)(Mathf.Sin(2 * Mathf.PI * frequency * sample / rate) * envelope * 4000));
                }
            }
            AssetDatabase.ImportAsset(path);
        }

        public static void BuildReview()
        {
            Directory.CreateDirectory("Artifacts/MenuReview");
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/MenuPrincipal.unity", "Assets/Scenes/SampleScene.unity" },
                locationPathName = "Artifacts/MenuReview/Parkour.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development
            });
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                throw new Exception("La compilación del menú ha fallado: " + report.summary.result);
            Debug.Log("PARKOUR_MENU_BUILD_OK");
        }
    }
}
