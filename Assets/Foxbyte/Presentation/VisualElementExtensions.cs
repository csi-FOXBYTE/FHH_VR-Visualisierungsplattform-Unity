using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Foxbyte.Presentation.Extensions
{
    public static class VisualElementExtensions
    {
        /// <summary>
        /// Creates a child VisualElement and adds it to the parent VisualElement.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="classes"></param>
        /// <returns></returns>
        public static VisualElement CreateChild(this VisualElement parent, params string[] classes)
        {
            var child = new VisualElement();
            child.AddClass(classes).AddTo(parent);
            return child;
        }

        /// <summary>
        /// Creates a child VisualElement of the specified type and adds it to the parent VisualElement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="classes"></param>
        /// <returns></returns>
        public static T CreateChild<T>(this VisualElement parent, params string[] classes)
            where T : VisualElement, new()
        {
            var child = new T();
            child.AddClass(classes).AddTo(parent);
            return child;
        }

        /// <summary>
        /// Adds a VisualElement to a parent VisualElement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static T AddTo<T>(this T child, VisualElement parent) where T : VisualElement
        {
            parent.Add(child);
            return child;
        }

        /// <summary>
        /// Adds a uss class to the VisualElement for styling.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="visualElement"></param>
        /// <param name="classes"></param>
        /// <returns></returns>
        public static T AddClass<T>(this T visualElement, params string[] classes) where T : VisualElement
        {
            foreach (string cls in classes)
            {
                if (!string.IsNullOrEmpty(cls))
                {
                    visualElement.AddToClassList(cls);
                }
            }

            return visualElement;
        }

        /// <summary>
        /// Adds a manipulator to the VisualElement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="visualElement"></param>
        /// <param name="manipulator"></param>
        /// <returns></returns>
        public static T WithManipulator<T>(this T visualElement, IManipulator manipulator) where T : VisualElement
        {
            visualElement.AddManipulator(manipulator);
            return visualElement;
        }

        /// <summary>
        /// Checks if the current VisualElement is a descendant of the specified potential ancestor.
        /// </summary>
        /// <param name="element">The VisualElement to check.</param>
        /// <param name="potentialAncestor">The potential ancestor VisualElement.</param>
        /// <returns>True if the element is a descendant of potentialAncestor, otherwise false.</returns>
        public static bool IsDescendantOf(this VisualElement element, VisualElement potentialAncestor)
        {
            while (element != null)
            {
                if (element == potentialAncestor)
                {
                    return true;
                }
                element = element.parent;
            }
            return false;
        }

        // several methods for setting style properties

        public static void SetMargin(this IStyle style, float margin) => style.marginBottom = style.marginLeft = style.marginRight = style.marginTop = margin;
        public static void SetMarginVertical(this IStyle style, float margin) => style.marginLeft = style.marginRight = margin;
        public static void SetMarginHorizontal(this IStyle style, float margin) => style.marginLeft = style.marginRight = margin;
        public static void SetPadding(this IStyle style, float padding) => style.paddingBottom = style.paddingLeft = style.paddingRight = style.paddingTop = padding;
        public static void SetPaddingVertical(this IStyle style, float padding) => style.paddingBottom = style.paddingTop = padding;
        public static void SetPaddingHorizontal(this IStyle style, float padding) => style.paddingLeft = style.paddingRight = padding;
        public static void SetBorderWidth(this IStyle style, float width) => style.borderBottomWidth = style.borderLeftWidth = style.borderRightWidth = style.borderTopWidth = width;
        public static void SetBorderColor(this IStyle style, Color color) => style.borderBottomColor = style.borderLeftColor = style.borderRightColor = style.borderTopColor = new StyleColor(color);
        public static void SetBorderRadius(this IStyle style, float radius) => style.borderBottomLeftRadius = style.borderBottomRightRadius = style.borderTopLeftRadius = style.borderTopRightRadius = radius;
        public static void SetSideDistances(this IStyle style, float position) => style.left = style.right = style.top = style.bottom = position;
     
        
        // Animation

        /// <summary>
        /// Fades a VisualElement to a specified target opacity over a given duration.
        /// </summary>
        /// <param name="element">The VisualElement to fade.</param>
        /// <param name="targetOpacity">The target opacity value (0 for fade out, 1 for fade in).</param>
        /// <param name="duration">Duration in seconds over which the fade occurs.</param>
        /// <returns>A UniTask that completes when the fade operation completes.</returns>
        public static async UniTask FadeToAsync(this VisualElement element, float targetOpacity, float duration)
        {
            float startOpacity = element.resolvedStyle.opacity;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float newOpacity = Mathf.Lerp(startOpacity, targetOpacity, elapsedTime / duration);
                element.style.opacity = newOpacity;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
            element.style.opacity = targetOpacity; // Ensure exact target opacity at the end
        }

        /// <summary>
        /// Moves a VisualElement in or out of the screen with cubic easing in a specified direction.
        /// </summary>
        /// <param name="element">The element to animate.</param>
        /// <param name="moveIn">True to move element in, false to move out.</param>
        /// <param name="direction">The direction from which to move the element.</param>
        /// <param name="duration">Animation duration in seconds.</param>
        /// <returns>A UniTask that completes when the animation is done.</returns>
        public static async UniTask MoveElementAsync(this VisualElement element, bool moveIn, MoveDirection direction, float duration)
        {
            var parent = element.parent;
            float screenWidth = parent.resolvedStyle.width;
            float screenHeight = parent.resolvedStyle.height;

            Vector3 startPosition = Vector3.zero;
            Vector3 endPosition = Vector3.zero;

            switch (direction)
            {
                case MoveDirection.Left:
                    startPosition = moveIn ? new Vector3(-screenWidth, 0, 0) : element.resolvedStyle.translate; //element.transform.position;
                    endPosition = moveIn ? Vector3.zero : new Vector3(-screenWidth, 0, 0);
                    break;
                case MoveDirection.Right:
                    startPosition = moveIn ? new Vector3(screenWidth, 0, 0) : element.resolvedStyle.translate;
                    endPosition = moveIn ? Vector3.zero : new Vector3(screenWidth, 0, 0);
                    break;
                case MoveDirection.Top:
                    startPosition = moveIn ? new Vector3(0, -screenHeight, 0) : element.resolvedStyle.translate;
                    endPosition = moveIn ? Vector3.zero : new Vector3(0, -screenHeight, 0);
                    break;
                case MoveDirection.Bottom:
                    startPosition = moveIn ? new Vector3(0, screenHeight, 0) : element.resolvedStyle.translate;
                    endPosition = moveIn ? Vector3.zero : new Vector3(0, screenHeight, 0);
                    break;
            }

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / duration;
                float easedProgress = moveIn ? EaseOutCubic(progress) : EaseInCubic(progress);
                Vector3 position = Vector3.Lerp(startPosition, endPosition, easedProgress);
                element.style.translate = position;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
            element.style.translate = endPosition;
        }

        private static float EaseInCubic(float x)
        {
            return x * x * x;
        }

        private static float EaseOutCubic(float x)
        {
            return 1 - Mathf.Pow(1 - x, 3);
        }

        public enum MoveDirection
        {
            Left,
            Right,
            Top,
            Bottom
        }

        
        // variations to test

        public static void Fade(this VisualElement element, bool on, Action onComplete = null, float duration = 0.2f)
        {
            var tween = element.experimental.animation.Start(on ? 0 : 1, on ? 1 : 0, (int)(duration * 1000), (e, v) => e.style.opacity = new StyleFloat(v));
            if (onComplete != null) tween.OnCompleted(onComplete);
        }

        public static void AnimateIn(this VisualElement newElement, Vector2 dir, float duration = 1f)
        {
            var targetInWorldSpace = newElement.parent.LocalToWorld(newElement.layout.position);
            var goalPosition = newElement.parent.parent.WorldToLocal(targetInWorldSpace);

            newElement.experimental.animation.Position(goalPosition, (int)(duration * 1000)).from = goalPosition + dir.normalized * 1000;
        }
    }
}