using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using System.IO;

public class FileExplorer : MonoBehaviour {

   private string LAST_PATH_KEY = "FileExplorer_Path";
   private string m_currentPath;

   // Controlled in editor
   public RectTransform rectTransform;
   public DraggableButton resizerButton;
   public InputField m_text;

   public SimpleLabel explorerLinkPrefab;
   public RectTransform mainPanel; // holds the explorer links

   private List<GameObject> m_explorerLinks = new List<GameObject>();
   public Color linkColor1;
   public Color linkColor2;



   void Start () {
      resizerButton.SubscribeToDrag(OnDragResizer);
      SetPath(GetStoredPath());
   }

   public void OnDragResizer(Vector2 delta) {
      Vector2 currentSize = rectTransform.sizeDelta;
      rectTransform.sizeDelta = new Vector2(currentSize.x + delta.x, currentSize.y - delta.y);
   }

   /**
    * Fired when the user clicks away from the path text field.
    */
   public void OnPathTextBlur(BaseEventData data) {
      Utils.Log("TODO: Create Buttons");
      SetPath(m_text.text);
   }

   /**
    * Fired when the user clicks into the text area to start editing.
    */
   public void OnPathTextFocus(BaseEventData data) {
      Utils.Log("TODO: Destroy Buttons");
   }

   /**
    * Fired when the user types into the text field to edit the path.
    */
   public void OnPathTextChanged() {
      SetPath(m_text.text);
   }

   private void SetPath(string path) {
      if (path == m_currentPath) {
         return;
      }
      m_currentPath = path;
      m_text.text = path;

      ClearLinks();

      string directoryName = Path.GetDirectoryName(m_currentPath);
      //string fileName = Path.GetFileName(m_currentPath); TODCO: Filtering

      string[] directories = Directory.GetDirectories(directoryName);
      AddLinks(directories, true);

      string[] files = Directory.GetFiles(directoryName);
      AddLinks(files, false);
   }

   private string GetStoredPath() {
      var lastPath = PlayerPrefs.GetString(LAST_PATH_KEY);
      if (lastPath == "") {
         lastPath = Path.GetFullPath(".") + "\\";
      }
      return lastPath;
   }

   private void StorePath() {
      PlayerPrefs.SetString(LAST_PATH_KEY, m_currentPath);
   }

   private void ClearLinks() {
      foreach (GameObject link in m_explorerLinks) {
         Destroy(link);
      }
      m_explorerLinks.Clear();
   }

   // private void ClickDirectory(string title) {

   // }

   private void AddLinks(string[] links, bool isDirectory) {
      foreach (string title in links) {
         SimpleLabel explorerLink = GameObject.Instantiate(explorerLinkPrefab);

         // Positioning
         RectTransform linkTransform = explorerLink.GetComponent<RectTransform>();
         linkTransform.SetParent(mainPanel);
         float height = linkTransform.rect.height;
         float top = height * m_explorerLinks.Count;
         linkTransform.sizeDelta = new Vector2(0, height);
         linkTransform.anchoredPosition = new Vector2(0, -top);

         // Update title
         explorerLink.labelText.text = Path.GetFileName(title);

         // Update color
         Button buttonComponent = explorerLink.GetComponent<Button>();
         ColorBlock colors = buttonComponent.colors;
         colors.normalColor = (m_explorerLinks.Count % 2 == 0) ? linkColor1 : linkColor2;
         buttonComponent.colors = colors;

         // Update action
         Button.ButtonClickedEvent btnClickedEvent = new Button.ButtonClickedEvent();
         //btnClickedEvent.AddListener(ClickDirectory(title));
         buttonComponent.onClick = btnClickedEvent;

         m_explorerLinks.Add(explorerLink.gameObject);
      }
   }
}
