using System.Runtime.Serialization;
using System.Collections.Generic;

[DataContract]
public class PanelState {

   [DataMember]
   public PanelState firstChild;

   [DataMember]
   public PanelState secondChild;

   [DataMember]
   public float splitRatio;

   [DataMember]
   public bool splitVertical;

   [DataMember]
   public List<AppState> appStates;
}