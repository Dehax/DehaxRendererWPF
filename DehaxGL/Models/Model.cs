using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DehaxGL.Models
{
	public class Model
	{
		private const int NUM_PARAMETERS = 12;

		public Mesh Mesh;
		public uint Color;

		private float[] _parameters = new float[NUM_PARAMETERS];

		private Vector3 _position;
		public Vector3 Position
		{
			get => _position;
			set
			{
				_position = value;
				_transformMatrix = Matrix4x4.CreateTranslation(value);
			}
		}

		private Vector3 _rotation;
		public Vector3 Rotation
		{
			get => _rotation;
			set
			{
				_rotation = value;
				_rotateMatrix = Matrix4x4.CreateRotationX(value.X) * Matrix4x4.CreateRotationY(value.Y) * Matrix4x4.CreateRotationZ(value.Z);
			}
		}

		private Vector3 _scale;
		public Vector3 Scale
		{
			get => _scale;
			set
			{
				_scale = value;
				_scaleMatrix = Matrix4x4.CreateScale(value);
			}
		}

		private Vector3 _pivot;
		public Vector3 Pivot
		{
			get => _pivot;
			set
			{
				_pivot = value;
			}
		}

		private Matrix4x4 _transformMatrix = Matrix4x4.Identity;
		private Matrix4x4 _rotateMatrix = Matrix4x4.Identity;
		private Matrix4x4 _scaleMatrix;
		private Matrix4x4 _pivotMatrix = Matrix4x4.Identity;
		private Matrix4x4 _pivotInverseMatrix = Matrix4x4.Identity;

		public Matrix4x4 WorldMatrix => _pivotMatrix * _rotateMatrix * _scaleMatrix * _pivotInverseMatrix * _transformMatrix;

		public string Name;

		public Model()
		{
			Scale = new Vector3(1f);
		}

		public Model(string name, Mesh mesh, uint color)
			: this()
		{
			Name = name;
			Mesh = mesh;
			Color = color;
		}

		public void SetParameters(float width, float length, float height, float radius, float lensWidth, float lensMountLength, float lensMountWidth, float marginWidth, float sideButtonsHeight, float shutterButtonHeight, float sideButtonsRadius, float shutterButtonRadius)
		{
			_parameters[0] = width;
			_parameters[1] = length;
			_parameters[2] = height;
			_parameters[3] = radius;
			_parameters[4] = lensWidth;
			_parameters[5] = lensMountLength;
			_parameters[6] = lensMountWidth;
			_parameters[7] = marginWidth;
			_parameters[8] = sideButtonsHeight;
			_parameters[9] = shutterButtonHeight;
			_parameters[10] = sideButtonsRadius;
			_parameters[11] = shutterButtonRadius;
		}
	}
}
