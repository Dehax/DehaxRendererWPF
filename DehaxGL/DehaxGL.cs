using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DehaxGL.Models;
using System.Numerics;
using DehaxGL.Vectors;

namespace DehaxGL
{
	public class DehaxGL
	{
		[Flags]
		public enum RenderMode
		{
			Wireframe = 1,
			Solid = 2,
			Both = Wireframe | Solid
		}

		private IViewport _viewport;
		public Scene Scene = new Scene();
		public Camera Camera = new Camera();
		private int[] _zBuffer;
		private bool _renderAxis = true;
		private int _width;
		private int _height;

		public DehaxGL(IViewport viewport)
		{
			_viewport = viewport;

			//SetViewportSize(_viewport.Width, _viewport.Height);
		}

		public void SetViewportSize(int width, int height)
		{
			_width = width;
			_height = height;

			Camera.Width = width;
			Camera.Height = height;
			_viewport.SetSize(width, height);
			_zBuffer = new int[width * height];
		}

		public unsafe void Render(RenderMode renderMode)
		{
			_viewport.Clear();

			int numObjects = Scene.NumObjects;
			int zBufferSize = _width * _height;

			fixed (int *zBufferPtr = &_zBuffer[0])
			{
				for (int i = 0; i < zBufferSize; i++)
				{
					*(zBufferPtr + i) = int.MinValue;
				}
			}

			for (int i = (_renderAxis ? 0 : 3); i < numObjects; i++)
			{
				RenderModel(Scene[i], renderMode);
			}
		}

		private void RenderModel(Model model, RenderMode renderMode)
		{
			Matrix4x4 worldMatrix = model.WorldMatrix;
			Matrix4x4 viewMatrix = Camera.ViewMatrix;
			Matrix4x4 projectionMatrix = Camera.ProjectionMatrix;

			Mesh mesh = model.Mesh;
			int numFaces = mesh.NumFaces;
			uint modelColor = model.Color;
			uint edgeColor = 0xFFFFFFFFu;

			Camera.ProjectionType projection = Camera.Projection;
			float viewDistance = Camera.ViewDistance;
			float zoom = Camera.Zoom;
			float fov = Camera.Fov;
			float nearZ = -Camera.NearPlaneZ;
			float farZ = -Camera.FarPlaneZ;

			float objectRadius = mesh.MaxLocalScale;
			Vector3 objectPosition = model.Position;
			Vector3 objectPositionView = Vector3.Transform(objectPosition, viewMatrix);
			float objectNearZ = objectPositionView.Z - objectRadius * model.Scale.Z;
			float objectFarZ = objectPositionView.Z + objectRadius * model.Scale.Z;
			float objectLeftX = objectPositionView.X - objectRadius * model.Scale.X;
			float objectRightX = objectPositionView.X + objectRadius * model.Scale.X;
			float objectBottomY = objectPositionView.Y - objectRadius * model.Scale.Y;
			float objectTopY = objectPositionView.Y + objectRadius * model.Scale.Y;

			float clipX = 0f;
			float clipY = 0f;

			if (objectNearZ >= nearZ || objectFarZ <= farZ)
				return;

			int width = _viewport.Width;
			int height = _viewport.Height;

			if (projection == Camera.ProjectionType.Parallel)
			{
				if (width > height)
				{
					clipX = 0.5f * zoom;
				}
				else
				{
					clipX = width * 0.5f * zoom / height;
				}
			}
			else if (projection == Camera.ProjectionType.Perspective)
			{
				//clipX = 0.5f * -width * objectPositionView.Z / viewDistance;
				clipX = (float)(Math.Tan(fov / 2.0f) * (width / (float)height) * -objectPositionView.Z);
			}

			if (objectLeftX < -clipX || objectRightX > clipX)
				return;

			if (projection == Camera.ProjectionType.Parallel)
			{
				if (width > height)
				{
					clipY = height * 0.5f * zoom / width;
				}
				else
				{
					clipY = 0.5f * zoom;
				}
			}
			else if (projection == Camera.ProjectionType.Perspective)
			{
				//clipY = 0.5f * height * objectPositionView.Z / viewDistance;
				clipY = (float)(Math.Tan(fov / 2.0f) * -objectPositionView.Z);
			}

			if (objectBottomY < -clipY || objectTopY > clipY)
				return;

			Face face;
			Vector3 local1;
			Vector3 local2;
			Vector3 local3;
			Vector3 world1;
			Vector3 world2;
			Vector3 world3;
			Vector3 view1;
			Vector3 view2;
			Vector3 view3;
			Vector4 result1;
			Vector4 result2;
			Vector4 result3;
			Vector3 hc1;
			Vector3 hc2;
			Vector3 hc3;
			Vector3 n;
			Vector3 lightDirection;
			bool backfaceCulling;

			for (int j = 0; j < numFaces; j++)
			{
				face = mesh.GetFace(j);

				local1 = mesh.GetVertex(face.v1);
				local2 = mesh.GetVertex(face.v2);
				local3 = mesh.GetVertex(face.v3);

				world1 = Vector3.Transform(local1, worldMatrix);
				world2 = Vector3.Transform(local2, worldMatrix);
				world3 = Vector3.Transform(local3, worldMatrix);

				n = Vector3.Cross(world3 - world1, world2 - world1);
				n = Vector3.Normalize(n);

				lightDirection = Camera.LookAt - Camera.Position;
				lightDirection = Vector3.Normalize(lightDirection);
				float intensity = -Vector3.Dot(n, lightDirection);

				if (intensity < 0.0f)
				{
					intensity = 0.0f;
				}

				uint faceColor = 0xFF000000u;
				faceColor |= (uint)(((modelColor & 0x0000FF00u) >> 8) * intensity) << 8;
				faceColor |= (uint)(((modelColor & 0x00FF0000u) >> 16) * intensity) << 16;
				faceColor |= (uint)(((modelColor & 0xFF000000u) >> 24) * intensity) << 24;

				lightDirection = (projection == Camera.ProjectionType.Parallel ? Camera.LookAt : world1) - Camera.Position;
				lightDirection = Vector3.Normalize(lightDirection);
				intensity = -Vector3.Dot(n, lightDirection);

				if (intensity <= 0.0f)
				{
					backfaceCulling = true;
				}
				else
				{
					backfaceCulling = false;
				}

				// Координаты вида
				view1 = Vector3.Transform(world1, viewMatrix);
				view2 = Vector3.Transform(world2, viewMatrix);
				view3 = Vector3.Transform(world3, viewMatrix);

				// Координаты проекции
				result1 = Vector4.Transform(view1, projectionMatrix);
				result2 = Vector4.Transform(view2, projectionMatrix);
				result3 = Vector4.Transform(view3, projectionMatrix);

				// Однородные итоговые координаты
				hc1 = new Vector3(result1.X / result1.W, result1.Y / result1.W, result1.Z / result1.W);
				hc2 = new Vector3(result2.X / result2.W, result2.Y / result2.W, result2.Z / result2.W);
				hc3 = new Vector3(result3.X / result3.W, result3.Y / result3.W, result3.Z / result3.W);

				DrawFace(hc1, hc2, hc3, faceColor, edgeColor, _zBuffer, renderMode, backfaceCulling);
			}
		}

		private void DrawFace(Vector3 v1, Vector3 v2, Vector3 v3, uint triangleColor, uint edgeColor, int[] zBuffer, RenderMode renderMode, bool backfaceCulling)
		{
			Vec3i s1 = CalculateScreenCoordinates(v1);
			Vec3i s2 = CalculateScreenCoordinates(v2);
			Vec3i s3 = CalculateScreenCoordinates(v3);

			if (!backfaceCulling && renderMode.HasFlag(RenderMode.Solid))
			{
				DrawTriangle(s1, s2, s3, triangleColor, ref zBuffer);
			}

			if (renderMode.HasFlag(RenderMode.Wireframe))
			{
				DrawLine(s1, s2, edgeColor);
				DrawLine(s2, s3, edgeColor);
				DrawLine(s3, s1, edgeColor);
			}
		}

		private unsafe void DrawTriangle(Vec3i t0, Vec3i t1, Vec3i t2, uint color, ref int[] zBuffer)
		{
			if (t0.Y == t1.Y && t0.Y == t2.Y)
				return;

			if (t0.Y > t1.Y)
			{
				Swap(ref t0, ref t1);
			}

			if (t0.Y > t2.Y)
			{
				Swap(ref t0, ref t2);
			}

			if (t1.Y > t2.Y)
			{
				Swap(ref t1, ref t2);
			}

			int total_height = t2.Y - t0.Y;

			fixed (int *zBufferPtr = &zBuffer[0])
			{
				for (int i = 0; i < total_height; i++)
				{
					bool second_half = i > t1.Y - t0.Y || t1.Y == t0.Y;
					int segment_height = second_half ? t2.Y - t1.Y : t1.Y - t0.Y;
					float alpha = i / (float)total_height;
					float beta = (i - (second_half ? t1.Y - t0.Y : 0)) / (float)segment_height;
					Vec3i t2t0 = (t2 - t0);
					Vec3i t2t1 = (t2 - t1);
					Vec3i t1t0 = (t1 - t0);
					Vec3i A = new Vec3i(new Vector3(t0.X, t0.Y, t0.Z) + new Vector3(t2t0.X, t2t0.Y, t2t0.Z) * alpha);
					Vec3i B = second_half ? new Vec3i(new Vector3(t1.X, t1.Y, t1.Z) + new Vector3(t2t1.X, t2t1.Y, t2t1.Z) * beta) : new Vec3i(new Vector3(t0.X, t0.Y, t0.Z) + new Vector3(t1t0.X, t1t0.Y, t1t0.Z) * beta);

					if (A.X > B.X)
					{
						Swap(ref A, ref B);
					}

					for (int j = A.X; j <= B.X; j++)
					{
						float phi = B.X == A.X ? 1f : (j - A.X) / (float)(B.X - A.X);
						Vec3i ba = (B - A);
						Vec3i P = new Vec3i(new Vector3(A.X, A.Y, A.Z) + new Vector3(ba.X, ba.Y, ba.Z) * phi);

						int idx = P.X + P.Y * _width;

						if (idx >= 0 && idx < _width * _height && *(zBufferPtr + idx) < P.Z)
						{
							*(zBufferPtr + idx) = P.Z;
							_viewport.SetPixel(P.X, P.Y, color);
						}
					}
				}
			}
		}

		private void DrawLine(Vec3i from, Vec3i to, uint color)
		{
			int x0 = from.X;
			int y0 = from.Y;
			int x1 = to.X;
			int y1 = to.Y;

			bool steep = false;

			if (Math.Abs(x0 - x1) < Math.Abs(y0 - y1))
			{
				Swap(ref x0, ref y0);
				Swap(ref x1, ref y1);
				steep = true;
			}

			if (x0 > x1)
			{
				Swap(ref x0, ref x1);
				Swap(ref y0, ref y1);
			}

			int dx = x1 - x0;
			int dy = y1 - y0;
			int derror2 = Math.Abs(dy) * 2;
			int error2 = 0;
			int y = y0;

			for (int x = x0; x <= x1; x++)
			{
				if (steep)
				{
					_viewport.SetPixel(y, x, color);
				}
				else
				{
					_viewport.SetPixel(x, y, color);
				}

				error2 += derror2;

				if (error2 > dx)
				{
					y += (y1 > y0 ? 1 : -1);
					error2 -= dx * 2;
				}
			}
		}

		private Vec3i CalculateScreenCoordinates(Vector3 v)
		{
			const int depth = int.MaxValue / 2;

			int x = (int)((v.X + 1.0f) * _width * 0.5f);
			int y = (int)((v.Y + 1.0f) * _height * 0.5f);
			int z = (int)((v.Z + 1.0f) * -depth);

			return new Vec3i(x, y, z);
		}

		private void Swap(ref int a, ref int b)
		{
			int tmp = b;
			b = a;
			a = tmp;
		}

		private void Swap(ref Vec3i a, ref Vec3i b)
		{
			Vec3i tmp = b;
			b = a;
			a = tmp;
		}
	}
}
