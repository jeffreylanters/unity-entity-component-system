namespace ElRaccoone.EntityComponentSystem {

  /// Describes an internal method within a core system, service or controller.
  [System.AttributeUsage (System.AttributeTargets.Method)]
  public class Internal : System.Attribute {
  }
}