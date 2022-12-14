// MIT License

// Copyright (c) 2021 NedMakesGames

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlitMaterialFeature : ScriptableRendererFeature
{
    class RenderPass : ScriptableRenderPass
    {

        private string profilingName;
        private Material material;
        private int materialPassIndex;
        private RenderTargetIdentifier sourceID;
        private RenderTargetHandle tempTextureHandle;

        public RenderPass(string profilingName, Material material, int passIndex) : base()
        {
            this.profilingName = profilingName;
            this.material = material;
            this.materialPassIndex = passIndex;
            tempTextureHandle.Init("_TempBlitMaterialTexture");
        }

        public void SetSource(RenderTargetIdentifier source)
        {
            this.sourceID = source;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(profilingName);

            RenderTextureDescriptor cameraTextureDesc = renderingData.cameraData.cameraTargetDescriptor;
            cameraTextureDesc.depthBufferBits = 0;

            cmd.GetTemporaryRT(tempTextureHandle.id, cameraTextureDesc, FilterMode.Bilinear);
            Blit(cmd, sourceID, tempTextureHandle.Identifier(), material, materialPassIndex);
            Blit(cmd, tempTextureHandle.Identifier(), sourceID);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempTextureHandle.id);
        }
    }

    [System.Serializable]
    public class Settings
    {
        public Material material;
        public int materialPassIndex = -1; // -1 means render all passes
        public RenderPassEvent renderEvent = RenderPassEvent.AfterRenderingOpaques;
        public bool lightCam;
    }

    [SerializeField]
    private Settings settings = new Settings();

    private RenderPass renderPass;

    public Material Material
    {
        get => settings.material;
    }

    public override void Create()
    {
        this.renderPass = new RenderPass(name, settings.material, settings.materialPassIndex);
        renderPass.renderPassEvent = settings.renderEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        /*if (settings.manCameraOnly == false || renderingData.cameraData.camera.CompareTag("LightCamera")) 
        {
            renderPass.SetSource(renderer.cameraColorTarget);
            renderer.EnqueuePass(renderPass);
        }*/
        if (settings.lightCam == true && renderingData.cameraData.camera.CompareTag("DarkCamera"))
        {
            renderPass.SetSource(renderer.cameraColorTarget);
            renderer.EnqueuePass(renderPass);


        }
        if (!settings.lightCam == true && renderingData.cameraData.camera.CompareTag("LightCamera"))
        {
            renderPass.SetSource(renderer.cameraColorTarget);
            renderer.EnqueuePass(renderPass);


        }
    }
}