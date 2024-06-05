## RPG
<details>
<summary>Target Camera</summary>

```C#
using UnityEngine;

namespace MyRPG.Cameras
{
    public class TopDownCam : MonoBehaviour
    {
        #region Variables
        public float height = 5f;
        public float distance = 10f;
        public float angle = 45f;
        public float lookAtHeight = 2f;
        public float smoothSpeed = 0.5f;

        Vector3 refVelocity;// = Vector3.zero; // 래퍼런스 밸로시티
        public Transform target = null;
        #endregion

        private void LateUpdate()
        {
            HandleCamera();
        }

        public void HandleCamera()
        {
            if (!target) return;

            Vector3 worldPos = (Vector3.forward * -distance) + (Vector3.up * height);
            //Debug.DrawLine(target.position, worldPos, Color.red);

            Vector3 rotVec = Quaternion.AngleAxis(angle, Vector3.up) * worldPos;
            //Debug.DrawLine(target.position, rotVec, Color.green);

            Vector3 targetPos = target.position;
            targetPos.y += lookAtHeight;

            Vector3 camPos = targetPos + rotVec;
            //Debug.DrawLine(target.position, camPos, Color.blue);

            transform.position = Vector3.SmoothDamp(transform.position, camPos, ref refVelocity, smoothSpeed);

            transform.LookAt(target.position);
        }
    }

}
```
</details>
<details>
<summary>Custom Editor</summary>

```C#
using UnityEditor;
using UnityEngine;

namespace MyRPG.Cameras
{
    [CustomEditor(typeof(TopDownCam))]
    public class TopDowCamEditor : Editor
    {
        #region Variables
        private TopDownCam targetCam;
        #endregion

        public override void OnInspectorGUI()
        {
            targetCam = (TopDownCam)target;
            base.OnInspectorGUI();
        }

        private void OnSceneGUI()
        {
            if (!targetCam || !targetCam.target)
                return;

            Transform camTarget = targetCam.target;
            Vector3 targetPos = camTarget.position;
            targetPos.y += targetCam.lookAtHeight;

            // Draw distance circle
            Handles.color = new Color(1f, 0f, 0f, 0.15f);
            Handles.DrawSolidDisc(targetPos, Vector3.up, targetCam.distance);

            Handles.color = new Color(0f, 1f, 0f, 0.75f);
            Handles.DrawWireDisc(targetPos, Vector3.up, targetCam.distance);

            // Create slider handles to adjust camera properties
            Handles.color = new Color(1f, 0f, 0f, 0.5f);
            targetCam.distance = Handles.ScaleSlider(targetCam.distance, targetPos, -camTarget.forward, 
                Quaternion.identity, targetCam.distance, 0.1f);
            targetCam.distance = Mathf.Clamp(targetCam.distance, 2f, float.MaxValue);

            Handles.color = new Color(0f, 0f, 1f, 0.5f);
            targetCam.height = Handles.ScaleSlider(targetCam.height, targetPos, camTarget.up,
                Quaternion.identity, targetCam.height, 0.1f);
            targetCam.height = Mathf.Clamp(targetCam.height, 0.5f, float.MaxValue);

            // Create Lables
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontSize = 15;
            labelStyle.normal.textColor = Color.white;
            labelStyle.alignment = TextAnchor.UpperCenter;

            Handles.Label(targetPos + (-camTarget.forward * targetCam.distance), "Distance", labelStyle);

            labelStyle.alignment = TextAnchor.MiddleRight;
            Handles.Label(targetPos + (-camTarget.forward * targetCam.distance), "Height", labelStyle);

            targetCam.HandleCamera();
        }
    }
}
```
</details>






---
<details>
<summary>Target Camera</summary>

```C#
```
</details>
