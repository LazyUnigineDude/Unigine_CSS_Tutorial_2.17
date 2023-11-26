//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Runtime.InteropServices;
//using System.Diagnostics;
//using Unigine;
//using ImGuiNET;
//using System.Numerics;

//public class ImGuiImpl
//{
//    static Texture font_texture;
//    static MeshDynamic imgui_mesh;
//    static Material imgui_material;
//    static ImDrawDataPtr frame_draw_data;

//    static IntPtr before_render_callback_handle;
//    static IntPtr draw_callback_handle;

//    static IntPtr key_pressed_handle;
//    static IntPtr key_released_handle;
//    static IntPtr button_pressed_handle;
//    static IntPtr button_released_handle;
//    static IntPtr unicode_key_pressed_handle;

//    struct StyleSizes
//    {
//        public Vector2 WindowPadding;
//        public float WindowRounding;
//        public Vector2 WindowMinSize;
//        public float ChildRounding;
//        public float PopupRounding;
//        public Vector2 FramePadding;
//        public float FrameRounding;
//        public Vector2 ItemSpacing;
//        public Vector2 ItemInnerSpacing;
//        public Vector2 CellPadding;
//        public Vector2 TouchExtraPadding;
//        public float IndentSpacing;
//        public float ColumnsMinSpacing;
//        public float ScrollbarSize;
//        public float ScrollbarRounding;
//        public float GrabMinSize;
//        public float GrabRounding;
//        public float LogSliderDeadzone;
//        public float TabRounding;
//        public float TabMinWidthForCloseButton;
//        public Vector2 DisplayWindowPadding;
//        public Vector2 DisplaySafeAreaPadding;
//        public float MouseCursorScale;
//    }

//    static StyleSizes sourceSizes = new StyleSizes();
//    static float lastScale = 1.0f;

//    public static void Init()
//    {
//        Input.MouseHandle = Input.MOUSE_HANDLE.SOFT;
//        ImGui.CreateContext();

//        key_pressed_handle = Input.AddCallback(Input.CALLBACK_INDEX.KEY_DOWN, on_key_pressed);
//        key_released_handle = Input.AddCallback(Input.CALLBACK_INDEX.KEY_UP, on_key_released);

//        button_pressed_handle = Input.AddCallback(Input.CALLBACK_INDEX.MOUSE_DOWN, on_button_pressed);
//        button_released_handle = Input.AddCallback(Input.CALLBACK_INDEX.MOUSE_UP, on_button_released);

//        unicode_key_pressed_handle = Input.AddCallback(Input.CALLBACK_INDEX.TEXT_PRESS, on_unicode_key_pressed);

//        before_render_callback_handle = Engine.AddCallback(Engine.CALLBACK_INDEX.BEGIN_RENDER, before_render_callback);

//        draw_callback_handle = Unigine.Render.AddCallback(Unigine.Render.CALLBACK_INDEX.END_SCREEN, draw_callback);
//        var io = ImGui.GetIO();
//        io.BackendFlags |= ImGuiBackendFlags.HasSetMousePos;
//        io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

//        io.KeyMap[(int)ImGuiKey.Tab] = (int)Input.KEY.TAB;
//        io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Input.KEY.LEFT;
//        io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Input.KEY.RIGHT;
//        io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Input.KEY.UP;
//        io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Input.KEY.DOWN;
//        io.KeyMap[(int)ImGuiKey.PageUp] = (int)Input.KEY.PGUP;
//        io.KeyMap[(int)ImGuiKey.PageDown] = (int)Input.KEY.PGDOWN;
//        io.KeyMap[(int)ImGuiKey.Home] = (int)Input.KEY.HOME;
//        io.KeyMap[(int)ImGuiKey.End] = (int)Input.KEY.END;
//        io.KeyMap[(int)ImGuiKey.Insert] = (int)Input.KEY.INSERT;
//        io.KeyMap[(int)ImGuiKey.Delete] = (int)Input.KEY.DELETE;
//        io.KeyMap[(int)ImGuiKey.Backspace] = (int)Input.KEY.BACKSPACE;
//        io.KeyMap[(int)ImGuiKey.Space] = (int)Input.KEY.SPACE;
//        io.KeyMap[(int)ImGuiKey.Enter] = (int)Input.KEY.ENTER;
//        io.KeyMap[(int)ImGuiKey.Escape] = (int)Input.KEY.ESC;
//        io.KeyMap[(int)ImGuiKey.KeyPadEnter] = (int)Input.KEY.ENTER;
//        io.KeyMap[(int)ImGuiKey.A] = (int)Input.KEY.A;
//        io.KeyMap[(int)ImGuiKey.C] = (int)Input.KEY.C;
//        io.KeyMap[(int)ImGuiKey.V] = (int)Input.KEY.V;
//        io.KeyMap[(int)ImGuiKey.X] = (int)Input.KEY.X;
//        io.KeyMap[(int)ImGuiKey.Y] = (int)Input.KEY.Y;
//        io.KeyMap[(int)ImGuiKey.Z] = (int)Input.KEY.Z;

//        io.ClipboardUserData = IntPtr.Zero;

//        create_font_texture();
//        create_imgui_mesh();
//        create_imgui_material();

//        ImGui.StyleColorsDark();

//        var style = ImGui.GetStyle();

//        sourceSizes.WindowPadding = style.WindowPadding;
//        sourceSizes.WindowRounding = style.WindowRounding;
//        sourceSizes.WindowMinSize = style.WindowMinSize;
//        sourceSizes.ChildRounding = style.ChildRounding;
//        sourceSizes.PopupRounding = style.PopupRounding;
//        sourceSizes.FramePadding = style.FramePadding;
//        sourceSizes.FrameRounding = style.FrameRounding;
//        sourceSizes.ItemSpacing = style.ItemSpacing;
//        sourceSizes.ItemInnerSpacing = style.ItemInnerSpacing;
//        sourceSizes.CellPadding = style.CellPadding;
//        sourceSizes.TouchExtraPadding = style.TouchExtraPadding;
//        sourceSizes.IndentSpacing = style.IndentSpacing;
//        sourceSizes.ColumnsMinSpacing = style.ColumnsMinSpacing;
//        sourceSizes.ScrollbarSize = style.ScrollbarSize;
//        sourceSizes.ScrollbarRounding = style.ScrollbarRounding;
//        sourceSizes.GrabMinSize = style.GrabMinSize;
//        sourceSizes.GrabRounding = style.GrabRounding;
//        sourceSizes.LogSliderDeadzone = style.LogSliderDeadzone;
//        sourceSizes.TabRounding = style.TabRounding;
//        sourceSizes.TabMinWidthForCloseButton = style.TabMinWidthForCloseButton;
//        sourceSizes.DisplayWindowPadding = style.DisplayWindowPadding;
//        sourceSizes.DisplaySafeAreaPadding = style.DisplaySafeAreaPadding;
//        sourceSizes.MouseCursorScale = style.MouseCursorScale;
//    }

//    public static void NewFrame()
//    {
//        EngineWindow main_window = WindowManager.MainWindow;
//        if (main_window == null)
//        {
//            Engine.Quit();
//            return;
//        }

//        var io = ImGui.GetIO();

//        ControlsApp.Enabled = !io.WantCaptureKeyboard;
//        if (io.WantCaptureKeyboard)
//        {
//            ControlsApp.MouseDX = 0;
//            ControlsApp.MouseDY = 0;
//        }

//        io.DisplaySize = new System.Numerics.Vector2(main_window.ClientRenderSize.x, main_window.ClientRenderSize.y);
//        io.DeltaTime = Engine.IFps;

//        io.KeyCtrl = Input.IsKeyPressed(Input.KEY.ANY_CTRL);
//        io.KeyShift = Input.IsKeyPressed(Input.KEY.ANY_SHIFT);
//        io.KeyAlt = Input.IsKeyPressed(Input.KEY.ANY_ALT);
//        io.KeySuper = Input.IsKeyPressed(Input.KEY.ANY_CMD);

//        if (io.WantSetMousePos)
//            Input.MousePosition = new ivec2((int)io.MousePos.X, (int)io.MousePos.Y);

//        io.MousePos = new System.Numerics.Vector2(Input.MousePosition.x - main_window.ClientPosition.x, Input.MousePosition.y - main_window.ClientPosition.y);
//        io.MouseWheel += Input.MouseWheel;
//        io.MouseWheelH += Input.MouseWheelHorizontal;

//        float scale = main_window.DpiScale;
//        if (MathLib.Equals(lastScale, scale) == false)
//        {
//            var style = ImGui.GetStyle();
//            lastScale = scale;

//            style.WindowPadding = sourceSizes.WindowPadding * scale;
//            style.WindowRounding = sourceSizes.WindowRounding * scale;
//            style.WindowMinSize = sourceSizes.WindowMinSize * scale;
//            style.ChildRounding = sourceSizes.ChildRounding * scale;
//            style.PopupRounding = sourceSizes.PopupRounding * scale;
//            style.FramePadding = sourceSizes.FramePadding * scale;
//            style.FrameRounding = sourceSizes.FrameRounding * scale;
//            style.ItemSpacing = sourceSizes.ItemSpacing * scale;
//            style.ItemInnerSpacing = sourceSizes.ItemInnerSpacing * scale;
//            style.CellPadding = sourceSizes.CellPadding * scale;
//            style.TouchExtraPadding = sourceSizes.TouchExtraPadding * scale;
//            style.IndentSpacing = sourceSizes.IndentSpacing * scale;
//            style.ColumnsMinSpacing = sourceSizes.ColumnsMinSpacing * scale;
//            style.ScrollbarSize = sourceSizes.ScrollbarSize * scale;
//            style.ScrollbarRounding = sourceSizes.ScrollbarRounding * scale;
//            style.GrabMinSize = sourceSizes.GrabMinSize * scale;
//            style.GrabRounding = sourceSizes.GrabRounding * scale;
//            style.LogSliderDeadzone = sourceSizes.LogSliderDeadzone * scale;
//            style.TabRounding = sourceSizes.TabRounding * scale;
//            style.TabMinWidthForCloseButton = sourceSizes.TabMinWidthForCloseButton * scale;
//            style.DisplayWindowPadding = sourceSizes.DisplayWindowPadding * scale;
//            style.DisplaySafeAreaPadding = sourceSizes.DisplaySafeAreaPadding * scale;
//            style.MouseCursorScale = sourceSizes.MouseCursorScale * scale;

//            io.FontGlobalScale = scale;
//        }

//        ImGui.NewFrame();
//    }

//    public static void Render()
//    {
//        ImGui.Render();
//        frame_draw_data = ImGui.GetDrawData();
//    }

//    public static void Shutdown()
//    {
//        imgui_material.DeleteLater();
//        imgui_mesh.DeleteLater();
//        font_texture.DeleteLater();

//        Engine.RemoveCallback(Engine.CALLBACK_INDEX.BEGIN_RENDER, before_render_callback_handle);
//        Unigine.Render.RemoveCallback(Unigine.Render.CALLBACK_INDEX.END_SCREEN, draw_callback_handle);

//        Input.RemoveCallback(Input.CALLBACK_INDEX.KEY_DOWN, key_pressed_handle);
//        Input.RemoveCallback(Input.CALLBACK_INDEX.KEY_UP, key_released_handle);

//        Input.RemoveCallback(Input.CALLBACK_INDEX.MOUSE_DOWN, button_pressed_handle);
//        Input.RemoveCallback(Input.CALLBACK_INDEX.MOUSE_UP, button_released_handle);

//        Input.RemoveCallback(Input.CALLBACK_INDEX.TEXT_PRESS, unicode_key_pressed_handle);

//        ImGui.DestroyContext();
//    }

//    static void on_key_pressed(Input.KEY key)
//    {
//        var io = ImGui.GetIO();
//        io.KeysDown[(int)key] = true;
//        //return io.WantCaptureKeyboard ? 1 : 0;
//    }

//    static void on_key_released(Input.KEY key)
//    {
//        var io = ImGui.GetIO();
//        io.KeysDown[(int)key] = false;
//        //return 0;
//    }

//    static void on_button_pressed(Input.MOUSE_BUTTON button)
//    {
//        var io = ImGui.GetIO();
//        switch (button)
//        {
//            case Input.MOUSE_BUTTON.LEFT:
//                io.MouseDown[0] = true;
//                break;
//            case Input.MOUSE_BUTTON.RIGHT:
//                io.MouseDown[1] = true;
//                break;
//            case Input.MOUSE_BUTTON.MIDDLE:
//                io.MouseDown[2] = true;
//                break;
//        }
//        //return 0;
//    }

//    static void on_button_released(Input.MOUSE_BUTTON button)
//    {
//        var io = ImGui.GetIO();
//        switch (button)
//        {
//            case Input.MOUSE_BUTTON.LEFT:
//                io.MouseDown[0] = false;
//                break;
//            case Input.MOUSE_BUTTON.RIGHT:
//                io.MouseDown[1] = false;
//                break;
//            case Input.MOUSE_BUTTON.MIDDLE:
//                io.MouseDown[2] = false;
//                break;
//        }
//        //return 0;
//    }

//    static void on_unicode_key_pressed(uint key)
//    {
//        var io = ImGui.GetIO();

//        io.AddInputCharacter(key);

//        //return 0;
//    }

//    static unsafe void create_font_texture()
//    {
//        var io = ImGui.GetIO();

//        io.Fonts.GetTexDataAsRGBA32(out byte* pixelData, out int width, out int height, out int bytesPerPixel);
//        var pixels = new byte[width * height * bytesPerPixel];
//        Marshal.Copy(new IntPtr(pixelData), pixels, 0, pixels.Length);

//        font_texture = new Texture();
//        font_texture.Create2D(width, height, Texture.FORMAT_RGBA8, Texture.SAMPLER_FILTER_LINEAR);

//        var blob = new Blob();
//        blob.SetData(pixels, Convert.ToUInt32(width) * Convert.ToUInt32(height) * 32);
//        font_texture.SetBlob(blob);
//        blob.SetData(null, 0);

//        io.Fonts.TexID = font_texture.GetPtr();
//    }

//    static unsafe void create_imgui_mesh()
//    {
//        imgui_mesh = new MeshDynamic(MeshDynamic.DYNAMIC_ALL);

//        MeshDynamic.Attribute[] attributes = new MeshDynamic.Attribute[3];
//        attributes[0].offset = 0;
//        attributes[0].size = 2;
//        attributes[0].type = MeshDynamic.TYPE_FLOAT;
//        attributes[1].offset = 8;
//        attributes[1].size = 2;
//        attributes[1].type = MeshDynamic.TYPE_FLOAT;
//        attributes[2].offset = 16;
//        attributes[2].size = 4;
//        attributes[2].type = MeshDynamic.TYPE_UCHAR;
//        imgui_mesh.SetVertexFormat(attributes);

//        Debug.Assert(imgui_mesh.GetVertexSize() == sizeof(ImDrawVert), "Vertex size of MeshDynamic is not equal to size of ImDrawVert");
//    }

//    static void create_imgui_material()
//    {
//        imgui_material = Materials.FindManualMaterial("imgui").Inherit();
//        imgui_material.SetTexture("imgui_texture", font_texture);
//    }

//    static void before_render_callback()
//    {
//        var io = ImGui.GetIO();
//        if (io.WantCaptureMouse)
//        {
//            Gui.GetCurrent().MouseButtons = 0;
//        }
//    }

//    static unsafe void draw_callback()
//    {
//        if (frame_draw_data.Equals(null))
//            return;

//        var draw_data = frame_draw_data;
//        if (draw_data.DisplaySize.X <= 0.0f || draw_data.DisplaySize.Y <= 0.0f)
//            return;

//        var render_target = Unigine.Render.GetTemporaryRenderTarget();
//        render_target.BindColorTexture(0, Renderer.TextureColor);

//        // Render state
//        RenderState.SaveState();
//        RenderState.ClearStates();
//        RenderState.SetBlendFunc(RenderState.BLEND_SRC_ALPHA, RenderState.BLEND_ONE_MINUS_SRC_ALPHA, RenderState.BLEND_OP_ADD);
//        RenderState.PolygonCull = RenderState.CULL_NONE;
//        RenderState.DepthFunc = RenderState.DEPTH_NONE;
//        RenderState.SetViewport((int)draw_data.DisplayPos.X, (int)draw_data.DisplayPos.Y, (int)draw_data.DisplaySize.X, (int)draw_data.DisplaySize.Y);

//        // Orthographic projection matrix
//        float left = draw_data.DisplayPos.X;
//        float right = draw_data.DisplayPos.X + draw_data.DisplaySize.X;
//        float top = draw_data.DisplayPos.Y;
//        float bottom = draw_data.DisplayPos.Y + draw_data.DisplaySize.Y;

//        mat4 proj = new mat4();
//        proj.m00 = 2.0f / (right - left);
//        proj.m03 = (right + left) / (left - right);
//        proj.m11 = 2.0f / (top - bottom);
//        proj.m13 = (top + bottom) / (bottom - top);
//        proj.m22 = 0.5f;
//        proj.m23 = 0.5f;
//        proj.m33 = 1.0f;

//        Renderer.Projection = proj;
//        var shader = imgui_material.FetchShader("imgui");
//        var pass = imgui_material.GetRenderPass("imgui");
//        Renderer.SetShaderParameters(pass, shader, imgui_material, false);

//        imgui_mesh.Bind();

//        // Write vertex and index data into dynamic mesh
//        imgui_mesh.ClearVertex();
//        imgui_mesh.ClearIndices();
//        imgui_mesh.AllocateVertex(draw_data.TotalVtxCount);
//        imgui_mesh.AllocateIndices(draw_data.TotalIdxCount);
//        for (int i = 0; i < draw_data.CmdListsCount; ++i)
//        {
//            ImDrawListPtr cmd_list = draw_data.CmdListsRange[i];

//            imgui_mesh.AddVertexArray(cmd_list.VtxBuffer.Data, cmd_list.VtxBuffer.Size);

//            // TODO:
//            // use the imgui_mesh.AddIndicesArray(cmd_list.IdxBuffer.Data, cmd_list.IdxBuffer.Size);
//            // instead of it:
//            for (int j = 0; j < cmd_list.IdxBuffer.Size; j++)
//                imgui_mesh.AddIndex(cmd_list.IdxBuffer[j]);
//            // We need to use it that way now because IdxBuffer uses 16-bit format indices, but MeshDynamic uses 32-bit
//        }
//        imgui_mesh.FlushVertex();
//        imgui_mesh.FlushIndices();

//        render_target.Enable();
//        {
//            int global_idx_offset = 0;
//            int global_vtx_offset = 0;
//            ivec2 clip_off = new ivec2((int)draw_data.DisplayPos.X, (int)draw_data.DisplayPos.Y);

//            // Draw command lists
//            for (int i = 0; i < draw_data.CmdListsCount; ++i)
//            {
//                ImDrawListPtr cmd_list = draw_data.CmdListsRange[i];
//                for (int j = 0; j < cmd_list.CmdBuffer.Size; ++j)
//                {
//                    ImDrawCmdPtr cmd = cmd_list.CmdBuffer[j];

//                    if (cmd.UserCallback == IntPtr.Zero)
//                    {
//                        float width = (cmd.ClipRect.Z - cmd.ClipRect.X) / draw_data.DisplaySize.X;
//                        float height = (cmd.ClipRect.W - cmd.ClipRect.Y) / draw_data.DisplaySize.Y;
//                        float x = (cmd.ClipRect.X - clip_off.x) / draw_data.DisplaySize.X;
//                        float y = 1.0f - height - (cmd.ClipRect.Y - clip_off.y) / draw_data.DisplaySize.Y;

//                        RenderState.SetScissorTest(x, y, width, height);
//                        RenderState.FlushStates();

//                        imgui_mesh.RenderInstancedSurface(MeshDynamic.MODE_TRIANGLES,
//                            (int)(cmd.VtxOffset + global_vtx_offset),
//                            (int)(cmd.IdxOffset + global_idx_offset),
//                            (int)(cmd.IdxOffset + global_idx_offset + cmd.ElemCount), 1);
//                    }
//                }
//                global_vtx_offset += cmd_list.VtxBuffer.Size;
//                global_idx_offset += cmd_list.IdxBuffer.Size;
//            }

//            RenderState.SetScissorTest(0.0f, 0.0f, 1.0f, 1.0f);
//        }
//        render_target.Disable();
//        imgui_mesh.Unbind();

//        RenderState.RestoreState();

//        render_target.UnbindColorTexture(0);
//        Unigine.Render.ReleaseTemporaryRenderTarget(render_target);
//    }
//}