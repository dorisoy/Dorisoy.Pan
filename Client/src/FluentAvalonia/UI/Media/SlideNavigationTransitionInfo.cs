﻿using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;

namespace FluentAvalonia.UI.Media.Animation;

/// <summary>
/// Provides the parameters for a slide navigation transition.
/// </summary>
public class SlideNavigationTransitionInfo : NavigationTransitionInfo
{
    /// <summary>
    /// Gets or sets the type of animation effect to play during the slide transition.
    /// </summary>
    public SlideNavigationTransitionEffect Effect { get; set; } = SlideNavigationTransitionEffect.FromRight;

    /// <summary>
    /// Gets or sets the HorizontalOffset used when animating from the Left or Right
    /// </summary>
    public double FromHorizontalOffset { get; set; } = 56;

    /// <summary>
    /// Gets or sets the VerticalOffset used when animating from the Top or Bottom
    /// </summary>
    public double FromVerticalOffset { get; set; } = 56;

    public async override void RunAnimation(Animatable ctrl, CancellationToken cancellationToken)
    {
        double length = 0;
        bool isVertical = false;
        switch (Effect)
        {
            case SlideNavigationTransitionEffect.FromLeft:
                length = -FromHorizontalOffset;
                break;
            case SlideNavigationTransitionEffect.FromRight:
                length = FromHorizontalOffset;
                break;
            case SlideNavigationTransitionEffect.FromTop:
                length = -FromVerticalOffset;
                isVertical = true;
                break;
            case SlideNavigationTransitionEffect.FromBottom:
                length = FromVerticalOffset;
                isVertical = true;
                break;
        }

        var animation = new Avalonia.Animation.Animation
        {
            Easing = new SplineEasing(0.1, 0.9, 0.2, 1.0),
            Children =
            {
                new KeyFrame
                {
                    Setters =
                    {
                        new Setter(isVertical ? TranslateTransform.YProperty : TranslateTransform.XProperty, length),
                        new Setter(Visual.OpacityProperty, 0d)
                    },
                    Cue = new Cue(0d)
                },
                new KeyFrame
                {
                    Setters=
                    {
                        new Setter(Visual.OpacityProperty, 1d)
                    },
                    Cue = new Cue(0.05d)
                },
                new KeyFrame
                {
                    Setters =
                    {
                        new Setter(Visual.OpacityProperty, 1d),
                        new Setter(isVertical ? TranslateTransform.YProperty : TranslateTransform.XProperty, 0.0)
                    },
                    Cue = new Cue(1d)
                }
            },
            Duration = TimeSpan.FromSeconds(0.167),
            FillMode = FillMode.Forward
        };

        await animation.RunAsync(ctrl, cancellationToken);

        (ctrl as Visual).Opacity = 1;
    }
}

