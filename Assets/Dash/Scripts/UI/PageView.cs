using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dash.Scripts.UI
{
    public class PageView : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        public RectTransform content;
        private int currentPageIndex = -1;
        private bool isDrag; //是否拖拽结束  
        public Action<int> OnPageChanged;
        private List<float> posList = new List<float>(); //求出每页的临界角，页索引从0开始  
        private ScrollRect rect; //滑动组件  
        public float sensitivity;
        public float smooting = 4; //滑动速度  

        private float startDragHorizontal;
        private float startTime;
        private bool stopMove = true;
        private float targethorizontal; //滑动的起始坐标  

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDrag = true;
            //开始拖动
            startDragHorizontal = rect.horizontalNormalizedPosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var posX = rect.horizontalNormalizedPosition;
            posX += (posX - startDragHorizontal) * sensitivity;
            posX = posX < 1 ? posX : 1;
            posX = posX > 0 ? posX : 0;
            var index = 0;

            var offset = Mathf.Abs(posList[index] - posX);
            //Debug.Log("offset " + offset);


            for (var i = 1; i < posList.Count; i++)
            {
                var temp = Mathf.Abs(posList[i] - posX);
                //Debug.Log("temp " + temp);
                //Debug.Log("i" + i);
                if (temp < offset)
                {
                    index = i;
                    offset = temp;
                }

                //Debug.Log("index " + index);
            }

            //Debug.Log(index);
            SetPageIndex(index);
            targethorizontal = posList[index]; //设置当前坐标，更新函数进行插值  
            isDrag = false;
            startTime = 0;
            stopMove = false;
        }

        private void Start()
        {
            rect = transform.GetComponent<ScrollRect>();
            var _rectWidth = GetComponent<RectTransform>();
            var tempWidth = content.transform.childCount * _rectWidth.rect.width;
            content.sizeDelta = new Vector2(tempWidth, _rectWidth.rect.height);
            //未显示的长度
            var horizontalLength = content.rect.width - _rectWidth.rect.width;
            for (var i = 0; i < rect.content.transform.childCount; i++)
                posList.Add(_rectWidth.rect.width * i / horizontalLength);
        }

        private void Update()
        {
            if (!isDrag && !stopMove)
            {
                startTime += Time.deltaTime;
                var t = startTime * smooting;
                rect.horizontalNormalizedPosition = Mathf.Lerp(rect.horizontalNormalizedPosition, targethorizontal, t);
                if (t >= 1)
                    stopMove = true;
            }

            //Debug.Log(rect.horizontalNormalizedPosition);
        }

        public void pageTo(int index)
        {
            if (index >= 0 && index < posList.Count)
            {
                rect.horizontalNormalizedPosition = posList[index];
                SetPageIndex(index);
            }
        }

        private void SetPageIndex(int index)
        {
            if (currentPageIndex != index)
            {
                currentPageIndex = index;
                if (OnPageChanged != null)
                    OnPageChanged(index);
            }
        }
    }
}