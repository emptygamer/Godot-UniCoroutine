using Godot;
using System;
using UniCoroutine;
using System.Collections;

public partial class CoroutineExample : Node
{
	private bool isSapcePressed;
	public override void _Ready()
	{
		var cs = CoroutineSystem.instance;
		// Note : Create Coroutine Tasks.
		cs.StartCoroutine(ImageTask());
		cs.StartCoroutine(TestTask());
		cs.StartCoroutine(TestTask2());
	}

	IEnumerator ImageTask(){
		Sprite2D sp = this.FindChild("Sprite2D") as Sprite2D;
		if(sp != null){
			GD.Print("Start ImageTask...");
			float endPosX = sp.Position.X+300;
 			while(true){
				float deltaTime = CoroutineSystem.instance.deltaTime;
				sp.Position += new Vector2(100 * deltaTime,0);
				sp.Rotation += 90*deltaTime*(float)Math.PI/180;
				if(sp.Position.X > endPosX){
					break;
				}
				yield return null;
			}
			GD.Print("ImageTask Done!");
		}
	}
	IEnumerator TestTask(){
		GD.Print("Start TestTask...");
		GD.Print("Wait 3 Sec");
		yield return new WaitTime(3);
		GD.Print("Wait Until Space Pressed.");
		isSapcePressed = false;
		yield return new WaitUntil(()=>{return isSapcePressed;});
		GD.Print("Wait 3 Sec");
		yield return new WaitTime(3);
		GD.Print("TestTask Done!");
	}
	IEnumerator TestTask2(){
		GD.Print("Start TestTask2...");
		GD.Print("Wait 5 Sec");
		yield return new WaitTime(5);
		GD.Print("TestTask2 Done!");
	}

	public override void _Process(double delta)
	{
		if(Input.IsKeyPressed(Key.Space)){
			isSapcePressed = true;
		}
		if(Input.IsKeyPressed(Key.S)){
			var cs = CoroutineSystem.instance;
			cs.StopCoroutine("ImageTask");
		}
	}
}
