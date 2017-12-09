
/*
==========================================
    Copyright (c) 2017 Dynamic_Static,
        Patrick Purcell
    Licensed under the MIT license
    http://opensource.org/licenses/MIT
==========================================
*/

namespace Dynamic_Static
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Provides control over a gradient.
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [AddComponentMenu("Dynamic_Static/GradientController")]
    public sealed class GradientController
        : MonoBehaviour
    {
        #region CONSTANTS
        private static readonly string[] ColorPropertyNames = new string[] { "_Color0", "_Color1" };
        private static readonly string[] HandlePropertyNames = new string[] { "_Handle0", "_Handle1" };
        #endregion

        #region FIELDS
        [SerializeField] private GradientHandle handle0;
        [SerializeField] private GradientHandle handle1;
        private Renderer gradientRenderer;
        #if UNITY_EDITOR
        [SerializeField] private bool _drawGizmo = true;
        [SerializeField] private float _gizmoSize = 0.25f;
        #endif
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Gets this GradientController's GradientHandle count.
        /// </summary>
        public int Count
        {
            get { return 2; }
        }

        /// <summary>
        /// Gets the GradientHandle at the specified index.
        /// </summary>
        /// <param name="i">The zero-based index of the GradientHandle to get</param>
        /// <returns>The GradientHandle at the specified index</returns>
        public GradientHandle this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0: return ValidateHandle(ref handle0);
                    case 1: return ValidateHandle(ref handle1);
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }
        #endregion

        #region AWAKE
        private void Awake()
        {
            ValidateHandle(ref handle0);
            ValidateHandle(ref handle1);
        }
        #endregion

        #region ON DESTROY
        private void OnDestroy()
        {
            for (int i = Count - 1; i >= 0; --i)
            {
                #if UNITY_EDITOR
                DestroyImmediate(this[i]);
                #else
                Destroy(this[i]);
                #endif
            }
        }
        #endregion

        #region ON RENDER OBJECT
        private void OnRenderObject()
        {
            if (gradientRenderer == null)
            {
                gradientRenderer = GetComponent<Renderer>();
            }

            var material = gradientRenderer != null ? gradientRenderer.sharedMaterial : null;
            if (material)
            {
                for (int i = 0; i < Count; ++i)
                {
                    var handle = this[i];
                    material.SetColor(ColorPropertyNames[i], handle.Color);
                    material.SetVector(HandlePropertyNames[i], handle.transform.localPosition);
                }
            }
        }
        #endregion

        #region METHODS
        private GradientHandle ValidateHandle(ref GradientHandle handle)
        {
            if (handle == null)
            {
                handle = new GameObject(typeof(GradientHandle).Name).AddComponent<GradientHandle>();
                handle.transform.parent = transform;
                handle.transform.localPosition = Vector3.zero;
            }

            return handle;
        }
        #endregion

        #if UNITY_EDITOR
        #region ON DRAW GIZMOS
        private void OnDrawGizmos()
        {
            if (_drawGizmo)
            {
                var gizmosColor = Gizmos.color;
                var gizmosMatrix = Gizmos.matrix;

                this[0].transform.LookAt(this[1].transform);
                this[1].transform.LookAt(this[0].transform);
                for (int i = 0; i < Count; ++i)
                {
                    var handle = this[i];
                    Gizmos.color = handle.Color;
                    Gizmos.matrix = Matrix4x4.TRS(handle.transform.position, handle.transform.rotation, Vector3.one);
                    Gizmos.DrawWireCube(Vector3.zero, Vector3.one * _gizmoSize * 0.85f);
                    handle.transform.localScale = Vector3.one * _gizmoSize;
                    Gizmos.matrix = gizmosMatrix;
                }

                int segments = 12;
                for (int i = 0; i < segments; ++i)
                {
                    float t0 = i / (float)segments;
                    float t1 = (i + 1) / (float)segments;

                    Color color0 = Color.Lerp(this[0].Color, this[1].Color, t0);
                    Color color1 = Color.Lerp(this[0].Color, this[1].Color, t1);
                    Gizmos.color = (color0 + color1) * 0.5f;

                    Vector3 position0 = this[0].transform.position;
                    Vector3 position1 = this[1].transform.position;
                    Vector3 v0 = Vector3.Lerp(position0, position1, t0);
                    Vector3 v1 = Vector3.Lerp(position0, position1, t1);
                    Gizmos.DrawLine(v0, v1);
                }

                Gizmos.matrix = gizmosMatrix;
                Gizmos.color = gizmosColor;
            }
        }
        #endregion
        #endif
    }
} // namespace Dynamic_Static
