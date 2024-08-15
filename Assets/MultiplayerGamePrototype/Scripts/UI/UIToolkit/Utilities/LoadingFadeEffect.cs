using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using Unity.Properties;


namespace MultiplayerGamePrototype.UIToolkit.Utilities
{
    internal enum FadeMode
    {
        None = 0,
        In = 1,
        Out = 2
    }

    [Serializable]
    public class FadeEffectData
    {
        [Range(0.1f, 2f)]
        public float AnimationTime;
        [CreateProperty]
        public List<TimeValue> AnimationTimeList => new List<TimeValue> { new TimeValue(AnimationTime) };

        //public Vector2 ScaleVector2;
        //public Scale ScaleValue => ScaleVector2;
    }


    [RequireComponent(typeof(UIDocument))]
    public class LoadingFadeEffect : MonoBehaviour
    {
        private const string k_FadeInClassName = "fade-effect__transition--fade-in";
        private const string k_FadeOutClassName = "fade-effect__transition--fade-out";
        private const string k_BackgroundElement = "element__background";

        public bool CAN_LOAD;       // boolean to determinate when the actual loading can happen

        //todo: add realtime bining for m_loadingStepTime and m_loadingStepValue
        /*
        [SerializeField]
        [Range(0f, 0.5f)]
        float m_loadingStepTime;            // A range of time to wait for every repetition on the effect
        [SerializeField]
        [Range(0f, 0.5f)]
        float m_loadingStepValue;           // The value to modify the alpha every steo time
        */
        [SerializeField]
        private FadeEffectData m_FadeEffectData;

        private VisualElement m_BackgroundElement;
        private bool m_IsLoop = false;
        private FadeMode m_FadeMode = FadeMode.None;



        private void OnEnable()
        {
            Debug.Log("LoadingFadeEffect-OnEnable");
            GetComponent<UIDocument>().rootVisualElement.dataSource = m_FadeEffectData;
            m_BackgroundElement = GetComponent<UIDocument>().rootVisualElement.Q(k_BackgroundElement);
            m_BackgroundElement.pickingMode = PickingMode.Ignore;


            //Debug.Log("TTT:" + nameof(m_BackgroundElement.style.width)+ ","+ nameof(m_BackgroundElement.style.scale) + "," + nameof(m_BackgroundElement.transform.scale));

            //m_BackgroundElement.SetBinding(nameof(m_BackgroundElement.style.width), new DataBinding // not working!
            //m_BackgroundElement.SetBinding("style.width", new DataBinding
            //{
            //    dataSourcePath = new PropertyPath(nameof(m_FadeEffectData.AnimationTime)),
            //    bindingMode = BindingMode.ToTarget
            //});

            //m_BackgroundElement.SetBinding(nameof(m_BackgroundElement.style.scale), new DataBinding// not working!
            //m_BackgroundElement.SetBinding("style.scale", new DataBinding//not working!
            //m_BackgroundElement.SetBinding("scale", new DataBinding//not working!
            //m_BackgroundElement.SetBinding("transform.scale", new DataBinding//not working!
            //m_BackgroundElement.SetBinding("style.transform.scale", new DataBinding//not working!

            //m_FadeEffectData.ScaleValue.value;

            //m_BackgroundElement.SetBinding("style.scale", new DataBinding
            //{
            //    dataSourcePath = new PropertyPath(nameof(m_FadeEffectData.ScaleValue)),
            //    bindingMode = BindingMode.ToTarget
            //});

            m_BackgroundElement.SetBinding("style.transitionDuration", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(m_FadeEffectData.AnimationTimeList)),
                bindingMode = BindingMode.ToTarget
            });
            
        }

        private void OnDisable()
        {
            if(m_BackgroundElement != null)
                m_BackgroundElement.UnregisterCallback<TransitionEndEvent>(OnTransitionEndBackgroundElement);
        }

        //todo:test-delete 
        private void Update()
        {
            return;

            if (Keyboard.current.iKey.wasPressedThisFrame)
                FadeIn();
            else if (Keyboard.current.oKey.wasPressedThisFrame)
                FadeOut();
            else if (Keyboard.current.aKey.wasPressedThisFrame)
                FadeAll();

            else if (Keyboard.current.bKey.wasPressedThisFrame)
            {
                m_BackgroundElement.style.scale = new Vector2(0.3f, 0.3f);
            }
        }


        // Start the fade-in effect
        public void FadeIn()
        {
            Debug.Log("LoadingFadeEffect-FadeIn");
            CAN_LOAD = false;
            AddTransitionEndEvent();
            m_FadeMode = FadeMode.In;
            m_BackgroundElement.pickingMode = PickingMode.Position;
            m_BackgroundElement.AddToClassList(k_FadeInClassName);
            m_BackgroundElement.RemoveFromClassList(k_FadeOutClassName);
        }

        // Start the fadeout effect
        public void FadeOut()
        {
            Debug.Log("LoadingFadeEffect-FadeOut");
            CAN_LOAD = false;
            m_FadeMode = FadeMode.Out;
            AddTransitionEndEvent();
            m_BackgroundElement.AddToClassList(k_FadeOutClassName);
            m_BackgroundElement.RemoveFromClassList(k_FadeInClassName);
        }

        private void AddTransitionEndEvent()
        {
            m_BackgroundElement.UnregisterCallback<TransitionEndEvent>(OnTransitionEndBackgroundElement);
            m_BackgroundElement.RegisterCallback<TransitionEndEvent>(OnTransitionEndBackgroundElement);
        }

        // Start a complete fade effect
        private void FadeAll()
        {
            Debug.Log("LoadingFadeEffect-FadeAll");
            m_IsLoop = true;
            FadeIn();
        }

        private void OnTransitionEndBackgroundElement(TransitionEndEvent evt)
        {
            bool isAffectsProperty = evt.AffectsProperty(new StylePropertyName("scale"));
            Debug.Log($"LoadingFadeEffect-OnTransitionEndBackgroundElement-isAffectsProperty:{isAffectsProperty}");
            if(!isAffectsProperty)
                return ;

            Debug.Log($"LoadingFadeEffect-OnTransitionEndBackgroundElement-elapsedTime:{evt.elapsedTime}, target:{evt.target}, target:{evt.currentTarget}, target:{evt}");
            Debug.Log($"LoadingFadeEffect-OnTransitionEndBackgroundElement-m_FadeMode:{m_FadeMode}, isLoop:{m_IsLoop}");

            if(m_FadeMode == FadeMode.In)
            {
                CAN_LOAD = true;

                if (m_IsLoop)
                {
                    m_IsLoop = false;
                    FadeOut();
                }
                else
                {
                    m_FadeMode = FadeMode.None;
                    m_BackgroundElement.pickingMode = PickingMode.Ignore;
                }

            }
            else if(m_FadeMode == FadeMode.Out)
            {
                m_FadeMode = FadeMode.None;
                m_BackgroundElement.pickingMode = PickingMode.Ignore;
            }
        }
    }
}