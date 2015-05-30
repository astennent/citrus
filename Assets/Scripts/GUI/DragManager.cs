class DragManager {
   private static bool s_isDragging = false;
   private static DragTarget s_dragTarget;

   public static DragTarget GetCurrentTarget() {
      return s_dragTarget;
   }

   public static void SetDragTarget(DragTarget dragTarget) {
      s_dragTarget = dragTarget;
   }

   public static bool IsDragging() {
      return s_isDragging;
   }

   public static void StartDrag() {
      s_isDragging = true;
      CursorManager.StartDrawingTabDrag();
   }

   public static void EndDrag() {
      s_isDragging = false;
      CursorManager.StopDrawingTabDrag();
   }
}