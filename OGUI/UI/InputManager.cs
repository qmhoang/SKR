using System;
using DEngine.Core;
using libtcod;

namespace OGUI.UI
{
	/// <summary>
	/// Handles all the input polling and message dispatch to the attached
	/// Window.  An InputManager is contained in and controlled by an Application object.
	/// </summary>
	public class InputManager
	{
		#region Constructors
		
		/// <summary>
		/// Create an InputManager instance by providing the attached Window.
		/// </summary>
		public InputManager(Component component)
		{
			if (component == null)
			{
				throw new ArgumentNullException("component");
			}

			attachedComponent = component;
		}
		
		#endregion
		#region Public Methods
		
		/// <summary>
		/// Called by the Application on each update tick to perform input polling.  The InputManager instance
		/// will send the appropriate user input messages to the attached window provided
		/// during construction.
		/// </summary>
		/// <param name="elapsed"></param>
		public void Update(uint elapsed)
		{
			PollMouse(elapsed);
			PollKeyboard();
		}

		/// <summary>
		/// Returns true if the specified <paramref name="key"/> is currently being
		/// pushed.  This method is here for non-standard use of the framework - it is
		/// typically better to use the normal keyboard messaging system provided by
		/// components.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool IsKeyDown(TCODKeyCode key)
		{
			return TCODConsole.isKeyPressed(key);
		}
		
		#endregion
		#region Private Fields
		
		private readonly Component attachedComponent;
		private Point lastMousePosition;
		private Point lastMousePixelPosition;
		private MouseButton lastMouseButton;
		private float lastMouseMoveTime;
		private bool isHovering;
		private Point startLBDown;
		private bool isDragging;
		
		#endregion
		#region Private Constants
		
		// NOTE: consider making these configurable instead of constants
		private const int dragPixelTol = 24;
		private const float hoverMSTol = 600f;
	    private const int delayUntilNextClick = 100;
		
		#endregion
		#region Keyboard Input
		
		private void PollKeyboard()
		{
			TCODKey key = TCODConsole.checkForKeypress((int)TCODKeyStatus.KeyPressed
				| (int)TCODKeyStatus.KeyReleased);
			
			if (key.KeyCode != TCODKeyCode.NoKey)
			{
				if (key.Pressed)
				{
					attachedComponent.OnKeyPressed(new KeyboardData(key));
				}
				else
				{
					attachedComponent.OnKeyReleased(new KeyboardData(key));
				}
			}
		}
		
		#endregion
		#region Mouse Input
		
		private void PollMouse(uint totalElapsed)
		{
			MouseData mouse = new MouseData(TCODMouse.getStatus());                       

			CheckMouseButtons(mouse);
				
			// check for mouse move
			//if (mouse.Position != lastMousePosition)
			//{
			//    DoMouseMove(mouse);
					
			//    lastMousePosition = mouse.Position;
			//    lastMouseMoveTime = totalElapsed;
			//}
			if ((mouse.PixelPosition != lastMousePixelPosition) && ((totalElapsed - lastMouseMoveTime) > delayUntilNextClick))
			{
				DoMouseMove(mouse);

				lastMousePosition = mouse.Position;
				lastMousePixelPosition = mouse.PixelPosition;
				lastMouseMoveTime = totalElapsed;
			}
				
			// check for hover
			if ( (totalElapsed - lastMouseMoveTime) > hoverMSTol &&
				isHovering == false)
			{
				StartHover(mouse);
			}
		}
		

		
		private void CheckMouseButtons(MouseData mouse)
		{
			if (mouse.MouseButton != lastMouseButton)
			{
				if (lastMouseButton == MouseButton.None)
				{
					DoMouseButtonDown(mouse);
				} else
				{
					DoMouseButtonUp(new MouseData(lastMouseButton, mouse.Position, mouse.PixelPosition));
				}
				
				lastMouseButton = mouse.MouseButton;
			}
		}
		

		
		private void StartHover(MouseData mouse)
		{
			attachedComponent.OnMouseHoverBegin(mouse);
			
			isHovering = true;
		}
		

		
		private void StopHover(MouseData mouse)
		{
			attachedComponent.OnMouseHoverEnd(mouse);
			
			isHovering = false;
		}
		

		
		private void StartDrag(MouseData mouse)
		{
			isDragging = true;
			
			// TODO fix this, it does not pass origin of drag as intended
			attachedComponent.OnMouseDragBegin(mouse.Position);
		}
		

		
		private void StopDrag(MouseData mouse)
		{
			isDragging = false;
			
			attachedComponent.OnMouseDragEnd(mouse.Position);
		}
		

		
		private void DoMouseMove(MouseData mouse)
		{
			StopHover(mouse);
			
			attachedComponent.OnMouseMoved(mouse);
			
			// check for BeginDrag
			if (mouse.MouseButton == MouseButton.LeftButton)
			{
				int delta = Math.Abs(mouse.PixelPosition.X - startLBDown.X) +
					Math.Abs(mouse.PixelPosition.Y - startLBDown.Y);
				
				if (delta > dragPixelTol && isDragging == false)
				{
					StartDrag(mouse);
				}
			}
		}
		

		
		private void DoMouseButtonDown(MouseData mouse)
		{
			if (isDragging)
				StopDrag(mouse);
			
			if (mouse.MouseButton == MouseButton.LeftButton)
			{
				startLBDown = mouse.PixelPosition;
			}
			
			attachedComponent.OnMouseButtonDown(mouse);
		}
		

		
		private void DoMouseButtonUp(MouseData mouse)
		{
			if (isDragging)
				StopDrag(mouse);
			
			attachedComponent.OnMouseButtonUp(mouse);
		}
		
		#endregion
	}
}

