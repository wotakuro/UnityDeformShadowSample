using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DeformShadow : MonoBehaviour
{
    private struct RenderingInfo
    {
        public Renderer targetRenderer;
        public Transform trans;
        public RenderingInfo(Renderer r)
        {
            this.targetRenderer = r;
            this.trans = r.transform;
        }
    }
    private static int _DeformMatrixPropId = Shader.PropertyToID("_DeformMatrix");
    private static int _ShadowOffsetYPropId = Shader.PropertyToID("_ShadowOffsetY");
    private static int _DeformShadowColorPropId = Shader.PropertyToID("_DeformShadowColor");

    private List<RenderingInfo> renderInfos;
    private MaterialPropertyBlock propertyBlock;
    private Matrix4x4 shadowMatrix = Matrix4x4.identity;

    public float groundOffsetY = 0;
    public Color shadowColor = new Color(0, 0, 0, 0.5f);
    public Vector3 lightDir = Vector3.forward;
    public Vector3 groundNormalVector = Vector3.up;

    void SetToRenderer(Renderer targetRenderer)
    {
        var mat = targetRenderer.transform.localToWorldMatrix;
        mat.m13 -= groundOffsetY;

        var finalMat = this.shadowMatrix  * mat;
            

        propertyBlock.SetMatrix(_DeformMatrixPropId, finalMat);
        targetRenderer.SetPropertyBlock(propertyBlock);
    }

    private void SetupPropertyBlock()
    { 
        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        propertyBlock.SetFloat(_ShadowOffsetYPropId, groundOffsetY);
        propertyBlock.SetColor(_DeformShadowColorPropId, this.shadowColor);


    }

    private void SetupShadowMatrix()
    {
        Vector3 forward = lightDir;

        shadowMatrix.m00 = 1;
        shadowMatrix.m01 = forward.x;
        shadowMatrix.m02 = 0;
        shadowMatrix.m03 = 0;

        shadowMatrix.m10 = 0;
        shadowMatrix.m11 = 0;
        shadowMatrix.m12 = 0;
        shadowMatrix.m13 = 0;

        shadowMatrix.m20 = 0;
        shadowMatrix.m21 = forward.z;
        shadowMatrix.m22 = 1;
        shadowMatrix.m23 = 0;

        shadowMatrix.m30 = shadowMatrix.m31 = shadowMatrix.m32 = 0.0f;
        shadowMatrix.m33 = 1;

        var rot = Quaternion.FromToRotation(Vector3.up, this.groundNormalVector);
        var groundMatrix = Matrix4x4.Rotate(rot);

        shadowMatrix.m13 = groundOffsetY;

        shadowMatrix = groundMatrix * shadowMatrix;
    }



    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        var renderers = this.GetComponentsInChildren<Renderer>();
        if (renderers == null) { return; }
        if (this.renderInfos == null)
        {
            this.renderInfos = new List<RenderingInfo>();
        }
        else
        {
            this.renderInfos.Clear();
        }
        foreach (var r in renderers)
        {
            RenderingInfo info = new RenderingInfo(r);
            renderInfos.Add(info);
        }

    }


    // Update is called once per frame
    void LateUpdate()
    {
        if(renderInfos != null)
        {
            SetupPropertyBlock();
            SetupShadowMatrix();
            foreach ( var info in renderInfos)
            {
                SetToRenderer(info.targetRenderer);
            }
        }
#if UNITY_EDITOR
        else
        {
            Initialize();
        }
#endif
    }
}
