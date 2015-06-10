using UnityEngine;
using System.Collections;

public class GUISchemeManager {

   // Tabs
   public static Color activeTabNormal = new Color(.3f, .3f, .3f);
   public static Color inactiveTabNormal = new Color(.4f, .4f, .4f);
   public static Color activeTabHighlighted = activeTabNormal;
   public static Color inactiveTabHighlighted = new Color(.5f, .5f, .5f);
   public static Color pressedTab = activeTabNormal;

   // Caption Bar 
   public static Color inactiveCaption = inactiveTabNormal;
   public static Color activeCaption = new Color(.45f, .45f, .45f);

   // Client Area
   public static Color clientBackground = activeTabNormal;

}
