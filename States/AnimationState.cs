using Unigine;

namespace PeAnim.States;

public class AnimationState : IAnimationState
{
	[HideInEditor]
	private AnimationLayer _layer;

	[ParameterAsset(Filter = "anim")]
	public AssetLink asset;

	public float speed = 1f;

	public float blendTime = 0.25f;

	[HideInEditor] public bool Enabled;

	private ObjectMeshSkinned meshSkinned;

	public int LayerIndex { get; set; }
	public float Weight { get; set; }

	public float Progress => _layer.frame / _layer.numFrames;
	public bool IsFinished => _layer.frame > _layer.numFrames;
	public bool IsFinishedClampBlendTime => (_layer.frame) > _layer.numFrames - blendTime * 30f;

	public void Init(ObjectMeshSkinned objectMeshSkinned)
	{
		meshSkinned = objectMeshSkinned;

		var animationId = meshSkinned.AddAnimation(asset.Path);
		var layerIndex = meshSkinned.AddLayer();
		meshSkinned.SetAnimation(layerIndex, animationId);
		meshSkinned.SetLayer(layerIndex, false, 0.0f);

		_layer.index = layerIndex;
		_layer.numFrames = meshSkinned.GetNumAnimationFrames(animationId);

		LayerIndex = layerIndex;
		Weight = 0f;
	}

	public void Update()
	{
		var weightDelta = Game.IFps / (Enabled ? blendTime : -blendTime);
		Weight = _layer.weight = MathLib.Clamp(_layer.weight + weightDelta, 0f, 1f);

		_layer.enabled = _layer.weight > 0f;

		if (_layer.enabled)
		{
			_layer.frame += Game.IFps * 30f * speed;
			meshSkinned.SetFrame(_layer.index, _layer.frame);

			meshSkinned.LerpLayer(0, 0, _layer.index, _layer.weight);
		}
		else
		{
			_layer.frame = 0f;
		}
	}
}