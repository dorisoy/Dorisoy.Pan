using Avalonia.ReactiveUI;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;
using Dorisoy.PanClient.ViewModels;

namespace Dorisoy.PanClient.Pages;

public partial class HomePage : ReactiveUserControl<HomePageViewModel>
{
    public HomePage()
    {
        InitializeComponent();
        this.WhenActivated(disposable => { });
    }

    private ImplicitAnimationCollection GetAnimations()
    {
        var compositor = ElementComposition.GetElementVisual(this).Compositor;

        var offsetAnimation = compositor.CreateVector3KeyFrameAnimation();
        offsetAnimation.Target = "Offset";
        offsetAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
        offsetAnimation.Duration = TimeSpan.FromMilliseconds(250);

        var rotationAnimation = compositor.CreateScalarKeyFrameAnimation();
        rotationAnimation.Target = "RotationAngle";
        rotationAnimation.InsertKeyFrame(.5f, 0f);
        rotationAnimation.InsertKeyFrame(1f, 0f);
        rotationAnimation.Duration = TimeSpan.FromMilliseconds(400);

        var animationGroup = compositor.CreateAnimationGroup();
        animationGroup.Add(offsetAnimation);
        animationGroup.Add(rotationAnimation);

        _animations = compositor.CreateImplicitAnimationCollection();
        _animations["Offset"] = animationGroup;

        return _animations;
    }

    private ImplicitAnimationCollection _animations;
}
