# Godot-UniCoroutine
A coroutine system similar to Unity engine for Godot C# version.

## Setup
Copy the code to your Godot C# project.

## Learn
### Usage
1. Define your coroutine tasks.
	- Waiting Task (Waiting for Time/Input)
		``` C#
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
		```
	- Animation Task (Movement/Rotation)
		```C#
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
		```

2. Start Coroutine

	Using the **StartCoroutine** method to start your tasks, it will auto create the CoroutineSystem node and add to your scene tree as "CoroutineSystem". 
	``` C#
	var cs = CoroutineSystem.instance;
	cs.StartCoroutine(ImageTask());
	cs.StartCoroutine(TestTask());
	```

3. Stop Coroutine

	Using **StopCoroutine** to stop your couroutine tasks.

	``` C#
	var cs = CoroutineSystem.instance;
	cs.StopCoroutine("ImageTask");
	```

### Get the delta time
- All the tasks are processed by the CoroutineSystem node.<br>
	You can get the delta time from the CoroutineSystem instance.

	``` C#
	IEnumerator GetDeltaTimeTask(){
		while(true){
			float deltaTime = CoroutineSystem.instance.deltaTime;
			GD.Print($"DeltaTime:{deltaTime}");
			yield return null;
		}
	}
	```

## License ###
This content is released under the (http://opensource.org/licenses/MIT) MIT License.
