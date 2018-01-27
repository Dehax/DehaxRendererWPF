using DehaxGL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DehaxGL
{
	public class Scene
	{
		private const float AXIS_SCALE = 100f;

		private List<Model> _objects = new List<Model>();

		public int NumObjects => _objects.Count;

		public Model this[int index] => _objects[index];

		public Scene()
		{
			CreateAxisModels();

			//Model box = ModelsFactory.Box(100f, 100f, 100f);
			//box.Position = new Vector3(0f, 0f, 100f);
			//box.Color = 0xFFFF0000u;
			//AddModel(box);

			//Model box2 = ModelsFactory.Box(100f, 100f, 100f);
			//box2.Position = new Vector3(0f, 0f, -100f);
			//box2.Color = 0xFFFF0000u;
			//AddModel(box2);

			AddModel(ModelsFactory.Camera());
		}

		public void MoveObject(int index, Vector3 offset)
		{
			Vector3 oldPosition = _objects[index].Position;
			Vector3 newPosition = oldPosition + offset;
			_objects[index].Position = newPosition;
		}

		public void RotateObject(int index, Vector3 rotation)
		{
			Vector3 oldRotation = _objects[index].Rotation;
			Vector3 newRotation = oldRotation + rotation;
			_objects[index].Rotation = newRotation;
		}

		public void ScaleObject(int index, Vector3 scale)
		{
			Vector3 oldScale = _objects[index].Scale;
			Vector3 newScale = new Vector3(oldScale.X * scale.X, oldScale.Y * scale.Y, oldScale.Z * scale.Z);
			_objects[index].Scale = newScale;
		}

		public void AddModel(Model model)
		{
			_objects.Add(model);
		}

		private void CreateAxisModels()
		{
			Model axisX = ModelsFactory.Box(0.01f * AXIS_SCALE, 1.0f * AXIS_SCALE, 0.01f * AXIS_SCALE);
			axisX.Position = new Vector3(0.5f * AXIS_SCALE, 0.0f, 0.0f);
			axisX.Color = 0xFFFF0000u;
			Model axisY = ModelsFactory.Box(0.01f * AXIS_SCALE, 0.01f * AXIS_SCALE, 1.0f * AXIS_SCALE);
			axisY.Position = new Vector3(0.0f, 0.5f * AXIS_SCALE, 0.0f);
			axisY.Color = 0xFF00FF00u;
			Model axisZ = ModelsFactory.Box(1.0f * AXIS_SCALE, 0.01f * AXIS_SCALE, 0.01f * AXIS_SCALE);
			axisZ.Position = new Vector3(0.0f, 0.0f, 0.5f * AXIS_SCALE);
			axisZ.Color = 0xFF0000FFu;

			AddModel(axisX);
			AddModel(axisY);
			AddModel(axisZ);
		}
	}
}
