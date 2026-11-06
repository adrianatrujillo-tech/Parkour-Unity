using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Parkour.Menu
{
    [RequireComponent(typeof(Button), typeof(CanvasRenderer))]
    public sealed class MenuButtonHighlight : MaskableGraphic, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private MenuPrincipalController controller;
        private bool hovered;
        private bool selected;
        private float intensity;

        protected override void Awake()
        {
            base.Awake();
            raycastTarget = true;
        }

        private void Update()
        {
            var target = (hovered || selected) && GetComponent<Button>().IsInteractable() ? 1f : 0f;
            intensity = Mathf.MoveTowards(intensity, target, Time.unscaledDeltaTime * 7f);
            SetVerticesDirty();
        }

        public void OnPointerEnter(PointerEventData data)
        {
            hovered = true;
            if (GetComponent<Button>().IsInteractable()) controller.PlayHover();
        }

        public void OnPointerExit(PointerEventData data) => hovered = false;
        public void OnSelect(BaseEventData data)
        {
            selected = true;
            controller.PlayHover();
        }
        public void OnDeselect(BaseEventData data) => selected = false;

        protected override void OnDisable()
        {
            hovered = selected = false;
            intensity = 0;
            base.OnDisable();
        }

        protected override void OnPopulateMesh(VertexHelper helper)
        {
            helper.Clear();
            var rect = rectTransform.rect;
            var points = new[] {
                new Vector2(rect.xMin, 0), new Vector2(rect.xMin + 35, rect.yMax),
                new Vector2(rect.xMax - 35, rect.yMax), new Vector2(rect.xMax, 0),
                new Vector2(rect.xMax - 35, rect.yMin), new Vector2(rect.xMin + 35, rect.yMin)
            };
            var fill = Color.Lerp(new Color(0.055f, 0.075f, 0.10f, 0.88f), new Color(0.43f, 0.31f, 0.09f, 0.95f), intensity);
            var border = Color.Lerp(new Color(0.86f, 0.78f, 0.59f, 0.9f), new Color(1f, 0.87f, 0.45f, 1f), intensity);
            helper.AddVert(Vector3.zero, fill, Vector2.zero);
            foreach (var point in points) helper.AddVert(point, fill, Vector2.zero);
            for (var index = 0; index < points.Length; index++) helper.AddTriangle(0, index + 1, (index + 1) % 6 + 1);
            for (var index = 0; index < points.Length; index++)
            {
                var next = (index + 1) % points.Length;
                var start = helper.currentVertCount;
                helper.AddVert(points[index], border, Vector2.zero);
                helper.AddVert(points[next], border, Vector2.zero);
                helper.AddVert(points[next] * 0.96f, Color.clear, Vector2.zero);
                helper.AddVert(points[index] * 0.96f, Color.clear, Vector2.zero);
                helper.AddTriangle(start, start + 1, start + 2);
                helper.AddTriangle(start, start + 2, start + 3);
            }
        }
    }
}
