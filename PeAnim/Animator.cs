using PeAnim.States;
using Unigine;

namespace PeAnim;

public class Animator
{
	private bool isInitialized;
	private IAnimationState[] states;

	public Animator(Node node, IAnimationState[] animationStates)
	{
		var meshSkinned = node as ObjectMeshSkinned;
		if (!meshSkinned)
		{
			Log.Error("Animator: Can't cast node to ObjectMeshSkinned\n");
			return;
		}

		meshSkinned.RemoveLayer(0);
		meshSkinned.AddLayer();
		meshSkinned.SetLayer(0, true, 1f);

		states = animationStates;

		foreach (var animationState in states)
		{
			animationState.Init(meshSkinned);
		}

		isInitialized = true;
	}

	public void Update()
	{
		if (!isInitialized)
		{
			return;
		}

		foreach (var animationState in states)
		{
			animationState.Update();
		}
	}
}