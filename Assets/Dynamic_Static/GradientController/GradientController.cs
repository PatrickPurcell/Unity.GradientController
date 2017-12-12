
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
    [RequireComponent(typeof(MeshRenderer))]
    [AddComponentMenu("Dynamic_Static/GradientController")]
    public sealed class GradientController
        : MonoBehaviour
    {
        #region CONSTANTS
        private static readonly string AnchorName = "GradientHandlesAnchor";
        private static readonly string OpaqueMaterialName = "GradientControllerOpaque";
        private static readonly string TransparentMaterialName = "GradientControllerTransparent";
        private static readonly string[] ColorPropertyNames = new string[] { "_Color0", "_Color1" };
        private static readonly string[] HandlePropertyNames = new string[] { "_Handle0", "_Handle1" };
        private static readonly string LengthPropertyName = "_Length";
        private static readonly Vector3 DefaultHandlePosition = new Vector3(0, 1.5f, 0);
        #endregion

        #region FIELDS
        [SerializeField] private GameObject anchor;
        [SerializeField] private GradientHandle handle0;
        [SerializeField] private GradientHandle handle1;
        private MaterialPropertyBlock materialPropertyBlock;
        private MeshRenderer meshRenderer;
        #if UNITY_EDITOR
        [SerializeField] private bool _transparent = false;
        [SerializeField] private bool _drawGizmo = true;
        [SerializeField] private float _gizmoSize = 0.25f;
        [SerializeField, HideInInspector] private bool _materialApplied = false;
        private Material _opaqueMaterial;
        private Material _transparentMaterial;
        #endif
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Gets this GradientController's GradientHandles anchor.
        /// </summary>
        private GameObject Anchor
        {
            get
            {
                if (anchor == null)
                {
                    anchor = new GameObject(AnchorName);
                    anchor.transform.parent = transform;
                    anchor.transform.localPosition = Vector3.zero;
                }

                return anchor;
            }
        }

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
                    case 0: return ValidateHandle(ref handle0, DefaultHandlePosition);
                    case 1: return ValidateHandle(ref handle1, -DefaultHandlePosition);
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        private MeshRenderer MeshRenderer
        {
            get
            {
                return
                    meshRenderer == null ?
                    meshRenderer = GetComponent<MeshRenderer>() :
                    meshRenderer;
            }
        }

        private MaterialPropertyBlock MaterialPropertyBlock
        {
            get
            {
                return
                    materialPropertyBlock == null ?
                    materialPropertyBlock = new MaterialPropertyBlock() :
                    materialPropertyBlock;
            }
        }
        #endregion

        #region AWAKE
        private void Awake()
        {
            #if UNITY_EDITOR
            if (!_materialApplied)
            {
                var material = ValidateMaterial(ref _opaqueMaterial, OpaqueMaterialName);
                MeshRenderer.sharedMaterial = material;
                _materialApplied = true;
            }
            #endif

            ValidateHandle(ref handle0, DefaultHandlePosition);
            ValidateHandle(ref handle1, -DefaultHandlePosition);
        }
        #endregion

        #region ON DESTROY
        private void OnDestroy()
        {
            for (int i = 0; i < Count; ++i)
            {
                DestroyObject(this[i].gameObject);
            }

            DestroyObject(anchor.gameObject);
        }

        private void DestroyObject<T>(T obj)
            where T : UnityEngine.Object
        {
            if (Application.isEditor)
            {
                DestroyImmediate(obj);
            }
            else
            {
                Destroy(obj);
            }
        }
        #endregion

        #if UNITY_EDITOR
        #region ON VALIDATE
        private void OnValidate()
        {
            MeshRenderer.sharedMaterial = _transparent ?
                ValidateMaterial(ref _transparentMaterial, TransparentMaterialName) :
                ValidateMaterial(ref _opaqueMaterial, OpaqueMaterialName);
        }
        #endregion
        #endif

        #region UPDATE
        private void Update()
        {
            MeshRenderer.GetPropertyBlock(MaterialPropertyBlock);
            for (int i = 0; i < Count; ++i)
            {
                var handle = this[i];
                MaterialPropertyBlock.SetColor(ColorPropertyNames[i], handle.Color);
                MaterialPropertyBlock.SetVector(HandlePropertyNames[i], handle.transform.position);
            }

            var length = Vector3.Distance(this[0].transform.position, this[1].transform.position);
            MaterialPropertyBlock.SetFloat(LengthPropertyName, length);
            MeshRenderer.SetPropertyBlock(MaterialPropertyBlock);
        }
        #endregion

        #region METHODS
        private GradientHandle ValidateHandle(ref GradientHandle handle, Vector3 defaultPosition)
        {
            if (handle == null)
            {
                handle = new GameObject(typeof(GradientHandle).Name).AddComponent<GradientHandle>();
                handle.transform.parent = Anchor.transform;
                handle.transform.localPosition = defaultPosition;
            }

            return handle;
        }

        #if UNITY_EDITOR
        private Material ValidateMaterial(ref Material material, string name)
        {
            if (material == null)
            {
                var guids = UnityEditor.AssetDatabase.FindAssets(name);
                if (guids.Length > 0)
                {
                    var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                    material = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(assetPath);
                }
            }

            return material;
        }
        #endif
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
                    float t0 =  i      / (float)segments;
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
