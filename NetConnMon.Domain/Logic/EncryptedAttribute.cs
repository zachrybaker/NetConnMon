using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetConnMon.Domain.Logic
{
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class EncryptedAttribute : Attribute
	{
		//readonly string _fieldName;

		//public EncryptedAttribute(string fieldName)
		//{
		//	_fieldName = fieldName;
		//}

		//public string FieldName => _fieldName;
	}
}
