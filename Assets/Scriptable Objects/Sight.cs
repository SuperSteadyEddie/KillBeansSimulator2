using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sight", menuName = "Sights")]
public class Sight : ScriptableObject
{
    public GameObject sight;
    public float cameraZoom;
    public float aimTime;
    public SightType type;
    [HideInInspector] public float minScopeZoom;
    [HideInInspector] public float scopeZoomChangeSpeed;
    [HideInInspector] public float maxScopeZoom;
    [HideInInspector] public float currentScopeZoom;


    public enum SightType
    {
        Irons,
        Reflector,
        Holographic,
        Telescopic
    }


    #region Editor 
#if UNITY_EDITOR
    [CustomEditor(typeof(Sight))]
    public class SightEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Sight sight = (Sight)target;

            EditorGUILayout.Space();
            if (sight.type == SightType.Telescopic)
            {
                EditorGUILayout.LabelField("Zoom");
                sight.minScopeZoom = EditorGUILayout.FloatField("MinScopeZoom", sight.minScopeZoom);
                sight.scopeZoomChangeSpeed = EditorGUILayout.FloatField("ScopeZoomChangeSpeed",sight.scopeZoomChangeSpeed);
                sight.maxScopeZoom = EditorGUILayout.FloatField("MaxScopeZoom",sight.maxScopeZoom);
            }
        }
    }
#endif
    #endregion
}
