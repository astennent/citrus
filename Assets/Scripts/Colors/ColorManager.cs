using UnityEngine;

public class ColorManager {

   public enum Theme {
      BRIGHT,
      PASTEL
   }

   public static Color GenColor(Theme theme) {

      switch(theme) {
         case Theme.BRIGHT:
            return GenBright();
         case Theme.PASTEL:
            return GenPastel();
      }

      return Color.magenta;

   }

   private static Color GenBright() {
      Color baseColor = new Color(Random.value, Random.value, Random.value);

      int zeroChannel = (int)(Random.value * 3);
      int oneChannel = zeroChannel;
      while (oneChannel == zeroChannel) {
         oneChannel = (int)(Random.value * 3);
      }

      for (int channelIndex = 0 ; channelIndex < 3 ; channelIndex++) {
         if (channelIndex == zeroChannel) {
            baseColor[channelIndex] = 0;
         }
         else if (channelIndex == oneChannel) {
            baseColor[channelIndex] = 1;
         }
      }

      return baseColor;
   }

   private static Color GenPastel() {
      return Color.magenta;
   }



}