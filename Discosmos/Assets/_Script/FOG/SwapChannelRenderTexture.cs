using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapChannelRenderTexture : MonoBehaviour
{
    private Camera cameraFog;
    private RenderTexture renderTexFog;
    // Start is called before the first frame update
    void Start()
    {
        cameraFog = GetComponent<Camera>();
        //ClearOutRenderTexture(cameraFog.targetTexture);
    }

    // Update is called once per frame
    void Update()
    {
        //ClearOutRenderTexture(cameraFog.targetTexture);
    }
    

    public void ClearOutRenderTexture(RenderTexture renderTexture)
    {
        RenderTexture rt = RenderTexture.active;
        RenderTexture.active = renderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = rt;
    }
}
