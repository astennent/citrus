using System.Runtime.Serialization;

[DataContract]
public class ForeignKey {

   //TODO: Multi-attribute targets.

   [DataMember]
   public int sourceTableId {get; private set;}
   [DataMember]
   public int sourceColumn {get; private set;}
   [DataMember]
   public int targetTableId {get; private set;}
   [DataMember]
   public int targetColumn {get; private set;}

   public Table sourceTable {
      get { return TableMap.FromId(sourceTableId); }
      set { Utils.Assert("Cannot set Table of ForeignKey."); }
   }

   public Table targetTable {
      get { return TableMap.FromId(targetTableId); }
      set { Utils.Assert("Cannot set Table of ForeignKey."); }
   }

   public ForeignKey(int _sourceTableId, int _sourceColumn, int _targetTableId, int _targetColumn) {
      sourceTableId = _sourceTableId;
      sourceColumn = _sourceColumn;
      targetTableId = _targetTableId;
      targetColumn = _targetColumn;
   }
}