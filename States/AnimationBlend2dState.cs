using Unigine;

namespace PeAnim.States;

public class Animation2dState
{
	public vec2 point;

	[ParameterAsset(Filter = "anim")]
	public AssetLink asset;

	public float speed = 1f;

	[HideInEditor]
	public AnimationLayer layer;
}

public class AnimationBlend2dState : IAnimationState
{
	[ShowInEditor]
	private Animation2dState[] _states;

	private const float _blendTime = 0.25f;

	[HideInEditor] public bool Enabled;
	[HideInEditor] public vec2 CurrentPoint;

	private ObjectMeshSkinned meshSkinned;

	public int LayerIndex { get; set; }
	public float Weight { get; set; }

	private int baseLayerIndex;
	private int tempLayerIndex;

	public void Init(ObjectMeshSkinned objectMeshSkinned)
	{
		meshSkinned = objectMeshSkinned;

		LayerIndex = meshSkinned.AddLayer();
		Weight = 0f;

		baseLayerIndex = meshSkinned.AddLayer();
		tempLayerIndex = meshSkinned.AddLayer();

		foreach (var state in _states)
		{
			ref var animationLayer = ref state.layer;
			var animationId = meshSkinned.AddAnimation(state.asset.Path);
			var layerIndex = meshSkinned.AddLayer();
			meshSkinned.SetAnimation(layerIndex, animationId);
			meshSkinned.SetLayer(layerIndex, false, 0.0f);

			animationLayer.index = layerIndex;
			animationLayer.numFrames = meshSkinned.GetNumAnimationFrames(animationId);

			if (state.point == vec2.ZERO)
			{
				meshSkinned.SetAnimation(baseLayerIndex, animationId);
				meshSkinned.SetLayer(baseLayerIndex, false, 0.0f);
			}
		}
	}

	public void Update()
	{
		var weightDelta = Game.IFps / (Enabled ? _blendTime : -_blendTime);
		Weight = MathLib.Clamp(Weight + weightDelta, 0f, 1f);

		if (Weight == 0f)
			return;

		var totalWeight = 0f;
		var statesCount = _states.Length;
		for (int i = 0; i < statesCount; i++)
		{
			var stateFirst = _states[i];
			var pointStateFirst = stateFirst.point;
			var weight = 1f;

			for (int j = 0; j < statesCount; j++)
			{
				if (j == i)
					continue;

				var pointStateSecond = _states[j].point;

				var vecFirstToCurrent = CurrentPoint - pointStateFirst;
				var vecFirstToSecond = pointStateSecond - pointStateFirst;

				var newWeight = MathLib.Dot(vecFirstToCurrent, vecFirstToSecond) / MathLib.Dot(vecFirstToSecond, vecFirstToSecond);
				newWeight = MathLib.Clamp(1.0f - newWeight, 0.0f, 1.0f);

				weight = MathLib.Min(newWeight, weight);
			}

			stateFirst.layer.weight = weight;
			totalWeight += weight;
		}

		meshSkinned.SetFrame(baseLayerIndex, 0f);
		meshSkinned.CopyLayer(LayerIndex, baseLayerIndex);

		for (int i = 0; i < statesCount; i++)
		{
			var state = _states[i];
			ref var layer = ref state.layer;

			layer.weight = (layer.weight / totalWeight) * Weight;
			layer.enabled = layer.weight > 0f;

			if (layer.enabled)
			{
				layer.frame += Game.IFps * 30f * state.speed;
				meshSkinned.SetFrame(layer.index, layer.frame);

				meshSkinned.InverseLayer(tempLayerIndex, baseLayerIndex);
				meshSkinned.MulLayer(tempLayerIndex, tempLayerIndex, layer.index);

				meshSkinned.MulLayer(LayerIndex, LayerIndex, tempLayerIndex, layer.weight);
			}
			else
			{
				layer.frame = 0;
			}
		}

		meshSkinned.LerpLayer(0, 0, LayerIndex, Weight);
	}
}