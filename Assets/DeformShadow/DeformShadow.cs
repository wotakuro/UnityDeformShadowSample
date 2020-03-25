using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DeformShadow : MonoBehaviour
{
    private struct RenderingInfo
    {
        public Renderer targetRenderer;
    }
    private static int _DeformMatrixPropId = Shader.PropertyToID("_DeformMatrix");
    private static int _ShadowOffsetYPropId = Shader.PropertyToID("_ShadowOffsetY");
    private static int _DeformShadowColorPropId = Shader.PropertyToID("_DeformShadowColor");

    private List<RenderingInfo> renderInfos;
    private MaterialPropertyBlock propertyBlock;
    public Matrix4x4 matrix = Matrix4x4.identity;

    public float groundOffsetY = 0;
    public Color shadowColor = new Color(0, 0, 0, 0.5f);
    public Vector3 lightDir = Vector3.forward;

    // todo
//    public Vector3 groundNormalVector = Vector3.zero;

    void SetToRenderer(Renderer targetRenderer)
    {
        targetRenderer.SetPropertyBlock(propertyBlock);
    }

    private void SetupPropertyBlock()
    { 

        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }



        Vector3 forward = lightDir;


        matrix.m00 = 1;
        matrix.m01 = forward.x;
        matrix.m02 = 0;

        matrix.m10 = matrix.m11 = matrix.m12 = 0.0f;

        matrix.m20 = 0;
        matrix.m21 = forward.z;
        matrix.m22 = 1;

        matrix.m13 = groundOffsetY;

        propertyBlock.SetMatrix(_DeformMatrixPropId, matrix);
        propertyBlock.SetFloat(_ShadowOffsetYPropId, groundOffsetY);
        propertyBlock.SetColor(_DeformShadowColorPropId, this.shadowColor);
    }

    /*
    private Matrix4x4 CreateGroundMatrix(Vector3 normal)
    {
        normal.Normalize();
        Vector3 forward = new Vector3(-normal.x,normal.z,normal.y);
        Vector3 crossForward = new Vector3(forward.z,forward.y,-forward.x);
        Matrix4x4 matrix = Matrix4x4.identity;
        
        matrix.m00 = crossForward.x;
        matrix.m01 = crossForward.y;
        matrix.m02 = crossForward.z;
        matrix.m10 = normal.x;
        matrix.m11 = normal.y;
        matrix.m12 = normal.z;
        matrix.m20 = forward.x;
        matrix.m21 = forward.y;
        matrix.m22 = forward.z;

        return matrix;
    }
    */


    private void Start()
    {
        SetupInfo();
    }

    private void SetupInfo()
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
            RenderingInfo info = new RenderingInfo();
            info.targetRenderer = r;
            renderInfos.Add(info);
        }

    }


    // Update is called once per frame
    void LateUpdate()
    {
        if(renderInfos != null)
        {
            SetupPropertyBlock();
            foreach ( var info in renderInfos)
            {
                SetToRenderer(info.targetRenderer);
            }
        }
#if UNITY_EDITOR
        else
        {
            SetupInfo();
        }
#endif
    }
}
