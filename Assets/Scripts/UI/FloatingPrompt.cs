using UnityEngine;
using TMPro;

namespace Farming_related.UI
{
    /// <summary>
    /// Small reusable component that manages a floating prompt UI (text) above a world position.
    /// It handles show/hide based on player distance and positions the prompt at a configurable offset.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class FloatingPrompt : MonoBehaviour
    {
        [SerializeField] private TMP_Text promptText;
        [SerializeField] private Vector3 worldOffset = default;

        void Reset()
        {
            worldOffset = Vector3.up * 1.5f;
        }

        /// <summary>
        /// Update and show/hide the prompt based on the player's distance to the target position.
        /// </summary>
        public void UpdatePrompt(Transform player, Vector3 worldPosition, float showDistance, string text)
        {
            if (player == null || promptText == null)
            {
                if (gameObject.activeSelf) gameObject.SetActive(false);
                return;
            }

            float distance = Vector3.Distance(player.position, worldPosition);
            if (distance > showDistance)
            {
                if (gameObject.activeSelf) gameObject.SetActive(false);
                return;
            }

            if (!gameObject.activeSelf) gameObject.SetActive(true);

            // position the prompt above the supplied world position
            transform.position = worldPosition + worldOffset;

            // update text
            promptText.text = text ?? string.Empty;
        }

        public void Hide()
        {
            if (gameObject.activeSelf) gameObject.SetActive(false);
        }

        public void SetText(TMP_Text textComponent)
        {
            promptText = textComponent;
        }
    }
}
