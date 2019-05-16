using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;


namespace Monofoxe.Demo.GameLogic
{
	public class Multibutton
	{
		public List<Buttons> Buttons;

		public Multibutton(params Buttons[] buttons)
		{
			Buttons = buttons.ToList();
		}

		
		public bool Check() 
		{
			foreach(var button in Buttons)
			{
				if (Input.CheckButton(button))
				{
					return true;
				}
			}
			return false;
		}


		public bool CheckPress() 
		{
			foreach(var button in Buttons)
			{
				if (Input.CheckButtonPress(button))
				{
					return true;
				}
			}
			return false;
		}

		public bool CheckRelease() 
		{
			foreach(var button in Buttons)
			{
				if (Input.CheckButtonRelease(button))
				{
					return true;
				}
			}
			return false;
		}



	}
}
