using System.Collections.Generic;
using UnityEngine;

namespace ElRaccoone.EntityComponentSystem {
  /// <summary>
  /// Base class for Controllers.
  /// </summary>
  public abstract class Controller : MonoBehaviour, IController {
    /// <summary>
    /// A reference to the controller.
    /// </summary>
    public static Controller Instance { private set; get; } = null;

    /// <summary>
    /// A list of the controller's instantiated entity systems.
    /// </summary>
    List<IEntitySystem> systems = new List<IEntitySystem> ();

    /// <summary>
    /// A list of the controller's instantiated entity systems which are enabled.
    /// </summary>
    List<IEntitySystem> enabledSystemsCache = new List<IEntitySystem> ();

    /// <summary>
    /// A list of the controller's instantiated services.
    /// </summary>
    List<IService> services = new List<IService> ();

    /// <summary>
    /// Defines whether this controller has been intialized.
    /// </summary>
    bool isInitialized;

    /// <summary>
    /// The assets that can be added to entities.
    /// </summary>
    public Object[] assets;

    /// <summary>
    /// Invoked by the Unity Engine when the controller is started.
    /// </summary>
    void Awake () {
      if (Controller.Instance != null) {
        throw new System.Exception ("A project cannot exceed the limit of one controller!");
      }
      // The controller will not be destroyed when a new scene is loaded.
      Object.DontDestroyOnLoad (transform.root.gameObject);
      // Setting the controller reference and invoke initialization.
      Controller.Instance = this;
      OnInitialize ();
    }

    /// <summary>
    /// Invoked by the Unity Engine when the controller is updated.
    /// </summary>
    void Update () {
      if (isInitialized == false) {
        // When the controller is not initialized, it will be initialized.
        OnInitialized ();
        isInitialized = true;
      }
      // Invoking internal update on each system and service.
      for (var systemIndex = 0; systemIndex < systems.Count; systemIndex++) {
        var systemInternals = systems[systemIndex] as IEntitySystemInternals;
        systemInternals.OnUpdateInternal ();
      }
      for (var serviceIndex = 0; serviceIndex < services.Count; serviceIndex++) {
        var serviceInternals = services[serviceIndex] as IServiceInternals;
        serviceInternals.OnUpdateInternal ();
      }
      // Invoking update method on controller.
      OnUpdate ();
      // Invoking update method on each enabled system.
      for (var systemIndex = 0; systemIndex < enabledSystemsCache.Count; systemIndex++) {
        var system = enabledSystemsCache[systemIndex];
        if (!system.ShouldUpdate ()) {
          continue;
        }
        system.OnUpdate ();
      }
    }

#if ECS_PHYSICS || ECS_ALL
    /// <summary>
    /// Method invoked by the Unity Engine when the controller is updated.
    /// </summary>
    void FixedUpdate () {
      for (var systemIndex = 0; systemIndex < enabledSystemsCache.Count; systemIndex++) {
        var system = enabledSystemsCache[systemIndex];
        system.OnPhysics ();
      }
    }
#endif

#if ECS_GRAPHICS || ECS_ALL
    /// <summary>
    /// Method invoked by the Unity Engine when the controller is rendered.
    /// </summary>
    void LateUpdate () {
      for (var systemIndex = 0; systemIndex < enabledSystemsCache.Count; systemIndex++) {
        var system = enabledSystemsCache[systemIndex];
        system.OnRender ();
      }
    }
#endif

    /// <summary>
    /// Method invoked by the Unity Engine when the controller is destroyed.
    /// </summary> 
    void OnDestroy () {
      // Invoking will destroy method on each system, service and controller.
      OnWillDestroy ();
      for (var systemIndex = 0; systemIndex < systems.Count; systemIndex++) {
        var system = systems[systemIndex];
        system.OnWillDestroy ();
      }
      for (var serviceIndex = 0; serviceIndex < services.Count; serviceIndex++) {
        var service = services[serviceIndex];
        service.OnWillDestroy ();
      }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Method invoked by the Unity Engine when the controller is drawn.
    /// </summary>
    void OnDrawGizmos () {
      if (!Application.isPlaying) {
        // When the application is not playing, the GUI will not be drawn since
        // the controller is not initialized at this stage.
        return;
      }
      // Invoking draw gizmos method on each system and service.
      for (var systemIndex = 0; systemIndex < enabledSystemsCache.Count; systemIndex++) {
        var system = enabledSystemsCache[systemIndex];
        system.OnDrawGizmos ();
      }
      for (var serviceIndex = 0; serviceIndex < services.Count; serviceIndex++) {
        var service = services[serviceIndex];
        service.OnDrawGizmos ();
      }
    }
#endif

    /// <summary>
    /// Method invoked by the Unity Engine when the GUI is drawn.
    /// </summary>
    void OnGUI () {
      // Invoking draw gui method on each system and service.
      for (var systemIndex = 0; systemIndex < enabledSystemsCache.Count; systemIndex++) {
        var system = enabledSystemsCache[systemIndex];
        system.OnDrawGui ();
      }
      for (var serviceIndex = 0; serviceIndex < services.Count; serviceIndex++) {
        var service = services[serviceIndex];
        service.OnDrawGui ();
      }
    }

    /// <summary>
    /// Method invoked when the controller is initializing.
    /// </summary>
    public virtual void OnInitialize () { }

    /// <summary>
    /// Method invoked when the controller is initialized.
    /// </summary>
    public virtual void OnInitialized () { }

    /// <summary>
    /// Method invoked when the controller updates, will be called every frame.
    /// </summary>
    public virtual void OnUpdate () { }

    /// <summary>
    /// Method invoked when the system will be destroyed, this will happen when
    /// the application is closing or the controller is being destroyed.
    /// </summary>
    public virtual void OnWillDestroy () { }

    /// <summary>
    /// Register your systems and services to the controller. This can only be
    /// done during 'OnInitialize' cycle.
    /// </summary>
    /// <param name="typesOf">The types of the systems and services to register.</param>
    /// <exception cref="System.Exception"></exception> 
    public void Register (params System.Type[] typesOf) {
      if (isInitialized == true) {
        // When the controller is already initialized, it is not possible to
        // register new systems and services. This will throw an exception.
        throw new System.Exception ("Cannot to registered System outsize of OnInitialize cycle");
      }
      for (var typeOfIndex = 0; typeOfIndex < typesOf.Length; typeOfIndex++) {
        // Create an instance of the type.
        var instance = System.Activator.CreateInstance (typesOf[typeOfIndex]);
        // When the instance is a type of System, add it to the systems.
        if (instance is IEntitySystem) {
          var system = instance as IEntitySystem;
          systems.Add (system);
          enabledSystemsCache.Add (system);
          system.OnInitialize ();
          var systemInternals = system as IEntitySystemInternals;
          // Invoke the internal initialize method.
          systemInternals.OnInitializeInternal ();
        }
        // When the instance is a type of System, add it to the services.
        if (instance is IService) {
          var service = instance as IService;
          services.Add (service);
          service.OnInitialize ();
          var serviceInternals = service as IServiceInternals;
          // Invoke the internal initialize method.
          serviceInternals.OnInitializeInternal ();
        }
      }
      // Set the values of properties and fields marked with the Injected 
      // attribute.
      Injected.SetAttributeValues (this);
      for (var systemIndex = 0; systemIndex < systems.Count; systemIndex++) {
        var system = systems[systemIndex];
        Injected.SetAttributeValues (system);
      }
      for (var serviceIndex = 0; serviceIndex < services.Count; serviceIndex++) {
        var service = services[serviceIndex];
        Injected.SetAttributeValues (service);
      }
      // Set the values of properties and fields marked with the Asset
      // attribute.
      Asset.SetAttributeValues (this);
      for (var systemIndex = 0; systemIndex < systems.Count; systemIndex++) {
        var system = systems[systemIndex];
        Asset.SetAttributeValues (system);
      }
      for (var serviceIndex = 0; serviceIndex < services.Count; serviceIndex++) {
        var service = services[serviceIndex];
        Asset.SetAttributeValues (service);
      }
    }

    /// <summary>
    /// Enables or disabled a system, enabling the systems allows them to invoke
    /// their cycle methods such as OnUpdate, OnPhysics, OnDrawGui and others.
    /// </summary>
    /// <typeparam name="SystemType">The type of the system to enable or disable.</typeparam>
    /// <param name="value">Whether to enable or disable the system.</param>
    public void SetSystemEnabled<SystemType> (bool value) {
      var typeOf = typeof (SystemType);
      for (var systemIndex = 0; systemIndex < enabledSystemsCache.Count; systemIndex++) {
        var systemType = enabledSystemsCache[systemIndex].GetType ();
        if (systemType == typeOf) {
          // If the system is already enabled or disabled, do nothing.
          if (value) {
            return;
          }
          // When the system is disabled, invoke the OnDisabled method and 
          // remove it from the enabled systems cache.
          enabledSystemsCache[systemIndex].OnDisabled ();
          enabledSystemsCache.RemoveAt (systemIndex);
          return;
        }
      }
      if (!value) {
        // When the system is already disabled, do nothing.
        return;
      }
      for (var systemIndex = 0; systemIndex < systems.Count; systemIndex++) {
        var systemType = systems[systemIndex].GetType ();
        if (systemType == typeOf) {
          // When the system is enabled, invoke the OnEnabled method and add
          // it to the enabled systems cache.
          enabledSystemsCache.Add (systems[systemIndex]);
          systems[systemIndex].OnEnabled ();
          return;
        }
      }
    }

    /// <summary>
    /// Returns whether a system is enabled.
    /// </summary>
    public bool IsSystemEnabled<SystemType> () {
      var typeOf = typeof (SystemType);
      for (var systemIndex = 0; systemIndex < enabledSystemsCache.Count; systemIndex++) {
        var systemType = enabledSystemsCache[systemIndex].GetType ();
        if (systemType == typeOf) {
          // When the system is enabled, return true.
          return true;
        }
      }
      // When the system is disabled, return false.
      return false;
    }

    /// <summary>
    /// Gets a system from this controller.
    /// </summary>
    /// <typeparam name="SystemType">The type of the system to get.</typeparam>
    /// <returns>The system of the given type.</returns>
    /// <exception cref="System.Exception">Thrown when the system is not registered to the controller.</exception>
    public SystemType GetSystem<SystemType> () where SystemType : IEntitySystem, new() {
      var typeOf = typeof (SystemType);
      for (var systemIndex = 0; systemIndex < systems.Count; systemIndex++) {
        var systemType = systems[systemIndex].GetType ();
        if (systemType == typeOf) {
          // If the system is present in the systems list, return it.
          var system = systems[systemIndex];
          return (SystemType)system;
        }
      }
      throw new System.Exception ($"Unable to get System of type {typeOf}, it was not registerd to the Controller");
    }

    /// <summary>
    /// Gets a system from this controller.
    /// </summary>
    /// <param name="typeOf">The type of the system to get.</param>
    /// <returns>The system of the given type.</returns>
    /// <exception cref="System.Exception">Thrown when the system is not registered to the controller.</exception>
    public System.Object GetSystem (System.Type typeOf) {
      for (var systemIndex = 0; systemIndex < systems.Count; systemIndex++) {
        var systemType = systems[systemIndex].GetType ();
        if (systemType == typeOf) {
          // If the system is present in the systems list, return it.
          return systems[systemIndex];
        }
      }
      throw new System.Exception ($"Unable to get System of type {typeOf}, it was not registerd to the Controller");
    }

    /// <summary>
    /// Check whether this controller has a system.
    /// </summary>
    /// <typeparam name="SystemType">The type of the system to check.</typeparam>
    /// <returns>Whether this controller has a system of the given type.</returns>
    public bool HasSystem<SystemType> () where SystemType : IEntitySystem, new() {
      var typeOf = typeof (SystemType);
      for (var systemIndex = 0; systemIndex < systems.Count; systemIndex++) {
        var systemType = systems[systemIndex].GetType ();
        if (systemType == typeOf) {
          // If the system is present in the systems list, return true.
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Gets a service from this controller.
    /// </summary>
    /// <typeparam name="ServiceType">The type of the service to get.</typeparam>
    /// <returns>The service of the given type.</returns>
    /// <exception cref="System.Exception">Thrown when the service is not registered to the controller.</exception>
    public ServiceType GetService<ServiceType> () where ServiceType : IService, new() {
      var typeOf = typeof (ServiceType);
      for (var serviceIndex = 0; serviceIndex < services.Count; serviceIndex++) {
        var serviceType = services[serviceIndex].GetType ();
        if (serviceType == typeOf) {
          // If the service is present in the services list, return it.
          var service = services[serviceIndex];
          return (ServiceType)service;
        }
      }
      throw new System.Exception ($"Unable to get Service of type {typeOf}, it was not registerd to the Controller");
    }

    /// <summary>
    /// Gets a system from this controller.
    /// </summary>
    /// <param name="typeOf">The type of the service to get.</param>
    /// <returns>The service of the given type.</returns>
    /// <exception cref="System.Exception">Thrown when the service is not registered to the controller.</exception>
    public System.Object GetService (System.Type typeOf) {
      for (var serviceIndex = 0; serviceIndex < services.Count; serviceIndex++) {
        var serviceType = services[serviceIndex].GetType ();
        if (serviceType == typeOf) {
          // If the service is present in the services list, return it.
          return services[serviceIndex];
        }
      }
      throw new System.Exception ($"Unable to get Service of type {typeOf}, it was not registerd to the Controller");
    }

    /// <summary>
    /// Check whether this controller has a service.
    /// </summary>
    /// <typeparam name="ServiceType">The type of the service to check.</typeparam>
    /// <returns>Whether this controller has a service of the given type.</returns>
    public bool HasService<ServiceType> () where ServiceType : IService, new() {
      var typeOf = typeof (ServiceType);
      for (var serviceIndex = 0; serviceIndex < services.Count; serviceIndex++) {
        var serviceType = services[serviceIndex].GetType ();
        if (serviceType == typeOf) {
          // If the service is present in the services list, return true.
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Gets an asset from this controller.
    /// </summary>
    /// <param name="name">The name of the asset to get.</param>
    /// <returns>The asset with the given name.</returns>
    /// <exception cref="System.Exception">Thrown when the asset is not found.</exception>
    public Object GetAsset (string name) {
      for (var assetIndex = 0; assetIndex < assets.Length; assetIndex++) {
        var asset = assets[assetIndex];
        if (asset.name == name) {
          // If the asset is present in the assets list, return it.
          return asset;
        }
      }
      throw new System.Exception ($"Unable to get Asset '{name}', it was not found on the Controller");
    }

    /// <summary>
    /// Gets an asset from this controller.
    /// </summary>
    /// <typeparam name="AssetType">The type of the asset to get.</typeparam>
    /// <param name="name">The name of the asset to get.</param>
    /// <returns>The asset with the given name.</returns>
    public AssetType GetAsset<AssetType> (string name) where AssetType : Object {
      var asset = GetAsset (name);
      // Return the asset as the given type.
      return asset as AssetType;
    }

    /// <summary>
    /// Check whether this controller has an asset.
    /// </summary>
    /// <param name="name">The name of the asset to check.</param>
    /// <returns>Whether this controller has an asset with the given name.</returns>
    public bool HasAsset (string name) {
      for (var assetIndex = 0; assetIndex < assets.Length; assetIndex++) {
        var asset = assets[assetIndex];
        if (asset.name == name) {
          // If the asset is present in the assets list, return true.
          return true;
        }
      }
      return false;
    }
  }
}
