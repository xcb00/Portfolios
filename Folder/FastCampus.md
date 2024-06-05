## RPG
### Lighting
> `Window > Rendering > Lighting`

### Physics.Overlap에서 Collider 순서
1. 근접도 순서 : 중심점에서 가까운 순서대로 반환
2. 레이어 순서 : 레이어마스크를 지정했다면, 해당 레이어가 먼저 반환
3. Static / Dynamic : 정적 오브젝트가 동적 오브젝트보다 먼저 반환
4. Trigger / Collider : Trigger가 일반 Collider보다 먼저 반환
    <details>
    <summary>Details</summary>
    
    > 유니티의 Physics.OverlapSphere 함수에서 반환되는 Collider 인덱스 순서에 대해 자세히 살펴보겠습니다.
    > 
    > **Physics.OverlapSphere에서 Collider 인덱스 순서**
    > 1. 근접도 순서: Physics.OverlapSphere 함수는 중심점에서 가까운 순서대로 Collider 객체를 반환합니다. 즉, 중심점에 가장 가까운 Collider가 첫 번째 인덱스에 위치합니다.
    > 
    > 2. 레이어 순서: 만약 레이어 마스크를 지정했다면, 해당 레이어의 Collider가 먼저 반환됩니다. 레이어 마스크에 포함된 Collider가 먼저 나오고, 그 다음 레이어 마스크에 포함되지 않은 Collider가 반환됩니다.
    > 
    > 3. Static vs Dynamic: 정적 오브젝트(Static)의 Collider가 동적 오브젝트(Dynamic)의 Collider보다 먼저 반환됩니다. 이는 정적 오브젝트가 동적 오브젝트보다 처리 속도가 빠르기 때문입니다.
    > 
    > 4. Trigger vs Collider: Trigger 모드의 Collider가 일반 Collider보다 먼저 반환됩니다. Trigger는 물리적인 충돌이 아닌 센서 역할을 하기 때문에 처리 속도가 빠릅니다.
    > 
    > 따라서 Physics.OverlapSphere 함수를 사용할 때는 이러한 인덱스 순서를 고려해야 합니다. 예를 들어, 특정 레이어의 Collider만 검출하고 싶다면 레이어 마스크를 사용하는 것이 좋습니다.
    > 
    > 또한 성능 최적화를 위해서는 Sphere Collider를 사용하는 것이 가장 좋습니다. Collider 종류에 따른 처리 속도 차이를 고려해야 합니다.
    </details>

### Scripts
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
