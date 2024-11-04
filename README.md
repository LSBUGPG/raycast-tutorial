# Raycast Tutorial

This tutorial covers how to use Unity's raycast feature to make a laser pointer

## Prerequisites

Before approaching this tutorial, you will need a current version of Unity and a code editor (such as Microsoft Visual Studio Community) installed and ready to use.

This tutorial was created with Unity 2022.3 LTS and Microsoft Visual Studio Community 2022 versions. It should work with earlier or later versions. But you should check the release notes for other versions as the Editor controls or Scripting API functions may have changed.

If you need help installing Unity you can find many online tutorials such as:
https://learn.unity.com/tutorial/install-the-unity-hub-and-editor

You will also need to know how to create an empty project, add primitive objects to your scene, create materials, create blank scripts, and run projects from within the editor. If you need help with this, there is a short video demonstrating how to do most of these things here: 

https://www.youtube.com/watch?v=eQpWPfP1T6g

## Objectives

The objective of this tutorial is to show how to cast a ray from a point and get the distance until it hits something. To demonstrate, this project simulates a laser pointer by casting a ray from the pointer into the scene. The laser line is set to the distance to the target and the target point is highlighted.

https://github.com/user-attachments/assets/d93b8ab3-b649-4e15-9596-60e5c6b04c49

## Create a scene

Create a new 3D Core Unity project.

Before we can demonstrate our laser effect, we'll need something to stop the light beam. Create a ground plane and scatter some primitives in front of the camera. By default, each of these primitive objects should already have a collider attached, which we will need for the ray to collide against.

![image](https://github.com/user-attachments/assets/a76140eb-06ad-4456-a33a-9ebdb6b00878)

If you want to copy my scene, I created the following:

| GameObject | position | rotation | scale |
| :--- | ---: | ---: | ---: |
| Ground | 0, 0, 0 | 0, 0, 0 | 100, 100, 100 |
| Box | 0.9, 0.5, -6.2 | 0, -22, 0 | 1, 1, 1 |
| Box (1) | -1, 0.5, -6.2 | 0, -6, 0 | 1, 1, 1 |
| Box (2) |  0.1, 1.5, -6.5 | 0, -6, 0 | 2, 1, 1 |
| Ball | 0, 2.5, -6.4 | 0, 0, 0 | 1, 1, 1 |

And I gave the objects some basic materials to make them show up better. I also moved the camera height up to `Y 1.8`. This is an approximate eye height assuming Unity units of $1 = 1m$.

> [!TIP]
> Unity physics defaults to a gravity of $-9.81\ units / second^2$. This implies a scale for Unity units of $1 = 1m$. You are free to build Unity projects at other scales, but if you want to approximately match Earth gravity you will need to adjust the gravity value to match your scaling, otherwise physical objects will either appear too floaty or fall too fast.

## Create the laser pointer

To create a pivot point for the laser pointer model, the laser effect, and the spot effect, create an empty object as a child of the camera.

| GameObject | position | rotation | scale |
| :--- | ---: | ---: | ---: |
| Pointer | 0.2, -0.2, 0.4 | 0, 0, 0 | 1, 1, 1 |

Then create a cylinder primitive as a child of `Pointer`.

> [!IMPORTANT]
> You need to create the model as a child of the empty object we are going to animate, as otherwise scales and rotations will override the adjustments we need to make to get the pointer to point along the Z (or forward) axis.

The default orientation and scale of a Unity cylinder primitive is upright and has a diameter of $1m$ and a height of $2m$. For this simulation I'll set the dimensions of the pointer to a diameter of $1cm$ and a height of $5cm$. I'm also tipping the cylinder forward around the $X$ axis and sliding it back along the $Z$ axis so that the front of the pointer is at $0, 0, 0$ with respect to its parent object.

![the pivot point of the parent object with respect to the laser pointer](https://github.com/user-attachments/assets/99b82697-ac24-44f1-a066-d9617cd3d1db)

| GameObject | position | rotation | scale |
| :--- | ---: | ---: | ---: |
| PointerModel | 0, 0, -0.025 | 90, 0, 0 | 0.01, 0.025, 0.01 |

> [!TIP]
> Although not strictly necessary, it is a good idea to remove the collider attached to your laser pointer model to prevent your ray from hitting this object. Alternatively, you could shrink the collider, project the ray in front of the model, or exclude the model from the collision system using layers. Of the many ways to avoid this issue, deleting the collider is the simplest.

In Unity, ray casts themselves are invisible. But for this demonstration, I'd like to see where the ray is. So to create a ray effect, we can add a `LineRenderer` as another child of the `Pointer` object. To create a `LineRenderer`, go to the `GameObject` menu and choose `Effects` and then `Line`. Name this object `Laser`.

| GameObject | position | rotation | scale |
| :--- | ---: | ---: | ---: |
| Laser | 0, 0, 0 | 0, 0, 0 | 1, 1, 1 |

By default this will look like a wide white blade because the default width of the line is $0.1m$ (or $10cm$) Unity units. :
![what the laser looks like with the default line settings](https://github.com/user-attachments/assets/1b2345ff-e703-4871-8ff5-17f75052916c)

We'll fix that now. In the `Line Renderer` component settings, change the width, by right clicking on the starting width dot and putting in a more realistic value for a laser beam. $0.002m$ (or $2mm$) should do.

![adjusting the laser width](https://github.com/user-attachments/assets/1120e394-e183-4b36-bc57-424a74c2d0cc)

I'll also take this opportunity to adjust the colour of the line. You can do this by clicking on the `Color` box. However, the colour editor in this case is different to normal as you can set multiple colour values. For example, you can set the start and end of the line to different colours. In this tutorial, I am setting both the start and end of the line to red.

![adjusting the laser colour](https://github.com/user-attachments/assets/848aa53d-6e09-43be-8aa0-6a500990c3ae)

The bottom left pointer sets the colour for the beginning of the line, and the bottom right pointer sets the colour for the end of the line.

You can also adjust the length of the laser line by modifying the `Positions` section. For `Index 1` set the `Z` position to `1000`.

![a thin red laser line](https://github.com/user-attachments/assets/0e270c60-f18f-452a-bdea-f079cf3e95e1)

## Moving the laser

Although this is not part of this tutorial, for demonstration purposes, we want to have the laser move around. To make this happen, create a new script called `Rotate` and add it to the parent `Pointer` object:

```cs
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float sensitivity = 30;
    public float debugSpeed = 0.1f;
    float pitch = 0;
    float yaw = 0;

    void Update()
    {
        pitch -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        yaw += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        transform.rotation = Quaternion.Euler(pitch, yaw, 0);
    }
}
```

To understand how this works, you will need to find a tutorial on using Unity input to move objects and one on rotation. But for the purposes of this tutorial, this script should make the pointer move as you move your mouse while the game is running.

## Performing the raycast

One problem you should notice with the current setup is that the laser can poke through your objects. You may need to adjust the length of the laser or the position of your objects to see this.

![the laser poking through a box](https://github.com/user-attachments/assets/f2ad5af3-a025-4012-90fd-16d1775b03fc)

We can fix this in the editor by shrinking the length of the laser. But to get this to work in game, we need a script to adjust the length of the laser line so that it goes up to, but not beyond, the object.

Create a new script called `Laser` and add it to the parent `Pointer` object.

Since we are going to be modifying the `Line Renderer` component in this script, we should begin by adding a variable to point to it:

```cs
    public LineRenderer trail;
```

Connect this to the `Line Renderer` component, by switching to Unity, selecting the parent `Pointer` object, and dragging the `Laser` object into the `Trail` slot in the script in the inspector window.

![connecting the Line Renderer to the Laser script on the Pointer object](https://github.com/user-attachments/assets/e6b2dcfd-935d-4b3b-b608-e232f6a60156)

Now we can test this connection by setting the length of the `Line Renderer` in the script. If we start by taking the case where the laser doesn't hit anything, the length of the line should be up to the furthest that the camera can draw. We can get this value from the camera.

Fill in the `Update` function:

```cs
void Update()
{
    float distance = Camera.main.farClipPlane;
    trail.SetPosition(1, Vector3.forward * distance);
}
```

With this code we are setting a local variable `distance` to the distance to the far clipping plane of the camera. Then we are setting the position of index 1 on our `trail` variable. Our variable declaration above means that `trail` points to the `Line Renderer` that we set in the Unity inspector.

The position we are setting is the forward vector `(0, 0, 1)` multiplied by the `distance` variable that we get from the main camera. If we run this script in Unity, and look at the `Positions` section on the `Line Renderer` we should see that is has been set to the camera's far clip distance. (The default far clip distance will be `1000`.)

![the line end position has been set to Z 1000](https://github.com/user-attachments/assets/41285ef7-c562-4acd-8643-9238e3b26c4f)

Assuming this works, we can move on to consider the case where the laser does hit an object. To perform a `Raycast` we need to call the function [`Physics.Raycast`](https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Physics.Raycast.html). There are several versions of this function, but we need a version of this function that returns a `RaycastHit` data structure as this contains the distance to the hit objects. The function returns a boolean value (true or false) to let us know if an object was hit, and if it was, it fills in a `RaycastHit` data structure containing the details of the hit.

```cs
if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, distance))
{
    distance = hit.distance;
}
```

In this conditional statement:
- `if` is the instruction
- `(`...`)` the condition to test goes within these brackets
- `Physics` is the name of Unity's physics class
- `Physics.Raycast` is the function of the physics class that we are calling
- `()` the brackets following the function give the arguments as a comma separated list
- `transform.position` we are using this object's position as the `origin` for the raycast
- `transform.forward` we are using this object's direction as the `direction` of the raycast
- `out` states that the following argument is an output value
- `RaycastHit` is the type of the output value
- `hit` is our name for the output value
- `distance` is the length of the raycast

> [!NOTE]
> We previously set the `distance` variable to the distance to the camera far plane, the ray should reach at least that far if the `origin` of the ray is in front of the camera. We are using the variable `distance` here for two different purposes in this code.

Following the condition is the code to execute if the condition is true. In this case, if the ray we specified, hit something.

Here:
- `{`...`}` the code to execute if the condition is true goes within these brackets
- `distance` is our variable representing the length of the ray
- `=` means we are going to overwrite the current value in this variable
- `hit` is the variable we defined in the `out` argument
- `hit.distance` is the distance to the hit point returned by the raycast
- `;` this defines the end of the instruction

Try out this code by switching back to Unity, hitting the play button, and examining the `Positions` data. You should see the `Z` value of `Index 1` changing as you move the laser around the scene.

![the Z position when targeting the laser pointer at a sphere](https://github.com/user-attachments/assets/4f70288b-91af-46ec-8a52-6719063d014b)

## Adding a spot

To make the laser hit point a bit more visible, we can add a point of light at the hit point of the raycast. There are several methods to achieve this kind of effect.

- small unlit sphere
- unlit quad with a particle texture
- realtime light with or without a halo

For this, I'm going to use the first method as it is the simplest to set up and code.

> [!WARNING]
> If you want to use the other methods, you have to take care that the hit point is slightly in front of the object geometry, otherwise the effect could be partially or fully obscured by the object. The unlit sphere method doesn't suffer this problem because the geometry of the sphere itself will poke through the geometry.

Start by create a new sphere object as a child of the `Laser` object and call it "Spot". Making it a child means we can control it's position just by adjusting the `Z` position value. We also want to remove the `Collider` that Unity automatically adds to this new object. This is to ensure that the raycast doesn't hit this object and break the behaviour.

The scale of the sphere should be small, but keep in mind that the further this spot is from the camera, the smaller it will appear. For this reason, I've set mine to the relatively large value of $0.02m$ ($2cm$) diameter.

You can create a material and select the unlit shader and set the colour to the same colour as your laser. The reason you want to use an unlit shader is that it will therefore be unaffected by lighting in the scene.

To position the spot in code, we just need to add a couple of extra pieces to our `Laser` script. To start, add a member variable under the existing `trail` variable:

```cs
public LineRenderer trail;
public Renderer spot;
```

Again, switch back to Unity and assign the `Spot` slot in the `Laser` script by dragging the object named "Spot" into it.

Now, in the script we can set the position of the spot based on the `distance` variable we already calculated. We can do this by adding a line to the end of our existing `Update` function:

```cs
spot.transform.localPosition = Vector3.forward * distance;
```

Here:
- `spot` is the member variable we defined to point to the unlit sphere
- `spot.transform` is the transform of that object
- `spot.transform.localPosition` is the position in local coordinates
- `=` means we are replacing the existing value
- `Vector3` is the Unity class representing 3D coordinates
- `Vector3.forward` is the vector (0, 0, 1)
- `*` means we are multiplying this value
- `distance` is the distance along the ray to the hit point
- `;` the end of the instruction

Test this code in Unity to see if the spot follows the hit point of the ray.

## Hiding the spot

There is a potential issue which could occur if the ray origin started behind the camera. In that case, the spot might be placed at the end of the laser line even if the ray didn't intersect anything.

To address this we can tell Unity to render the spot only in the case that ray hits an object. To do that, we can change our `if` conditional to an `if`...`else`. With an `else` directive, the bracketed section after the `else` statement is executed only if the condition is not true.

```cs
if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, distance))
{
    distance = hit.distance;
    spot.enabled = true;
}
else
{
    spot.enabled = false;
}
```

You can test this by monitoring the `Mesh Renderer` flag on the `Spot` object in the inspector window while the project is playing. Or you can see if the rendering switches off in the Scene view by zooming in on the spot location when the ray is pointing into the sky.

## Review

In this tutorial we have simulated a laser pointer by using Unity's raycast functionality to detect the point of intersection between a ray and an object in the scene.

The completed code looks like this:

```cs
using UnityEngine;

public class Laser : MonoBehaviour
{
    public LineRenderer trail;
    public Renderer spot;

    void Update()
    {
        float distance = Camera.main.farClipPlane;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, distance))
        {
            distance = hit.distance;
            spot.enabled = true;
        }
        else
        {
            spot.enabled = false;
        }
        trail.SetPosition(1, Vector3.forward * distance);
        spot.transform.localPosition = Vector3.forward * distance;
    }
}
```

Executing the project should produce a result similar to that set out in our objective:

https://github.com/user-attachments/assets/d93b8ab3-b649-4e15-9596-60e5c6b04c49