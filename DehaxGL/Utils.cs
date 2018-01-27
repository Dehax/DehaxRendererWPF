using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DehaxGL
{
	public static class Utils
	{
		public static float DegreeToRadian(float angle)
		{
			return (float)Math.PI * angle / 180f;
		}

		public static float RadianToDegree(float angle)
		{
			return angle * (180f / (float)Math.PI);
		}
	}
}
