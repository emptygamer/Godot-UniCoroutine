using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UniCoroutine{
    public class Coroutine : IEnumerable
    {
		private IEnumerator _coroutineTask; 
		public Coroutine(IEnumerator coroutineTask){
			_coroutineTask = coroutineTask;
		}
        public IEnumerator GetEnumerator()
        {
			return _coroutineTask;
        }
		public bool CompareTaskName(string taskName){
			string _coroutineTaskName = _coroutineTask.GetType().Name;
			return (_coroutineTaskName == taskName) || _coroutineTaskName.Contains($"<{taskName}>");
		}
    }

    public partial class CoroutineSystem : Node
	{
		private static CoroutineSystem _instance;
		public static CoroutineSystem instance{
			get {
				if((_instance == null) || (!IsInstanceValid(_instance))){
					_instance = new CoroutineSystem();
				}
				return _instance;
			}
		}
		public float deltaTime;
		public List<Coroutine> coroutineTasks = new List<Coroutine>();
		public void Init(){
			this.Name = "CoroutineSystem";
			var mainScene = (SceneTree)Engine.GetMainLoop();
			mainScene.CurrentScene.AddChild(this);
		}
		
		/// <summary>
		/// Start a coroutine task.
		/// </summary>
		/// <param name="coroutineTask"> Your coroutine task. </param>
		public void StartCoroutine(IEnumerator coroutineTask){
			if(this.GetParent() == null){
				this.Init();
			}
			ProcessTask(coroutineTask);
			coroutineTasks.Insert(0,new Coroutine(coroutineTask));
		}

		/// <summary>
		/// <para>Stop a coroutine task by the function name. </para>
		/// <para>Set stopAll flag to true to stop all the coroutine tasks matched the function name. </para>
		/// </summary>
		/// <param name="functionName"> Your coroutine task/function name. </param>
		/// <param name="stopAll"> Stop all coroutine tasks that matched the function name. </param>
		public void StopCoroutine(string functionName, bool stopAll=false){
			for(int i=coroutineTasks.Count-1;i>=0;i--){
				var coroutine = coroutineTasks[i];
				if(coroutine.CompareTaskName(functionName)){
					coroutineTasks.RemoveAt(i);
					if(!stopAll)break;
				}
			}
		}

		public override void _Ready()
		{
			ProcessPriority = -1;
		}
		public bool ProcessTask(IEnumerator task){
			// Note : If current in sub task, process it first.
			if(task.Current is IEnumerator){
				bool s = ProcessTask(task.Current as IEnumerator);
				if(!s){
					return task.MoveNext();
				}else{
					return true;
				}
			}else{
				return task.MoveNext();
			}
		}
		public override void _Process(double delta)
		{
			deltaTime = (float)delta;
			for(int i=coroutineTasks.Count-1;i>=0;i--){
				var en = coroutineTasks[i].GetEnumerator();
				if(!ProcessTask(en)){
					coroutineTasks.RemoveAt(i);
				}
			}
		}
	}

    class WaitTime : IEnumerator
    {
		private float waitTimeSec;
		private float timer = 0;
        object IEnumerator.Current => timer;

        public WaitTime(float waitTimeSec){
			timer = waitTimeSec;
		}

        bool IEnumerator.MoveNext()
        {
            var cs = CoroutineSystem.instance;
			timer -= cs.deltaTime;
			return timer>0;
        }

        void IEnumerator.Reset()
        {
            timer = waitTimeSec;
        }
    }

	class WaitUntil : IEnumerator
    {
		private Func<bool> _statement;
        object IEnumerator.Current => _statement();

		public WaitUntil(Func<bool> statement){
			_statement = statement;
		}

        bool IEnumerator.MoveNext()
        {
			return !_statement();
        }

        void IEnumerator.Reset()
        {

        }
    }
}
