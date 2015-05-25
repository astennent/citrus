using UnityEngine;
using System.Collections;

public class Tab : MonoBehaviour {

   public static float width = 70f;

   private string m_name;
   public UnityEngine.UI.Text text;

	public static Tab Instantiate(string name, Transform parent) {
      Tab tab = (Tab)GameObject.Instantiate(PanelManager.GetTabPrefab(),
            Vector3.zero, new Quaternion(0,0,0,0));
      tab.transform.SetParent(parent);
      tab.SetName(name);

      return tab;
   }

   public void SetName(string name) {
      m_name = name;
      text.text = name;
   }

   public string GetName() {
      return m_name;
   }
}
