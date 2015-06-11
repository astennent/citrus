public class ForeignKey {

   //TODO: Multi-attribute targets.
   public Table sourceTable {get; private set;}
   public int sourceColumn {get; private set;}
   public Table targetTable {get; private set;}
   public int targetColumn {get; private set;}

   public ForeignKey(Table _sourceTable, int _sourceColumn, Table _targetTable, int _targetColumn) {
      sourceTable = _sourceTable;
      sourceColumn = _sourceColumn;
      targetTable = _targetTable;
      targetColumn = _targetColumn;
   }
}