using UnityEngine;
using System.Collections.Generic;

namespace Uzu
{
    public class UiPanelMgr : MonoBehaviour
    {
        /// <summary>
        /// Changes the current panel to the specified panel id.
        /// </summary>
        public void ChangeCurrentPanel (string panelId)
        {
            UiPanel panel;
            if (_uiPanelDataHolder.TryGetValue (panelId, out panel)) {
                UiPanel prevPanel = _currentPanel;

                if (prevPanel != null) {
                    prevPanel.Deactivate ();
                }

                _currentPanel = panel;
                _currentPanelId = panelId;
                _currentPanel.Activate ();
            } else {
                Debug.LogError ("Unable to activate a panel that is not registered: " + panelId);
            }
        }

        /// <summary>
        /// Gets the currently active panel id.
        /// </summary>
        public string CurrentPanelId {
            get { return _currentPanelId; }
        }

        /// <summary>
        /// Gets the currently active panel.
        /// </summary>
        public UiPanel CurrentPanel {
            get { return _currentPanel; }
        }

        #region Implementation.
        private Dictionary<string, UiPanel> _uiPanelDataHolder = new Dictionary<string, UiPanel> ();
        private UiPanel _currentPanel;
        private string _currentPanelId;

        private void RegisterPanel (string name, UiPanel panel)
        {
    #if UNITY_EDITOR
            if (_uiPanelDataHolder.ContainsKey(name)) {
                Debug.LogWarning("Panel with name [" + name + "] already exists.");
            }
    #endif // UNITY_EDITOR

            _uiPanelDataHolder [name] = panel;

            // Initialize the panel.
            panel.Initialize (this);
        }

        protected void Awake ()
        {
            // Register all child panels.
            MonoBehaviour[] panels = this.gameObject.GetComponentsInChildren<MonoBehaviour> (true);
            for (int i = 0; i < panels.Length; i++) {
                UiPanel panel = panels [i] as UiPanel;
                if (panel != null) {
                    RegisterPanel (panel.gameObject.name, panel);
                }
            }
        }
        #endregion
    }
}