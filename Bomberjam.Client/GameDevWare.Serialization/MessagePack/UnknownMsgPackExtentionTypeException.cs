/* 
	Copyright (c) 2016 Denis Zykov, GameDevWare.com

	This a part of "Json & MessagePack Serialization" Unity Asset - https://www.assetstore.unity3d.com/#!/content/59918

	THIS SOFTWARE IS DISTRIBUTED "AS-IS" WITHOUT ANY WARRANTIES, CONDITIONS AND 
	REPRESENTATIONS WHETHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION THE 
	IMPLIED WARRANTIES AND CONDITIONS OF MERCHANTABILITY, MERCHANTABLE QUALITY, 
	FITNESS FOR A PARTICULAR PURPOSE, DURABILITY, NON-INFRINGEMENT, PERFORMANCE 
	AND THOSE ARISING BY STATUTE OR FROM CUSTOM OR USAGE OF TRADE OR COURSE OF DEALING.
	
	This source code is distributed via Unity Asset Store, 
	to use it in your project you should accept Terms of Service and EULA 
	https://unity3d.com/ru/legal/as_terms
*/

using System;
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace Hackathon.Framework.GameDevWare.Serialization.MessagePack
{
	[Serializable]
	public sealed class UnknownMsgPackExtentionTypeException : SerializationException
	{
		public UnknownMsgPackExtentionTypeException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public UnknownMsgPackExtentionTypeException(string message) : base(message)
		{
		}

		public UnknownMsgPackExtentionTypeException(sbyte invalidExtType)
			: base(string.Format("Unknown MessagePack extention type '{0}' was readed from stream.", invalidExtType))
		{
		}

		private UnknownMsgPackExtentionTypeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}