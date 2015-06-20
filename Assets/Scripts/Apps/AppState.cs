using System.Runtime.Serialization;

[DataContract]
public class AppState {

   [DataMember]
   public string type = "foo";
}