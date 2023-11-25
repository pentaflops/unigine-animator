using Unigine;

namespace PeAnim.States;

public interface IAnimationState
{
	public int LayerIndex { get; protected set; }
	public float Weight { get; protected set; }

	public void Init(ObjectMeshSkinned meshSkinned);
	public void Update();
}