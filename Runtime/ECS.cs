using System.Collections.Generic;

namespace UnityPackages.EntityComponentSystem {

	public class ECS {

		/// <summary>
		/// Controller.
		/// </summary>
		public abstract class Controller : UnityEngine.MonoBehaviour {

			private List<ISystem> systems;
			private bool isInitialized;

			public static Controller Instance;

			public virtual void OnInitialize () { }
			public virtual void OnInitialized () { }
			public virtual void OnUpdate () { }

			public bool debugging;

			private void Awake () {
				Instance = this;
				this.systems = new List<ISystem> ();
				this.OnInitialize ();
				Log ("Initialized controller", "");
			}

			private void Update () {
				for (var _i = 0; _i < this.systems.Count; _i++)
					this.systems[_i].OnUpdate ();
				if (this.isInitialized == false) {
					this.isInitialized = true;
					this.OnInitialized ();
				}
				this.OnUpdate ();
			}

#if UNITY_EDITOR
			private void OnDrawGizmos () {
				if (UnityEngine.Application.isPlaying == true)
					for (var _i = 0; _i < this.systems.Count; _i++)
						this.systems[_i].OnDrawGizmos ();
			}
#endif

			private void OnGUI () {
				for (var _i = 0; _i < this.systems.Count; _i++)
					this.systems[_i].OnGUI ();
			}

			public void RegisterSystems (params ISystem[] systems) {
				for (var _i = 0; _i < systems.Length; _i++) {
					this.systems.Add (systems[_i]);
					Log ("Added system", systems[_i].GetType ());
				}
				for (var _i = 0; _i < systems.Length; _i++) {
					systems[_i].OnInitialize ();
					systems[_i].OnInitializedInternal ();
					Log ("Initialized system", systems[_i].GetType ());
				}
			}

			public S GetSystem<S> () where S : ISystem, new () {
				var _typeOfS = typeof (S);
				for (var _i = 0; _i < this.systems.Count; _i++)
					if (this.systems[_i].GetType () == _typeOfS)
						return (S) this.systems[_i];
				return new S ();
			}
		}

		/// <summary>
		/// System.
		/// </summary>
		public interface ISystem {
			void OnInitialize ();
			void OnUpdate ();
			void OnDrawGizmos ();
			void OnGUI ();
			void OnInitializedInternal ();
		}

		public abstract class System<S, C> : ISystem
		where S : System<S, C>, new ()
		where C : Component<C, S>, new () {

			public static S Instance;

			public virtual void OnInitialize () { }
			public virtual void OnUpdate () { }
			public virtual void OnDrawGizmos () { }
			public virtual void OnGUI () { }
			public virtual void OnEntityInitialize (C entity) { }
			public virtual void OnEntityInitialized (C entity) { }
			public virtual void OnEntityStart (C entity) { }
			public virtual void OnEntityEnabled (C entity) { }
			public virtual void OnEntityDisabled (C entity) { }
			public virtual void OnEntityWillDestroy (C entity) { }

			/// The list of instanced entities of this system.
			public List<C> entities;

			/// Gets the first entity instances by this system.
			public C firstEntity { get { return this.entities[0]; } }

			/// Creates a new system.
			public System () {
				this.entities = new List<C> ();
			}

			// This is an interal function that triggers when the system is
			// initialized. It sets the ref to the system.
			public void OnInitializedInternal () {
				Instance = Controller.Instance.GetSystem<S> ();
			}

			/// Adds an entity component to the system.
			public void AddEntity (C component) {
				Log ("Added Entity",
					component.transform.name + " to " + this.ToString ());
				this.entities.Add (component);
				this.OnEntityInitialize (component);
			}

			/// Removes an entity component from the system.
			public void RemoveEntry (C component) {
				Log ("Removed Entity",
					component.transform.name + " from " + this.ToString ());
				this.OnEntityWillDestroy (component);
				this.entities.Remove (component);
			}

			/// Gets a component from an entity.
			public void GetComponentOnEntity<GEC> (C entity, System.Action<GEC> action) {
				var _entity = entity.GetComponent<GEC> ();
				if (_entity != null)
					action (_entity);
			}

			/// Check wether a component has an entity.
			public bool HasComponentOnEntity<GEC> (C entity) {
				return entity.GetComponent<GEC> () != null;
			}
		}

		/// <summary>
		/// Component.
		/// </summary>
		public abstract class Component<C, S> : UnityEngine.MonoBehaviour
		where C : Component<C, S>, new ()
		where S : System<S, C>, new () {

			private bool isEntityEnabled = false;
			private bool isEntityInitialized = false;
			private S system;

			private void Start () {
				this.system = Controller.Instance.GetSystem<S> ();
				this.system.AddEntity ((C) this);
				this.system.OnEntityStart ((C) this);
			}

			private void Update () {
				if (this.system != null) {
					if (this.isEntityEnabled == false) {
						this.isEntityEnabled = true;
						this.system.OnEntityEnabled ((C) this);
					}
					if (this.isEntityInitialized == false) {
						this.isEntityInitialized = true;
						this.system.OnEntityInitialized ((C) this);
					}
				} else Error (
					"Component on " + this.gameObject.name,
					"Update tried to access the system before it was initialized");
			}

			private void OnDisable () {
				this.isEntityEnabled = false;
				if (this.system != null)
					this.system.OnEntityDisabled ((C) this);
				else Error (
					"Component on " + this.gameObject.name,
					"OnDisable tried to access the system before it was initialized");
			}

			private void OnDestroy () {
				if (this.system != null)
					this.system.RemoveEntry ((C) this);
				else Error (
					"Component on " + this.gameObject.name,
					"OnDestroy tried to access the system before it was initialized");
			}
		}

		/// <summary>
		/// Describes a protected property within a component.
		/// </summary>
		public class Protected : UnityEngine.PropertyAttribute { }

		/// <summary>
		/// Describes a reference property within a component.
		/// </summary>
		public class Reference : UnityEngine.PropertyAttribute { }

		/// <summary>
		/// Logs a message to the console.
		/// </summary>
		public static void Log (object title, object message) {
			if (Controller.Instance.debugging == true)
				UnityEngine.Debug.Log ("<b>ECS</b> " +
					title.ToString ().ToUpper () + "\n" +
					message.ToString ());
		}

		/// <summary>
		/// Logs an error to the console.
		/// </summary>
		public static void Error (object title, object message) {
			UnityEngine.Debug.LogError ("<b>ECS</b> " +
				title.ToString ().ToUpper () + "\n" +
				message.ToString ());
		}
	}
}