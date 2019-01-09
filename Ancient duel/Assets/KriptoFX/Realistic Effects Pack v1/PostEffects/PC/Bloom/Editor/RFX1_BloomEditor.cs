using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityStandardAssets.CinematicEffects
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RFX1_Bloom))]
    public class RFX1_BloomEditor : Editor
    {
        [NonSerialized]
        private List<SerializedProperty> m_Properties = new List<SerializedProperty>();

        RFX1_BloomGraphDrawer _graph;

        bool CheckHdr(RFX1_Bloom target)
        {
            var camera = target.GetComponent<Camera>();
#if UNITY_5_6_OR_NEWER
            return camera != null && camera.allowHDR;
#else
            return camera != null && camera.hdr;
#endif
        }

        void OnEnable()
        {
            var settings = RFX1_FieldFinder<RFX1_Bloom>.GetField(x => x.settings);
            foreach (var setting in settings.FieldType.GetFields())
            {
                var prop = settings.Name + "." + setting.Name;
                m_Properties.Add(serializedObject.FindProperty(prop));
            }

            _graph = new RFX1_BloomGraphDrawer();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!serializedObject.isEditingMultipleObjects)
            {
                EditorGUILayout.Space();
                var bloom = (RFX1_Bloom)target;
                _graph.Prepare(bloom.settings, CheckHdr(bloom));
                _graph.DrawGraph();
                EditorGUILayout.Space();
            }

            foreach (var property in m_Properties)
                EditorGUILayout.PropertyField(property);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
