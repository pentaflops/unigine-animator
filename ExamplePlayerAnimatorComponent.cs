using PeAnim;
using PeAnim.States;
using Unigine;

namespace UnigineApp.data.csharp_template.FP_controller.components;

public class ExamplePlayerAnimatorComponent : Component
{
	public AnimationBlend2dState movementAnimation;
	public AnimationState startJumpAnimation;
	public AnimationState inAirAnimation;
	public AnimationState endJumpAnimation;

	private Animator animator;

	private void Init()
	{
		animator = new Animator(node, new IAnimationState[]
		{
			movementAnimation,
			startJumpAnimation,
			inAirAnimation,
			endJumpAnimation,
		});
	}

	private void Update()
	{
		animator.Update();
	}
}