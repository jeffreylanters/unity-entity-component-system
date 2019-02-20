using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityPackages.EntityComponentSystem {

	public static class ECS {

		/// <summary>
		/// Controller.
		/// </summary>
		public class Controller : UnityEngine.MonoBehaviour {

			public virtual void OnInitialize () { }
			public virtual void OnInitialized () { }
			public virtual void OnUpdate () { }

			public static Controller Instance;

			public bool debugging;

			private List<ISystem> systems;
			private bool isInitialized;

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
					Log ("Initialized system", systems[_i].GetType ());
				}
			}

			public S GetSystem<S> () where S : ISystem, new () {
				for (var _i = 0; _i < this.systems.Count; _i++)
					if (this.systems[_i].GetType () == typeof (S))
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
		}

		public class System<S, C> : ISystem where S : System<S, C>, new () where C : Component<C, S>, new () {

			public virtual void OnInitialize () { }
			public virtual void OnUpdate () { }
			public virtual void OnDrawGizmos () { }
			public virtual void OnGUI () { }
			public virtual void OnEntityInitialize (C entity) { }
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
			public bool HasComponentEntity<GEC> (C entity) {
				return entity.GetComponent<GEC> () != null;
			}
		}

		/// <summary>
		/// Component.
		/// </summary>
		public class Component<C, S> : UnityEngine.MonoBehaviour where C : Component<C, S>, new () where S : System<S, C>, new () {

			private bool isEntityEnabled = false;

			private void Start () {
				var _system = GetSystem<S> ();
				_system.AddEntity ((C) this);
				_system.OnEntityStart ((C) this);
			}

			private void Update () {
				if (this.isEntityEnabled == false) {
					this.isEntityEnabled = true;
					var _system = GetSystem<S> ();
					_system.OnEntityEnabled ((C) this);
				}
			}

			private void OnDisable () {
				this.isEntityEnabled = false;
				var _system = GetSystem<S> ();
				if (_system != null)
					_system.OnEntityDisabled ((C) this);
			}

			private void OnDestroy () {
				GetSystem<S> ().RemoveEntry ((C) this);
			}
		}

		public class Protected : UnityEngine.PropertyAttribute { }
#if UNITY_EDITOR
		[CustomPropertyDrawer (typeof (Protected))]
		public class ProtectedPropertyDrawer : PropertyDrawer {
			public override void OnGUI (UnityEngine.Rect position, SerializedProperty serializedProperty, UnityEngine.GUIContent label) {
				EditorGUI.BeginDisabledGroup (true);
				EditorGUI.PropertyField (position, serializedProperty, label, true);
				EditorGUI.EndDisabledGroup ();
			}
		}
#endif

		public class Reference : UnityEngine.PropertyAttribute { }
#if UNITY_EDITOR
		[CustomPropertyDrawer (typeof (Reference))]
		public class ReferencePropertyDrawer : PropertyDrawer {
			public override void OnGUI (UnityEngine.Rect position, SerializedProperty serializedProperty, UnityEngine.GUIContent label) {
				EditorGUI.BeginDisabledGroup (true);
				EditorGUI.PropertyField (position, serializedProperty, label, true);
				EditorGUI.EndDisabledGroup ();
			}
		}
#endif

		/// <summary>
		/// Gets a system.
		/// </summary>
		public static S GetSystem<S> () where S : ISystem, new () {
			return Controller.Instance.GetSystem<S> ();
		}

		/// <summary>
		/// Log.
		/// </summary>
		public static void Log (object title, object message) {
			if (Controller.Instance.debugging == true)
				UnityEngine.Debug.Log ("<b>ECS</b> " +
					title.ToString ().ToUpper () + "\n" +
					message.ToString ());
		}
	}
}