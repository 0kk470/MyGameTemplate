using Saltyfish.Util;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Saltyfish.UI
{
    public enum UILayer
    {
        None = 0,
        Hud = 1,
        Lowest = 10,
        Bottom = 50,
        Middle = 100,
        Top = 500,
        Tip = 1000,
        Curtain = 9999,
    }

    class UIManager : ManagerBase<UIManager>
    {
        private Canvas m_UIRootCanvas;

        private RectTransform m_RootTransform;

        private EventSystem m_EventSystem;

        private Camera m_UICamera;

        private Dictionary<string, UIBase> m_UIdic = new Dictionary<string, UIBase>();

        private Dictionary<UILayer, Transform> m_layers = new Dictionary<UILayer, Transform>();


        private List<UIBase> uiToClose = new List<UIBase>();


        public Camera UICamera => m_UICamera;

        public Canvas UIRootCanvas => m_UIRootCanvas;

        public RectTransform UIRoot => m_RootTransform;

        public override void Init()
        {
            base.Init();
            m_UIRootCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            m_RootTransform = m_UIRootCanvas.GetComponent<RectTransform>();
            m_EventSystem = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<EventSystem>();
            m_UICamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
            InitLayer();

            DontDestroyOnLoad(m_UIRootCanvas.gameObject);
            DontDestroyOnLoad(m_EventSystem.gameObject);
            DontDestroyOnLoad(m_UICamera);
        }

        private void InitLayer()
        {
            m_layers.Clear();
            var layerNameList = Enum.GetNames(typeof(UILayer));
            for (int i = 0; i < layerNameList.Length; i++)
            {
                if (layerNameList[i] == "None")
                    continue;

                GameObject go = new GameObject();
                go.layer = LayerMask.NameToLayer("UI");
                go.transform.SetParent(UIRoot, false);
                go.name = layerNameList[i];

                var rect = go.AddComponent<RectTransform>();
                rect.sizeDelta = Vector2.zero;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;

                var _canvas = go.AddComponent<Canvas>();
                var uiLayer = (UILayer)Enum.Parse(typeof(UILayer), layerNameList[i]);
                _canvas.overrideSorting = true;
                _canvas.sortingOrder = (int)uiLayer;

                var gRaycaster = go.AddComponent<GraphicRaycaster>();
                gRaycaster.SetBlockingMask(LayerMask.GetMask("UI"));

                m_layers.Add(uiLayer, go.transform);
            }
        }

        public Transform GetLayer(UILayer layer)
        {
            m_layers.TryGetValue(layer, out Transform t);
            return t;
        }

        public UIBase OpenUI(string name)
        {
            Debug.Log("Open:" + name);
            UIBase ui;
            if (m_UIdic.TryGetValue(name, out ui))
            {
                ui.Show();
            }
            else
            {
                if (UIConfig.UIPath.TryGetValue(name, out UIConfig.UIConfigParams config))
                {
                    var go = Resources.Load<GameObject>(config.Path);
                    if (go != null)
                    {
                        Transform parent = null;
                        m_layers.TryGetValue(config.Layer, out parent);
                        var uiObj = GameObject.Instantiate(go, parent != null ? parent : m_UIRootCanvas.transform, false);
                        ui = uiObj.GetComponent<UIBase>();
                        ui.uiName = name;
                        ui.Layer = config.Layer;
                        m_UIdic.Add(name, ui);
                    }
                    ui.Show();
                }
                else
                {
                    Debug.LogError("No UI Config, UIName:" + name);
                }
            }
            return ui;
        }

        public UIBase GetUI(string name)
        {
            m_UIdic.TryGetValue(name, out UIBase ui);
            return ui;
        }

        public bool IsUIOpen(string name)
        {
            if (!m_UIdic.ContainsKey(name))
                return false;
            return m_UIdic[name] != null && m_UIdic[name].IsOpen;
        }

        public void HideUI(string name)
        {
            var ui = GetUI(name);
            if (ui != null)
            {
                ui.Hide();
            }
        }

        public void CloseUI(string name)
        {
            Debug.Log("Close:" + name);
            if (m_UIdic.ContainsKey(name))
            {
                var ui = m_UIdic[name];
                if (ui != null)
                    ui.Close();
            }
        }

        public void CloseAllUI()
        {
            foreach (var ui in m_UIdic.Values)
            {
                uiToClose.Add(ui);
            }
            m_UIdic.Clear();
            foreach (var ui in uiToClose)
            {
                ui.Close();
            }
            uiToClose.Clear();
        }

        public void CloseAllUI_Except(params string[] names)
        {
            foreach (var kv in m_UIdic)
            {
                if (ExistUIName(names, kv.Key))
                    continue;
                uiToClose.Add(kv.Value);
            }
            foreach (var ui in uiToClose)
            {
                ui.Close();
            }
            uiToClose.Clear();
        }

        private bool ExistUIName(string[] names, string uiName)
        {
            if (names == null || names.Length == 0)
                return false;
            return Array.Exists(names, (nameFind) => { return uiName == nameFind; });
        }

        public void RemoveUI(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;
            if (m_UIdic.ContainsKey(name))
            {
                m_UIdic.Remove(name);
            }
        }
    }
}