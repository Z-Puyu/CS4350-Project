using UnityEngine;

public class MouseDebugger : MonoBehaviour
{
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("Mouse click detected - Input system is working");
            Debug.Log($"Mouse position: {Input.mousePosition}");
            
            Camera cam = Camera.main;
            if (cam != null) {
                Vector3 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
                Debug.Log($"World position: {worldPos}");
            }
        }
    }
}
