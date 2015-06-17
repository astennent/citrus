using System.Runtime.Serialization;

[DataContract]
public class Attribute {

   [DataMember]
   public string name {get; set; } // TODO implement set to update gui.

   public Attribute(string attributeName) {
      name = attributeName;
   }
	
}
