using UnityEngine;

namespace Parkour.Menu
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public sealed class MenuCompositionFit : MonoBehaviour
    {
        private void LateUpdate()
        {
            var parent = transform.parent as RectTransform;
            if (parent == null) return;
            var scale = Mathf.Min(parent.rect.width / 1672f, parent.rect.height / 941f);
            transform.localScale = new Vector3(scale, scale, 1);
        }
    }
}
