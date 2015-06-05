class ForeignKey {
   
   public Table targetTable {get; private set;}
   public int targetColumn {get; private set;}

   public ForeignKey(Table table, int column) {
      targetTable = table;
      targetColumn = column;
   }

}