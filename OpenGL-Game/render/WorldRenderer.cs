﻿using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;
using GL = OpenTK.Graphics.OpenGL.GL;
using TextureTarget = OpenTK.Graphics.OpenGL.TextureTarget;
using TextureUnit = OpenTK.Graphics.OpenGL.TextureUnit;

namespace OpenGL_Game
{
    internal class WorldRenderer
    {
        private CubeOutlineModel selectionOutlineModel;

        private Vector4 selectionOutlineColor;

        private ModelLight modelLight;

        private Stopwatch updateTimer;

        private int hue;

        /// <summary>
        /// Render distance radius in chunks
        /// </summary>
        private int _renderDistance;

        public int RenderDistance
        {
            get => _renderDistance;
            set => _renderDistance = MathHelper.Clamp(value, 2, int.MaxValue);
        }

        public WorldRenderer()
        {
            modelLight = new ModelLight(new Vector3(-8, 12, -10f) * 750, Vector3.One);
            selectionOutlineModel = new CubeOutlineModel(new BlockShaderWireframe());

            RenderDistance = 5;

            updateTimer = Stopwatch.StartNew();
        }

        private void bindModel(IModel m)
        {
            GL.BindVertexArray(m.rawModel.vaoID);

            GL.EnableVertexAttribArray(0);

            if (m.rawModel.hasUVs)
                GL.EnableVertexAttribArray(1);
            if (m.rawModel.hasNormals)
                GL.EnableVertexAttribArray(2);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureManager.blockTextureAtlasID);
        }

        private void unbindModel(IModel m)
        {
            GL.DisableVertexAttribArray(0);

            if (m.rawModel.hasUVs)
                GL.DisableVertexAttribArray(1);
            if (m.rawModel.hasNormals)
                GL.EnableVertexAttribArray(2);

            GL.BindVertexArray(0);
        }

        public void render(Matrix4 viewMatrix)
        {
            if (Game.INSTANCE.world == null)
                return;

            var hit = Game.INSTANCE.mouseOverObject;

            if (hit.hit != null && hit.hit is EnumBlock block && block != EnumBlock.AIR)
                renderBlockSelectionOutline(viewMatrix, block, hit.blockPos);

            var nodes = Game.INSTANCE.world.getChunkDataNodes();

            for (var index = 0; index < nodes.Length; index++)
            {
                var node = nodes[index];

                if (node == null)
                    continue;

                var pos = node.chunk.chunkPos +
                          new BlockPos(8, 0, 8);

                var dist = MathUtil.distance(pos.vector.Xz, Game.INSTANCE.player.camera.pos.Xz);

                if (dist > _renderDistance * 16)
                    continue;

                var shaders = node.model.getShadersPresent();

                for (int j = 0; j < shaders.Count; j++)
                {
                    var shader = shaders[j];

                    var chunkFragmentModel = node.model.getFragmentModelWithShader(shader);

                    bindModel(chunkFragmentModel);

                    shader.start();

                    shader.loadLight(modelLight);
                    shader.loadViewMatrix(viewMatrix);

                    shader.loadTransformationMatrix(
                        MatrixHelper.createTransformationMatrix(node.chunk.chunkPos.vector));

                    GL.DrawArrays(shader.renderType, 0, chunkFragmentModel.rawModel.vertexCount);

                    unbindModel(chunkFragmentModel);
                    shader.stop();
                }
            }

            if (Game.INSTANCE.player != null)
            {
                renderSelectedBlock(Matrix4.Identity);
            }
        }

        private void renderBlockSelectionOutline(Matrix4 viewMatrix, EnumBlock block, BlockPos pos)
        {
            if (updateTimer.ElapsedMilliseconds >= 50)
            {
                hue = (hue + 5) % 365;

                selectionOutlineColor = getHue(hue);

                updateTimer.Restart();
            }

            var shader = (BlockShaderWireframe)selectionOutlineModel.shader;
            var bb = ModelManager.getModelForBlock(block, Game.INSTANCE.world.getMetadata(pos))?.boundingBox;

            if (bb == null)
                return;

            var size = bb.size + Vector3.One * 0.005f;

            bindModel(selectionOutlineModel);
            shader.start();

            shader.loadColor(selectionOutlineColor);
            shader.loadViewMatrix(viewMatrix);
            shader.loadTransformationMatrix(
                MatrixHelper.createTransformationMatrix(pos.vector - Vector3.One * 0.0025f, size));

            GL.Disable(EnableCap.CullFace);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            GL.DrawArrays(shader.renderType, 0, selectionOutlineModel.rawModel.vertexCount);

            GL.Enable(EnableCap.CullFace);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);

            unbindModel(selectionOutlineModel);

            shader.stop();
        }

        private void renderSelectedBlock(Matrix4 viewMatrix)
        {
            var stack = Game.INSTANCE.player.getEquippedItemStack();

            if (stack?.Item is ItemBlock itemBlock)
            {
                var model = ModelManager.getModelForBlock(itemBlock.getBlock(), stack.Meta);

                bindModel(model);
                model.shader.start();
                model.shader.loadLight(modelLight);
                model.shader.loadViewMatrix(Matrix4.Identity);

                model.shader.loadTransformationMatrix(MatrixHelper.createTransformationMatrix(
                    new Vector3(0.04f, -0.065f, -0.1f),
                    new Vector3(-2, -10, 0),
                    0.045f));

                GL.Enable(EnableCap.DepthClamp);
                GL.DrawArrays(model.shader.renderType, 0, model.rawModel.vertexCount);
                model.shader.stop();
                unbindModel(model);
            }
        }

        private Vector4 getHue(int hue)
        {
            var rads = MathHelper.DegreesToRadians(hue);

            var r = (float)(Math.Sin(rads) * 0.5 + 0.5);
            var g = (float)(Math.Sin(rads + MathHelper.PiOver3 * 2) * 0.5 + 0.5);
            var b = (float)(Math.Sin(rads + MathHelper.PiOver3 * 4) * 0.5 + 0.5);

            return Vector4.UnitX * r + Vector4.UnitY * g + Vector4.UnitZ * b + Vector4.UnitW;
        }
    }
}