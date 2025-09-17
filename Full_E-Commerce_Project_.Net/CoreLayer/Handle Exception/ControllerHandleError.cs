using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Handle_Exception
{
	public class ControllerHandleError
	{

		public ControllerHandleError(int code,string? message=null,List<string>? messages=null)
		{
			if(message == null&&messages==null)
			{
				message= ReasonPhrases.GetReasonPhrase(code)?? "UnKnown Error!!";
			}
			this.code = code;
			this.Message = message;
			this.Messages = messages;

		}
		public int code { get; set; }
		public string? Message { get; set; }
		public List<string>? Messages { get; set; }
	}
}
