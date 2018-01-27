using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DehaxGL.Models
{
	public static class ModelsFactory
	{
		public static Model Box(float width, float length, float height)
		{
			width /= 2.0f;
			length /= 2.0f;
			height /= 2.0f;

			Mesh mesh = new Mesh();
			Model model = new Model("(G) box", mesh, 0xFFFF0000u);

			// +Z
			mesh.AddVertex(new Vector3(-length, -height, width));  // 0
			mesh.AddVertex(new Vector3(-length, height, width));   // 1
			mesh.AddVertex(new Vector3(length, height, width));    // 2
			mesh.AddVertex(new Vector3(length, -height, width));   // 3

			// -Z
			mesh.AddVertex(new Vector3(-length, -height, -width)); // 4
			mesh.AddVertex(new Vector3(-length, height, -width));  // 5
			mesh.AddVertex(new Vector3(length, height, -width));   // 6
			mesh.AddVertex(new Vector3(length, -height, -width));  // 7

			mesh.AddFace(new Face(0, 1, 3));
			mesh.AddFace(new Face(3, 1, 2));
			mesh.AddFace(new Face(3, 2, 7));
			mesh.AddFace(new Face(7, 2, 6));
			mesh.AddFace(new Face(7, 6, 4));
			mesh.AddFace(new Face(4, 6, 5));
			mesh.AddFace(new Face(4, 5, 0));
			mesh.AddFace(new Face(0, 5, 1));
			mesh.AddFace(new Face(1, 5, 2));
			mesh.AddFace(new Face(2, 5, 6));
			mesh.AddFace(new Face(3, 4, 0));
			mesh.AddFace(new Face(7, 4, 3));

			return model;
		}

		public static Model Cylinder(float radius, float height, int sides)
		{
			height /= 2.0f;

			Mesh mesh = new Mesh();
			Model model = new Model("(G) cylinder", mesh, 0xFF00FF00u);

			int nbVerticesCap = sides + 1;

			int verticesLength = nbVerticesCap + nbVerticesCap;// + sides * 2 + 2;

			Vector3[] vertices = new Vector3[verticesLength];
			int vert = 0;
			const float _2pi = (float)Math.PI * 2.0f;

			// Bottom cap
			// Центральная вершина нижней крышки
			vertices[vert++] = new Vector3(0.0f, -height, 0.0f);

			while (vert <= sides)
			{
				float rad = (float)vert / sides * _2pi;
				vertices[vert] = new Vector3((float)Math.Cos(rad) * radius, -height, (float)-Math.Sin(rad) * radius);
				vert++;
			}

			// Top cap
			vertices[vert++] = new Vector3(0.0f, height, 0.0f);

			while (vert <= sides * 2 + 1)
			{
				float rad = (float)(vert - sides - 1) / sides * _2pi;
				vertices[vert] = new Vector3((float)Math.Cos(rad) * radius, height, (float)-Math.Sin(rad) * radius);
				vert++;
			}

			// Triangles
			int nbTriangles = sides * 4;//sides * sides;// + sides * 2;
			int[] triangles = new int[nbTriangles * 3/* + 3*/];

			// Bottom cap
			int tri = 0;
			int i = 0;

			while (tri < sides - 1)
			{
				triangles[i] = 0;
				triangles[i + 1] = tri + 1;
				triangles[i + 2] = tri + 2;
				tri++;
				i += 3;
			}

			triangles[i] = 0;
			triangles[i + 1] = tri + 1;
			triangles[i + 2] = 1;
			tri++;
			i += 3;

			// Top cap
			tri++;

			while (tri < sides * 2)
			{
				triangles[i] = tri + 2;
				triangles[i + 1] = tri + 1;
				triangles[i + 2] = nbVerticesCap;
				tri++;
				i += 3;
			}

			triangles[i] = nbVerticesCap + 1;
			triangles[i + 1] = tri + 1;
			triangles[i + 2] = nbVerticesCap;
			tri++;
			i += 3;
			tri++;

			// Sides
			int j = tri;

			while (j <= nbTriangles - 2)
			{
				triangles[i] = tri - nbVerticesCap * 2 + 1;
				triangles[i + 1] = tri - nbVerticesCap + 1;
				triangles[i + 2] = tri - nbVerticesCap * 2 + 2;
				//tri++;
				j++;
				i += 3;

				triangles[i] = tri - nbVerticesCap * 2 + 2;
				triangles[i + 1] = tri - nbVerticesCap + 1;
				triangles[i + 2] = tri - nbVerticesCap + 2;
				tri++;
				j++;
				i += 3;
			}

			triangles[i] = tri - nbVerticesCap * 2 + 1;
			triangles[i + 1] = tri - nbVerticesCap + 1;
			triangles[i + 2] = 1;
			i += 3;

			triangles[i] = 1;
			triangles[i + 1] = tri - nbVerticesCap + 1;
			triangles[i + 2] = 1 + nbVerticesCap;

			for (i = 0; i < verticesLength; i++)
			{
				mesh.AddVertex(vertices[i]);
			}

			for (i = 0; i < nbTriangles * 3; i += 3)
			{
				mesh.AddFace(new Face(triangles[i], triangles[i + 1], triangles[i + 2]));
			}

			return model;
		}

		public static Model LensMount(float width, float frontLength, float backLength, float height)
		{
			width /= 2.0f;
			frontLength /= 2.0f;
			backLength /= 2.0f;
			height /= 2.0f;

			Mesh mesh = new Mesh();
			Model model = new Model("(G) lens mount", mesh, 0xFF0000FFu);

			// -Z
			mesh.AddVertex(new Vector3(-frontLength, -height, width)); // 0
			mesh.AddVertex(new Vector3(-frontLength, height, width));  // 1
			mesh.AddVertex(new Vector3(frontLength, height, width));   // 2
			mesh.AddVertex(new Vector3(frontLength, -height, width));  // 3

			// +Z
			mesh.AddVertex(new Vector3(-backLength, -height, -width));   // 4
			mesh.AddVertex(new Vector3(-backLength, height, -width));    // 5
			mesh.AddVertex(new Vector3(backLength, height, -width));     // 6
			mesh.AddVertex(new Vector3(backLength, -height, -width));    // 7

			mesh.AddFace(new Face(0, 1, 3));
			mesh.AddFace(new Face(3, 1, 2));
			mesh.AddFace(new Face(3, 2, 7));
			mesh.AddFace(new Face(7, 2, 6));
			mesh.AddFace(new Face(7, 6, 4));
			mesh.AddFace(new Face(4, 6, 5));
			mesh.AddFace(new Face(4, 5, 0));
			mesh.AddFace(new Face(0, 5, 1));
			mesh.AddFace(new Face(1, 5, 2));
			mesh.AddFace(new Face(2, 5, 6));
			mesh.AddFace(new Face(3, 4, 0));
			mesh.AddFace(new Face(7, 4, 3));

			return model;
		}

		public static Model Camera(float width = 40.0f, float length = 140.0f, float height = 70.0f, float radius = 25.0f, float lensWidth = 30.0f, float lensMountLength = 50.0f, float lensMountWidth = 10.0f, float marginWidth = 40.0f, float sideButtonsHeight = 10.0f, float shutterButtonHeight = 15.0f, float sideButtonsRadius = 15.0f, float shutterButtonRadius = 5.0f)
		{
			if (width <= 0.0f || 3.0f * width >= length || 1.5f * width >= height)
			{
				throw new Exception("Ширина должна быть больше 0.0, меньше 1/3 длины и меньше 2/3 высоты!");
			}

			if (length <= 0.0f)
			{
				throw new Exception("Длина должна быть больше 0.0!");
			}

			if (height <= 0.0f || height >= length)
			{
				throw new Exception("Высота должна быть больше 0.0 и меньше длины!");
			}

			if (radius <= 0.0f || radius > lensMountLength)
			{
				throw new Exception("Радиус объектива должен быть больше 0.0 и меньше либо равен длине крепления объектива!");
			}

			if (lensWidth <= 0.0f)
			{
				throw new Exception("Ширина объектива должна быть больше 0.0!");
			}

			if (lensMountLength <= 0.25f * length || lensMountLength >= 0.5f * length)
			{
				throw new Exception("Длина крепления объектива должна быть больше 1/4 и меньше 1/2 длины фотоаппарата!");
			}

			if (lensMountWidth <= 0.0f || lensMountWidth >= width)
			{
				throw new Exception("Ширина крепления объектива должна быть больше 0.0 и меньше ширины фотоаппарата!");
			}

			if (marginWidth <= 0.0f || 2.0f * marginWidth + lensMountLength >= length)
			{
				throw new Exception("Длина свободной части должна быть больше 0.0 и соответствовать длине крепления объектива!");
			}

			if (sideButtonsHeight <= 0.0f)
			{
				throw new Exception("Высота боковых кнопок должна быть больше 0.0!");
			}

			if (shutterButtonHeight <= 0.0f)
			{
				throw new Exception("Высота кнопки спуска затвора должна быть больше 0.0!");
			}

			if (sideButtonsRadius <= 0.0f || sideButtonsRadius >= width)
			{
				throw new Exception("Радиус боковых кнопок должен быть больше 0.0 и меньше ширины фотоаппарата!");
			}

			if (shutterButtonRadius <= 0.0f || 3.0f * shutterButtonRadius >= width)
			{
				throw new Exception("Радиус кнопки спуска затвора должен быть больше 0.0 и меньше 1/3 ширины фотоаппарата!");
			}

			Mesh mesh = new Mesh();
			Model result = new Model("Camera", mesh, 0xFFFF00FFu);
			result.SetParameters(width, length, height, radius, lensWidth, lensMountLength, lensMountWidth, marginWidth, sideButtonsHeight, shutterButtonHeight, sideButtonsRadius, shutterButtonRadius);

			Model basement = Box(width, length, height);

			Mesh currentMesh = basement.Mesh;
			int numVertices = currentMesh.NumVertices;
			int numFaces = currentMesh.NumFaces;
			int verticesOffset = 0;

			for (int i = 0; i < numVertices; i++)
			{
				mesh.AddVertex(currentMesh.GetVertex(i));
			}

			for (int i = 0; i < numFaces; i++)
			{
				Face face = currentMesh.GetFace(i);
				face.v1 += verticesOffset;
				face.v2 += verticesOffset;
				face.v3 += verticesOffset;
				mesh.AddFace(face);
			}

			verticesOffset += numVertices;

			Model lensMount = LensMount(lensMountWidth, lensMountLength, length - marginWidth * 2.0f, height);

			currentMesh = lensMount.Mesh;
			numVertices = currentMesh.NumVertices;
			numFaces = currentMesh.NumFaces;

			for (int i = 0; i < numVertices; i++)
			{
				Vector3 vertex = currentMesh.GetVertex(i);
				mesh.AddVertex(new Vector3(vertex.X, vertex.Y, vertex.Z - (width / 2.0f + lensMountWidth / 2.0f)));
			}

			for (int i = 0; i < numFaces; i++)
			{
				Face face = currentMesh.GetFace(i);
				face.v1 += verticesOffset;
				face.v2 += verticesOffset;
				face.v3 += verticesOffset;
				mesh.AddFace(face);
			}

			verticesOffset += numVertices;

			Model lens = Cylinder(radius, lensWidth, 12);

			currentMesh = lens.Mesh;
			numVertices = currentMesh.NumVertices;
			numFaces = currentMesh.NumFaces;

			for (int i = 0; i < numVertices; i++)
			{
				Vector3 vertex = currentMesh.GetVertex(i);
				Matrix4x4 rotationMatrix = Matrix4x4.CreateRotationX(Utils.DegreeToRadian(90.0f));
				Vector3 newPosition = Vector3.Transform(vertex, rotationMatrix);
				mesh.AddVertex(new Vector3(newPosition.X, newPosition.Y, newPosition.Z - (width / 2.0f + lensMountWidth + lensWidth / 2.0f)));
			}

			for (int i = 0; i < numFaces; i++)
			{
				Face face = currentMesh.GetFace(i);
				face.v1 += verticesOffset;
				face.v2 += verticesOffset;
				face.v3 += verticesOffset;
				mesh.AddFace(face);
			}

			verticesOffset += numVertices;

			Model rightSideButton = Cylinder(sideButtonsRadius, sideButtonsHeight, 12);

			currentMesh = rightSideButton.Mesh;
			numVertices = currentMesh.NumVertices;
			numFaces = currentMesh.NumFaces;

			for (int i = 0; i < numVertices; i++)
			{
				Vector3 vertex = currentMesh.GetVertex(i);
				mesh.AddVertex(new Vector3(vertex.X - length / 2.0f + marginWidth / 2.0f, vertex.Y + height / 2.0f + sideButtonsHeight / 2.0f, vertex.Z));
			}

			for (int i = 0; i < numFaces; i++)
			{
				Face face = currentMesh.GetFace(i);
				face.v1 += verticesOffset;
				face.v2 += verticesOffset;
				face.v3 += verticesOffset;
				mesh.AddFace(face);
			}

			verticesOffset += numVertices;

			Model leftSideButton = Cylinder(sideButtonsRadius, sideButtonsHeight, 12);

			currentMesh = leftSideButton.Mesh;
			numVertices = currentMesh.NumVertices;
			numFaces = currentMesh.NumFaces;

			for (int i = 0; i < numVertices; i++)
			{
				Vector3 vertex = currentMesh.GetVertex(i);
				mesh.AddVertex(new Vector3(vertex.X + length / 2.0f - marginWidth / 2.0f, vertex.Y + height / 2.0f + sideButtonsHeight / 2.0f, vertex.Z));
			}

			for (int i = 0; i < numFaces; i++)
			{
				Face face = currentMesh.GetFace(i);
				face.v1 += verticesOffset;
				face.v2 += verticesOffset;
				face.v3 += verticesOffset;
				mesh.AddFace(face);
			}

			verticesOffset += numVertices;

			Model centerButton = Cylinder(sideButtonsRadius, sideButtonsHeight / 2.0f, 12);

			currentMesh = centerButton.Mesh;
			numVertices = currentMesh.NumVertices;
			numFaces = currentMesh.NumFaces;

			for (int i = 0; i < numVertices; i++)
			{
				Vector3 vertex = currentMesh.GetVertex(i);
				mesh.AddVertex(new Vector3(vertex.X, vertex.Y + height / 2.0f + sideButtonsHeight / 4.0f, vertex.Z));
			}

			for (int i = 0; i < numFaces; i++)
			{
				Face face = currentMesh.GetFace(i);
				face.v1 += verticesOffset;
				face.v2 += verticesOffset;
				face.v3 += verticesOffset;
				mesh.AddFace(face);
			}

			verticesOffset += numVertices;

			Model shutterButton = Cylinder(shutterButtonRadius, shutterButtonHeight, 12);

			currentMesh = shutterButton.Mesh;
			numVertices = currentMesh.NumVertices;
			numFaces = currentMesh.NumFaces;

			for (int i = 0; i < numVertices; i++)
			{
				Vector3 vertex = currentMesh.GetVertex(i);
				mesh.AddVertex(new Vector3(vertex.X - length / 4.0f + marginWidth / 4.0f, vertex.Y + height / 2.0f + shutterButtonHeight / 2.0f, vertex.Z + width / 4.0f));
			}

			for (int i = 0; i < numFaces; i++)
			{
				Face face = currentMesh.GetFace(i);
				face.v1 += verticesOffset;
				face.v2 += verticesOffset;
				face.v3 += verticesOffset;
				mesh.AddFace(face);
			}

			verticesOffset += numVertices;

			return result;
		}

	}
}
