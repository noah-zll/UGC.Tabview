using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UGC.Tabview
{
    /// <summary>
    /// 用于为标签页切换添加动画效果的类
    /// </summary>
    [Serializable]
    public class TabViewAnimator
    {
        /// <summary>
        /// 动画类型
        /// </summary>
        public AnimationType AnimationType { get; private set; } = AnimationType.Fade;
        
        /// <summary>
        /// 动画持续时间
        /// </summary>
        public float Duration { get; private set; } = 0.3f;
        
        /// <summary>
        /// 设置页面切换动画
        /// </summary>
        /// <param name="animationType">动画类型</param>
        /// <param name="duration">动画持续时间</param>
        public void SetPageSwitchAnimation(AnimationType animationType, float duration)
        {
            AnimationType = animationType;
            Duration = Mathf.Max(0.1f, duration); // 确保动画持续时间不小于0.1秒
        }
        
        /// <summary>
        /// 播放页面进入动画
        /// </summary>
        /// <param name="page">页面对象</param>
        /// <param name="onComplete">动画完成回调</param>
        /// <returns>协程</returns>
        public IEnumerator PlayPageEnterAnimation(GameObject page, Action onComplete = null)
        {
            if (page == null) yield break;
            
            // 确保页面可见
            page.SetActive(true);
            
            switch (AnimationType)
            {
                case AnimationType.None:
                    // 无动画，直接显示
                    break;
                    
                case AnimationType.Fade:
                    yield return PlayFadeInAnimation(page);
                    break;
                    
                case AnimationType.Slide:
                    yield return PlaySlideInAnimation(page);
                    break;
                    
                case AnimationType.Scale:
                    yield return PlayScaleInAnimation(page);
                    break;
            }
            
            onComplete?.Invoke();
        }
        
        /// <summary>
        /// 播放页面退出动画
        /// </summary>
        /// <param name="page">页面对象</param>
        /// <param name="onComplete">动画完成回调</param>
        /// <returns>协程</returns>
        public IEnumerator PlayPageExitAnimation(GameObject page, Action onComplete = null)
        {
            if (page == null) yield break;
            
            switch (AnimationType)
            {
                case AnimationType.None:
                    // 无动画，直接隐藏
                    page.SetActive(false);
                    break;
                    
                case AnimationType.Fade:
                    yield return PlayFadeOutAnimation(page);
                    break;
                    
                case AnimationType.Slide:
                    yield return PlaySlideOutAnimation(page);
                    break;
                    
                case AnimationType.Scale:
                    yield return PlayScaleOutAnimation(page);
                    break;
            }
            
            // 确保页面隐藏
            page.SetActive(false);
            
            onComplete?.Invoke();
        }
        
        #region 淡入淡出动画
        
        private IEnumerator PlayFadeInAnimation(GameObject page)
        {
            CanvasGroup canvasGroup = page.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = page.AddComponent<CanvasGroup>();
            }
            
            canvasGroup.alpha = 0f;
            
            float elapsedTime = 0f;
            while (elapsedTime < Duration)
            {
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / Duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            canvasGroup.alpha = 1f;
        }
        
        private IEnumerator PlayFadeOutAnimation(GameObject page)
        {
            CanvasGroup canvasGroup = page.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = page.AddComponent<CanvasGroup>();
            }
            
            canvasGroup.alpha = 1f;
            
            float elapsedTime = 0f;
            while (elapsedTime < Duration)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / Duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            canvasGroup.alpha = 0f;
        }
        
        #endregion
        
        #region 滑动动画
        
        private IEnumerator PlaySlideInAnimation(GameObject page)
        {
            RectTransform rectTransform = page.GetComponent<RectTransform>();
            if (rectTransform == null) yield break;
            
            Vector2 endPosition = rectTransform.anchoredPosition;
            Vector2 startPosition = new Vector2(endPosition.x + 100f, endPosition.y);
            
            rectTransform.anchoredPosition = startPosition;
            
            float elapsedTime = 0f;
            while (elapsedTime < Duration)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, elapsedTime / Duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            rectTransform.anchoredPosition = endPosition;
        }
        
        private IEnumerator PlaySlideOutAnimation(GameObject page)
        {
            RectTransform rectTransform = page.GetComponent<RectTransform>();
            if (rectTransform == null) yield break;
            
            Vector2 startPosition = rectTransform.anchoredPosition;
            Vector2 endPosition = new Vector2(startPosition.x - 100f, startPosition.y);
            
            float elapsedTime = 0f;
            while (elapsedTime < Duration)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, elapsedTime / Duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            rectTransform.anchoredPosition = startPosition; // 重置位置，以便下次显示
        }
        
        #endregion
        
        #region 缩放动画
        
        private IEnumerator PlayScaleInAnimation(GameObject page)
        {
            RectTransform rectTransform = page.GetComponent<RectTransform>();
            if (rectTransform == null) yield break;
            
            Vector3 endScale = rectTransform.localScale;
            Vector3 startScale = Vector3.zero;
            
            rectTransform.localScale = startScale;
            
            float elapsedTime = 0f;
            while (elapsedTime < Duration)
            {
                rectTransform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / Duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            rectTransform.localScale = endScale;
        }
        
        private IEnumerator PlayScaleOutAnimation(GameObject page)
        {
            RectTransform rectTransform = page.GetComponent<RectTransform>();
            if (rectTransform == null) yield break;
            
            Vector3 startScale = rectTransform.localScale;
            Vector3 endScale = Vector3.zero;
            
            float elapsedTime = 0f;
            while (elapsedTime < Duration)
            {
                rectTransform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / Duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            rectTransform.localScale = startScale; // 重置缩放，以便下次显示
        }
        
        #endregion
    }
}