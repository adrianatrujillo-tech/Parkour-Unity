using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Parkour.Menu
{
    [RequireComponent(typeof(Button))]
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
            helper.AddVert(Vector3.zero, new Color(1f, 0.76f, 0.26f, intensity * 0.17f), Vector2.zero);
            foreach (var point in points) helper.AddVert(point, new Color(1f, 0.76f, 0.26f, intensity * 0.17f), Vector2.zero);
            for (var index = 0; index < points.Length; index++) helper.AddTriangle(0, index + 1, (index + 1) % 6 + 1);
            for (var index = 0; index < points.Length; index++)
            {
                var next = (index + 1) % points.Length;
                var start = helper.currentVertCount;
                helper.AddVert(points[index], new Color(1, 0.85f, 0.4f, intensity), Vector2.zero);
                helper.AddVert(points[next], new Color(1, 0.85f, 0.4f, intensity), Vector2.zero);
                helper.AddVert(points[next] * 0.96f, new Color(1, 0.85f, 0.4f, 0), Vector2.zero);
                helper.AddVert(points[index] * 0.96f, new Color(1, 0.85f, 0.4f, 0), Vector2.zero);
                helper.AddTriangle(start, start + 1, start + 2);
                helper.AddTriangle(start, start + 2, start + 3);
            }
        }
    }
}
