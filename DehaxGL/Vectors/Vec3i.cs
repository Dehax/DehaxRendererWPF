using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DehaxGL.Vectors
{
	public struct Vec3i
	{
		public int X, Y, Z;

		public Vec3i(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public Vec3i(Vector3 v)
		{
			X = (int)(v.X + 0.5f);
			Y = (int)(v.Y + 0.5f);
			Z = (int)(v.Z + 0.5f);
		}

		public static Vec3i operator -(Vec3i a, Vec3i b)
		{
			return new Vec3i(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}
	}
}
