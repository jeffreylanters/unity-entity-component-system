using System.Collections.Generic;

namespace UnityPackages.EntityComponentSystem {

	public class ECS {

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
					systems[_i].OnInitializeInternal ();
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

			public bool HasSystem<S> () where S : ISystem, new () {
				var _typeOfS = typeof (S);
				for (var _i = 0; _i < this.systems.Count; _i++)
					if (this.systems[_i].GetType () == _typeOfS)
						return true;
				return false;
			}
		}

		public interface ISystem {
			void OnInitialize ();
			void OnUpdate ();
			void OnDrawGizmos ();
			void OnGUI ();
			void OnInitializeInternal ();
		}

		public abstract class System<S, C> : ISystem
		where S : System<S, C>, new ()
		where C : Component<C, S>, new () {

			// An instance reference to the controller. This reference will be set during the
			//  'OnInitializeInternal' method inside this class. This method will be called
			//  after the controller and system initialization.
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

			public List<C> entities;

			public C firstEntity { get { return this.entities[0]; } }

			public System () {
				this.entities = new List<C> ();
			}

			// This is an interal function that triggers when the system is
			// initialized. It sets the ref to the system.
			public void OnInitializeInternal () {
				Instance = Controller.Instance.GetSystem<S> ();
			}

			public void AddEntity (C component) {
				Log ("Added Entity",
					component.transform.name + " to " + this.ToString ());
				this.entities.Add (component);
				this.OnEntityInitialize (component);
			}

			public void RemoveEntry (C component) {
				Log ("Removed Entity",
					component.transform.name + " from " + this.ToString ());
				this.OnEntityWillDestroy (component);
				this.entities.Remove (component);
			}

			public void GetComponentOnEntity<GEC> (C entity, System.Action<GEC> action) {
				var _entity = entity.GetComponent<GEC> ();
				if (_entity != null)
					action (_entity);
			}

			public bool HasComponentOnEntity<GEC> (C entity) {
				return entity.GetComponent<GEC> () != null;
			}
		}

		public abstract class Component<C, S> : UnityEngine.MonoBehaviour
		where C : Component<C, S>, new ()
		where S : System<S, C>, new () {

			private bool isEntityEnabled = false;
			private bool isEntityInitialized = false;
			private S system;

			private S GetSystem () {
				if (this.system == null) {
					if (Controller.Instance.HasSystem<S> () == false) {
						Error (
							typeof (C) + " on " + this.gameObject.name,
							"Tried to access the system before it was registered!");
						return null;
					}
					this.system = Controller.Instance.GetSystem<S> ();
				}
				return this.system;
			}

			private void Start () {
				this.GetSystem ().AddEntity ((C) this);
				this.GetSystem ().OnEntityStart ((C) this);
			}

			private void Update () {
				if (this.isEntityEnabled == false) {
					this.isEntityEnabled = true;
					this.GetSystem ().OnEntityEnabled ((C) this);
				}
				if (this.isEntityInitialized == false) {
					this.isEntityInitialized = true;
					this.GetSystem ().OnEntityInitialized ((C) this);
				}
			}

			private void OnDisable () {
				this.isEntityEnabled = false;
				this.GetSystem ().OnEntityDisabled ((C) this);
			}

			private void OnDestroy () {
				this.GetSystem ().RemoveEntry ((C) this);
			}
		}

		/// Describes a protected property within a component.
		public class Protected : UnityEngine.PropertyAttribute { }

		/// Describes a reference property within a component.
		public class Reference : UnityEngine.PropertyAttribute { }

		public static void Log (object title, object message) {
			if (Controller.Instance.debugging == true)
				UnityEngine.Debug.Log ("<b>ECS</b> " +
					title.ToString ().ToUpper () + "\n" +
					message.ToString ());
		}

		public static void Error (object title, object message) {
			UnityEngine.Debug.LogError ("<b>ECS</b> " +
				title.ToString ().ToUpper () + "\n" +
				message.ToString ());
		}
	}
}