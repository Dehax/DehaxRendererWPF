using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DehaxGL.Models
{
	public class Mesh
	{
		private List<Vector3> _vertices = new List<Vector3>();
		private List<Face> _faces = new List<Face>();
		public float MaxLocalScale;

		public int NumVertices => _vertices.Count;
		public int NumFaces => _faces.Count;

		public Vector3 GetVertex(int index) => _vertices[index];
		public Face GetFace(int index) => _faces[index];

		public void AddVertex(Vector3 vertex)
		{
			float x = vertex.X;
			float y = vertex.Y;
			float z = vertex.Z;

			if (x > MaxLocalScale)
			{
				MaxLocalScale = x;
			}

			if (y > MaxLocalScale)
			{
				MaxLocalScale = y;
			}

			if (z > MaxLocalScale)
			{
				MaxLocalScale = z;
			}

			_vertices.Add(vertex);
		}

		public void AddFace(Face face)
		{
			_faces.Add(face);
		}

		public void ClearVertices()
		{
			_vertices.Clear();
		}

		public void ClearFaces()
		{
			_faces.Clear();
		}
	}
}
