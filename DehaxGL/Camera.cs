using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DehaxGL
{
	public class Camera
	{
		public enum ProjectionType
		{
			Parallel,
			Perspective
		}

		private const ProjectionType DEFAULT_PROJECTION = ProjectionType.Perspective;
		private readonly Vector3 DEFAULT_POSITION = new Vector3(0f, 0f, 200f);
		private readonly Vector3 DEFAULT_LOOK_AT = new Vector3(0f, 0f, 0f);
		private readonly Vector3 DEFAULT_UP = new Vector3(0f, 1f, 0f);
		private const float DEFAULT_FOV = (float)(Math.PI / 2);
		private const float MIN_FOV = 20f;
		private const float MAX_FOV = 150f;
		private const int DEFAULT_PARALLEL_ZOOM = 500;
		private const float DEFAULT_NEAR_Z = 1f;
		private const float DEFAULT_FAR_Z = 500f;

		public int Width;
		public int Height;
		public int Zoom;

		public ProjectionType Projection;
		public Vector3 Position;
		public Vector3 LookAt;
		public Vector3 Up;
		public float Fov;
		public float NearPlaneZ;
		public float FarPlaneZ;

		private float _theta;
		private float _phi;

		public Matrix4x4 ViewMatrix => Matrix4x4.CreateLookAt(Position, LookAt, Up);
		public Matrix4x4 ProjectionMatrix => Projection == ProjectionType.Parallel ? Matrix4x4.CreateOrthographic(Width, Height, NearPlaneZ, FarPlaneZ) : Matrix4x4.CreatePerspectiveFieldOfView(Fov, Width / (float)Height, NearPlaneZ, FarPlaneZ);
		public float ViewDistance => (float)(Width / (2 * Math.Tan(Fov / 2f)));

		public Camera()
		{
			Position = DEFAULT_POSITION;
			LookAt = DEFAULT_LOOK_AT;
			Up = DEFAULT_UP;
			Fov = DEFAULT_FOV;
			Projection = DEFAULT_PROJECTION;
			Zoom = DEFAULT_PARALLEL_ZOOM;
			NearPlaneZ = DEFAULT_NEAR_Z;
			FarPlaneZ = DEFAULT_FAR_Z;

			_theta = (float)(Math.PI / 2);
			_phi = (float)Math.PI;
		}

		public void Rotate(float angleX, float angleY)
		{
			Vector3 oldPosition = Position - LookAt;

			float r = oldPosition.Length();
			
			if (Utils.RadianToDegree(_theta) >= 5f && Utils.RadianToDegree(_theta) <= 175f)
			{
				_theta += angleX;
			}
			else
			{
				if (Utils.RadianToDegree(_theta) < 5f)
				{
					_theta = Utils.DegreeToRadian(5f);
				}
				else
				{
					_theta = Utils.DegreeToRadian(175f);
				}
			}

			_phi += angleY;

			Vector3 newPosition = new Vector3(r * (float)Math.Sin(_theta) * (float)Math.Sin(_phi), r * (float)Math.Cos(_theta), -r * (float)Math.Sin(_theta) * (float)Math.Cos(_phi));

			Position = newPosition + LookAt;
		}
	}
}
