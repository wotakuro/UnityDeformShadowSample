using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShadowParameterUpdate : MonoBehaviour
{
    public DeformShadow deformShadow;
    public Light light;
    public Transform ground;

    public void Update()
    {
        deformShadow.lightDir = light.transform.forward;
        deformShadow.groundOffsetY = ground.position.y + 0.01f;

        deformShadow.groundNormalVector = ground.transform.up;
    }
}
