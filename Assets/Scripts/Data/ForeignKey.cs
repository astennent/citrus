public class ForeignKey {

   //TODO: Multi-attribute targets.
   public int sourceColumn {get; private set;}
   public Table targetTable {get; private set;}
   public int targetColumn {get; private set;}

   public ForeignKey(int _sourceColumn, Table _targetTable, int _targetColumn) {
      sourceColumn = _sourceColumn;
      targetTable = _targetTable;
      targetColumn = _targetColumn;
   }
}