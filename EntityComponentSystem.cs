using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityPackages.EntityComponentSystem {

	public static class ECS {

		/// <summary>
		/// Log.
		/// </summary>
		public static void Log (object title, object message) {
			if (Controller.Instance.debugging == true)
				UnityEngine.Debug.Log ("<b>ECS</b> " +
					title.ToString ().ToUpper () + "\n" +
					message.ToString ());
		}

		/// <summary>
		/// Controller.
		/// </summary>
		public class Controller : UnityEngine.MonoBehaviour {

			public virtual void OnInitialize () { }
			public virtual void OnInitialized () { }

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
			}

#if UNITY_EDITOR
			private void OnDrawGizmos () {
				if (UnityEngine.Application.isPlaying == true)
					for (var _i = 0; _i < this.systems.Count; _i++)
						this.systems[_i].OnDrawGizmos ();
			}
#endif

			public void RegisterSystems (params ISystem[] systems) {
				for (var _i = 0; _i < systems.Length; _i++) {
					systems[_i].OnInitialize ();
					this.systems.Add (systems[_i]);
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
		}

		public class System<S, C> : ISystem where S : System<S, C>, new () where C : Component<C, S>, new () {

			public virtual void OnInitialize () { }
			public virtual void OnUpdate () { }
			public virtual void OnDrawGizmos () { }
			public virtual void OnEntityInitialize (C component) { }
			public virtual void OnEntityWillDestroy (C component) { }

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
		}

		/// <summary>
		/// Component.
		/// </summary>
		public class Component<C, S> : UnityEngine.MonoBehaviour where C : Component<C, S>, new () where S : System<S, C>, new () {

			private void Start () {
				Controller.Instance.GetSystem<S> ().AddEntity ((C) this);
			}

			private void OnDestroy () {
				Controller.Instance.GetSystem<S> ().RemoveEntry ((C) this);
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
	}
}